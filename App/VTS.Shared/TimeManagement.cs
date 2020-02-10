using System;
using System.Collections.Generic;
using System.Text;

namespace VTS.Shared
{
    public class TimeManagement
    {
        public DateTime EstTimeArrival { get; set; }
        public DateTime EstTimeBerthing { get; set; }

        public DateTime EstStartWork { get; set; }

        public DateTime EstEndWork { get; set; }

        public DateTime EstTimeDeparture { get; set; }

        public DateTime RealTimePlan { get; set; }

        public DateTime RealTimeAnchor { get; set; }
        public DateTime RealTimeBerthing { get; set; }

    }
}
