using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
using MKE_complex.Vector;
using Xunit;

namespace MKE_complex.Tests
{
    public class TetrahedronLocalCoordinatesTest
    {
        [Fact]
        public void Test1()
        {
            Vector3D[] vertices = [new(0d,0d,0d), new(0d,5d,0d), new(3d,-1d,0d), new(3.8095, 4.44364, 4)];

            double h = 0.1;

            List<double[]> localPoints = [];
            
            int n = (int)(1d/ h);

            for(int i = 0; i <= n; ++i)
            {
                var l4 = i * h;
                for(int j = 0; j <= n - i; ++j)
                {
                    var l3 = j*h;
                    for(int p = 0; p <= n - i - j; ++p)
                    {
                        var l2 = p*h;
                        var l1 = 1d - l4 - l3 - l2;
                        localPoints.Add([l1,l2,l3,l4]);
                    }
                }
            }

            var GlobalPoints = localPoints.Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(vertices, i)).ToArray();

            var Alpha= TetrahedronLocalCoordinates.Alpha.CalcAlphas(vertices);

            var newLocalPoints = GlobalPoints.Select(i => TetrahedronLocalCoordinates.LocalCoordinates.Select(j => j(i,Alpha)));

            var newGlobalPoints = newLocalPoints.Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(vertices, i.ToArray())).ToArray();

            for(int i = 0; i < GlobalPoints.Length; ++i)
            {
                Console.WriteLine($"{i}\t{GlobalPoints[i].AsString("f"," ")}\t{newGlobalPoints[i].AsString("f"," ")}\t{(GlobalPoints[i] - newGlobalPoints[i]).AsString("e"," ")}");
            }

            var det = TetrahedronLocalCoordinates.Alpha.CalcSignedDetD(vertices);

            var Absdet = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(vertices);

            Console.WriteLine($"{det}   {Absdet}");

        }
    }
}