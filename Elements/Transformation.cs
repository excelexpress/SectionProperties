using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public struct Coordinate
    {
        public double x;
        public double y;
    }

    public partial class SectionElements
    {

        public static Coordinate TransformCoordinate(Coordinate point1_global, Coordinate point1_shape, Coordinate point2_shape, double theta, bool mirrorX = false, bool mirrorY = false)
        {
            double delx_shp = point2_shape.x - point1_shape.x;
            double dely_shp = point2_shape.y - point1_shape.y;

            if (mirrorX)
            {
                dely_shp = -dely_shp;
            }

            if (mirrorY)
            {
                delx_shp = -delx_shp;
            }

            point2_shape.x = point1_shape.x + delx_shp;
            point2_shape.y = point1_shape.y + dely_shp;

            //return point2_global
            Coordinate prime = new Coordinate();

            prime.x = point1_global.x + delx_shp * Math.Cos(theta) - dely_shp * Math.Sin(theta);
            prime.y = point1_global.y + delx_shp * Math.Sin(theta) + dely_shp * Math.Cos(theta);

            return prime;
        }

        public static Coordinate ConvertXYtoCoordinate(double x, double y)
        {
            Coordinate coord = new Coordinate();

            coord.x = x;
            coord.y = y;

            return coord;
        }

    }
}
