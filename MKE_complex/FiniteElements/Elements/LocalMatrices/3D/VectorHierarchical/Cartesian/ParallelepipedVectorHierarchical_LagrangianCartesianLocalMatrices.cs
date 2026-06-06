using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._3D.VectorHierarchical.Cartesian;

public class ParallelepipedVectorHierarchical_LagrangianCartesianLocalMatrices
{
    public static double[][] GetLocalMassMatrix(int Order, double hx, double hy, double hz)
    {
        var baseMatrix = Order switch
        {
            1 => M1,
            2 => M2,
            _ => throw new ArgumentException()
        };

        return baseMatrix.Select(i => i.Select(j => j*hx*hy*hz).ToArray()).ToArray();
    }

    private static string directory = "VectorHierarchical";

    private static double[][] M1 = buildLocalBaseMassMatrixLinear();
    private static double[][] M2 = buildLocalBaseMassMatrixQuadratic();

    private static double[][] buildLocalBaseMassMatrixLinear()
    {
        string path = Path.Combine(directory, "LocalBaseHierarchical_LagrangeMassMatrixLinear");
        return MatrixReader.ReadMatrixFromFile(path);
    }

    private static double[][] buildLocalBaseMassMatrixQuadratic()
    {
        string path = Path.Combine(directory, "LocalBaseHierarchical_LagrangeMassMatrixQuadratic");
        return MatrixReader.ReadMatrixFromFile(path);
    }
}
