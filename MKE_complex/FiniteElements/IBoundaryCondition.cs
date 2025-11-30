using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements;

public interface IBoundaryCondition<VectorT> where VectorT : VectorBase<double>
{
    IFiniteElementGeometry<VectorT> Geometry { get; }
    string VolumeMaterial { get; }
    string EdgeMaterial { get; }
    int[] DOFs { get; }
    int DofsOnEdgeCount { get; }
    int DofsOnVertexCount { get; }
    void SetVertexDofs(int localVertexNumber, int dofNumber);
    void SetVericesDofs(ReadOnlySpan<int> dofsNumbers);
    void SetEdgeDofs(int localEdgeNumber, int dofNumber);
    void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers);
}