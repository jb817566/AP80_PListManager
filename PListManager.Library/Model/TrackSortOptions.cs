using System.Collections.Generic;
using AP80_PListManager.Enum;

namespace AP80_PListManager.Utility
{
    public class TrackSortOptions
    {
        public TrackSortOptions(List<SortParameter> parameterOrder, bool deleteDupeTracks)
        {
            this.ParameterOrder = parameterOrder;
            this.DeleteDupeTracks = deleteDupeTracks;
        }

        public bool DeleteDupeTracks { get; set; }

        public List<SortParameter> ParameterOrder { get; set; }
    }
}