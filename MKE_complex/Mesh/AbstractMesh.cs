using MKE_complex.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public abstract class AbstractMesh<VectorT> : IFiniteElementMesh<VectorT> where VectorT : IVector
{
    public required List<VectorT> Vertices { get; init; }
    IReadOnlyList<VectorT> IFiniteElementMesh<VectorT>.Vertices => Vertices;
    public required List<IFiniteElement<VectorT>> Elements { get; init; }
    IReadOnlyList<IFiniteElement<VectorT>> IFiniteElementMesh<VectorT>.Elements => Elements;


}
