using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
using MKE_complex.FiniteElements.Elements.LocalMatrices;
using MKE_complex.Vector;
using Xunit;

namespace MKE_complex.Tests
{
    public class AlphaTest
    {
        [Fact]
        public void Test1()
        {
            double[] A = [-4.98755, -5.65241, -2.30958];
            double[] B = [-3.3154, -3.46337, -0.44449];
            double[] C = [-7.60147, -1.6869, 1.0];
            double[] D = [-2.55932, -6.84278, 8.0];

            Vector3D[] vertices = [new(A[0],A[1],A[2]),new(B[0],B[1],B[2]),new(C[0],C[1],C[2]),new(D[0],D[1],D[2])];

            var Alpha = TetrahedronLocalCoordinates.Alpha.CalcAlphas(vertices);
            var AlphaT = MatrixReader.ReadMatrixFromFile("Tests/AlphaT1");
            var discr = 0d;
            for(int i = 0; i < Alpha.Length; ++i)
            {
                for(int j = 0; j < Alpha.Length; ++j)
                    discr += (Alpha[i][j] - AlphaT[i][j]) * (Alpha[i][j] - AlphaT[i][j]);
            }

            Console.WriteLine(discr);

            Assert.True(discr < 1.0E-15);
        }
    }
}