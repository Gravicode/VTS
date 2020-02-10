using System;
using System.Collections.Generic;
using System.Text;

namespace VTS.Shared
{
    public class VesselData
    {
        public long Id { get; set; }
        public uint Mmsi { get; set; }
        public VesselInformation Info { get; set; }
        public VesselActivity Activity { get; set; }
        public TimeManagement Schedule { get; set; }
    }
}
