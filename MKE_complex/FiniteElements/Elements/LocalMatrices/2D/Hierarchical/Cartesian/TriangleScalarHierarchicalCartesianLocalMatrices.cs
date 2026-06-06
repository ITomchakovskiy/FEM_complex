using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;
using MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Hierarchical;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Hierarchical.Cartesian;
public static class TriangleScalarHierarchicalCartesianLocalMatrices
{
    private static string directory = "Scalar/Hierarchical";
    private static string MassMatrixFileName = "TriangleScalarHierarchicalSimpleMassMatrix";
    private static string Hierarchical_LagrangianLinearMassMatrixFileName = "TriangleScalarHierarchical_LagrangianLinearMassMatrix";
    private static string Hierarchical_LagrangianQuadraticMassMatrixFileName = "TriangleScalarHierarchical_LagrangianQuadraticMassMatrix";
    private static string Hierarchical_LagrangianCubicMassMatrixFileName = "TriangleScalarHierarchical_LagrangianCubicMassMatrix"; //"TriangleScalarHierarchicalSimpleMassMatrix";

    private static double[][] BaseM3;

    static TriangleScalarHierarchicalCartesianLocalMatrices()
    {
        BaseM3 = MatrixReader.ReadMatrixFromFile(Path.Join(directory, MassMatrixFileName));
    }

    public static double[][] CalculateLocalMassMatrix(int order, double AbsdetD, double Coefficient, PolinomialType polinomial)
    {
        if(order > 3) throw new NotImplementedException();

        int N = TriangleHierarchicalBases.CalcDofsCount(order);
        var baseMatrix = BaseM3.Take(N);

        return [.. baseMatrix.Select(i => i.Select(j => j * AbsdetD * Coefficient).ToArray())];
    }

    public static double[][] CalculateLocalStiffnessMatrix(int order, double[][] alphas, double AbsdetD, double Coefficient, PolinomialType polinomial)
    {
        if(order > 3) throw new NotImplementedException();

        int N = TriangleHierarchicalBases.CalcDofsCount(order);

        //toAdd
    
        return [[]];
    }

    public static double[][] CalculateLocalHierarchical_LagrangianMassMatrix(int order, double AbsdetD, PolinomialType polinomial)
    {
        if(order > 3) throw new NotImplementedException();

        int N = TriangleHierarchicalBases.CalcDofsCount(order);

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