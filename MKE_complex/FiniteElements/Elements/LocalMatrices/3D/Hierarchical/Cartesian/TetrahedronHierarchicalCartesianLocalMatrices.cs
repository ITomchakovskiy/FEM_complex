using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;
using MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._3D.Hierarchical.Cartesian;
public class TetrahedronHierarchicalCartesianLocalMatrices
{
    private static string directory = "Scalar/Hierarchical";
    private static string MassMatrixFileName = "TetrahedronScalarHierarchicalCubicMassMatrix";
    private static string Hierarchical_LagrangianLinearMassMatrixFileName = "TetrahedronScalarHierarchical_LagrangianLinearMassMatrix";
    private static string Hierarchical_LagrangianQuadraticMassMatrixFileName = "TetrahedronScalarHierarchical_LagrangianQuadraticMassMatrix";
    private static string Hierarchical_LagrangianCubicMassMatrixFileName = "TetrahedronScalarHierarchical_LagrangianCubicMassMatrix"; //"TriangleScalarHierarchicalSimpleMassMatrix";

    private static double[][] BaseM3;

    static TetrahedronHierarchicalCartesianLocalMatrices()
    {
        BaseM3 = MatrixReader.ReadMatrixFromFile(Path.Join(directory, MassMatrixFileName));
    }

    public static double[][] CalculateLocalMassMatrix(int order, double AbsdetD, double Coefficient, PolinomialType polinomial)
    {
        if(order > 3) throw new NotImplementedException();

        int N = TetrahedronHierarchicalBases.CalcDofsCount(order);
        var baseMatrix = BaseM3.Take(N);

        return [.. baseMatrix.Select(i => i.Select(j => j * AbsdetD * Coefficient).ToArray())];
    }

    public static double[][] CalculateLocalStiffnessMatrix(int order, double[][] Alpha, double AbsdetD, double Coefficient, PolinomialType polinomial)
    {
        var baseMatrix = order switch
        {
            1 => G1(Alpha),
            2 => G2(Alpha),
            3 => G3(Alpha),
            _ => throw new NotImplementedException()
        };

        return [.. baseMatrix.Select(i => i.Select(j => j * AbsdetD * Coefficient).ToArray())];
    }

    private static double[][] G1(double[][] Alpha)
    {
        int N = TetrahedronHierarchicalBases.CalcDofsCount(1);
        var G1 = new double[N][];
        for(int i = 0; i < N; ++i)
        {
            G1[i] = new double[i+1];
            for(int j = 0; j < G1[i].Length; ++j)
            {
                for(int k = 1; k <= 3; ++k)
                    G1[i][j] += Alpha[i][k] * Alpha[j][k];
                G1[i][j] /= 6d;
            }
        }
            
        return G1;
    }

    private static double[][] G2(double[][] Alpha)
    {
        int N = TetrahedronHierarchicalBases.CalcDofsCount(2);
        int N0 = TetrahedronHierarchicalBases.CalcDofsCount(1);
        var G2New = new double[N - N0][];

        for(int i = 0; i < N - N0 ; ++i)     //initialization
            G2New[i] = new double[N0 + i + 1];
        
        for(int i = 0; i < N - N0; ++i) //vertices to edges 
        {
            var edge = Tetrahedron.LocalEdgeS(i);
            for(int j = 0; j < N0; ++j)
            {
                for(int k = 1; k <= 3; ++k)
                    G2New[i][j] += Alpha[j][k] * (Alpha[edge.Item1][k] + Alpha[edge.Item2][k]);
                G2New[i][j] /= 24d;
            }
                
        }

        for(int i = 0; i < N - N0; ++i) //edges to edges, i =j
        {
            var edge = Tetrahedron.LocalEdgeS(i);
            for(int k = 1; k <= 3; ++k)
                G2New[i][N0 + i] += Alpha[edge.Item1][k] * Alpha[edge.Item1][k] + 
                                    Alpha[edge.Item1][k] * Alpha[edge.Item2][k] + 
                                    Alpha[edge.Item2][k] * Alpha[edge.Item2][k];
            G2New[i][N0 + i] /= 60d;
        }

        for(int i = 0; i < N - N0; ++i) //edges to edges
        {
            var edgeI = Tetrahedron.LocalEdgeS(i);
            for(int j = N0; j < i + N0; ++j)
            {
                var edgeJ = Tetrahedron.LocalEdgeS(j-N0);
                int commonVertex;
                (int, int) sideVertices;
                if     (edgeI.Item1 == edgeJ.Item1)
                {
                    commonVertex = edgeI.Item1;
                    sideVertices = (edgeI.Item2, 
                                    edgeJ.Item2);
                }
                else if(edgeJ.Item2 == edgeI.Item1)
                {
                    commonVertex = edgeI.Item1;
                    sideVertices = (edgeI.Item2, 
                                    edgeJ.Item1);
                }
                else if(edgeI.Item2 == edgeJ.Item2)
                {
                    commonVertex = edgeI.Item2;
                    sideVertices = (edgeI.Item1, 
                                    edgeJ.Item1);
                } 
                else  //no common vertices for edges
                {
                    for(int k = 1; k <= 3; ++k)
                        G2New[i][j] += (Alpha[edgeI.Item1][k] + Alpha[edgeI.Item2][k]) * 
                                       (Alpha[edgeJ.Item1][k] + Alpha[edgeJ.Item2][k]);
                    G2New[i][j] /= 120d;
                    continue;
                }

                for(int k = 1; k <= 3; ++k) //common vertex on edges
                    G2New[i][j] += Alpha[commonVertex][k] * (Alpha[commonVertex][k] + Alpha[sideVertices.Item1][k] + 
                                                                                      Alpha[sideVertices.Item2][k]) +
                                                                                 2d * Alpha[sideVertices.Item1][k] * 
                                                                                      Alpha[sideVertices.Item2][k];
                G2New[i][j] /= 120d;
            }
        }
            
        return [.. G1(Alpha).Concat(G2New)];
    }

    private static double[][] G3(double[][] Alpha)
    {
        int N = TetrahedronHierarchicalBases.CalcDofsCount(3);
        int N0 = TetrahedronHierarchicalBases.CalcDofsCount(2);
        var G3New = new double[N - N0][];

        for(int i = 0; i < N - N0 ; ++i)     //initialization
            G3New[i] = new double[N0 + i + 1];

        //edges 3 order function to vertices = 0

        int vertexCount = 4;

        for(int i = 0; i < Tetrahedron.EdgesCountS; ++i) //order 2 and 3 functions on edges
        {
            var edgeI = Tetrahedron.LocalEdgeS(i);
            for(int j = 0; j < Tetrahedron.EdgesCountS; ++j)
            {
                var edgeJ = Tetrahedron.LocalEdgeS(j);
                int commonVertex;
                (int, int) sideVertices;
                double sign;
                if(i == j)  //order 2 and 3 functions on the same edge
                {
                    for(int k = 1; k <= 3; ++k)
                        G3New[i][i + vertexCount] += -Alpha[edgeI.Item1][k] * Alpha[edgeI.Item1][k] +
                                                      Alpha[edgeI.Item2][k] * Alpha[edgeI.Item2][k];
                    G3New[i][i + vertexCount] /= 360d;
                    continue;
                }
                else if(edgeI.Item1 == edgeJ.Item1)
                {
                    commonVertex = edgeI.Item1;
                    sideVertices = (edgeI.Item2, edgeJ.Item2);
                    sign = 1d;
                }
                else if(edgeI.Item2 == edgeJ.Item1)
                {
                    commonVertex = edgeI.Item2;
                    sideVertices = (edgeI.Item1, edgeJ.Item2);
                    sign = -1d;
                }
                else if(edgeI.Item2 == edgeJ.Item2)
                {
                    commonVertex = edgeI.Item2;
                    sideVertices = (edgeI.Item1, edgeJ.Item1);
                    sign = -1d;
                }
                else if(edgeI.Item1 == edgeJ.Item2)
                {
                    commonVertex = edgeI.Item1;
                    sideVertices = (edgeI.Item2, edgeJ.Item1);
                    sign = 1d;
                }
                else
                    continue;
                for(int k = 1; k <= 3; ++k)
                    G3New[i][j + vertexCount] += Alpha[sideVertices.Item2][k] * (Alpha[commonVertex][k] +
                                                                                 Alpha[sideVertices.Item1][k]);
                G3New[i][j + vertexCount] *= sign / 360d;
            }
        }

        for(int i = 0; i < Tetrahedron.EdgesCountS; ++i) //order 3 same functions on edges
        {
            var edge = Tetrahedron.LocalEdgeS(i);
            for(int k = 1; k <= 3; ++k)
                G3New[i][i + N0] += 2d * Alpha[edge.Item1][k] * Alpha[edge.Item1][k] +
                                         Alpha[edge.Item1][k] * Alpha[edge.Item2][k] +
                                    2d * Alpha[edge.Item2][k] * Alpha[edge.Item2][k];
            G3New[i][i + N0] /= 630d;
        }

        for(int i = 0; i < Tetrahedron.EdgesCountS; ++i) //order 3 functions on edges
        {
            var edgeI = Tetrahedron.LocalEdgeS(i);
            for(int j = 0; j < i; ++j)
            {
                var edgeJ = Tetrahedron.LocalEdgeS(j);
                int commonVertex;
                (int, int) sideVertices;
                double sign;
                if(edgeI.Item1 == edgeJ.Item1)
                {
                    commonVertex = edgeI.Item1;
                    sideVertices = (edgeI.Item2, edgeJ.Item2);
                    sign = 1d;
                }
                else if(edgeI.Item1 == edgeJ.Item2)
                {
                    commonVertex = edgeI.Item1;
                    sideVertices = (edgeI.Item2, edgeJ.Item1);
                    sign = -1d;
                }
                else if(edgeI.Item2 == edgeJ.Item2)
                {
                    commonVertex = edgeI.Item2;
                    sideVertices = (edgeI.Item1, edgeJ.Item1);
                    sign = 1d;
                }
                else
                    continue;
                for(int k = 1; k <= 3; ++k)
                    G3New[i][j + N0] += Alpha[commonVertex][k] * 
                                        (Alpha[commonVertex][k] + Alpha[sideVertices.Item1][k] + Alpha[sideVertices.Item2][k]) +
                                                             2d * Alpha[sideVertices.Item1][k] * Alpha[sideVertices.Item2][k];
                G3New[i][j + N0] *= sign / 1260d;
            }
        }

        for(int i = Tetrahedron.EdgesCountS; i < N - N0; ++i) //faces to vertices
        {
            var face = Tetrahedron.LocalFaceS(i - Tetrahedron.EdgesCountS);
            for(int j = 0; j < vertexCount; ++j)
            {
                for(int k = 1; k <= 3; ++k)
                    G3New[i][j] += Alpha[j][k] *
                                  (Alpha[face[0]][k] + 
                                   Alpha[face[1]][k] + 
                                   Alpha[face[2]][k]);
                G3New[i][j] /= 120d;
            }
        }

        for(int i = Tetrahedron.EdgesCountS; i < N - N0; ++i) //faces to edges 2 order
        {
            var face = Tetrahedron.LocalFaceS(i - Tetrahedron.EdgesCountS);
            for(int j = vertexCount; j < vertexCount + Tetrahedron.EdgesCountS; ++j)
            {
                var edge = Tetrahedron.LocalEdgeS(j - vertexCount);
                if(face.Contains(edge.Item1) && face.Contains(edge.Item2)) //faces to edges on face
                {
                    int vertexNotOnEdge = face.First(i => i != edge.Item1 && i != edge.Item2);
                    for(int k =1; k <= 3; ++k)
                        G3New[i][j] += Alpha[edge.Item1][k] * (Alpha[edge.Item1][k] + Alpha[vertexNotOnEdge][k]) +
                                       Alpha[edge.Item2][k] * (Alpha[edge.Item2][k] + Alpha[vertexNotOnEdge][k]) +
                                       Alpha[edge.Item1][k] * Alpha[edge.Item2][k];  
                    G3New[i][j] /= 360d;
                }
                else  //faces to edges not on faces
                {
                    edge = face.Contains(edge.Item1) ? edge : (edge.Item2, edge.Item1);
                    var oppositeEdge = face.Except([edge.Item1, edge.Item2]).ToArray();
                    for(int k =1; k <= 3; ++k)
                        G3New[i][j] += Alpha[edge.Item1][k] * (Alpha[0][k] + Alpha[1][k] + Alpha[2][k] + Alpha[3][k]) + 
                        2d * Alpha[edge.Item2][k] * (Alpha[oppositeEdge[0]][k] + Alpha[oppositeEdge[1]][k]);
                    G3New[i][j] /= 720d;
                }
            }
        }

        for(int i = Tetrahedron.EdgesCountS; i < N - N0; ++i) //faces to edges 3 order
        {
            var face = Tetrahedron.LocalFaceS(i - Tetrahedron.EdgesCountS);
            for(int j = N0; j < N0 + Tetrahedron.EdgesCountS; ++j)
            {
                var edge = Tetrahedron.LocalEdgeS(j - N0);
                if(face.Contains(edge.Item1) && face.Contains(edge.Item2)) //faces to edges on face
                {
                    int vertexNotOnEdge = face.First(i => i != edge.Item1 && i != edge.Item2);
                    for(int k =1; k <= 3; ++k)
                        G3New[i][j] += Alpha[edge.Item2][k] * (Alpha[edge.Item2][k] - Alpha[vertexNotOnEdge][k]) -
                                       Alpha[edge.Item1][k] * (Alpha[edge.Item1][k] - Alpha[vertexNotOnEdge][k]);
                    G3New[i][j] /= 2520d;
                }
                else  //faces to edges not on faces
                {
                    var oppositeEdge = face.Except([edge.Item1, edge.Item2]).ToArray();
                    double sign = face.Contains(edge.Item1) ? 1d : -1d;
                    for(int k = 1; k <= 3; ++k)
                        G3New[i][j] += (Alpha[edge.Item1][k] + Alpha[edge.Item2][k]) *
                                       (Alpha[oppositeEdge[0]][k] + Alpha[oppositeEdge[1]][k]);
                    G3New[i][j] *= sign / 2520d;
                }
            }
        }
        for(int i = Tetrahedron.EdgesCountS; i < N - N0; ++i) //faces to same faces
        {
            var face = Tetrahedron.LocalFaceS(i - Tetrahedron.EdgesCountS);
            for(int k = 1; k <= 3; ++k)
                G3New[i][N0 + i] += Alpha[face[0]][k] * Alpha[face[0]][k] +
                                    Alpha[face[1]][k] * Alpha[face[1]][k] +
                                    Alpha[face[2]][k] * Alpha[face[2]][k] +
                                    Alpha[face[0]][k] * Alpha[face[1]][k] +
                                    Alpha[face[0]][k] * Alpha[face[2]][k] +
                                    Alpha[face[1]][k] * Alpha[face[2]][k];
            G3New[i][N0 + i] /= 1260d;
        }

        for(int i = Tetrahedron.EdgesCountS; i < N - N0; ++i) //faces to different faces
        {
            var faceI = Tetrahedron.LocalFaceS(i - Tetrahedron.EdgesCountS);
            for(int j = N0 + Tetrahedron.EdgesCountS; j < i + N0; ++j)
            {
                var faceJ = Tetrahedron.LocalFaceS(j - N0 - Tetrahedron.EdgesCountS);
                var commonEdge = faceI.Intersect(faceJ).ToArray();
                var oppositeEdge = Enumerable.Range(0,4).Except(commonEdge).ToArray();
                for(int k = 1; k <= 3; ++k)
                    G3New[i][j] += Alpha[commonEdge[0]][k] * (Alpha[commonEdge[0]][k] + Alpha[oppositeEdge[0]][k] + Alpha[oppositeEdge[1]][k]) +
                                   Alpha[commonEdge[1]][k] * (Alpha[commonEdge[1]][k] + Alpha[oppositeEdge[0]][k] + Alpha[oppositeEdge[1]][k]) +
                               Alpha[commonEdge[0]][k] * Alpha[commonEdge[1]][k] + 2d * Alpha[oppositeEdge[0]][k] * Alpha[oppositeEdge[1]][k];
                G3New[i][j] /= 2520d;
            }
        }

        return [.. G2(Alpha), .. G3New];
    }

    public static double[][] CalculateLocalHierarchical_LagrangianMassMatrix(int order, double AbsdetD, PolinomialType polinomial)
    {
        if(order > 3) throw new NotImplementedException();

        int N = TetrahedronHierarchicalBases.CalcDofsCount(order);

        var matrixFileName = order switch
        {
            1 => Hierarchical_LagrangianLinearMassMatrixFileName,
            2 => Hierarchical_LagrangianQuadraticMassMatrixFileName,
            3 => Hierarchical_LagrangianCubicMassMatrixFileName,
            _ => throw new NotImplementedException()
        };

        var baseMatrix = MatrixReader.ReadMatrixFromFile(Path.Join(directory, matrixFileName));

        return [.. baseMatrix.Select(i => i.Select(j => j * AbsdetD).ToArray())];
    }
}