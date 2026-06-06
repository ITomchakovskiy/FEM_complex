using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class ScalarEllipticProblemTest
    {
        [Fact]
        public void Test1()
        {
            Vector2D[] PointsOnRectangle(Vector2D A, Vector2D B, Vector2D h)
            {
                var dif = B - A;
                int k_x = (int)(dif.X / h.X);
                int k_y = (int)(dif.Y / h.Y);

                Vector2D[] res = new Vector2D[(k_x + 1) * (k_y + 1)];

                for (int i = 0; i < k_y; ++i)
                {
                    double y = A.Y + i * h.Y;
                    for (int j = 0; j < k_x; ++j)
                    {
                        double x = A.X + j * h.X;
                        res[i * (k_x + 1) + j] = new(x, y);
                    }
                    {
                        double x = B.X;
                        res[(i + 1) * (k_x + 1) - 1] = new(x, y);
                    }
                }
                {
                    double y = B.Y;
                    for (int j = 0; j < k_x; ++j)
                    {
                        double x = A.X + j * h.X;
                        res[^(k_x - j + 1)] = new(x, y);
                    }
                }
                res[^1] = B;
                return res;
            }



            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Assembly assembly = Assembly.GetExecutingAssembly();

            FiniteElementsCreator.LoadFiniteElementTypes(assembly);

            MaterialCreator.LoadMaterialsAssemblyInfo(assembly);

            Console.WriteLine("Choose dimension");

            foreach (Dimension d in Enum.GetValues(typeof(Dimension)))
                Console.WriteLine($"{d} : {(int)d}");

            Dimension dimension = (Dimension)int.Parse(Console.ReadLine()!);

            var problem = new ScalarEllipticProblem<Vector2D>();

            //problem.InputUserDefinedData();

            problem.Solve();

            var points = PointsOnRectangle(new(0d, 0d), new(10d, 4d), new(0.3, 0.3));

            var discr = problem.EvaluateDiscrepancy(points, (vec) => 2d * vec.X * vec.X + 3d * vec.Y * vec.Y + 6d * vec.X * vec.Y);

            //problem.Mesh.

            Console.WriteLine(discr);
        }
    }
}