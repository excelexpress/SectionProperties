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
        public class MachinedAngle : Shape
        {
            public override IsoMaterial Material { get; set; }
            public override double theta { get; set; }
            public override bool mirrorX { get; set; } = false;
            public override bool mirrorY { get; set; } = false;

            public override string point { get; set; }
            public override double xp { get; set; }
            public override double yp { get; set; }

            public static List<string> _pointlist = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "cg" };
            public override List<string> ShapePointList { get { return _pointlist; } }

            [Dimension("Width")]
            public double b1    { get; set; }
            [Dimension("Horizontal Leg Thickness")]
            public double t1    { get; set; }
            [Dimension("Height")]
            public double b2    { get; set; }
            [Dimension("Vertical Leg Thickness")]
            public double t2    { get; set; }
            [Dimension("Fillet Radius")]
            public double r{ get; set; }

            
            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);

                int _b1 = (int)(b1 * SF);
                int _t1 = (int)(t1 * SF);
                int _b2 = (int)(b2 * SF);
                int _t2 = (int)(t2 * SF);
                int _r = (int)(r * SF);

                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(0, 0, 0, _b2);
                path.AddLine(0, _b2, _t2, _b2);
                path.AddLine(_t2, _b2, _t2, _t1 + _r);
                path.AddArc(new System.Drawing.Rectangle(_t2, _t1, 2 * _r, 2 * _r), 180, 90);
                path.AddLine(_t2 + _r, _t1, _b1, _t1);
                path.AddLine(_b1, _t1, _b1, 0);
                path.AddLine(_b1, 0, 0, 0);
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
                CircularFillet flt = new CircularFillet { Material = Material, r = r, point = "a", xp = t2, yp = t1, theta = 0 };
                Rectangle rec1 = new Rectangle { Material = Material, b = b1 - t2, t = t1, point = "a", xp = t2, yp = 0, theta = 0 };
                Rectangle rec2 = new Rectangle { Material = Material, b = b2, t = t2, point = "c", xp = 0, yp = 0, theta = 90 * Math.PI / 180 };

                Section MA = new Section();
                MA.AddShape(flt);
                MA.AddShape(rec1);
                MA.AddShape(rec2);

                return MA.CalculateSecProp();

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
                        point1_sh = ConvertXYtoCoordinate(0, b2 / 2);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(0, b2);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(t2, b2);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(t2, (b2 + t1 + r) /2); //average of d and f
                        break;
                    case "f":
                        point1_sh = ConvertXYtoCoordinate(t2, t1 + r);
                        break;
                    case "g":
                        point1_sh = ConvertXYtoCoordinate(t2 + r, t1);
                        break;
                    case "h":
                        point1_sh = ConvertXYtoCoordinate((t2 + r + b1)/2, t1); //average of g and i
                        break;
                    case "i":
                        point1_sh = ConvertXYtoCoordinate(b1, t1);
                        break;
                    case "j":
                        point1_sh = ConvertXYtoCoordinate(b1, 0);
                        break;
                    case "k":
                        point1_sh = ConvertXYtoCoordinate(b1 / 2, 0);
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

