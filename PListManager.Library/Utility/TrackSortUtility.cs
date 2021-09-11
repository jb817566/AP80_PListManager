using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AP80_PListManager.Enum;

namespace AP80_PListManager.Utility
{
    public class TrackSortUtility
    {
        public List<SongInfo> SortAndGetBest(List<SongInfo> songs, TrackSortOptions opts)
        {
            IEnumerable<SongInfo> _info = songs.AsEnumerable();
            IOrderedEnumerable<SongInfo> _ordered = null;
            foreach (SortParameter sortParameter in opts.ParameterOrder)
            {
                switch (sortParameter)
                {
                    case SortParameter.BITRATE:
                        _ordered = _ordered == null
                            ? _info.OrderByDescending(a => a.Bitrate) : _ordered.ThenBy(a => a.Bitrate);
                        break;
                    case SortParameter.EXPLICIT:
                        _ordered = _ordered == null
                            ? _info.OrderByDescending(a => a.Bitrate) : _ordered.ThenBy(a => a.Bitrate);
                        break;
                    case SortParameter.LENGTH:
                        _ordered = _ordered == null
                            ? _info.OrderByDescending(a => a.Bitrate) : _ordered.ThenBy(a => a.Bitrate);
                        break;
                }
            }

            return null;

        }
    }
}
