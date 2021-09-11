using System.IO;

namespace AP80_PListManager.Utility
{
    public class SongInfo //: EqualityComparer<SongInfo>
    {

        public SongInfo(string filePath, string artist, string songTitle, string extension, double duration = 0, int bitrate = 0)
        {
            FilePath = filePath;
            IsExplicit = FilePath.ToLower().Contains("explicit");
            Artist = artist;
            SongTitle = songTitle;
            Extension = extension;
            Duration = (int)duration;
            Bitrate = bitrate;
        }

        public bool IsExplicit { get; set; }
        public string FilePath { get; set; }
        public string Artist { get; set; }
        public string SongTitle { get; set; }
        public long Bitrate { get; set; }
        public long Duration { get; set; }
        public string Extension { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            var _s = (obj as SongInfo);
            return _s.Artist == this.Artist
                   && _s.SongTitle == this.SongTitle;
        }

        public override int GetHashCode()
        {
            return (this.Artist + this.SongTitle).GetHashCode();
        }

        // public override bool Equals(SongInfo x, SongInfo y)
        // {
        //     var _s = (x as SongInfo);
        //     return _s.Artist == y.Artist
        //            && _s.SongTitle == y.SongTitle;
        // }
        //
        // public override int GetHashCode(SongInfo obj)
        // {
        //     throw new NotImplementedException();
        // }
    }
}