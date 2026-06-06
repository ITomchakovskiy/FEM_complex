using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._2D.VectorHierarchical.Cartesian;
public static class RectangleVectorHierarchicalCartesianLocalMatrices
{

    public static double[][] CalcLocalMassMatrix(int Order, double Gamma, double hx, double hy)
    {
        var baseMatrix = Order switch
        {
            1 => M2.Take(4),
            2 => M2,
            _ => throw new ArgumentException()
        };

        return baseMatrix.Select(i => i.Select(j => j*hx*hy*Gamma).ToArray()).ToArray();
    }

    private static readonly double[][] M2;

    static RectangleVectorHierarchicalCartesianLocalMatrices()
    {
        M2 = buildLocalBaseMassMatrixQuadratic();
        //M1 = M2;
    }

    private static string directory = "VectorHierarchical";

    private static double[][] buildLocalBaseMassMatrixQuadratic()
    {
        string path = Path.Combine(directory, "LocalBaseRectangleVectorHierarchicalMassMatrixQuadratic");
        return MatrixReader.ReadMatrixFromFile(path);
    }
}