using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTS.Web.Helpers
{
    public class Vector
    {
        public double Y { get; set; }
        public double X { get; set; }
        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

    }
    public static class VectorExtentions
    {
        public static PointXY Rotate(this PointXY pt, double angle, PointXY center)
        {
            Vector v = new Vector(pt.X - center.X, pt.Y - center.Y).Rotate(angle);
            return new PointXY(v.X + center.X, v.Y + center.Y);
        }
        public static Vector Rotate(this Vector v, double degrees)
        {
            return v.RotateRadians(degrees * Math.PI / 180);
        }
        public static Vector RotateRadians(this Vector v, double radians)
        {
            double ca = Math.Cos(radians);
            double sa = Math.Sin(radians);
            return new Vector(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y);
        }
    }

    public class PointXY
    {
        public double Y { get; set; }
        public double X { get; set; }
        public PointXY(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}