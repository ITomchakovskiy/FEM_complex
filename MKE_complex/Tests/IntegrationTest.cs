using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Mesh;
using MKE_complex.Problems;
using MKE_complex.Problems.Materials;
using MKE_complex.Vector;
using Xunit;

namespace MKE_complex.Tests
{
    public class IntegrationTest
    {
        private void TestFunc(int refinement)
        {
            BasisType basis = BasisType.Hierarchical; int order = 3; double h = 0.4d;
            double Length = 5d;
            int scheme = 3;
            //var materialsfile = "materials5.json";
            var materialsfile = "Cubic.json";
            var materialsFolder = "TetrahedronHierarchical";

            Assembly assembly = Assembly.GetExecutingAssembly();

            FiniteElementsCreator.LoadFiniteElementTypes(assembly);

            MaterialCreator.LoadMaterialsAssemblyInfo(assembly);

            //var Mesh = CubeMesh2(basis, order, Length);

            var Mesh = SingleTetrahedron(basis, order, Length);

            for(int i = 0; i < refinement; ++i)
                Mesh = (FiniteElementMesh<Vector3D>)Mesh.Refine();

            Console.WriteLine("Is mesh conforming: " + Mesh.IsMeshConforming());

            var problem = new ScalarHierarchicalEllipticProblem<Vector3D>
            //var problem = new ScalarEllipticProblem<Vector3D>
            {
                Mesh = Mesh
            };

            problem.LoadMaterials(materialsFolder, materialsfile);

            problem.Solve();

            //var points = PointsOnRectangle(new(h/2d, h/2d, h/2d), new(Length-h/2d, Length-h/2d,Length-h/2d), new(h, h, h));

            double A(Vector3D point)
            {
                double x = point.X, y = point.Y, z = point.Z;
                //return point.X * point.X + 2d * point.Y * point.Y + 3d * point.Y * point.Z;
                // if(x + z <= 10d)
                //     return x*x*x + 2d*y*y*y + 3d*z*z*y;
                // return x*x*x + 2d*y*y*y + 3d*z*z*y - (3d/4d*x*x + 3d/2d*z*y)*(x+z-10);

                //return y*y*y*y;

                //return Math.Sin(x/5d) * Math.Cos(y/5d) * Math.Sin(z/5d);

                //return Math.Exp((x + y + z) / 5d);
                //return x*x*x + y*y*y + z*z*z;
                return x*x*x;
            }

            //var discr = problem.EvaluateDiscrepancy(points, A) * Math.Sqrt(Length*Length*Length);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //BuildTrueLagrangianSolution(problem,A);

            // var discr = EvaluateDiscrepancyGaussParallelepiped(new(0d,0d,0d),new(Length,Length,Length),
            //                                                   new(h,h,h), A, CalculateFunctionAtPoint);
            //double discr = Mesh.IntegrateDiscrepancy(A, problem.Solution, scheme);

            double discr = Mesh.Integrate(A, scheme);
        
            //var discr = IntegrationTest(problem, A);


            Console.WriteLine(discr);
        }

        FiniteElementMesh<Vector3D> SingleTetrahedron(BasisType basis, int order, double Length)
        {
            Vector3D[] vertices = [new(0d, 0d, 0d),
                                   new(Length, 0d, 0d),
                                   new(Length/2d, Length*Math.Sqrt(3d)/2d, 0d),
                                   new(Length/2d,Length * Math.Sqrt(3d)/6d, Length * Math.Sqrt(2d/3d))
                                   ];

            IFiniteElement<Vector3D>[] elements = [FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, basis, order, "m1",new Tetrahedron([0,1,2,3]))];
                                                   

            IBoundaryCondition<Vector3D>[] boundaries = [FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([0,1,2])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([0,1,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([0,2,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([1,2,3])),
                                                         ];
            
            return new FiniteElementMesh<Vector3D>(vertices.ToList(),elements.ToList(),boundaries.ToList());
        }
        [Fact]
        public void Test1()
        {
            int refinement = 10;
            for(int i = 0; i <= refinement; ++i)
                TestFunc(i);
        }
    }
}