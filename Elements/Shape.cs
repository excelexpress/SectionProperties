using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public abstract class Shape : IShape
    {
        /*
          Main advantage to using abstract class instead of interface is 
          we can create a collection of type Shape ie List<Shape> 
          allowing enumerating a little easier

          using abstract methods forces those methods to be utilized in the
          classes inherited from the base class
        */
                        
        public abstract string point { get; set; }
        public abstract double xp { get; set; }
        public abstract double yp { get; set; }
        public abstract IsoMaterial Material { get; set; }
        public abstract double theta { get; set; }
        public virtual bool mirrorX { get; set; } = false;
        public virtual bool mirrorY { get; set; } = false;
        public abstract List<string> ShapePointList { get;}


        protected Color borderColor = Color.Black;
        protected int borderThickness = (int)2;


        public virtual SecProp CalculateSecProp()
        {
            double E = Material.E;
            double _E = E;

            if (E == 0)
            {
                _E = 1E-10;
            }
            SecProp SP_shape = ShapeSecProp();

            //find center of gravity
            Coordinate cg_global = GetGlobalCGcoord();
            double xcg = cg_global.x;
            double ycg = cg_global.y;

            //Shape Section Properties (before rotation)
            double Ixx_sh = SP_shape.EIxx / _E;
            double Iyy_sh = SP_shape.EIyy / _E;
            double Ixy_sh = SP_shape.EIxy / _E;

            //adjust for shape mirroring (if both are true then Ixy_l is positive again)
            if (mirrorX)
            {
                Ixy_sh = -1 * Ixy_sh;
            }

            if (mirrorY)
            {
                Ixy_sh = -1 * Ixy_sh;
            }

            //Local Section Properties (after rotation)
            double[] Irotated = SectionElements.RotatedElementProperties(Ixx_sh, Iyy_sh, Ixy_sh, theta);
            double Ixx_l = Irotated[0];
            double Iyy_l = Irotated[1];
            double Ixy_l = Irotated[2];


            double thetap = SectionElements.Theta_Principal(Ixx_l, Iyy_l, Ixy_l);
            double[] Iprincipal = SectionElements.RotatedAxesProperties(Ixx_l, Iyy_l, Ixy_l, thetap);
            double I1p = Iprincipal[0];
            double I2p = Iprincipal[1];

            double A = SP_shape.EA / _E;

            SecProp sp = new SecProp
            {
                Xcg = xcg,
                Ycg = ycg,
                EA = E * A,
                EIxx = E * Ixx_l,
                EIyy = E * Iyy_l,
                EIxy = E * Ixy_l,
                EI1p = E * I1p,
                EI2p = E * I2p,
                thetap = thetap
            };

            return sp;
        }

        protected abstract SecProp ShapeSecProp();

        public abstract void Draw(ref Bitmap bitmap, PlotProperties plotprops);

        protected virtual PointF CreateImagePoint(double ScaleFactor)
        {
            //converts real coordinates to Image coordinate (assuming point a is at 0,0)
            PointF pnt;

            Coordinate coord = LocalPointCoordinate(point);
            pnt = new PointF((float)(coord.x * ScaleFactor), (float)(coord.y * ScaleFactor));

            return pnt;
        }

        public virtual void FlipX()
        {
            mirrorX = (mirrorX == false) ? true : false;
        }

        public virtual void FlipY()
        {
            mirrorY = (mirrorY == false) ? true : false;
        }
        
        public virtual Coordinate GlobalPointCoordinate(string PointID)
        {
            //Returns the coordinate of defined point
            //takes into account translation (defined by properties xp & yp)
            //and rotation (defined by property theta)
            Coordinate pnt1_shp = LocalPointCoordinate(point);
            Coordinate pnt2_shp = LocalPointCoordinate(PointID);

            Coordinate point1Coordinate = SectionElements.ConvertXYtoCoordinate(xp, yp);

            Coordinate pnt2_global = SectionElements.TransformCoordinate(point1Coordinate, pnt1_shp, pnt2_shp, theta, mirrorX, mirrorY);

            return pnt2_global;
        }

        protected abstract Coordinate LocalPointCoordinate(string PointID);
        
        public virtual Coordinate GetGlobalCGcoord()
        {
            return GlobalPointCoordinate("cg");
        }

        public virtual EnvelopeCoords GetEnvelopeCoord()
        {
            Coordinate gpc = new Coordinate();
            double xmax = -1E+256;
            double xmin = 1E+256;
            double ymax = -1E+256;
            double ymin = 1E+256;

            foreach (string mypoint in ShapePointList)
            {
                gpc = GlobalPointCoordinate(mypoint);
                xmax = (xmax > gpc.x) ? xmax : gpc.x;
                xmin = (xmin < gpc.x) ? xmin : gpc.x;
                ymax = (ymax > gpc.y) ? ymax : gpc.y;
                ymin = (ymin < gpc.y) ? ymin : gpc.y;
            }

            EnvelopeCoords Ecoords = new EnvelopeCoords();

            Ecoords.Max = new Coordinate { x = xmax, y = ymax };
            Ecoords.Min = new Coordinate { x = xmin, y = ymin };

            return Ecoords;

        }

    }
}
