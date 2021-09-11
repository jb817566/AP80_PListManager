using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AP80_PListManager.Utility;
using FlacLibSharp;
using Newtonsoft.Json;

namespace AP80_PListManager.Model
{
    class ArtistTrack : M3UFileInfo
    {
        public ArtistTrack(string path, string baseRel = null, bool isLocal = false) : base(path, baseRel, isLocal)
        {
        }

        private static string TempFolderHashes { get; set; }

        static ArtistTrack()
        {
            TempFolderHashes = Path.Combine(Path.GetTempPath(), "songinfohashes");
            if (!Directory.Exists(TempFolderHashes))
            {
                Directory.CreateDirectory(TempFolderHashes);
            }

            Console.WriteLine($"HashfilePath: {TempFolderHashes}");
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
                    info.Extension, tfile.Properties.Duration.TotalSeconds, tfile.Properties.AudioBitrate
                );
            }
        }
    }
}
