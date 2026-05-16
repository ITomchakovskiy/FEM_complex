using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry;

public interface IFiniteElementGeometry<VectorT> where VectorT : VectorBase<double, VectorT>
{
   // GeometryType GeometryType { get; }
    int[] VertexNumber { get; }

    int EdgesCount { get; }

    (int, int) LocalEdge(int edgeNumber);

    (int, int) GlobalEdge(int edgeNumber);

    bool IsPointInElement(VectorT point, VectorT[] vertices);

    IFiniteElementGeometry<VectorT>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded);
}

public interface IFiniteElementGeometry3D : IFiniteElementGeometry<Vector3D>
{
    int FacesCount { get; }
    int[] LocalFace(int faceNumber);
    int[] GlobalFace(int faceNumber);
}
