using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;
using MKE_complex.FiniteElements.Elements.LocalMatrices;
using MKE_complex.FiniteElements.Elements.LocalMatrices._2D.Hierarchical.Cartesian;
using MKE_complex.FiniteElements.Elements.LocalMatrices._3D.Hierarchical.Cartesian;
using MKE_complex.FiniteElements.Elements.LocalMatrices._3D.Lagrangian.Cartesian;
using Xunit;

namespace MKE_complex.Tests
{
    public class HierarchicalLinearMassMatrixTest
    {
        [Fact]
        public void Test1()
        {
            double[][] M = [[1d/60d], [1d/120d, 1d/60d], [1d/120d, 1d/120d, 1d/60d], [1d/120d, 1d/120d, 1d/120d, 1d/60d]];
            double[][] M2 = TetrahedronHierarchicalCartesianLocalMatrices.CalculateLocalMassMatrix(1,1d,1d,PolinomialType.Simple);
            var M3 = TetrahedronHierarchicalCartesianLocalMatrices.CalculateLocalHierarchical_LagrangianMassMatrix(1,1d,PolinomialType.Simple);
            double discr = 0d;
            for(int i =0; i < M.Length; ++i)
            {
                for(int j = 0; j < M[i].Length; ++j)
                {
                    discr += (M[i][j] - M2[i][j]) * (M[i][j] - M2[i][j]);
                    discr += (M[i][j] - M3[i][j]) * (M[i][j] - M3[i][j]);
                }
                    
            }

            double[][] MT = [[1d/12d], [1d/24d, 1d/12d], [1d/24d, 1d/24d, 1d/12d]];
            var M2T = TriangleScalarHierarchicalCartesianLocalMatrices.CalculateLocalMassMatrix(1,1d,1d,PolinomialType.Simple);
            var M3T = TriangleScalarHierarchicalCartesianLocalMatrices.CalculateLocalHierarchical_LagrangianMassMatrix(1,1d,PolinomialType.Simple);

            for(int i =0; i < MT.Length; ++i)
            {
                for(int j = 0; j < MT[i].Length; ++j)
                {
                    discr += (MT[i][j] - M2T[i][j]) * (MT[i][j] - M2T[i][j]);
                    discr += (MT[i][j] - M3T[i][j]) * (MT[i][j] - M3T[i][j]);
                }
                    
            }
            Console.WriteLine(discr);
            Assert.True(discr < 1.0E-20);
        }
    }
}