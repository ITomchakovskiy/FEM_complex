using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar.Lagrangian;
using MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Hierarchical;
using MKE_complex.FiniteElements.Elements.ElementsClasses._3D.Lagrangian;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;
using Xunit;

namespace MKE_complex.Tests
{
    public class LagrangianVerticesTest
    {
        [Fact]
        public void Test1()
        {
            var tetrahedron2 = new TetrahedronScalarHierarchicalFiniteElement("",new Tetrahedron([0,1,1,1]),2);

            var tetrahedron3 = new TetrahedronScalarHierarchicalFiniteElement("",new Tetrahedron([0,1,1,1]),3);

            var Vert2 = tetrahedron2.LocalLagrangianVerticesAtDofs();

            var Vert3 = tetrahedron3.LocalLagrangianVerticesAtDofs();

            Console.WriteLine("2");
            for(int i = 0; i < Vert2.Length; ++i)
            {
                for(int j = 0; j < Vert2[i].Length; ++j)
                    Console.Write($"{Vert2[i][j]} ");
                Console.Write("\n");
            }

            Console.WriteLine("3");
            for(int i = 0; i < Vert3.Length; ++i)
            {
                for(int j = 0; j < Vert3[i].Length; ++j)
                    Console.Write($"{Vert3[i][j]} ");
                Console.Write("\n");
            }

            var boundary2 = new TriangleHierarchicalBoundaryCondition("",new TriangleBoundary([0,1,1]),2);

            var boundary3 = new TriangleHierarchicalBoundaryCondition("",new TriangleBoundary([0,1,1]),3);

            Vert2 = boundary2.LagrangianVerticesAtDofs();

            Vert3 = boundary3.LagrangianVerticesAtDofs();

            Console.WriteLine("2");
            for(int i = 0; i < Vert2.Length; ++i)
            {
                for(int j = 0; j < Vert2[i].Length; ++j)
                    Console.Write($"{Vert2[i][j]} ");
                Console.Write("\n");
            }

            Console.WriteLine("3");
            for(int i = 0; i < Vert3.Length; ++i)
            {
                for(int j = 0; j < Vert3[i].Length; ++j)
                    Console.Write($"{Vert3[i][j]} ");
                Console.Write("\n");
            }
            
            //Assert.True(true);
        }
    }
}