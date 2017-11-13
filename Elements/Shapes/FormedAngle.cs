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
        public class FormedAngle : Shape
        {
            public override IsoMaterial Material { get; set; }
            public override double theta { get; set; }
            public override bool mirrorX { get; set; } = false;
            public override bool mirrorY { get; set; } = false;

            public override string point { get; set; }
            public override double xp { get; set; }
            public override double yp { get; set; }

            public static List<string> _pointlist = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "cg" };
            public override List<string> ShapePointList { get { return _pointlist; } }

            [Dimension("Width")]
            public double b1    { get; set; }
            [Dimension("Height")]
            public double b2    { get; set; }
            [Dimension("Thickness")]
            public double t     { get; set; }
            [Dimension("Inner Bend Radius")]
            public double r     { get; set; }

            
            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);

                int _b1 = (int)(b1 * SF);
                int _b2 = (int)(b2 * SF);
                int _t = (int)(t * SF);
                int _r = (int)(r * SF);
                int _R = (_r + _t);

                if(_r < 1) { _r = 1; } //_r must be at least one pixel
                if(_R < 1) { _R = 1; } //_R must also be at least one pixel (not an issue as long as t isn't super small)

                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(0, _R, 0, _b2);
                path.AddLine(0, _b2, _t, _b2);
                path.AddLine(_t, _b2, _t, _t + _r);
                path.AddArc(new System.Drawing.Rectangle(_t, _t, 2 * _r, 2 * _r), 180, 90);
                path.AddLine(_t + _r, _t, _b1, _t);
                path.AddLine(_b1, _t, _b1, 0);
                path.AddLine(_b1, 0, _R, 0);
                path.AddArc(new System.Drawing.Rectangle(1, 1, 2 * _R, 2 * _R), 270, -90);
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
                CircularArc bend = new CircularArc { Material = Material, r = r, t = t, phi = 90 * Math.PI/180, point = "j", xp = 0, yp = 0, theta = -(45 + 90) * Math.PI / 180 };
                Rectangle rec1 = new Rectangle { Material = Material, b = b1 - r - t, t = t, point = "a", xp = t + r, yp = 0, theta = 0 };
                Rectangle rec2 = new Rectangle { Material = Material, b = b2 - r - t, t = t, point = "c", xp = 0, yp = r + t, theta = 90 * Math.PI / 180 };

                Section FA = new Section();
                FA.AddShape(bend);
                FA.AddShape(rec1);
                FA.AddShape(rec2);

                return FA.CalculateSecProp();

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
                        point1_sh = ConvertXYtoCoordinate(0, r + t);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(0, ((r+t)+(b2))/2);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(0, b2);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(t/2, b2); 
                        break;
                    case "f":
                        point1_sh = ConvertXYtoCoordinate(t, b2);
                        break;
                    case "g":
                        point1_sh = ConvertXYtoCoordinate(t, ((r + t) + (b2)) / 2);
                        break;
                    case "h":
                        point1_sh = ConvertXYtoCoordinate(t, t + r); 
                        break;
                    case "i":
                        point1_sh = ConvertXYtoCoordinate(t + r, t);
                        break;
                    case "j":
                        point1_sh = ConvertXYtoCoordinate(((t+r)+(b1))/2, t);
                        break;
                    case "k":
                        point1_sh = ConvertXYtoCoordinate(b1, t);
                        break;
                    case "l":
                        point1_sh = ConvertXYtoCoordinate(b1, t/2);
                        break;
                    case "m":
                        point1_sh = ConvertXYtoCoordinate(b1, 0);
                        break;
                    case "n":
                        point1_sh = ConvertXYtoCoordinate(((b1) + (t+r))/2, 0);
                        break;
                    case "o":
                        point1_sh = ConvertXYtoCoordinate(t+r, 0);
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

