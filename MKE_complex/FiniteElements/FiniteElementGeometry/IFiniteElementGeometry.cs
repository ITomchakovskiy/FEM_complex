using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry;

public interface IFiniteElementGeometry<VectorT> where VectorT : VectorBase
{
   // GeometryType GeometryType { get; }
    int[] VertexNumber { get; }

    int EdgesCount { get; }

    (int, int) LocalEdge(int edgeNumber);
}
