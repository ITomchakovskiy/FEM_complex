using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements;

public interface IFiniteElementEdge<VectorT> where VectorT : VectorBase
{
    IFiniteElementGeometry<VectorT> Geometry { get; }

    string Material { get; }
}
