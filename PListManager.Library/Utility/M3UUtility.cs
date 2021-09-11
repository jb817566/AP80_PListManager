using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AP80_PListManager.Model;

namespace AP80_PListManager.Utility
{
    public class M3UUtility
    {
        public List<M3UFileInfo> M3UToFileList(FileInfo m3uFile, string musicFolder)
        {
            string[] _lines = File.ReadAllLines(m3uFile.ToString());
            return _lines.Select(a => new M3UFileInfo(a, musicFolder)).ToList();
        }
    }
}
