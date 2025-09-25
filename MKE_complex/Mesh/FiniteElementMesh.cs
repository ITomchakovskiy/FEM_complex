using MKE_complex.FiniteElements;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public class FiniteElementMesh<VectorT>(IReadOnlyList<VectorT> vertices, IReadOnlyList<IFiniteElement<VectorT>> elements) : IFiniteElementMesh<VectorT> where VectorT : IVector
{
    public List<VectorT> Vertices { get; init; } = (List<VectorT>)vertices;
    IReadOnlyList<VectorT> IFiniteElementMesh<VectorT>.Vertices => Vertices;
    public List<IFiniteElement<VectorT>> Elements { get; init; } = (List<IFiniteElement<VectorT>>)elements;
    IReadOnlyList<IFiniteElement<VectorT>> IFiniteElementMesh<VectorT>.Elements => Elements;

    //public Mesh<VectorT> RegularMesh(string meshFileName, string meshFragmentationFile)
    //{
        
    //}
}
