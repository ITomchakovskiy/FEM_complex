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
    public class TetrahedronHierarchicalTest
    {

        FiniteElementMesh<Vector3D> Mesh1()
        {
            Vector3D[] vertices = [new(0d, 0d, 0d),
                                   new(10d, 0d, 0d),
                                   new(0d, 10d, 0d),
                                   new(0d,0d,10d)];

            IFiniteElement<Vector3D>[] elements = [FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, 3, "m",new Tetrahedron([0,1,2,3]))];

            IBoundaryCondition<Vector3D>[] boundaries = [FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([0,1,2])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([0,1,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([0,2,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([1,2,3]))];
            
            return new FiniteElementMesh<Vector3D>(vertices.ToList(),elements.ToList(),boundaries.ToList());
        }

        FiniteElementMesh<Vector3D> Mesh12()
        {
            Vector3D[] vertices = [new(0d, 0d, 0d),
                                   new(10d, 0d, 0d),
                                   new(0d, 10d, 0d),
                                   new(0d,0d,10d),
                                   new(10d,10d,10d)];

            IFiniteElement<Vector3D>[] elements = [FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, 3, "m",new Tetrahedron([0,1,2,3])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, 3, "m",new Tetrahedron([1,2,3,4]))];

            IBoundaryCondition<Vector3D>[] boundaries = [FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([0,1,2])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([0,1,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([0,2,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([4,1,2])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([4,1,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed", new TriangleBoundary([4,2,3])),
                                                         ];
            
            return new FiniteElementMesh<Vector3D>(vertices.ToList(),elements.ToList(),boundaries.ToList());
        }

        FiniteElementMesh<Vector3D> Mesh2()
        {
            Vector3D[] vertices = [new(0d, 0d, 0d),
                                   new(10d, 0d, 0d),
                                   new(0d, 10d, 0d),
                                   new(10d, 10d, 0d),
                                   new(0d, 0d, 10d),
                                   new(10d, 0d, 10d)];

            IFiniteElement<Vector3D>[] elements = [FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, 3, "m1",new Tetrahedron([0,2,3,4])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, 3, "m1",new Tetrahedron([0,1,3,4])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, 3, "m2",new Tetrahedron([1,3,4,5]))];

            IBoundaryCondition<Vector3D>[] boundaries = [FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed1", new TriangleBoundary([0,2,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed1", new TriangleBoundary([0,1,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed2", new TriangleBoundary([0,2,4])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed3", new TriangleBoundary([1,3,5])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed4", new TriangleBoundary([0,1,4])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed5", new TriangleBoundary([1,4,5])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed6", new TriangleBoundary([2,3,4])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, 3, "ed7", new TriangleBoundary([3,4,5]))];
            
            return new FiniteElementMesh<Vector3D>(vertices.ToList(),elements.ToList(),boundaries.ToList());
        }

        FiniteElementMesh<Vector3D> Mesh3()
        {
            int order = 3;
            Vector3D[] vertices = [new(0d, 0d, 0d),
                                   new(10d, 0d, 0d),
                                   new(0d, 10d, 0d),
                                   new(10d, 10d, 0d),
                                   new(0d, 0d, 10d),
                                   new(10d, 0d, 10d),
                                   new(0d,10d,10d),
                                   new(10d,10d,10d)];

            IFiniteElement<Vector3D>[] elements = [FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, order, "m1",new Tetrahedron([0,2,3,4])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, order, "m1",new Tetrahedron([0,1,3,4])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, order, "m1",new Tetrahedron([1,3,4,5])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, order, "m1",new Tetrahedron([2,3,4,6])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, order, "m1",new Tetrahedron([3,4,5,7])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, BasisType.Hierarchical, order, "m1",new Tetrahedron([3,4,6,7]))];

            IBoundaryCondition<Vector3D>[] boundaries = [FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([0,2,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([0,1,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([0,2,4])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([1,3,5])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([0,1,4])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([1,4,5])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([2,4,6])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([3,5,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([2,3,6])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([3,6,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([4,5,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, BasisType.Hierarchical, order, "ed1", new TriangleBoundary([4,6,7])),];
            
            return new FiniteElementMesh<Vector3D>(vertices.ToList(),elements.ToList(),boundaries.ToList());
        }

        FiniteElementMesh<Vector3D> Mesh4()
        {
            int order = 2;
            Vector3D[] vertices = [new(0d, 0d, 0d),
                                   new(5d, 0d, 0d),
                                   new(0d, 5d, 0d),
                                   new(5d, 5d, 0d),
                                   new(0d, 0d, 5d),
                                   new(5d, 0d, 5d),
                                   new(0d,5d,5d),
                                   new(5d,5d,5d)];
            var basis = BasisType.Lagrangian;

            IFiniteElement<Vector3D>[] elements = [FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, basis, order, "m1",new Tetrahedron([0,1,2,4])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, basis, order, "m1",new Tetrahedron([1,2,3,7])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, basis, order, "m1",new Tetrahedron([1,4,5,7])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, basis, order, "m1",new Tetrahedron([1,2,4,7])),
                                                   FiniteElementsCreator.CreateFiniteElement(GeometryType.Tetrahedron, basis, order, "m1",new Tetrahedron([2,4,6,7])),
                                       ];

            IBoundaryCondition<Vector3D>[] boundaries = [FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([0,2,4])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([2,4,6])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([1,5,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([1,3,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([0,1,4])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([1,4,5])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([2,3,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([2,6,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([0,1,2])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([1,2,3])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([4,6,7])),
                                                         FiniteElementsCreator.CreateBoundaryCondition(GeometryType.TriangleBoundary, basis, order, "ed1", new TriangleBoundary([4,5,7])),];
            
            return new FiniteElementMesh<Vector3D>(vertices.ToList(),elements.ToList(),boundaries.ToList());
        }

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

    // public double EvaluateDiscrepancyGaussParallelepiped(Vector3D A, Vector3D B, Vector3D H, Func<Vector3D,double> u, Агтс)
    // {
    //     double[] localPoints1D = [-Math.Sqrt(3d/5d), 0d, Math.Sqrt(3d/5d) ];
    //     double[] weights1D = [5d/9d,8d/9d,5d/9d];

    //     Vector3D LocalPointToGlobal(Vector3D local, Vector3D a, Vector3D b)
    //     {
    //         var local01 = (local + new Vector3D(1d,1d,1d)) / 2d;
    //         Vector3D point = new(local01.X*(b.X-a.X) + a.X,
    //                              local01.Y*(b.Y-a.Y) + a.Y,
    //                              local01.Z*(b.Z-a.Z) + a.Z);
    //         return point;
    //     }

    //     Vector3D[] localPoints = new Vector3D[localPoints1D.Length * localPoints1D.Length * localPoints1D.Length];

    //     double[] weights = new double[weights1D.Length * weights1D.Length * weights1D.Length];

    //     for(int i = 0; i < localPoints1D.Length; ++i)
    //     {
    //         for(int j = 0; j < localPoints1D.Length; ++j)
    //         {
    //             for(int p = 0; p < localPoints1D.Length; ++p)
    //             {
    //                 localPoints[i*localPoints1D.Length*localPoints1D.Length + j*localPoints1D.Length + p] = new(localPoints1D[p], 
    //                                                                                                             localPoints1D[j],
    //                                                                                                             localPoints1D[i]);
    //                 weights[i*localPoints1D.Length*localPoints1D.Length + j*localPoints1D.Length + p] = weights1D[i]*
    //                                                                                                     weights1D[j]*
    //                                                                                                     weights1D[p];
    //             }
    //         }
    //     }

    //     int Nx = (int)((B.X - A.X) / H.X);
    //     int Ny = (int)((B.Y - A.Y) / H.Y);
    //     int Nz = (int)((B.Z - A.Z) / H.Z);

    //     H = new((B.X - A.X) / Nx, 
    //             (B.Y - A.Y) / Ny, 
    //             (B.Z - A.Z) / Nz);

    //     double discrepancy = 0;

    //     double[] discrepancies = new double[Nz];

    //     //for(int i = 0; i < Nz; ++i)
    //     Parallel.For(0, Nz, i =>
    //     {
    //         double Z = A.Z + H.Z * i;
    //         for(int j = 0; j < Ny; ++j)
    //         {
    //             double Y = A.Y + H.Y * j;
    //             for(int p = 0; p < Nx; ++p)
    //             {
    //                 double X = A.X + H.X * p;

    //                 Vector3D a = new(X,Y,Z);
    //                 Vector3D b = a + H;
    //                 double localDiscrepancy = 0d;
    //                 for(int q = 0; q < localPoints.Length; ++q)
    //                 {
    //                     var point = LocalPointToGlobal(localPoints[q],a,b);
    //                     //VectorT pointT;
    //                     var weight = weights[q];

    //                     if(point is VectorT pointT)
    //                     {
    //                         if (CalculateFunctionAtPoint(pointT, out double value))
    //                         {
    //                             //Console.WriteLine($" {pointT.AsString(format, separator)}{separator}{value.ToString(format)}{separator}{u(point).ToString(format)}{separator}{Math.Abs(value - u(point)):E3}");
    //                             localDiscrepancy += (value - u(point)) * (value - u(point)) * weight;
    //                         }
    //                     }
    //                 }

    //                 discrepancies[i] += localDiscrepancy;

    //                 //discrepancy += localDiscrepancy;
    //             }
    //         }
    //     });

    //     discrepancy = Math.Sqrt(discrepancies.Sum() * H.X * H.Y * H.Z / 8d);

    //     return discrepancy;
    // }


        [Fact]
        public void Test1()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            FiniteElementsCreator.LoadFiniteElementTypes(assembly);

            MaterialCreator.LoadMaterialsAssemblyInfo(assembly);

            var Mesh = Mesh4();

            int refinement = 3;

            for(int i = 0; i < refinement; ++i)
                Mesh = (FiniteElementMesh<Vector3D>)Mesh.Refine();

            Console.WriteLine(Mesh.IsMeshConforming());

            //var problem = new ScalarHierarchicalEllipticProblem<Vector3D>
            var problem = new ScalarEllipticProblem<Vector3D>
            {
                Mesh = Mesh
            };

            problem.Solve();

            var points = PointsOnRectangle(new(0.1, 0.1, 0.1), new(9.9, 9.9, 9.9), new(0.2, 0.2, 0.2));

            double A(Vector3D point)
            {
                double x = point.X, y = point.Y, z = point.Z;
                //return point.X * point.X + 2d * point.Y * point.Y + 3d * point.Y * point.Z;
                // if(x + z <= 10d)
                //     return x*x*x + 2d*y*y*y + 3d*z*z*y;
                // return x*x*x + 2d*y*y*y + 3d*z*z*y - (3d/4d*x*x + 3d/2d*z*y)*(x+z-10);

                //return y*y*y*y;

                //return Math.Sin(x/5d) * Math.Cos(y/5d) * Math.Sin(z/5d);

                return Math.Exp((x + y + z) / 5d);
            }

            //var discr = problem.EvaluateDiscrepancy(points, A);
            double h = 0.2;

            // var discr = problem.EvaluateDiscrepancyGaussParallelepiped(new(0d,0d,0d),new(10d,10d,10d),
            //                                                            new(h,h,h), A);
            var discr = problem.EvaluateDiscrepancyGaussParallelepiped(new(0d,0d,0d),new(5d,5d,5d),
                                                                       new(h,h,h), A);

            //problem.Mesh.

            Console.WriteLine(discr);
        }
    }
}