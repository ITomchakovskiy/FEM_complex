using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.interfaces;

public interface IFiniteElementGeometry<VectorT> where VectorT : IVector
{
    int[] VertexNumber { get; }
}
