using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements;

public interface IBoundaryCondition<VectorT> where VectorT : VectorBase<double, VectorT>
{
    IFiniteElementGeometry<VectorT> Geometry { get; }
    string VolumeMaterial { get; }
    string EdgeMaterial { get; }
    int[] DOFs { get; }
    int[] SortedDofs { get; }
    int[] SortedDofIndices { get; }
    bool IsDofsConnected(int dof1, int dof2); // returns true if basis functions associated with global dof1 and dof2 are connected
    int DofsOnEdgeCount { get; }
    int DofsOnVertexCount { get; }
    void SetVertexDofs(int localVertexNumber, int dofNumber);
    void SetVericesDofs(ReadOnlySpan<int> dofsNumbers);
    void SetEdgeDofs(int localEdgeNumber, int dofNumber);
    void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers);
}