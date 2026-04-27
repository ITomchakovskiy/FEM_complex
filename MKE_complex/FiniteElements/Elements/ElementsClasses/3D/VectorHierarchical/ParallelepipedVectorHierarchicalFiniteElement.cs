using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.LocalMatrices._3D.VectorHierarchical.Cartesian;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._3D.VectorHierarchical;

[FiniteElementAttribute(GeometryType.Parallelepiped, BasisType.VectorHierarchical)]
public class ParallelepipedVectorHierarchicalFiniteElement : IFiniteElement3D, IFiniteElementVectorProblemCalculation<Vector3D>
{
    public ParallelepipedVectorHierarchicalFiniteElement(string material, Parallelepiped geometry, int order)
    {
        if (order < 1) throw new ArgumentException("");
        Material = material;
        this.geometry = geometry;
        Order = order;
        DOFs = new int[CalcDofsCount(Order)];

        sortedDofIndices = new Lazy<int[]>(()=>
        {
            var dofs = DOFs.ToArray();
            var indices = Enumerable.Range(0,DOFs.Length).ToArray();
            Array.Sort(dofs, indices);
            return indices;
        });
    }

    private Parallelepiped geometry;

    public IFiniteElementGeometry3D Geometry => geometry;

    IFiniteElementGeometry<Vector3D> IFiniteElement<Vector3D>.Geometry => Geometry;

    public int Order { get; }

    public string Material { get; }

    public int[] DOFs { get; private set; }

    private static int NewDofsOnEdgesCountForOrder(int order) => 12;
    private static int NewDofsOnFacesCountForOrder(int order) => 24 * (order - 1);
    private static int NewDofsOnElementCountForOrder(int order) => order * (9 * order - 21) + 12;
    private static int CalcDofsCount(int order) => 3 * (order + 1) * (order + 1) * order;

    public int DofsOnEdgeCount => Order;

    public int DofsOnFaceCount => 2 * (Order - 1) * Order;

    public int DofsOnVertexCount => 0;

    public int DofsOnElementCount => 3 * Order * (Order * (Order - 2) + 1);

    private Lazy<int[]> sortedDofIndices;

    public int[] SortedDofIndices => sortedDofIndices.Value;

    public int[] SortedDofs => SortedDofIndices.Select(i => DOFs[i]).ToArray();

    public bool IsDofsConnected(int dof1, int dof2)
    {
        throw new NotImplementedException();
    }

    private int[] IndexShiftForElementDOFS()
    {
        var res = new int[Order - 1];
        for( int i = 1; i < Order; ++i)
            res[i-1] = CalcDofsCount(i) + NewDofsOnEdgesCountForOrder(i + 1) + NewDofsOnFacesCountForOrder(i + 1);
        return res;
    }

    public void SetElementDofs(int startDofNumber)
    {
        var indices = IndexShiftForElementDOFS();
        int dofNumberShift = 0;
        for(int i = 0; i < indices.Length; ++i)
        {
            for(int j = 0; j < NewDofsOnElementCountForOrder(i + 2); ++j, ++dofNumberShift)
                DOFs[indices[i]+j] = startDofNumber + dofNumberShift;
        }
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        if(localVertexNumber >= geometry.VertexNumber.Length) throw new ArgumentOutOfRangeException();
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        ;
    }

    //private static int[] Parallelepiped

    private int[] IndexShiftForEdgeDOFS()
    {
        var res = new int[Order];
        for( int i = 1; i < Order; ++i)
            res[i] = CalcDofsCount(i);
        return res;
    }

    public void SetEdgeDofs(int localEdgeNumber, int dofNumber)
    {
        if(localEdgeNumber >= geometry.EdgesCount) throw new ArgumentOutOfRangeException();
        var shifts = IndexShiftForEdgeDOFS();
        var dofIndex = Parallelepiped.LocalEdgeNumToLocalEdgeNumForVectorHierarchicalBasis[localEdgeNumber];
        for(int i = 0; i < shifts.Length; ++i)
            DOFs[dofIndex + shifts[i]] = dofNumber + i;
    }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        throw new NotSupportedException();
        // if(dofsNumbers.Length != Geometry.EdgesCount * DofsOnEdgeCount) throw new ArgumentOutOfRangeException();
        // for(int i = 0; i < dofsNumbers.Length; ++i)
        //     SetEdgeDofs(i, dofsNumbers[i]);
    }

    private int[] IndexShiftForFaceDOFS()
    {
        var res = new int[Order - 1];
        for( int i = 1; i < Order; ++i)
            res[i-1] = CalcDofsCount(i) + NewDofsOnEdgesCountForOrder(i + 1);
        return res;
    }

    public void SetFaceDofs(int localFaceNumber, int[] baseVertices, int dofNumber)
    {
        if(localFaceNumber >= geometry.FacesCount) throw new ArgumentOutOfRangeException();
        var shifts = IndexShiftForFaceDOFS();
        var FacedofIndex = Parallelepiped.LocalFaceNumToLocalFaceNumForVectorHierarchicalBasis[localFaceNumber];
        int dofShift = 0;
        for(int i = 0; i < shifts.Length; ++i)
        {
            int dofsOnFaceCount = NewDofsOnFacesCountForOrder(i + 2) / geometry.FacesCount;
            for(int j = 0; j < dofsOnFaceCount ; ++j, ++dofShift)
                DOFs[shifts[i] + FacedofIndex * dofsOnFaceCount + j] = dofNumber + dofShift;
        }
    }

    public IFiniteElement<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        throw new NotImplementedException();
    }

    public double[][] CalcLocalMatrix(Vector3D[] vertices, Func<Vector3D, double> Mu, Func<Vector3D, double> Gamma)
    {
        var MuAvg = vertices.Average(i => Mu(i));
        var GamAvg = vertices.Average(i => Gamma(i));

        var H = Parallelepiped.CalcH(vertices);

        var M = ParallelepipedVectorHierarchicalCartesianLocalMatrices.CalculateLocalMassMatrix(Order, GamAvg, H.X, H.Y, H.Z);
        var G = ParallelepipedVectorHierarchicalCartesianLocalMatrices.CalculateLocalStiffnessMatrix(Order, MuAvg, H.X, H.Y, H.Z);

        var res = M;

        for(int i = 0; i < res.Length; ++i)
        {
            for(int j = 0; j <= i; ++j)
                res[i][j] += G[i][j];
        }
        return res;
    }

    public double[] CalcLocalRightPart(Vector3D[] vertices, Func<Vector3D, Vector3D> F)
    {
        throw new NotImplementedException();
    }

    public double CalcResultAtPoint(Vector3D[] vertices, ReadOnlySpan<double> localSolution, Vector3D point)
    {
        throw new NotImplementedException();
    }
}