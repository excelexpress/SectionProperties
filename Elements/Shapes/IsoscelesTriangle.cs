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
        public class IsoscelesTriangle : Shape
        {
            public override IsoMaterial Material { get; set; }
            public override double theta { get; set; }
            public override bool mirrorX { get; set; } = false;
            public override bool mirrorY { get; set; } = false;

            public override string point { get; set; }
            public override double xp { get; set; }
            public override double yp { get; set; }

            public static List<string> _pointlist = new List<string>() { "a", "b", "c", "d", "e", "f", "cg" };
            public override List<string> ShapePointList { get { return _pointlist; } }

            [Dimension("Width")]
            public double b    { get; set; }
            [Dimension("Height")]
            public double h     { get; set; }


            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);

                int _b = (int)(b * SF);
                int _h = (int)(h * SF);

                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(0, 0, (float)(-_b / 2), 0);
                path.AddLine((float)(-_b / 2), 0, 0, _h);
                path.AddLine(0, _h, (float)(_b / 2), 0);
                path.AddLine((float)(_b / 2), 0, 0, 0);
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
                RightTriangle rt1 = new RightTriangle { Material = Material, b = b / 2, h = h, point = "a", xp = 0, yp = 0, theta = 0 };
                RightTriangle rt2 = new RightTriangle { Material = Material, b = b / 2, h = h, point = "a", xp = 0, yp = 0, theta = 0, mirrorY = true };

                Section IsoTri = new Section();
                IsoTri.AddShape(rt1);
                IsoTri.AddShape(rt2);

                return IsoTri.CalculateSecProp();

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
                        point1_sh = ConvertXYtoCoordinate(-b / 2, 0);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(-b / 4, h / 2);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(0, h);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(b / 4, h / 2);
                        break;
                    case "f":
                        point1_sh = ConvertXYtoCoordinate(b / 2, 0);
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

