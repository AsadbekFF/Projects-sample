using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Common.Models
{
    public class Items<T>
    {
        public long Total { get; set; }

        public IReadOnlyList<T> Rows { get; set; }
    }
}
