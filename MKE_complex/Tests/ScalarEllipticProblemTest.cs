using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;
using MKE_complex.Mesh.MeshBuilder;
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

            void BuildTrueLagrangianSolution(ScalarEllipticProblem<Vector2D> problem, Func<Vector2D,double> u)
        {
            var elements = problem.Mesh.Elements;
            int ielem = 0;
            for(int i = 0; i < problem.Solution.Length; ++i) problem.Solution[i] = 0d;
            foreach(var elem in elements.ToArray().OfType<TriangleLagrangianFiniteElement>())
            {
                //var lagrangianLocal = elem.LocalLagrangianVerticesAtDofs();
                var vertices = elem.Geometry.VertexNumber.Select(i => problem.Mesh.Vertices[i]).ToArray();
                var A = vertices[0]; var B = vertices[1]; var C = vertices[2];
                //var lagrangianGlobal = lagrangianLocal.Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(vertices, i)).ToArray();
                Vector2D[] lagrangianGlobal = [A,B,C,(A+B)/2d,(B+C)/2d,(A+C)/2d];
                int[] ElementIndex = new int[problem.Solution.Length];
                for(int i = 0; i < lagrangianGlobal.Length; ++i)
                {
                    var prev = problem.Solution[elem.DOFs[i]];
                    var cur = u(lagrangianGlobal[i]);
                    if( prev == 0d );
                    else
                    {
                        var relDiscrepancy = Math.Abs(prev - cur) / Math.Max(Math.Abs(prev), Math.Abs(cur));
                        //Console.WriteLine(relDiscrepancy);
                        if(relDiscrepancy >= 1.0E-5)
                        {
                            var prvInd = ElementIndex[elem.DOFs[i]];
                            int dof = elem.DOFs[i];
                        }
                    }
                    ElementIndex[elem.DOFs[i]] = ielem;
                    problem.Solution[elem.DOFs[i]] = u(lagrangianGlobal[i]);
                }
                ++ielem;
            }
        }

            int order = 2;
            int refinement = 4;

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Assembly assembly = Assembly.GetExecutingAssembly();

            FiniteElementsCreator.LoadFiniteElementTypes(assembly);

            MaterialCreator.LoadMaterialsAssemblyInfo(assembly);

            // Console.WriteLine("Choose dimension");

            // foreach (Dimension d in Enum.GetValues(typeof(Dimension)))
            //     Console.WriteLine($"{d} : {(int)d}");

            // Dimension dimension = (Dimension)int.Parse(Console.ReadLine()!);

            PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();

            
            string[] fileNames = ["Mesh.txt", "MeshFragmentation.txt", "Edges.txt"]; //Console.ReadLine()!.Split(' ');
            var problem = new ScalarEllipticProblem<Vector2D>()
            {
                Mesh =  builder.BuildMesh<Vector2D>(Dimension.D2, GeometryType.Triangle, BasisType.Lagrangian, order, fileNames)
            };

            for(int i = 0; i <= refinement; ++i)
                problem.Mesh = problem.Mesh.Refine();

            problem.LoadMaterials("","material4.json");

            //problem.InputUserDefinedData();

            

            

            problem.Solve();

            BuildTrueLagrangianSolution(problem,(vec) => Math.Exp((vec.X + vec.Y)/3.0));

            var points = PointsOnRectangle(new(0.05, 0.05), new(10d, 4d), new(0.1, 0.1));

            //var discr = problem.EvaluateDiscrepancy(points, (vec) => 2d * vec.X * vec.X + 3d * vec.Y * vec.Y + 6d * vec.X * vec.Y);
            var discr = problem.EvaluateDiscrepancy(points, (vec) => Math.Exp((vec.X + vec.Y)/3.0));

            //problem.Mesh.

            Console.WriteLine(discr);
        }
    }
}