using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements;

public interface IFiniteElement<VectorT> where VectorT : VectorBase<double, VectorT>
{
    IFiniteElementGeometry<VectorT> Geometry { get;}
    string Material { get; }
    int[] DOFs { get; }
    int[] SortedDofs { get; }
    int[] SortedDofIndices { get; }
    bool IsDofsConnected(int dof1, int dof2); // returns true if basis functions associated with global dof1 and dof2 are connected
    int DofsOnEdgeCount { get; }
    int DofsOnVertexCount { get; }
    int DofsOnElementCount { get; }
    void SetElementDofs(int startDofNumber);
    void SetVertexDofs(int localVertexNumber, int dofNumber);
    void SetVericesDofs(ReadOnlySpan<int> dofsNumbers);
    void SetEdgeDofs(int localEdgeNumber, int dofNumber);
    void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers);
    double[][] CalcLocalMatrix(VectorT[] vertices, Func<VectorT, double> Lambda, Func<VectorT, double> Gamma);
    double[] CalcLocalRightPart(VectorT[] vertices, Func<VectorT, double> F);
    double CalcResultAtPoint(VectorT[] vertices, ReadOnlySpan<double> localSolution, VectorT point);
    IFiniteElement<VectorT>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded);

    IFiniteElement<VectorT>[] Triangulate();


    //void SetDOFsOnVertices(int );
    //void SetDOFsOnEdges(int );
    //void SetDOFsOnElement();
}
