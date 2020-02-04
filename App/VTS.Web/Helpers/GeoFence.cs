using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Helpers
{
    public class GeoFence
    {
        public static bool PolyContainsPointXY(List<PointXY> PointXYs, PointXY p)
        {
            bool inside = false;

            // An imaginary closing segment is implied,
            // so begin testing with that.
            PointXY v1 = PointXYs[PointXYs.Count - 1];

            foreach (PointXY v0 in PointXYs)
            {
                double d1 = (p.Y - v0.Y) * (v1.X - v0.X);
                double d2 = (p.X - v0.X) * (v1.Y - v0.Y);

                if (p.Y < v1.Y)
                {
                    // V1 below ray
                    if (v0.Y <= p.Y)
                    {
                        // V0 on or above ray
                        // Perform intersection test
                        if (d1 > d2)
                        {
                            inside = !inside; // Toggle state
                        }
                    }
                }
                else if (p.Y < v0.Y)
                {
                    // V1 is on or above ray, V0 is below ray
                    // Perform intersection test
                    if (d1 < d2)
                    {
                        inside = !inside; // Toggle state
                    }
                }

                v1 = v0; //Store previous endPointXY as next startPointXY
            }

            return inside;
        }
    }
}
