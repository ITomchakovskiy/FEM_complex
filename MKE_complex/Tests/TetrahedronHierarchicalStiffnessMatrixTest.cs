using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Hierarchical;
using MKE_complex.FiniteElements.Elements.LocalMatrices;
using MKE_complex.FiniteElements.Elements.LocalMatrices._3D.Hierarchical.Cartesian;
using Xunit;

namespace MKE_complex.Tests
{
    public class TetrahedronHierarchicalStiffnessMatrixTest
    {
        [Fact]
        public void Test1()
        {
            double[][] Alpha = [[0d, 1d, 2d, 3d],
                                [0d, 4d, 5d, 6d],
                                [0d, 7d, 8d, 9d],
                                [0d, -1d - 4d - 7d, -2d - 5d - 8d, -3d - 6d - 9d],
            ];

            var G3 = TetrahedronHierarchicalCartesianLocalMatrices.CalculateLocalStiffnessMatrix(3,Alpha,1d,1d,PolinomialType.Simple);

            var G3True = MatrixReader.ReadMatrixFromFile("Output");

            double discrepancy = 0d;

            for(int i = 0; i < G3.Length; ++i)
            {
                for(int j = 0; j <= i; ++j)
                {
                    if(Math.Abs(G3[i][j] - G3True[i][j]) > 0.1)
                        Console.WriteLine($"{i}  {j}");
                    discrepancy += Math.Abs(G3[i][j] - G3True[i][j]);
                }
            }
            Console.WriteLine(discrepancy);
        }
    }
}