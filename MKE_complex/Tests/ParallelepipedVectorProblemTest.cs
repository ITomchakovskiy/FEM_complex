using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.Problems;
using MKE_complex.Problems.Materials;
using MKE_complex.Vector;
using Xunit;

namespace MKE_complex.Tests
{
    public class ParallelepipedVectorProblemTest
    {
        [Fact]
        public void Test1()
        {
            Vector3D[] PointsOnRectangle(Vector3D A, Vector3D B, Vector3D h)
            {
                var dif = B - A;
                int k_x = (int)(dif.X / h.X);
                int k_y = (int)(dif.Y / h.Y);
                int k_z = (int)(dif.Z / h.Z);

                Vector3D[] res = new Vector3D[(k_x + 1) * (k_y + 1) * (k_z + 1)];

                for (int i = 0; i < k_z; ++i)
                {
                    double z = A.Z + i * h.Z;
                    for (int j = 0; j < k_y; ++j)
                    {
                        double y = A.Y + j * h.Y;
                        for(int p = 0; p < k_x; ++p)
                        {
                            double x = A.X + p * h.X;
                            res[i * (k_x + 1) * (k_y + 1) + j * (k_x + 1) + p] = new(x, y, z);
                        }
                        {
                            double x = B.X;
                            res[i * (k_x + 1) * (k_y + 1) + j * (k_x + 1) + k_x] = new(x, y, z);
                        }
                    }
                    {
                        double y = B.Y;
                        for(int p = 0; p < k_x; ++p)
                        {
                            double x = A.X + p * h.X;
                            res[i * (k_x + 1) * (k_y + 1) + k_y * (k_x + 1) + p] = new(x, y, z);
                        }
                        {
                            double x = B.X;
                            res[i * (k_x + 1) * (k_y + 1) + k_y * (k_x + 1) + k_x] = new(x, y, z);
                        }
                    }
                }
                {
                    double z = B.Z;
                    for (int j = 0; j < k_y; ++j)
                    {
                        double y = A.Y + j * h.Y;
                        for(int p = 0; p < k_x; ++p)
                        {
                            double x = A.X + p * h.X;
                            res[k_z * (k_x + 1) * (k_y + 1) + j * (k_x + 1) + p] = new(x, y, z);
                        }
                        {
                            double x = B.X;
                            res[k_z * (k_x + 1) * (k_y + 1) + j * (k_x + 1) + k_x] = new(x, y, z);
                        }
                    }
                    {
                        double y = B.Y;
                        for(int p = 0; p < k_x; ++p)
                        {
                            double x = A.X + p * h.X;
                            res[k_z * (k_x + 1) * (k_y + 1) + k_y * (k_x + 1) + p] = new(x, y, z);
                        }
                        {
                            double x = B.X;
                            res[k_z * (k_x + 1) * (k_y + 1) + k_y * (k_x + 1) + k_x] = new(x, y, z);
                        }
                    }
                }
                res[^1] = B;
                return res;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            FiniteElementsCreator.LoadFiniteElementTypes(assembly);

            MaterialCreator.LoadMaterialsAssemblyInfo(assembly);

            Console.WriteLine("Choose dimension");

            foreach (Dimension d in Enum.GetValues(typeof(Dimension)))
                Console.WriteLine($"{d} : {(int)d}");

            Dimension dimension = (Dimension)int.Parse(Console.ReadLine()!);

            var problem = new VectorProblem<Vector3D>();

            problem.InputUserDefinedData();

            problem.Solve();

            var points = PointsOnRectangle(new(0.5d, 0.25d, 0.7d), new(4.5d, 2.25d, 6.3d), new(4d/10d, 2d/10d, 5.6d/10d));

            Vector3D A(Vector3D point)
            {
                //return new (1d + point.Y,5d * point.Z,2d * point.X + 2d);
                // return new (point.Y * point.Y + point.X + 1d,
                //     5d* point.Z * point.Z + 3d * point.Y * point.Z,
                //         2d * point.X * point.Z + 2d);
                        // return new (point.Y*point.Y*point.Y,
                        //             5d*point.Z*point.Z*point.Z,
                        //             2d*point.X*point.X*point.X);
                return new (Math.Sin(point.Y)*Math.Cos(point.Z),
                                    Math.Sin(point.X)*Math.Cos(point.Z),
                                    Math.Sin(point.X)*Math.Cos(point.Y));

            }

            var discr = problem.EvaluateDiscrepancy(points, A);

            //problem.Mesh.

            Console.WriteLine(discr);
        }
    }
}