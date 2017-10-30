using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public class Section : IShape
    {
        private List<IShape> ShapeList;

        public Section()
        {
            ShapeList = new List<IShape>();
        }

        public void AddShape(IShape shape)
        {
            ShapeList.Add(shape);
        }

        public SecProp CalculateSecProp()
        {
            List<SecProp> SecPropList = new List<SecProp>();
            foreach (IShape shape in ShapeList)
            {
                SecPropList.Add(shape.CalculateSecProp());
            };

            return SectionElements.SecProp_Section(SecPropList);
        }

        public EnvelopeCoords GetEnvelopeCoord()
        {
            double xmin = 1E+256;
            double xmax = -1E+256;
            double ymin = 1E+256;
            double ymax = -1E+256;

            EnvelopeCoords Ecoords;

            foreach (IShape shape in ShapeList)
            {
                Ecoords = shape.GetEnvelopeCoord();

                xmin = (Ecoords.Min.x < xmin) ? Ecoords.Min.x : xmin;
                xmax = (Ecoords.Max.x > xmax) ? Ecoords.Max.x : xmax;
                ymin = (Ecoords.Min.y < ymin) ? Ecoords.Min.y : ymin;
                ymax = (Ecoords.Max.y > ymax) ? Ecoords.Max.y : ymax;
            }

            EnvelopeCoords env = new EnvelopeCoords();

            Coordinate xyMax = new Coordinate();
            xyMax.x = xmax;
            xyMax.y = ymax;

            Coordinate xyMin = new Coordinate();
            xyMin.x = xmin;
            xyMin.y = ymin;

            env.Max = xyMax;
            env.Min = xyMin;

            return env;

        }

        public void Draw(ref Bitmap bitmap, PlotProperties plotprops)
        {
            foreach(IShape shape in ShapeList)
            {
                shape.Draw(ref bitmap, plotprops);
            }
        }
    }
}
