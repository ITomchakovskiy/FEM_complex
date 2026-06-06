using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._3D.Lagrangian.Cartesian;
public static class TetrahedronScalarLagrangianCartesianLocalMatrices
{
    private static string directory = "Scalar/Lagrangian";

    private static string QuadraticMassMatrixFileName = "TetrahedronLagrangianQuadraticMassMatrix";

    private static double[][] BaseM2;
    static TetrahedronScalarLagrangianCartesianLocalMatrices()
    {
        BaseM2 = MatrixReader.ReadMatrixFromFile(Path.Join(directory, QuadraticMassMatrixFileName));
    }

    public static double[][] CalculateLocalMassMatrix(int order, double AbsdetD, double Coefficient)
    {
        var baseMatrix = order switch
        {
            2 => BaseM2,
            _ => throw new NotImplementedException()
        };

        return [.. baseMatrix.Select(i => i.Select(j => j * AbsdetD * Coefficient).ToArray())];
    }

    public static double[][] CalculateLocalStiffnessMatrix(int order, double[][] Alpha, double AbsdetD, double Coefficient)
    {
        var baseMatrix = order switch
        {
            //1 => G1(Alpha),
            2 => G2(Alpha),
            //3 => G3(Alpha),
            _ => throw new NotImplementedException()
        };

        return [.. baseMatrix.Select(i => i.Select(j => j * AbsdetD * Coefficient).ToArray())];
    }

    private static double[][] G2(double[][] Alpha)
    {
        int N = 10;  //ToChange
        double[][] Result = new double[N][];
        for(int i = 0; i < N; ++i)
            Result[i] = new double[i+1];

        for(int i = 0; i < Tetrahedron.VertexCountS; ++i)
        {
            for(int k = 1; k <= 3; ++k)
                Result[i][i] += Alpha[i][k] * Alpha[i][k];
            Result[i][i] /= 10d;
        }

        for(int i = 0; i < Tetrahedron.VertexCountS; ++i)
        {
            for(int j = 0; j < i; ++j)
            {
                for(int k = 1; k <= 3; ++k)
                    Result[i][j] += Alpha[i][k] * Alpha[j][k];
                Result[i][j] *= -1d/30d;
            }
        }

        for(int i = Tetrahedron.VertexCountS; i < N; ++i)
        {
            var edge = Tetrahedron.LocalEdgeS(i-Tetrahedron.VertexCountS);
            for(int k = 1; k <= 3; ++k)
                Result[i][i] += Alpha[edge.Item1][k] * Alpha[edge.Item1][k] +
                                Alpha[edge.Item1][k] * Alpha[edge.Item2][k] +
                                Alpha[edge.Item2][k] * Alpha[edge.Item2][k];
            Result[i][i] *= 4d/15d;
        }

        for(int i = Tetrahedron.VertexCountS; i < N; ++i)
        {
            var edge = Tetrahedron.LocalEdgeS(i-Tetrahedron.VertexCountS);
            for(int j = 0; j < Tetrahedron.VertexCountS; ++j)
            {
                if(edge.Item1 == j || edge.Item2 == j)
                {
                    for(int k = 1; k <= 3; ++k)
                        Result[i][j] += -Alpha[j][k] * Alpha[j][k] +
                                    3d * Alpha[edge.Item1][k] * Alpha[edge.Item2][k];
                    Result[i][j] /= 30d;
                }
                else
                {
                    for(int k = 1; k <= 3; ++k)
                        Result[i][j] += Alpha[j][k] * (Alpha[edge.Item1][k] +
                                                       Alpha[edge.Item2][k]);
                    Result[i][j] *= -1d/30d;
                }
            }
        }

        for(int i = Tetrahedron.VertexCountS; i < N; ++i)
        {
            var edgeI = Tetrahedron.LocalEdgeS(i-Tetrahedron.VertexCountS);
            for(int j = Tetrahedron.VertexCountS; j < i; ++j)
            {
                var edgeJ = Tetrahedron.LocalEdgeS(j-Tetrahedron.VertexCountS);

                int commonVertex;
                (int,int) differentVertices;
                if(edgeI.Item1 == edgeJ.Item1)
                {
                    commonVertex = edgeI.Item1;
                    differentVertices = (edgeI.Item2, edgeJ.Item2);
                }
                else if(edgeJ.Item2 == edgeI.Item1)
                {
                    commonVertex = edgeJ.Item2;
                    differentVertices = (edgeI.Item2, edgeJ.Item1);
                }
                else if(edgeI.Item2 == edgeJ.Item2)
                {
                    commonVertex = edgeJ.Item2;
                    differentVertices = (edgeI.Item1, edgeJ.Item1);
                }
                else
                {
                    for(int k = 1; k <= 3; ++k)
                        Result[i][j] += (Alpha[edgeI.Item1][k] + Alpha[edgeI.Item2][k]) *
                                        (Alpha[edgeJ.Item1][k] + Alpha[edgeJ.Item2][k]);
                    Result[i][j] *= 2d/15d;
                    //
                    continue;
                }
                for(int k = 1; k <= 3; ++k)
                    Result[i][j] += Alpha[commonVertex][k] * (Alpha[commonVertex][k] + Alpha[differentVertices.Item1][k] + 
                                                                                       Alpha[differentVertices.Item2][k]) +
                                                                                  2d * Alpha[differentVertices.Item1][k] *
                                                                                       Alpha[differentVertices.Item2][k];
                Result[i][j] *= 2d/15d;
            }
        }

        return Result;
    }
}