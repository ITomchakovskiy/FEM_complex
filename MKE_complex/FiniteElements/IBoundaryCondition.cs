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
    string Material { get; }
    int Order {get;}
    int[] DOFs { get; }
    int[] SortedDofs { get; }
    int[] SortedDofIndices { get; }
    //bool IsDofsConnected(int dof1, int dof2); // returns true if basis functions associated with global dof1 and dof2 are connected
    int DofsOnEdgeCount { get; }
    int DofsOnVertexCount { get; }
    void SetVertexDofs(int localVertexNumber, int dofNumber);
    void SetVericesDofs(ReadOnlySpan<int> dofsNumbers);
    void SetEdgeDofs(int localEdgeNumber, int dofNumber);
    void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers);
    
    IBoundaryCondition<VectorT>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices);
}

public interface IBoundaryCondition3D : IBoundaryCondition<Vector3D>
{
    public new IFiniteElementGeometry3D Geometry { get; }
    int DofsOnFaceCount { get; }
    void SetFaceDofs(int[] baseVerices, int dofNumber);
}

public interface IBoundaryConditionScalarEllipticProblemCalculation<VectorT> where VectorT : VectorBase<double, VectorT>
{
    double[][] CalcLocalMatrixForRobinCondition(VectorT[] vertices, Func<VectorT, double> Beta);
    double[] CalcLocalRightPartForNeumannCondition(VectorT[] vertices, Func<VectorT, double> Theta);
    double[] CalcLocalRightPartForRobinCondition(VectorT[] vertices, Func<VectorT, double> Beta, Func<VectorT, double> UBeta);
    double[] CalcLocalRightPartForDirichletCondition(VectorT[] vertices, Func<VectorT, double> Ug);
}

public interface IBoundaryConditionVectorEllipticProblemCalculation<VectorT> where VectorT : VectorBase<double, VectorT>
{
    double[][] CalcLocalMatrixForDirichletCondition(ReadOnlySpan<VectorT> vertices);
    double[] CalcLocalRightPart(ReadOnlySpan<VectorT> vertices, Func<VectorT, VectorT> Ag);
    void SetDofs(ReadOnlySpan<int> newDofs);
}