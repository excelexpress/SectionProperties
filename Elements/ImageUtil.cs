using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public class PlotProperties
    {
        //Plot Properties are the envelope bounds of the plot (can be set to anything)
        public double Xmin { get; set; }
        public double Xmax { get; set; }
        public double Ymin { get; set; }
        public double Ymax { get; set; }
    }

    public class ImageUtil
    {
        public static double CalculateScaleFactor(Bitmap b, PlotProperties PlotProps)
        {
            double w = Convert.ToDouble(b.Width);
            double h = Convert.ToDouble(b.Height);

            double xmin = PlotProps.Xmin;
            double xmax = PlotProps.Xmax;
            double ymin = PlotProps.Ymin;
            double ymax = PlotProps.Ymax;

            double delX = xmax - xmin;
            double delY = ymax - ymin;

            double SFX = w / delX;
            double SFY = h / delY;

            double SF;

            SF = (SFX < SFY) ? SFX : SFY;

            return SF;
        }

        public static Coordinate CalculateImageXYmin(Bitmap b, PlotProperties PlotProps)
        {
            //Calculate Bottom Corner Point after scaling
            double w = Convert.ToDouble(b.Width);
            double h = Convert.ToDouble(b.Height);

            double xmin = PlotProps.Xmin;
            double xmax = PlotProps.Xmax;
            double ymin = PlotProps.Ymin;
            double ymax = PlotProps.Ymax;

            double delX = xmax - xmin;
            double delY = ymax - ymin;

            double SF = CalculateScaleFactor(b, PlotProps);

            //calculate empty space -- one of these will be zero
            double cx = 0.5 * (w / SF - delX);
            double cy = 0.5 * (h / SF - delY);

            //Image bottom corner coordinate
            double _xmin = xmin - cx;
            double _ymin = ymin - cy;

            Coordinate coord = new Coordinate { x = _xmin, y = _ymin };

            return coord;

        }

        public static Matrix ImageTransformationMatrix(Bitmap bitmap, PointF pnt, double theta_degree, double Xdel, double Ydel, double ScaleFactor, bool mirrorX, bool mirrorY)
        {
            Matrix mtrx = new Matrix();

            double SF = ScaleFactor;

            mtrx.Translate(-pnt.X, -pnt.Y, MatrixOrder.Append);
            if (mirrorX == true)
            {
                mtrx.Scale(1.0F, -1.0F, MatrixOrder.Append);
            }
            if (mirrorY == true)
            {
                mtrx.Scale(-1.0F, 1.0F, MatrixOrder.Append);
            }
            mtrx.RotateAt((float)(theta_degree), new PointF(0.0F, 0.0F), MatrixOrder.Append);
            mtrx.Scale(1.0F, -1.0F, MatrixOrder.Append);
            mtrx.Translate(0, 1 * bitmap.Height, MatrixOrder.Append);
            mtrx.Translate((float)(Xdel * SF), (float)(-1 * Ydel * SF), MatrixOrder.Append);

            return mtrx;

        }

    }

}
