using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Flee.PublicTypes;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar.Lagrangian;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
using MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Hierarchical;
using MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Lagrangian;
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

        FiniteElementMesh<Vector3D> CubeMesh2(BasisType basis, int order, double Length)
        {
            Vector3D[] vertices = [new(0d, 0d, 0d),
                                   new(Length, 0d, 0d),
                                   new(0d, Length, 0d),
                                   new(Length, Length, 0d),
                                   new(0d, 0d, Length),
                                   new(Length, 0d, Length),
                                   new(0d,Length,Length),
                                   new(Length,Length,Length)];

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

        public double EvaluateDiscrepancyGaussParallelepiped(Vector3D A, Vector3D B, Vector3D H, Func<Vector3D,double> u, Func<Vector3D, (bool isCalculated, double value)> CalculateFunctionAtPoint)
        {
            double[] localPoints1D = [-Math.Sqrt(3d/5d), 0d, Math.Sqrt(3d/5d) ];
            double[] weights1D = [5d/9d,8d/9d,5d/9d];

            Vector3D LocalPointToGlobal(Vector3D local, Vector3D a, Vector3D b)
            {
                var local01 = (local + new Vector3D(1d,1d,1d)) / 2d;
                Vector3D point = new(local01.X*(b.X-a.X) + a.X,
                                     local01.Y*(b.Y-a.Y) + a.Y,
                                     local01.Z*(b.Z-a.Z) + a.Z);
                return point;
            }

            Vector3D[] localPoints = new Vector3D[localPoints1D.Length * localPoints1D.Length * localPoints1D.Length];

            double[] weights = new double[weights1D.Length * weights1D.Length * weights1D.Length];

            for(int i = 0; i < localPoints1D.Length; ++i)
            {
                for(int j = 0; j < localPoints1D.Length; ++j)
                {
                    for(int p = 0; p < localPoints1D.Length; ++p)
                    {
                        localPoints[i*localPoints1D.Length*localPoints1D.Length + j*localPoints1D.Length + p] = new(localPoints1D[p], 
                                                                                                                    localPoints1D[j],
                                                                                                                    localPoints1D[i]);
                        weights[i*localPoints1D.Length*localPoints1D.Length + j*localPoints1D.Length + p] = weights1D[i]*
                                                                                                            weights1D[j]*
                                                                                                            weights1D[p];
                    }
                }
            }

            int Nx = (int)((B.X - A.X) / H.X);
            int Ny = (int)((B.Y - A.Y) / H.Y);
            int Nz = (int)((B.Z - A.Z) / H.Z);

            H = new((B.X - A.X) / Nx, 
                    (B.Y - A.Y) / Ny, 
                    (B.Z - A.Z) / Nz);

            double discrepancy = 0;

            double[] discrepancies = new double[Nz];

            //for(int i = 0; i < Nz; ++i)
            Parallel.For(0, Nz, i =>
            {
                double Z = A.Z + H.Z * i;
                for(int j = 0; j < Ny; ++j)
                {
                    double Y = A.Y + H.Y * j;
                    for(int p = 0; p < Nx; ++p)
                    {
                        double X = A.X + H.X * p;

                        Vector3D a = new(X,Y,Z);
                        Vector3D b = a + H;
                        double localDiscrepancy = 0d;
                        for(int q = 0; q < localPoints.Length; ++q)
                        {
                            var point = LocalPointToGlobal(localPoints[q],a,b);
                            //VectorT pointT;
                            var weight = weights[q];

                            var uNumeric = CalculateFunctionAtPoint(point);

                            if (uNumeric.isCalculated)
                            {
                                //Console.WriteLine($" {pointT.AsString(format, separator)}{separator}{value.ToString(format)}{separator}{u(point).ToString(format)}{separator}{Math.Abs(value - u(point)):E3}");
                                localDiscrepancy += (uNumeric.value - u(point)) * (uNumeric.value - u(point)) * weight;
                            }
                        }

                        discrepancies[i] += localDiscrepancy;

                        //discrepancy += localDiscrepancy;
                    }
                }
            });

            discrepancy = Math.Sqrt(discrepancies.Sum() * H.X * H.Y * H.Z / 8d);

            return discrepancy;
        }

        private void InitQuadratures(int num)
        {
            switch(num)
            {
                case 1:
                    {
                        p1 = [ 1.0 / 4.0, 1.0 / 2.0, 1.0 / 6.0, 1.0 / 6.0, 1.0 / 6.0 ];
                        p2 = [ 1.0 / 4.0, 1.0 / 6.0, 1.0 / 2.0, 1.0 / 6.0, 1.0 / 6.0 ];
                        p3 = [ 1.0 / 4.0, 1.0 / 6.0, 1.0 / 6.0, 1.0 / 2.0, 1.0 / 6.0 ];
                        w = [ -4.0 / 5.0, 9.0 / 20.0, 9.0 / 20.0, 9.0 / 20.0, 9.0 / 20.0 ];
                        w = w.Select(i => i/6d).ToArray();
                        break;
                    }
                case 2:
                    {
                        p1 = [1d/4d, 0, 1d/3d, 1d/3d, 1d/3d, 8d/11d, 1d/11d, 1d/11d, 1d/11d, 0.066550153573664, 0.066550153573664, 0.433449846426336, 0.433449846426336, 0.066550153573664, 0.433449846426336];
                        p2 = [1d/4d, 1d/3d, 0, 1d/3d, 1d/3d, 1d/11d, 8d/11d, 1d/11d, 1d/11d, 0.066550153573664, 0.433449846426336, 0.433449846426336, 0.066550153573664, 0.433449846426336, 0.066550153573664];
                        p3 = [1d/4d, 1d/3d, 1d/3d, 0, 1d/3d, 1d/11d, 1d/11d, 8d/11d, 1d/11d, 0.433449846426336, 0.433449846426336, 0.066550153573664, 0.066550153573664, 0.066550153573664, 0.433449846426336];
                        w = [ 0.030283678097089,
                         0.006026785714286, 0.006026785714286, 0.006026785714286, 0.006026785714286,
                         0.011645249086029, 0.011645249086029, 0.011645249086029, 0.011645249086029,
                         0.010949141561386, 0.010949141561386, 0.010949141561386, 0.010949141561386, 0.010949141561386, 0.010949141561386 ];
                        break;
                    }
                case 3:
                    {
                        const double w1 = 0.665379170969464506e-2;
        const double w2 = 0.167953517588677620e-2;
        const double w3 = 0.922619692394239843e-2;
        const double w4 = 0.803571428571428248e-2;

        const double x1a = 0.214602871259151684;
        const double x1b = 0.356191386222544953;

        const double x2a = 0.406739585346113397e-1;
        const double x2b = 0.877978124396165982;

        const double x3a = 0.322337890142275646;
        const double x3b = 0.329863295731730594e-1;

        const double x4a = 0.636610018750175299e-1;
        const double x4b = 0.269672331458315867;
        const double x4c = 0.603005664791649076;

        p1 = [ x1a, x1a, x1a, x1b, x2a, x2a, x2a, x2b, x3a, x3a, x3a, x3b, x4a, x4a, x4a, x4a, x4b, x4c, x4a, x4a, x4b, x4b, x4c, x4c ];
        p2 = [ x1a, x1a, x1b, x1a, x2a, x2a, x2b, x2a, x3a, x3a, x3b, x3a, x4a, x4a, x4b, x4c, x4a, x4a, x4b, x4c, x4a, x4c, x4a, x4b ];
        p3 = [ x1a, x1b, x1a, x1a, x2a, x2b, x2a, x2a, x3a, x3b, x3a, x3a, x4b, x4c, x4a, x4a, x4a, x4a, x4c, x4b, x4c, x4a, x4b, x4a ];
        w = [ w1, w1, w1, w1, w2, w2, w2, w2, w3, w3, w3, w3, w4, w4, w4, w4, w4, w4, w4, w4, w4, w4, w4, w4 ];
        break;
                    }
            }

            p4 = new double[w.Length];
            for(int i = 0; i < w.Length; ++i)
            {
                p4[i] = 1d - p1[i] - p2[i] - p3[i];
            }
        }
        private double[] p1;
        private double[] p2;
        private double[] p3;

        private double[] p4;

 //double[] w = { -4.0 / 5.0, 9.0 / 20.0, 9.0 / 20.0, 9.0 / 20.0, 9.0 / 20.0 };
        private double[] w;
        private double integrateTetrahedron(ScalarHierarchicalEllipticProblem<Vector3D> problem, IFiniteElement3D tetrElem, Func<Vector3D,double> u)
        {
            double[][] localPoints = new double[p1.Length][];

            for(int i = 0; i < localPoints.Length; ++i)
                localPoints[i] = [p1[i], p2[i], p3[i], p4[i]];

            var tetrVertices = tetrElem.Geometry.VertexNumber.Select(j => problem.Mesh.Vertices[j]).ToArray();

            var GlobalPoints = localPoints.Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(tetrVertices, i)).ToArray();

            var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(tetrVertices);

            double discrepancy = 0d;

            var localSolution = tetrElem.DOFs.Select(i => problem.Solution[i]).ToArray();

            if(tetrElem is IFiniteElementScalarEllipticProblemCalculation<Vector3D> elem)
            {
                for(int i = 0; i < w.Length; ++i)
                {
                    double value = elem.CalcResultAtPoint(tetrVertices, localSolution, GlobalPoints[i]);
                    discrepancy += w[i] * (value - u(GlobalPoints[i])) * (value - u(GlobalPoints[i]));
                }
                    
            }

            return discrepancy * AbsDetD;
        }

        private double integrateTetrahedron(ScalarEllipticProblem<Vector3D> problem, IFiniteElement3D tetrElem, Func<Vector3D,double> u)
        {
            double[][] localPoints = new double[p1.Length][];

            for(int i = 0; i < localPoints.Length; ++i)
                localPoints[i] = [p1[i], p2[i], p3[i], p4[i]];

            var tetrVertices = tetrElem.Geometry.VertexNumber.Select(j => problem.Mesh.Vertices[j]).ToArray();

            var GlobalPoints = localPoints.Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(tetrVertices, i)).ToArray();

            var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(tetrVertices);

            double discrepancy = 0d;

            var localSolution = tetrElem.DOFs.Select(i => problem.Solution[i]).ToArray();

            if(tetrElem is TetrahedronScalarLagrangianFiniteElement eleml)
            {
                for(int i = 0; i < w.Length; ++i)
                {
                    double value = eleml.CalcResultAtPointLocal(localSolution, [p1[i], p2[i], p3[i], p4[i]]);
                    discrepancy += w[i] * (value - u(GlobalPoints[i])) * (value - u(GlobalPoints[i]));
                }
            }
            else if(tetrElem is TetrahedronScalarHierarchicalFiniteElement elemh)
            {
                for(int i = 0; i < w.Length; ++i)
                {
                    double value = elemh.CalcResultAtPointLocal(localSolution, [p1[i], p2[i], p3[i], p4[i]]);
                    discrepancy += w[i] * (value - u(GlobalPoints[i])) * (value - u(GlobalPoints[i]));
                }
            }
            // else if(tetrElem is IFiniteElementScalarEllipticProblemCalculation<Vector3D> elem)
            // {
            //     for(int i = 0; i < w.Length; ++i)
            //     {
            //         double value = elem.CalcResultAtPoint(tetrVertices, localSolution, GlobalPoints[i]);
            //         discrepancy += w[i] * (value - u(GlobalPoints[i])) * (value - u(GlobalPoints[i]));
            //     }
                    
            // }

            return discrepancy * AbsDetD;
        }

        private double TetrIntegrationTest(ScalarEllipticProblem<Vector3D> problem, IFiniteElement3D tetrElem, Func<Vector3D,double> u)
        {   
            double[][] localPoints = new double[p1.Length][];

            for(int i = 0; i < localPoints.Length; ++i)
                localPoints[i] = [p1[i], p2[i], p3[i], p4[i]];

            var tetrVertices = tetrElem.Geometry.VertexNumber.Select(j => problem.Mesh.Vertices[j]).ToArray();

            var GlobalPoints = localPoints.Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(tetrVertices, i)).ToArray();

            var AbsDetD = TetrahedronLocalCoordinates.Alpha.CalcAbsDetD(tetrVertices);

            double discrepancy = 0d;

            var localSolution = tetrElem.DOFs.Select(i => problem.Solution[i]).ToArray();

            if(tetrElem is TetrahedronScalarLagrangianFiniteElement eleml)
            {
                for(int i = 0; i < w.Length; ++i)
                {
                    double value = eleml.CalcResultAtPointLocal(localSolution, [p1[i], p2[i], p3[i], p4[i]]);
                    discrepancy += w[i] * u(GlobalPoints[i]);
                }
            }
            else if(tetrElem is TetrahedronScalarHierarchicalFiniteElement elemh)
            {
                for(int i = 0; i < w.Length; ++i)
                {
                    double value = elemh.CalcResultAtPointLocal(localSolution, [p1[i], p2[i], p3[i], p4[i]]);
                   discrepancy += w[i] * u(GlobalPoints[i]);
                }
            }
            // else if(tetrElem is IFiniteElementScalarEllipticProblemCalculation<Vector3D> elem)
            // {
            //     for(int i = 0; i < w.Length; ++i)
            //     {
            //         double value = elem.CalcResultAtPoint(tetrVertices, localSolution, GlobalPoints[i]);
            //         discrepancy += w[i] * (value - u(GlobalPoints[i])) * (value - u(GlobalPoints[i]));
            //     }
                    
            // }

            return discrepancy * AbsDetD;
        }

        private double CalcDiscrepancy(ScalarHierarchicalEllipticProblem<Vector3D> problem, Func<Vector3D,double> u)
        {
            double discrepancy = 0d;

            foreach(var elem in problem.Mesh.Elements.ToArray().OfType<IFiniteElement3D>())
            {
                discrepancy += integrateTetrahedron(problem, elem, u);
            }

            return Math.Sqrt(discrepancy);
        }

        private double IntegrationTest(ScalarEllipticProblem<Vector3D> problem, Func<Vector3D,double> u)
        {
            double discrepancy = 0d;

            foreach(var elem in problem.Mesh.Elements.ToArray().OfType<IFiniteElement3D>())
            {
                discrepancy += TetrIntegrationTest(problem, elem, u);
            }

            return discrepancy;
        }


        private void BuildTrueLagrangianSolution(ScalarEllipticProblem<Vector3D> problem, Func<Vector3D,double> u)
        {
            var elements = problem.Mesh.Elements;
            int ielem = 0;
            for(int i = 0; i < problem.Solution.Length; ++i) problem.Solution[i] = 0d;
            foreach(var elem in elements.ToArray().OfType<TetrahedronScalarLagrangianFiniteElement>())
            {
                //var lagrangianLocal = elem.LocalLagrangianVerticesAtDofs();
                var vertices = elem.Geometry.VertexNumber.Select(i => problem.Mesh.Vertices[i]).ToArray();
                var A = vertices[0]; var B = vertices[1]; var C = vertices[2]; var D = vertices[3];
                //var lagrangianGlobal = lagrangianLocal.Select(i => TetrahedronLocalCoordinates.LocalCoordinatesToGlobal(vertices, i)).ToArray();
                Vector3D[] lagrangianGlobal = [A,B,C,D,(A+B)/2d,(A+C)/2d,(A+D)/2d,(B+C)/2d,(B+D)/2d,(C+D)/2d];
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

        private double CalcDiscrepancy(ScalarEllipticProblem<Vector3D> problem, Func<Vector3D,double> u)
        {
            double discrepancy = 0d;

            for(int i = 0; i < problem.Mesh.Elements.Length; ++i)
            {
                discrepancy += integrateTetrahedron(problem, (IFiniteElement3D)problem.Mesh.Elements[i], u);
            }

            return Math.Sqrt(discrepancy);
        }

        private void TestFunc(int refinement)
        {
            BasisType basis = BasisType.Hierarchical; int order = 1; double h = 0.4d;
            double Length = 5d;
            //var materialsfile = "materials5.json";
            var materialsfile = "Cubic.json";
            var materialsFolder = "TetrahedronHierarchical";

            InitQuadratures(3);
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

            var points = PointsOnRectangle(new(h/2d, h/2d, h/2d), new(Length-h/2d, Length-h/2d,Length-h/2d), new(h, h, h));

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

            (bool isCalculated, double value) CalculateFunctionAtPoint(Vector3D point)
            {
                var isCalculated = problem.CalculateFunctionAtPoint(point, out double value);
                return (isCalculated, value);
            }

            // var discr = EvaluateDiscrepancyGaussParallelepiped(new(0d,0d,0d),new(Length,Length,Length),
            //                                                   new(h,h,h), A, CalculateFunctionAtPoint);

            var discr = CalcDiscrepancy(problem, A);
            //var discr = IntegrationTest(problem, A);


            Console.WriteLine(discr);
        }

        [Fact]
        public void Test1()
        {
            // int refinement = 4;
            // TestFunc(refinement);
        }

        [Fact]
        public void Test2()
        {
            int refinement = 10;
            for(int i = 0; i <= refinement; ++i)
                TestFunc(i);
        }

        
    }
}