using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
using MKE_complex.Vector;
using Xunit;

namespace MKE_complex.Tests
{
    public class TriangleLocalCoordinateTest
    {
        [Fact]
        public void Test1()
        {
            // Vector3D[] vertices = [new(-1.35497, -1.84037, -4.61531),
            //                         new(-5.16338, 0.67334, 0),new(-2.38094, -1.48817, 3.28219)];

            // Vector3D[] vertices = [new(0, 0.67, -5.16),
            //                        new(4.58, -1.84, -1.36),
            //                        new(-3.27, -1.49, -2.38)];
            
            Vector3D[] vertices = [new(-2.39, -3.28, -1.48),
                                   new(-5.16, 0, 0.67),
                                   new(-1.35, 4.62, -1.84)];

            double h = 0.1;

            List<double[]> localPoints = [];
            
            int n = (int)(1d/ h);

            for(int i = 0; i <= n; ++i)
            {
                var l3 = i * h;
                for(int j = 0; j <= n - i; ++j)
                {
                    var l2 = j*h;
                    var l1 = 1d  - l3 - l2;
                        localPoints.Add([l1,l2,l3]);

                }
            }

            var GlobalPoints = localPoints.Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices, i)).ToArray();

            var Alpha= TriangleLocalCoordinates.Alpha.CalcAlphas(vertices, out string projectionPlane);

            var newLocalPoints = GlobalPoints.Select(i => TriangleLocalCoordinates.GetLocalCoordinates(projectionPlane).Select(j => j(i,Alpha)));

            var newGlobalPoints = newLocalPoints.Select(i => TriangleLocalCoordinates.LocalCoordinatesToGlobal(vertices, i.ToArray())).ToArray();

            for(int i = 0; i < GlobalPoints.Length; ++i)
            {
                Console.WriteLine($"{i}\t{GlobalPoints[i].AsString("f"," ")}\t{newGlobalPoints[i].AsString("f"," ")}\t{(GlobalPoints[i] - newGlobalPoints[i]).AsString("e"," ")}");
            }

            //var det = TriangleLocalCoordinates.Alpha.CalcSignedDetD(vertices);

            var Absdet = TriangleLocalCoordinates.Alpha.CalcAbsDetD(vertices);

            Console.WriteLine($"   {Absdet}");

            Console.WriteLine(projectionPlane);
        }
    }
}