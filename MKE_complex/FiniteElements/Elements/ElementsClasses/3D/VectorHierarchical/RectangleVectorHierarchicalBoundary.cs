using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Lagrangian;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
using MKE_complex.FiniteElements.Elements.LocalMatrices._2D.VectorHierarchical.Cartesian;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.ElementsClasses._3D.VectorHierarchical;

[FiniteElement(GeometryType.Rectangle, BasisType.VectorHierarchical)]
public class RectangleVectorHierarchicalBoundary : IBoundaryCondition3D, IBoundaryConditionVectorEllipticProblemCalculation<Vector3D>
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

    private int NewDofsOnEdgesCountForOrder(int order) => 4;

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

    public void SetVericesDofs(ReadOnlySpan<int> dofsNumbers) {}

    public void SetVertexDofs(int localVertexNumber, int dofNumber) {}

    public void SetDofs(ReadOnlySpan<int> newDofs) => DOFs = newDofs.ToArray();

    public double[][] CalcLocalMatrixForDirichletCondition(ReadOnlySpan<Vector3D> vertices)
    {
        var H = Rectangle<Vector3D>.CalcH(vertices);
        Vector2D HXY;

        if(Math.Abs(H.X) < 1.0E-15) HXY = new(H.Y,H.Z);
        else if(Math.Abs(H.Y) < 1.0E-15) HXY = new(H.X,H.Z);
        else if(Math.Abs(H.Z) < 1.0E-15) HXY = new(H.X,H.Y);
        else throw new ArgumentException();

        var matrix = RectangleVectorHierarchicalCartesianLocalMatrices.CalcLocalMassMatrix(Order,1d,HXY.X,HXY.Y);

        return matrix;
    }

    private static int[] nonZeroBasisComponentsIndices = [1,1,0,0,1,1,0,0,1,1,0,0];

    private double[][] GetLocalCoordinatesForLagrangianDofs()
    {
        int N = (Order + 1)*(Order + 1); //scalar lagrangian dofs count
        var res = new double[N][];
        for(int i = 0; i < res.Length; ++i)
            res[i] = new double[2];
        double[] values = new double[Order + 1];
        for(int i = 1; i < values.Length - 1;++i)
            values[i] = (double)i/(double)Order;
        values[^1] = 1d;
        for(int i = 0; i < N; i++)
        {
            res[i][0] = values[RectangleScalarLagrangianBases.LocalXDofNum(i,Order)];
            res[i][1] = values[RectangleScalarLagrangianBases.LocalYDofNum(i,Order)];
        }
        return res;
    }

    public double[] CalcLocalRightPart(ReadOnlySpan<Vector3D> vertices, Func<Vector3D, Vector3D> Ag)
    {
        var verticesArray = vertices.ToArray();
        var LagrangianDofsVertices = GetLocalCoordinatesForLagrangianDofs().Select(i => 
                                        RectangleLocalCoordinates.LocalCoordinatesToGlobal(verticesArray,(i[0],i[1]))).ToArray();

        var H = Rectangle<Vector3D>.CalcH(vertices);

        int zeroCoordNum;
        if(Math.Abs(H.X) < 1.0E-15) zeroCoordNum = 0;
        else if(Math.Abs(H.Y) < 1.0E-15) zeroCoordNum = 1;
        else if(Math.Abs(H.Z) < 1.0E-15) zeroCoordNum = 2;
        else throw new ArgumentException();

        double[][] weights = new double[2][];
        for(int i = 0;i < weights.Length; ++i)
            weights[i] = new double[LagrangianDofsVertices.Length];

        for(int i = 0; i < LagrangianDofsVertices.Length; ++i)
        {
            var Agvalue = Ag(LagrangianDofsVertices[i]);
            _ = zeroCoordNum switch
            {
                0 => weights[0][i] = Agvalue.Y,
                _ => weights[0][i] = Agvalue.X
            };
            _ = zeroCoordNum switch
            {
                2 => weights[1][i] = Agvalue.Y,
                _ => weights[1][i] = Agvalue.Z,
            };
        }
        
        Vector2D HXY = zeroCoordNum switch
        {
            0 => new(H.Y,H.Z),
            1 => new(H.X,H.Z),
            2 => new(H.X,H.Y),
            _ => throw new Exception()
        };

        var M = RectangleVectorHierarchical_LagrangianCartesianLocalMatrices.CalcLocalMassMatrix(Order, HXY.X, HXY.Y);

        var Indices = nonZeroBasisComponentsIndices;

        var res = new double[DOFs.Length];

        for(int i = 0; i < res.Length; ++i)
        {
            for(int j = 0; j < M[i].Length; ++j)
                res[i] += weights[Indices[i]][j] * M[i][j];
        }
        return res;
    }
}