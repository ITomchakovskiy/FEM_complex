using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;
using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.FiniteElements.Elements.ElementsClasses._3D.VectorHierarchical;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Mesh;
using MKE_complex.Problems.Materials;
using MKE_complex.Vector;
using Xunit;

namespace MKE_complex.Tests
{
    public class ParallelepipedVectorHierarchicalFiniteElementDofsEnumerationTest
    {
        [Fact]
        public void Test1()
        {
            int order = 3;

            double[] X = [0d, 5d, 10d];
            double[] Y = [0d,6d, 11d];
            double[] Z = [1d, 4d , 9d];

            List<Vector3D> points = [];

            foreach(var z in Z)
            {
                foreach(var y in Y)
                {
                    foreach(var x in X)
                        points.Add(new(x,y,z));
                }
            }
            List<int[]> numbers = [[0, 1, 3, 4, 9, 10, 12, 13]];
            int[] shifts = [1, 3, 4, 9, 10, 12, 13];
            foreach(var shift in shifts)
            {
                var arr = numbers[0].ToArray();
                for(int j = 0; j < 8; ++j)
                    arr[j] += shift;
                numbers.Add(arr);
            }
            //var geometryType = GeometryType.Parallelepiped;
            //var 
            //FiniteElementsCreator.CreateFiniteElement(GeometryType.Parallelepiped, BasisType.VectorHierarchical, order, "",)
            Assembly assembly = Assembly.GetExecutingAssembly();

            FiniteElementsCreator.LoadFiniteElementTypes(assembly);

            MaterialCreator.LoadMaterialsAssemblyInfo(assembly);

                
            var geometries = numbers.Select(i => new Parallelepiped(i));

            var elems = geometries.Select(i => FiniteElementsCreator.CreateFiniteElement(GeometryType.Parallelepiped, BasisType.VectorHierarchical, order, "", i));

            var mesh = new FiniteElementMesh<Vector3D>(points, elems.ToList(), new List<IBoundaryCondition<Vector3D>>());

            DofsEnumerator.EnumerateMeshDofs(mesh);
        }
    }
}