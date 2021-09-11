using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AP80_PListManager.Model;
using AP80_PListManager.Utility;
using Microsoft.Extensions.Configuration;

namespace AP80_PListManager
{
    class Program
    {
        static Program()
        {
            LoadConfig();
        }

        public const string PLAYLIST_DIR = "playlist_data";

        static async Task Main(string[] args)
        {
            while (1 == 1)
            {
                try
                {
                    string _menu = @"
Menu:
    1. Transfer Playlist to Device
    2. Transfer Playlist from Device
    3. List Playlist Size
    4. Cleanup Duplicate Music
    5. Merge albums per artist";
                    Console.WriteLine(_menu);
                    Console.Write("\nOption: ");
                    int opt = int.Parse(Console.ReadLine());
                    DirectoryInfo _pDir = new DirectoryInfo(Path.Combine(GetMusicFolder(), PLAYLIST_DIR));
                    switch (opt)
                    {
                        case 1:
                            Console.Write("\nPlaylist Path: ");
                            string _path = Console.ReadLine();
                            //Console.Write("\nBase Relative Path: ");
                            //string _relpath = Console.ReadLine();
                            List<string> _playlistLines = File.ReadAllLines(_path)
                                .Select(a => new M3UFileInfo(a, Program.GetMusicFolder()).ToM3UPath()).ToList();
                            var _date = $"{DateTime.Now:MM_dd_yyyy}.m3u";
                            var _fname = $"{new FileInfo(_path).Name.Replace(".m3u", _date)}";
                            File.WriteAllLines(Path.Combine(_pDir.ToString(), _fname), _playlistLines);
                            break;
                        case 2:
                            List<FileInfo> _pFiles = _pDir.EnumerateFiles("*.m3u").ToList();
                            M3UUtility _util = new M3UUtility();
                            _pFiles.ForEach(_p =>
                            {
                                File.WriteAllLines(Path.Combine(Program.GetLocalFolder(), "playlist_") +
                                                   $"{DateTime.Now:MM_dd_yyyyH_mm_ss}{new Random().Next(0, 10)}" +
                                                   "_fromdevice.m3u",
                                    _util.M3UToFileList(_p, Program.GetMusicFolder()).Select(a => a.ToFilePath()));
                            });
                            break;
                        case 3:
                            List<FileInfo> _files = _pDir.EnumerateFiles("*.m3u").ToList();
                            ListPlaylistSize(_files);
                            break;
                        case 4:
                            new FolderCleanupUtility().CleanupFolder(Program.GetMusicFolder());
                            break;
                        case 5:
                            await new FolderCleanupUtility().MergeAlbumsPerArtist(Program.GetMusicFolder());
                            break;
                    }
                }
                catch
                {
                }
            }
        }

        private static void ListPlaylistSize(List<FileInfo> _flist)
        {
            M3UUtility _util = new M3UUtility();
            _flist.ForEach(_f =>
            {
                List<M3UFileInfo> _flist = _util.M3UToFileList(_f, Program.GetMusicFolder());
                double _mb = _flist.Sum(a => a.Info.Length) / (1e9);
                Console.WriteLine($"{_f} Playlist size: {_mb} GB");
            });
        }

        private static void LoadConfig()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            Program.Config = configuration;
        }

        public static string GetMusicFolder()
        {
            return Program.Config.GetSection("MusicFolder").Value;
        }

        public static string GetLocalFolder()
        {
            return Program.Config.GetSection("LocalFolder").Value;
        }

        public static IConfigurationRoot Config { get; set; }
    }
}