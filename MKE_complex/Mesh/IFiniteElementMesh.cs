using MKE_complex.FiniteElements;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public interface IFiniteElementMesh<VectorT> where VectorT : VectorBase
{
    ReadOnlySpan<VectorT> Vertices { get; }

    ReadOnlySpan<IFiniteElement<VectorT>> Elements { get; }

    ReadOnlySpan<IBoundaryCondition<VectorT>> Boundaries { get; }

    public void SortElementsByMinimumVertexNumber();
}
