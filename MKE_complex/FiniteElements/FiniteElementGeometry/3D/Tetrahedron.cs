using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Tetrahedron(int[] VertexNumber) : IFiniteElementGeometry3D
{
    public GeometryType GeometryType => GeometryType.Tetrahedron;

    public static int VertexCountS => 4;

    public static int EdgesCountS => 6;
    public int EdgesCount => EdgesCountS;

    public static int FacesCountS => 4;
    public int FacesCount => FacesCountS;

    public bool IsPointInElement(Vector3D point, Vector3D[] vertices)
    {
        var Alpha = TetrahedronLocalCoordinates.Alpha.CalcAlphas(vertices);
        var LocalCoordinates = TetrahedronLocalCoordinates.LocalCoordinates.Select(f => f(point, Alpha));
        return LocalCoordinates.All(l => l > -1.0E-15);
    }

    public static (int, int) LocalEdgeS(int edgeNumber)
    {
        return edgeNumber switch
        {
            0 => (0,1),
            1 => (0,2),
            2 => (0,3),
            3 => (1,2),
            4 => (1,3),
            5 => (2,3),
            _=> throw new ArgumentOutOfRangeException()
        };
    }

    public (int, int) LocalEdge(int edgeNumber) => LocalEdgeS(edgeNumber);

    public IFiniteElementGeometry<Vector3D>[] Refine(ReadOnlySpan<int> FaceVertices, ReadOnlySpan<int> EdgeVertices, int ElementVertex, out bool IsElementVertexNeeded)
    {
        IsElementVertexNeeded = false;
        int[][] VertexNumbers = [[VertexNumber[0], EdgeVertices[0], EdgeVertices[1], EdgeVertices[2]],
                                 [VertexNumber[1], EdgeVertices[0], EdgeVertices[3], EdgeVertices[4]],
                                 [VertexNumber[2], EdgeVertices[1], EdgeVertices[3], EdgeVertices[5]],
                                 [VertexNumber[3], EdgeVertices[2], EdgeVertices[4], EdgeVertices[5]],
                                 [EdgeVertices[0], EdgeVertices[1], EdgeVertices[2], EdgeVertices[3]],
                                 [EdgeVertices[0], EdgeVertices[2], EdgeVertices[3], EdgeVertices[4]],
                                 [EdgeVertices[1], EdgeVertices[2], EdgeVertices[3], EdgeVertices[5]],
                                 [EdgeVertices[2], EdgeVertices[3], EdgeVertices[4], EdgeVertices[5]]];
        
        return [.. VertexNumbers.Select(i => new Tetrahedron(i))];
    }

    public (int, int) GlobalEdge(int edgeNumber)
    {
        var local = LocalEdge(edgeNumber);
        return (VertexNumber[local.Item1], VertexNumber[local.Item2]);
    }

    public static int[] LocalFaceS(int faceNumber)
    {
        return faceNumber switch
        {
            0 => [0,1,2],
            1 => [0,1,3],
            2 => [0,2,3],
            3 => [1,2,3],
            _=> throw new ArgumentOutOfRangeException()
        };
    }

    public int[] LocalFace(int faceNumber) => LocalFaceS(faceNumber);

    public int[] GlobalFace(int faceNumber)
    {
        var local = LocalFace(faceNumber);
        return [.. local.Select(i => VertexNumber[i])];
    }
}
