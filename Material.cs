using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    //public abstract class Material { };

    public class IsoMaterial
    {
        public string Name { get; set; }

        private double _E;
        public double E
        {
            get
            {
                return _E;
            }

            set
            {
                if (value == 0)
                {
                    _E = 1E-256;
                }
                else
                {
                    _E = value;
                }
            }
        }

        public double G { get; set; }
        public double v { get; set; }
        public string MaterialColor_Hex { get; set; }

        

        public Color ConvertFillColor()
        {
            Color _fillColor = ColorTranslator.FromHtml(MaterialColor_Hex);
            return _fillColor;
        }
    }

    public class OrthoMaterial
    {
        public string Name { get; set; }
        public double E1 { get; set; }
        public double E2 { get; set; }
        public double G12 { get; set; }
        public double v12 { get; set; }
        public string MaterialColor_Hex { get; set; }

        public Color ConvertFillColor()
        {
            Color _fillColor = ColorTranslator.FromHtml(MaterialColor_Hex);
            return _fillColor;
        }
    }

    public class Lamina
    {
        public OrthoMaterial Material { get; set; }
        public double theta { get; set; }
        public double thickness { get; set; }

        public double CalculateEx()
        {
            double Ex = 0;
            return Ex;
        }

        public double CalculateEy()
        {
            double Ey = 0;
            return Ey;
        }

        public double CalculateGxy()
        {
            double Gxy = 0;
            return Gxy;
        }

        public double Calculatevxy()
        {
            double vxy = 0;
            return vxy;
        }

        public double Calculatevyx()
        {
            double vyx = 0;
            return vyx;
        }

        public IsoMaterial GetEffectiveIsoMaterial()
        {
            IsoMaterial isomat = new IsoMaterial();

            isomat.Name = Material.Name + "Lamina";
            isomat.E = CalculateEx();
            isomat.G = CalculateGxy();
            isomat.v = Calculatevxy();
            isomat.MaterialColor_Hex = Material.MaterialColor_Hex;
            
            return isomat;
        }

    }

    public class Laminate
    {
        List<Lamina> LaminaList;

        public Laminate()
        {
            LaminaList = new List<Lamina>();
        }

        public void AddLamina(Lamina lamina)
        {
            LaminaList.Add(lamina);
        }
    }
    
}
