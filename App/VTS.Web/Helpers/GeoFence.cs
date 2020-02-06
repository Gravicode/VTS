using GoogleMapsComponents.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Helpers
{
    public class GeoFence
    {
        public static bool PointInPolygon(LatLngLiteral p, List<LatLngLiteral> poly)
        {
            int n = poly.Count();

            poly.Add(new LatLngLiteral { Lat = poly[0].Lat, Lng = poly[0].Lng });
            LatLngLiteral[] v = poly.ToArray();

            int wn = 0;    // the winding number counter

            // loop through all edges of the polygon
            for (int i = 0; i < n; i++)
            {   // edge from V[i] to V[i+1]
                if (v[i].Lat <= p.Lat)
                {         // start y <= P.y
                    if (v[i + 1].Lat > p.Lat)      // an upward crossing
                        if (isLeft(v[i], v[i + 1], p) > 0)  // P left of edge
                            ++wn;            // have a valid up intersect
                }
                else
                {                       // start y > P.y (no test needed)
                    if (v[i + 1].Lat <= p.Lat)     // a downward crossing
                        if (isLeft(v[i], v[i + 1], p) < 0)  // P right of edge
                            --wn;            // have a valid down intersect
                }
            }
            if (wn != 0)
                return true;
            else
                return false;

        }

        private static int isLeft(LatLngLiteral P0, LatLngLiteral P1, LatLngLiteral P2)
        {
            double calc = ((P1.Lng - P0.Lng) * (P2.Lat - P0.Lat)
                    - (P2.Lng - P0.Lng) * (P1.Lat - P0.Lat));
            if (calc > 0)
                return 1;
            else if (calc < 0)
                return -1;
            else
                return 0;
        }
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
