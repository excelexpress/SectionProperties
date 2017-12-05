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
        public class AngledFillet : Shape
        {
            public override IsoMaterial Material { get; set; }
            public override double theta { get; set; }
            public override bool mirrorX { get; set; } = false;
            public override bool mirrorY { get; set; } = false;

            public override string point { get; set; }
            public override double xp { get; set; }
            public override double yp { get; set; }

            public static List<string> _pointlist = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",  "s", "t", "u", "v", "cg" };
            public override List<string> ShapePointList { get { return _pointlist; } }

            [Dimension("Lower Width")]
            public double b1    { get; set; }
            [Dimension("Upper Width")]
            public double b2    { get; set; }
            [Dimension("Height")]
            public double h     { get; set; }
            [Dimension("Thickness")]
            public double t     { get; set; }
            [Dimension("Inner Bend Radius - Lower")]
            public double r1    { get; set; }
            [Dimension("Inner Bend Radius - Upper")]
            public double r2 { get; set; }


            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);

                int _b1 = (int)(b1 * SF);
                int _b2 = (int)(b2 * SF);
                int _t = (int)(t * SF);
                int _r1 = (int)(r1 * SF);
                int _R1 = (_r1 + _t);
                int _r2 = (int)(r2 * SF);
                int _R2 = (_r2 + _t);
                int _h = (int)(h * SF);

                _r1 = Math.Max(1, _r1);
                _r2 = Math.Max(1, _r2);
                _R1 = Math.Max(1, _R1);
                _R2 = Math.Max(1, _R2);

                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(0, _R1, 0, _h - _R2);
                path.AddArc(new System.Drawing.Rectangle(0, _h - 2 * _R2, 2 * _R2, 2 * _R2), 180, -90);
                path.AddLine(_R2, _h, _b2, _h);
                path.AddLine(_b2, _h, _b2, _h - _t);
                path.AddLine(_b2, _h - _t, _R2, _h - _t);
                path.AddArc(new System.Drawing.Rectangle(_t, _h - _t - 2 * _r2, 2 * _r2, 2 * _r2), 90, 90);
                path.AddLine(_t, _h - _R2, _t, _R1);
                path.AddArc(new System.Drawing.Rectangle(_t, _t, 2 * _r1, 2 * _r1), 180, 90);
                path.AddLine(_R1, _t, _b1, _t);
                path.AddLine(_b1, _t, _b1, 0);
                path.AddLine(_b1, 0, _R1, 0);
                path.AddArc(new System.Drawing.Rectangle(0, 0, 2 * _R1, 2 * _R1), 270, -90);
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
                CircularArc bendl = new CircularArc { Material = Material, r = r1, t = t, phi = 90 * Math.PI/180, point = "j", xp = 0, yp = 0, theta = -(45 + 90) * Math.PI / 180 };
                CircularArc bendu = new CircularArc { Material = Material, r = r2, t = t, phi = 90 * Math.PI / 180, point = "j", xp = 0, yp = h, theta = -(45 + 90 + 90) * Math.PI / 180 };
                Rectangle recl = new Rectangle { Material = Material, b = b1 - r1 - t, t = t, point = "a", xp = t + r1, yp = 0, theta = 0 };
                Rectangle recu = new Rectangle { Material = Material, b = b2 - r2 - t, t = t, point = "c", xp = t + r2, yp = h, theta = 0 };
                Rectangle vert = new Rectangle { Material = Material, b = h - r1 - r2 - 2*t, t = t, point = "c", xp = 0, yp = r1 + t, theta = 90 * Math.PI / 180 };

                Section FC = new Section();
                FC.AddShape(bendl);
                FC.AddShape(bendu);
                FC.AddShape(recl);
                FC.AddShape(recu);
                FC.AddShape(vert);

                return FC.CalculateSecProp();

            }


            protected override Coordinate LocalPointCoordinate(string PointID)
            {
                //Returns the coordinate of a defined point 
                //assumes zero rotation and translation
                Coordinate point1_sh = new Coordinate();

                SecProp sp = ShapeSecProp();
                double x_cg = sp.Xcg;
                double y_cg = sp.Ycg;

                switch (PointID)
                {
                    case "a":
                        point1_sh = ConvertXYtoCoordinate(0, 0);
                        break;
                    case "b":
                        point1_sh = ConvertXYtoCoordinate(0, r1 + t);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(0, ((r1+t)+(h - r2 - t))/2);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(0, h - r2 - t);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(0, h); 
                        break;
                    case "f":
                        point1_sh = ConvertXYtoCoordinate(t+r2, h);
                        break;
                    case "g":
                        point1_sh = ConvertXYtoCoordinate(((t+r2)+(b2))/2, h);
                        break;
                    case "h":
                        point1_sh = ConvertXYtoCoordinate(b2, h); 
                        break;
                    case "i":
                        point1_sh = ConvertXYtoCoordinate(b2, h - t/2);
                        break;
                    case "j":
                        point1_sh = ConvertXYtoCoordinate(b2, h - t);
                        break;
                    case "k":
                        point1_sh = ConvertXYtoCoordinate(((b2) + (t + r2))/2, h - t);
                        break;
                    case "l":
                        point1_sh = ConvertXYtoCoordinate(t + r2, h - t);
                        break;
                    case "m":
                        point1_sh = ConvertXYtoCoordinate(t, h - t - r2);
                        break;
                    case "n":
                        point1_sh = ConvertXYtoCoordinate(t, ((h - t - r2) + (t + r1))/2);
                        break;
                    case "o":
                        point1_sh = ConvertXYtoCoordinate(t, t + r1);
                        break;
                    case "p":
                        point1_sh = ConvertXYtoCoordinate(t+r1, t);
                        break;
                    case "q":
                        point1_sh = ConvertXYtoCoordinate(((t + r1) + (b1)) / 2, t);
                        break;
                    case "r":
                        point1_sh = ConvertXYtoCoordinate(b1, t);
                        break;
                    case "s":
                        point1_sh = ConvertXYtoCoordinate(b1, t / 2);
                        break;
                    case "t":
                        point1_sh = ConvertXYtoCoordinate(b1, 0);
                        break;
                    case "u":
                        point1_sh = ConvertXYtoCoordinate(((b1) + (t + r1))/2, 0);
                        break;
                    case "v":
                        point1_sh = ConvertXYtoCoordinate(t + r1, 0);
                        break;
                    case "cg":
                        point1_sh = ConvertXYtoCoordinate(x_cg, y_cg);
                        break;
                }

                return point1_sh;

            }

        }

        
    }
}

