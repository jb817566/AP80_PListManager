using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AP80_PListManager.Model
{
    public class EqualsList<T> : List<T> where T : class
    {
        public EqualsList()
        {

        }

        public bool Add(T o)
        {
            if (!base.Exists(Equals))
            {
                base.Add(o);
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
