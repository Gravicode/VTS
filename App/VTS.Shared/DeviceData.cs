using System;
using System.Collections.Generic;
using System.Text;

namespace VTS.Shared
{
    public class DeviceData
    {
        public string SensorId { get; set; }
        public uint Mmsi { get; set; }
        public double FlowIn { get; set; }
        public double FlowOut { get; set; }
        public DateTime Created { get; set; }
    }


}
