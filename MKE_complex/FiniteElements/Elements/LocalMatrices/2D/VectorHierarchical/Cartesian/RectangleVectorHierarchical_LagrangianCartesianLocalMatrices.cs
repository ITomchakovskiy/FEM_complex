using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._2D.VectorHierarchical.Cartesian;
public static class RectangleVectorHierarchical_LagrangianCartesianLocalMatrices
{
    public static double[][] CalcLocalMassMatrix(int Order, double hx, double hy)
    {
        var baseMatrix = Order switch
        {
            1 => M1,
            2 => M2,
            _ => throw new ArgumentException()
        };

        return baseMatrix.Select(i => i.Select(j => j*hx*hy).ToArray()).ToArray();
    }

    private static string directory = "VectorHierarchical";

    private static double[][] M1 = buildLocalBaseMassMatrixLinear();
    private static double[][] M2 = buildLocalBaseMassMatrixQuadratic();

    private static double[][] buildLocalBaseMassMatrixLinear()
    {
        string path = Path.Combine(directory, "RectangleHierarchical_LagrangianMassMatrixLinear");
        return MatrixReader.ReadMatrixFromFile(path);
    }

    private static double[][] buildLocalBaseMassMatrixQuadratic()
    {
        string path = Path.Combine(directory, "RectangleHierarchical_LagrangianMassMatrixQuadratic");
        return MatrixReader.ReadMatrixFromFile(path);
    }
}