using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace AP80_PListManager.Model
{
    public class M3UFileInfo
    {
        public string BaseRelativePath { get; set; }
        public FileInfo Info { get; set; }

        public M3UFileInfo(string path, string baseRel = null, bool isLocal = false)
        {
            if (!string.IsNullOrWhiteSpace(baseRel))
            {
                this.BaseRelativePath = baseRel.EndsWith(Path.DirectorySeparatorChar)
                    ? baseRel
                    : $"{baseRel}{Path.PathSeparator}";
            }

            if (isLocal)
            {
                Info = new FileInfo(path);
            }
            else
            {
                string _path = path.Replace("a:\\", BaseRelativePath);
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    _path = _path.Replace("\\", "/");
                }

                Info = new FileInfo(_path);
            }
        }


        public string ToFilePath()
        {
            return Info.ToString();
        }

        public string ToM3UPath()
        {
            if (!string.IsNullOrWhiteSpace(BaseRelativePath))
            {
                return $"a:\\{Info.ToString().Replace(BaseRelativePath, "")}";
            }
            else
            {
                return $"a:\\{Info}";
            }
        }
    }
}