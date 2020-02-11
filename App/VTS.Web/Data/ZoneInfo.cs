using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Data
{
    public class ZoneInfo
    {
        public int No { get; set; }
        public string Title { get; set; }
        public string ZoneName { get; set; }
        public float Height { get; set; }
        public int RowCount { get; set; } = 1;
    }
}
