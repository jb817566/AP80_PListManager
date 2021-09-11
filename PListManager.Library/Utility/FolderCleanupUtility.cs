using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlacLibSharp;
using Newtonsoft.Json;

namespace AP80_PListManager.Utility
{
    public class FolderCleanupUtility
    {
        private static List<string> _invalidFiles = new List<string>();
        private static string TempFolderHashes { get; set; }

        static FolderCleanupUtility()
        {
            TempFolderHashes = Path.Combine(Path.GetTempPath(), "songinfohashes");
            if (!Directory.Exists(TempFolderHashes))
            {
                Directory.CreateDirectory(TempFolderHashes);
            }

            Console.WriteLine($"HashfilePath: {TempFolderHashes}");
        }

        public async Task MergeAlbumsPerArtist(string folder)
        {
            List<FileInfo> _allFiles = FindSongs(folder);
            Dictionary<string, FileInfo> _artistSongMap = new Dictionary<string, FileInfo>();
        }
        public void CleanupFolder(string folder)
        {
            List<FileInfo> _allFiles = FindSongs(folder);
            HashSet<SongInfo> _unique = new HashSet<SongInfo>();
            int count = _allFiles.Count;
            Console.WriteLine($"Found songs: {count}");

            List<SongInfo> _allTracks = FilesToSongInfo(_allFiles);

            #region sublists
            List<SongInfo> _m4aTracks = new List<SongInfo>();
            List<SongInfo> _cleanTracks = new List<SongInfo>();
            List<SongInfo> _dupes = new List<SongInfo>();
            #endregion
            for (var index = 0; index < _allTracks.Count; index++)
            {
                SongInfo _songInfo = _allTracks.ElementAt(index);
                try
                {
                    if (_songInfo.Extension == ".m4a")
                    {
                        _m4aTracks.Add(_songInfo);
                    }
                    else if (!_songInfo.IsExplicit)
                    {
                        _cleanTracks.Add(_songInfo);
                    }
                    else
                    {
                        if (!_unique.Add(_songInfo))
                        {
                            _dupes.Add(_songInfo);
                        }
                    }
                }
                catch
                {
                }
            }

            foreach (SongInfo m4ATrack in _m4aTracks)
            {
                if (!_unique.Add(m4ATrack))
                {
                    _dupes.Add(m4ATrack);
                }
            }

            foreach (SongInfo _cleanTrack in _cleanTracks)
            {
                if (!_unique.Add(_cleanTrack))
                {
                    _dupes.Add(_cleanTrack);
                }
            }


            if (!_dupes.Any())
            {
                Console.WriteLine("No Dupes Found");
                return;
            }
            else
            {
                Console.WriteLine($"Deduped To: {_unique.Count}");
            }

            if (!_dupes.Any())
            {
                Console.WriteLine("No Invalid Found");
                return;
            }
            else
            {
                Console.WriteLine($"Invalid Files: {_invalidFiles.Count}");
            }

            string _menu = @"
Menu:
    1. Delete Dupes
    2. List Dupes
    3. List Invalid
    4. Delete Invalid
    5. Done";
            do
            {
                Console.WriteLine(_menu);
                int _opt = int.Parse(Console.ReadLine());
                switch (_opt)
                {
                    case 1:
                        foreach (var _file in _dupes.Select(f => new FileInfo(f.FilePath)))
                        {
                            _file.Delete();
                        }

                        _dupes.Clear();
                        break;
                    case 2:
                        foreach (var songInfo in _dupes)
                        {
                            Console.WriteLine(songInfo.FilePath);
                        }

                        break;
                    case 3:
                        foreach (var _file in _invalidFiles.Select(f => new FileInfo(f)))
                        {
                            _file.Delete();
                        }
                        _invalidFiles.Clear();
                        break;
                    case 4:
                        foreach (var _path in _invalidFiles)
                        {
                            Console.WriteLine(_path);
                        }
                        break;

                    case 5:
                        return;
                        break;
                }
            } while (_dupes.Any() || _invalidFiles.Any());
        }

        private List<SongInfo> FilesToSongInfo(List<FileInfo> _allFiles)
        {
            List<SongInfo> _allTracks = _allFiles.AsParallel().Select(GetSongInfo).Where(a => a != null)
                .OrderByDescending(a => new FileInfo(a.FilePath).Length).ToList();
            return _allTracks;
        }

        private static List<FileInfo> FindSongs(string folder)
        {
            var _rootFolder = new DirectoryInfo(folder);
            List<FileInfo> _allFiles =
                _rootFolder.EnumerateFiles("*.flac", SearchOption.AllDirectories)
                    .Concat(_rootFolder.EnumerateFiles("*.m4a", SearchOption.AllDirectories)).ToList();
            return _allFiles;
        }

        private SongInfo GetSongInfo(FileInfo fileInfo)
        {
            var _code = fileInfo.FullName.SHA1Hash();
            var _codeFile = Path.Combine(TempFolderHashes, $"{_code}.hash");
            if (File.Exists(_codeFile))
            {
                return JsonConvert.DeserializeObject<SongInfo>(File.ReadAllText(_codeFile));
            }

            try
            {
                SongInfo _info = fileInfo.Extension == ".m4a" ? GetSongInfoM4A(fileInfo) : GetSongInfoFLAC(fileInfo);
                File.WriteAllText(_codeFile, JsonConvert.SerializeObject(_info));
                return _info;
            }
            catch (Exception e)
            {
                _invalidFiles.Add(fileInfo.FullName);
                // Console.WriteLine($"Found Broken File {fileInfo.FullName}");
                // fileInfo.Delete();
                return null;
            }
        }

        private SongInfo GetSongInfoFLAC(FileInfo info)
        {
            using (FlacFile file = new FlacFile(info.FullName))
            {
                var vorbisComment = file.VorbisComment;
                return new SongInfo(info.FullName, vorbisComment.Artist.Value, vorbisComment.Title.Value,
                    info.Extension);
            }
        }


        private SongInfo GetSongInfoM4A(FileInfo info)
        {
            using (var tfile = TagLib.File.Create(info.FullName))
            {
                return new SongInfo(info.FullName,
                    string.Join(';', tfile.Tag.Performers.OrderBy(a => a)),
                    tfile.Tag.Title,
                    info.Extension
                );
            }
        }
    }
}