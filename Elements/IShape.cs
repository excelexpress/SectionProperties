using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public interface IShape
    {
        EnvelopeCoords GetEnvelopeCoord();
        SecProp CalculateSecProp();
        void Draw(ref Bitmap bitmap, PlotProperties plotprops);
    }
}
