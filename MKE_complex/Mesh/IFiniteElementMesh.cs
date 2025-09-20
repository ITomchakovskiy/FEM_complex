using MKE_complex.FiniteElements;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public interface IFiniteElementMesh<VectorT> where VectorT : IVector
{
    IReadOnlyList<VectorT> Vertices { get; }

    IReadOnlyList<IFiniteElement<VectorT>> Elements { get; }
}
