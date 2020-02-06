using GoogleMapsComponents.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Data
{
    public class VesselLocation
    {
        public VesselInfo ShipInfo { get; set; }
        public Polygon ShipLocation { get; set; }
        public Marker ShipMarker { get; set; }
    }
}
