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

        public class CircularArc : Shape
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
                        
            private double _r = 0;
            [Dimension("Radius")]
            public double r
            {
                get
                {
                    return _r;
                }
                set
                {
                    _r = value;
                    _R = value + _t;
                }
            }
                        
            private double _t = 0;
            [Dimension("Thickness")]
            public double t
            {
                get
                {
                    return _t;
                }
                set
                {
                    _t = value;
                    _R = value + _r;
                }
            }

            [Dimension("Arc Angle")]
            public double alpha { get; set; }

            private double _R; //outer radius

            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);

                int _r = (int)(r * SF);

                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(0, 0, 0, _r);
                path.AddArc(new System.Drawing.Rectangle(0, 0, 2 * _r, 2 * _r), 180, 90);
                path.AddLine(_r, 0, 0, 0);
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

                double E = Material.E;

                double I_Y1 = alpha / 4 * (Math.Pow(_R, 4) - Math.Pow(r, 4)) * (1 + Math.Sin(alpha) * Math.Cos(alpha) / alpha);

                sp.EA = E * (_R*_R - r*r) * alpha;
                sp.EIxx = E * alpha / 4 * (Math.Pow(_R, 4) - Math.Pow(r, 4)) * (1 - Math.Sin(alpha) * Math.Cos(alpha) / alpha);
                sp.EIyy = E * (I_Y1 - 1 / (alpha*(_R*_R - r*r))*Math.Pow((2*Math.Sin(alpha)*(Math.Pow(_R,3) - Math.Pow(r,3))/3),2));
                sp.EIxy = 0;
                sp.Xcg = (2 * (Math.Pow(_R, 3) - Math.Pow(r, 3)) * Math.Sin(alpha)) / (3 * alpha * (_R * _R - r * r));
                sp.Ycg = 0;

                return sp;

            }


            protected override Coordinate LocalPointCoordinate(string PointID)
            {
                //Returns the coordinate of a defined point 
                //assumes zero rotation and translation
                Coordinate point1_sh = new Coordinate();

                double cg_pnt = ((10 - 3 * Math.PI) * r) / (3 * (4 - Math.PI));
                double cos = Math.Cos(alpha);
                double sin = Math.Sin(alpha);
                double Ravg = (_R + r) / 2;

                switch (PointID)
                {
                    case "a":
                        point1_sh = ConvertXYtoCoordinate(0, 0);
                        break;
                    case "b":
                        point1_sh = ConvertXYtoCoordinate(r*cos, r*sin);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(Ravg * cos, Ravg * sin);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(_R * cos, _R * sin);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(_R, 0);
                        break;
                    case "f":
                        point1_sh = ConvertXYtoCoordinate(_R * cos, -_R * sin);
                        break;
                    case "g":
                        point1_sh = ConvertXYtoCoordinate(Ravg * cos, -Ravg * sin);
                        break;
                    case "h":
                        point1_sh = ConvertXYtoCoordinate(r * cos, -r * sin);
                        break;
                    case "i":
                        point1_sh = ConvertXYtoCoordinate(r,0);
                        break;
                    case "cg":
                        double cgx = (2 * (Math.Pow(_R,3)-Math.Pow(r,3)) * sin ) / (3 * alpha * (_R * _R - r * r));
                        point1_sh = ConvertXYtoCoordinate(cgx, 0);
                        break;
                }

                return point1_sh;

            }


        }
    }
}

