using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._3D.VectorHierarchical;

public class RectangleVectorHierarchicalBoundary : IBoundaryCondition3D
{
    public RectangleVectorHierarchicalBoundary(string material, RectangleBoundary geometry, int order)
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

    private static int CalcDofsCount(int order) => 2 * order * (order + 1);
    private RectangleBoundary geometry;
    public IFiniteElementGeometry3D Geometry => geometry;

    public int DofsOnFaceCount => 2 * (Order - 1) * Order;

    public string Material {get;}

    public int[] DOFs {get; private set;}

    public int[] SortedDofs => SortedDofIndices.Select(i => DOFs[i]).ToArray();

    private Lazy<int[]> sortedDofIndices;

    public int[] SortedDofIndices => sortedDofIndices.Value;

    public int DofsOnEdgeCount => Order;

    public int DofsOnVertexCount => 0;

    IFiniteElementGeometry<Vector3D> IBoundaryCondition<Vector3D>.Geometry => Geometry;

    public int Order {get;}

    public IBoundaryCondition<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices)
    {
        throw new NotImplementedException();
    }

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
        var dofIndex = RectangleBoundary.LocalEdgeNumToLocalEdgeNumForVectorHierarchicalBasis[localEdgeNumber];
        for(int i = 0; i < shifts.Length; ++i)
            DOFs[dofIndex + shifts[i]] = dofNumber + i;
    }

    public void SetEdgesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        throw new NotSupportedException();
    }

    private int NewDofsOnEdgesCountForOrder(int order) => 12;

    private int[] IndexShiftForFaceDOFS()
    {
        var res = new int[Order - 1];
        for( int i = 1; i < Order; ++i)
            res[i-1] = CalcDofsCount(i) + NewDofsOnEdgesCountForOrder(i + 1);
        return res;
    }

    private int NewDofsOnFacesCountForOrder(int order) => 4 * (order - 1);

    public void SetFaceDofs(int[] baseVerices, int dofNumber)
    {
        var shifts = IndexShiftForFaceDOFS();
        int dofShift = 0;
        for(int i = 0; i < shifts.Length; ++i)
        {
            int dofsOnFaceCount = NewDofsOnFacesCountForOrder(i + 2) / geometry.FacesCount;
            for(int j = 0; j < dofsOnFaceCount / 2 ; ++j, ++dofShift)
                DOFs[shifts[i] + dofsOnFaceCount / 2 + j] = dofNumber + dofShift;
            for(int j = 0; j < dofsOnFaceCount / 2 ; ++j, ++dofShift)
                DOFs[shifts[i] + j] = dofNumber + dofShift;
        }
    }

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers)
    {
        ;
    }

    public void SetVertexDofs(int localVertexNumber, int dofNumber)
    {
        ;
    }
}