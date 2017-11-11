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
        public class MachinedChannel : Shape
        {
            public override IsoMaterial Material { get; set; }
            public override double theta { get; set; }
            public override bool mirrorX { get; set; } = false;
            public override bool mirrorY { get; set; } = false;

            public override string point { get; set; }
            public override double xp { get; set; }
            public override double yp { get; set; }

            public static List<string> _pointlist = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "cg" };
            public override List<string> ShapePointList { get { return _pointlist; } }

            [Dimension("Width - Bottom Member")]
            public double b1 { get; set; }
            [Dimension("Thickness - Bottom Member")]
            public double t1 { get; set; }
            [Dimension("Width - Top Member")]
            public double b2 { get; set; }
            [Dimension("Thickness - Top Member")]
            public double t2 { get; set; }
            [Dimension("Height")]
            public double h { get; set; }
            [Dimension("Vertical Thickness")]
            public double tw { get; set; }
            [Dimension("Bottom Fillet Radius")]
            public double r1 { get; set; }
            [Dimension("Top Fillet Radius")]
            public double r2 { get; set; }


            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);

                int _b1 = (int)(b1 * SF);
                int _t1 = (int)(t1 * SF);
                int _b2 = (int)(b2 * SF);
                int _t2 = (int)(t2 * SF);
                int _r1 = (int)(r1 * SF);
                int _r2 = (int)(r2 * SF);
                int _h = (int)(h * SF);
                int _tw = (int)(tw * SF);

                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                if(_r1 < 1) { _r1 = 1; }
                if(_r2 < 1) { _r2 = 1; }

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(0, 0, 0, _h);
                path.AddLine(0, _h, _b2, _h);
                path.AddLine(_b2, _h, _b2, _h - _t2);
                path.AddLine(_b2, _h - _t2, _tw + _r2, _h - _t2);
                path.AddArc(new System.Drawing.Rectangle(_tw, _h - _t2 - _r2 * 2, _r2 * 2, _r2 * 2), 90, 90);
                path.AddLine(_tw, _h - _t2 - _r2, _tw, _t1 + _r1);
                path.AddArc(new System.Drawing.Rectangle(_tw, _t1, _r1 * 2, _r1 * 2), 180, 90);
                path.AddLine(_tw + _r1, _t1, _b1, _t1);
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
                CircularFillet flt1 = new CircularFillet { Material = Material, r = r1, point = "a", xp = tw, yp = t1, theta = 0 };
                CircularFillet flt2 = new CircularFillet { Material = Material, r = r2, point = "a", xp = tw, yp = h - t2, theta = 0, mirrorX = true };
                Rectangle rec1 = new Rectangle { Material = Material, b = b1 - tw, t = t1, point = "a", xp = tw, yp = 0, theta = 0 };
                Rectangle rec2 = new Rectangle { Material = Material, b = b2 - tw, t = t2, point = "c", xp = tw, yp = h, theta = 0 };
                Rectangle web = new Rectangle { Material = Material, b = h, t = tw, point = "c", xp = 0, yp = 0, theta = 90 * Math.PI / 180 };

                Section MA = new Section();
                MA.AddShape(flt1);
                MA.AddShape(flt2);
                MA.AddShape(rec1);
                MA.AddShape(rec2);
                MA.AddShape(web);

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
                        point1_sh = ConvertXYtoCoordinate(0, h / 2);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(0, h);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(b2/2, h);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(b2, h); 
                        break;
                    case "f":
                        point1_sh = ConvertXYtoCoordinate(b2, h - t2/2);
                        break;
                    case "g":
                        point1_sh = ConvertXYtoCoordinate(b2, h - t2);
                        break;
                    case "h":
                        point1_sh = ConvertXYtoCoordinate((b2 + tw + r2) / 2, h - t2); 
                        break;
                    case "i":
                        point1_sh = ConvertXYtoCoordinate(tw + r2, h - t2);
                        break;
                    case "j":
                        point1_sh = ConvertXYtoCoordinate(tw, h - t2 - r2);
                        break;
                    case "k":
                        point1_sh = ConvertXYtoCoordinate(tw, ((h - t2 - r2) + (t1 + r1))/2);
                        break;
                    case "l":
                        point1_sh = ConvertXYtoCoordinate(tw, t1 + r1);
                        break;
                    case "m":
                        point1_sh = ConvertXYtoCoordinate(tw + r1, t1);
                        break;
                    case "n":
                        point1_sh = ConvertXYtoCoordinate(((tw + r1) + (b1))/2, t1);
                        break;
                    case "o":
                        point1_sh = ConvertXYtoCoordinate(b1, t1);
                        break;
                    case "p":
                        point1_sh = ConvertXYtoCoordinate(b1, t1 / 2);
                        break;
                    case "q":
                        point1_sh = ConvertXYtoCoordinate(b1, 0);
                        break;
                    case "r":
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