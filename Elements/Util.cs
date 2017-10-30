using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public class EnvelopeCoords
    {
        public Coordinate Max { get; set; }
        public Coordinate Min { get; set; }

        public PlotProperties GetPlotProperties(double plotpadding)
        {
            PlotProperties pp = new PlotProperties
            {
                Xmin = Min.x - plotpadding,
                Xmax = Max.x + plotpadding,
                Ymin = Min.y - plotpadding,
                Ymax = Max.y + plotpadding
            };

            return pp;
        }
    }

    public partial class SectionElements
    {
                
        public static double[] RotatedElementProperties(double Ixx, double Iyy, double Ixy, double theta)
        {
            double Ixx_rotated = (Ixx + Iyy) / 2 + (Ixx - Iyy) / 2 * Math.Cos(-2 * theta) - Ixy * Math.Sin(-2 * theta);
            double Iyy_rotated = (Ixx + Iyy) / 2 - (Ixx - Iyy) / 2 * Math.Cos(-2 * theta) + Ixy * Math.Sin(-2 * theta);
            double Ixy_rotated = (Ixx - Iyy) / 2 * Math.Sin(-2 * theta) + Ixy * Math.Cos(-2 * theta);

            double[] ans = { Ixx_rotated, Iyy_rotated, Ixy_rotated };

            return ans;
        }

        public static double Theta_Principal(double Ixx, double Iyy, double Ixy)
        {
            double relief = 1E-256;
            if (Iyy == Ixx)
            {
                Iyy = Iyy + relief;
            }

            double rhs = 2 * Ixy / (Iyy - Ixx);
            double thetaP = Math.Atan(rhs) / 2;

            return thetaP;
        }

        public static double[] RotatedAxesProperties(double Ixx, double Iyy, double Ixy, double theta)
        {
            double Ixx_rotated = (Ixx + Iyy) / 2 + (Ixx - Iyy) / 2 * Math.Cos(2 * theta) - Ixy * Math.Sin(2 * theta);
            double Iyy_rotated = (Ixx + Iyy) / 2 - (Ixx - Iyy) / 2 * Math.Cos(2 * theta) + Ixy * Math.Sin(2 * theta);
            double Ixy_rotated = (Ixx - Iyy) / 2 * Math.Sin(2 * theta) + Ixy * Math.Cos(2 * theta);

            double[] ans = { Ixx_rotated, Iyy_rotated, Ixy_rotated };

            return ans;
        }

        public static SecProp SecProp_Section(List<SecProp> SecPropList)
        {
            double sumEA = 0, sumXcgEA = 0, sumYcgEA = 0;
            foreach (SecProp sp in SecPropList)
            {
                sumEA = sumEA + sp.EA;
                sumXcgEA = sumXcgEA + sp.EA * sp.Xcg;
                sumYcgEA = sumYcgEA + sp.EA * sp.Ycg;
            }
            double Xcg_global = sumXcgEA / sumEA;
            double Ycg_global = sumYcgEA / sumEA;


            double sumEIxx = 0, sumEIyy = 0, sumEIxy = 0;
            foreach (SecProp sp in SecPropList)
            {
                sumEIxx = sumEIxx + sp.EIxx + sp.EA * Math.Pow(sp.Ycg - Ycg_global, 2);
                sumEIyy = sumEIyy + sp.EIyy + sp.EA * Math.Pow(sp.Xcg - Xcg_global, 2);
                sumEIxy = sumEIxy + sp.EIxy + sp.EA * (sp.Xcg - Xcg_global) * (sp.Ycg - Ycg_global);
            }

            double thetap = Theta_Principal(sumEIxx, sumEIyy, sumEIxy);
            double[] EIPrincipal = RotatedAxesProperties(sumEIxx, sumEIyy, sumEIxy, thetap);
            double sumEI1p = EIPrincipal[0];
            double sumEI2p = EIPrincipal[1];

            SecProp SPGlobal = new SecProp
            {
                Xcg = Xcg_global,
                Ycg = Ycg_global,
                EA = sumEA,
                EIxx = sumEIxx,
                EIyy = sumEIyy,
                EIxy = sumEIxy,
                EI1p = sumEI1p,
                EI2p = sumEI2p,
                thetap = thetap
            };

            return SPGlobal;
        }

    }
}
