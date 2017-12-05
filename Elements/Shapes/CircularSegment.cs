using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public partial class SectionElements
    {

        public class CircularSegment : Shape
        {
            public override IsoMaterial Material { get; set; }
            public override double theta { get; set; }
            public override bool mirrorX { get; set; } = false;
            public override bool mirrorY { get; set; } = false;

            public override string point { get; set; }
            public override double xp { get; set; }
            public override double yp { get; set; }

            public static List<string> _pointlist = new List<string>() { "a", "b", "c", "d", "e", "cg" };
            public override List<string> ShapePointList { get { return _pointlist; } }
                        
            [Dimension("Radius")]
            public double R { get; set; }

            private double _phi = 0;
            private double alpha;
            [Dimension("Arc Angle")]
            public double phi
            {
                get
                {
                    return _phi;
                }
                set
                {
                    _phi = value;
                    alpha = value / 2;
                }
            }

            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);
                
                int int_R = (int)(R * SF);
                int deg_alpha = (int)(alpha * 180.0 / Math.PI);

                int Rcos = (int)(int_R * Math.Cos(alpha));
                int Rsin = (int)(int_R * Math.Sin(alpha));
                
                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(Rcos, 0, Rcos, -Rsin);
                path.AddArc(new System.Drawing.Rectangle(-int_R, -int_R, 2 * int_R, 2 * int_R), 360 - deg_alpha, 2 * deg_alpha);
                path.AddLine(Rcos, Rsin, Rcos, 0);
                path.CloseFigure();

                PointF pnt = CreateImagePoint(SF);

                Coordinate xy_min = ImageUtil.CalculateImageXYmin(bitmap, plotprops);

                double Xdel = xp - xy_min.x;
                double Ydel = yp - xy_min.y;

                Matrix mtrx = ImageUtil.ImageTransformationMatrix(bitmap, pnt, theta_degree, Xdel, Ydel, SF, mirrorX, mirrorY);
                path.Transform(mtrx);

                Color _fillColor = Material.ConvertFillColor();

                Pen _pen = new Pen(borderColor, borderThickness);
                _pen.Alignment = PenAlignment.Inset;
                g.FillPath(new SolidBrush(_fillColor), path);
                g.DrawPath(_pen, path);

                _pen.Dispose();
                g.Dispose();
            }

            protected override SecProp ShapeSecProp()
            {
                SecProp sp = new SecProp();

                double cos = Math.Cos(alpha);
                double sin = Math.Sin(alpha);

                double E = Material.E;

                double A = R * R / 2.0 * (2.0 * alpha - Math.Sin(2.0 * alpha));
                double I_Y1 = A * R * R / 4.0 * (1.0 + (2.0 * Math.Pow(sin, 3.0) * cos) / (alpha - sin * cos));

                double Ixx = A * R * R / 4.0 * (1.0 - (2.0 / 3.0) * (Math.Pow(sin, 3.0) * cos) / (alpha - sin * cos));
                double Iyy = I_Y1 - 4.0 * Math.Pow(R, 6.0) * Math.Pow(sin, 6) / (9 * A);
                double Ixy = 0.0;

                sp.EA = E * A;
                sp.EIxx = E * Ixx;
                sp.EIyy = E * Iyy;
                sp.EIxy = E * Ixy;
                sp.Xcg = 4.0 * R * Math.Pow(sin,3) / (3.0 * (2 * alpha - Math.Sin(2*alpha)));
                sp.Ycg = 0;

                return sp;

            }


            protected override Coordinate LocalPointCoordinate(string PointID)
            {
                //Returns the coordinate of a defined point 
                //assumes zero rotation and translation
                Coordinate point1_sh = new Coordinate();
                
                double cos = Math.Cos(alpha);
                double sin = Math.Sin(alpha);

                switch (PointID)
                {
                    case "a":
                        point1_sh = ConvertXYtoCoordinate(0, 0);
                        break;
                    case "b":
                        point1_sh = ConvertXYtoCoordinate(R*cos, R*sin);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(R, 0);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(R * cos, -R * sin);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(R * cos, 0);
                        break;
                    case "cg":
                        SecProp sp = ShapeSecProp();
                        double x_cg = sp.Xcg;
                        double y_cg = sp.Ycg;
                        point1_sh = ConvertXYtoCoordinate(x_cg, y_cg);
                        break;
                }

                return point1_sh;

            }           


        }
    }
}

