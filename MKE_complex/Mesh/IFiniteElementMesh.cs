using MKE_complex.FiniteElements;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public interface IFiniteElementMesh<VectorT> where VectorT : VectorBase<double, VectorT>
{
    ReadOnlySpan<VectorT> Vertices { get; }
    ReadOnlySpan<IFiniteElement<VectorT>> Elements { get; }
    ReadOnlySpan<IBoundaryCondition<VectorT>> Boundaries { get; }
    int? DofsCount { get; set; }
    public void SortElementsByMinimumVertexNumber();
}

// public interface IFiniteElementMesh3D : IFiniteElementMesh<Vector3D>
// {
//     new ReadOnlySpan<IFiniteElement3D> Elements { get; }
// }
