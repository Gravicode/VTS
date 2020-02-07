using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Data
{
    public class VesselInfo
    {
        public double Lat { set; get; }
        public double Lng { set; get; }
        public string Dest { set; get; }
        public double Speed { set; get; }
        public double Course { set; get; }
        public string Status { set; get; }
        public string ETA { set; get; }
        public string RTA { set; get; }
        public string ShipName { set; get; }
        public uint? Direction { get; set; }
        public uint Mmsi { get; set; }
        public double Fuel { get; set; }
        public double LastFlowIn { set; get; }
        public DateTime LastFlowInDate { get; set; }
        public double LastFlowOut { get; set; }
        public DateTime LastFlowOutDate { get; set; }
    }
}
