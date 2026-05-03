using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.Mesh.MeshBuilder;
using Xunit;

namespace MKE_complex.Tests
{
    public class ParallelepipedRegularMeshTests
    {
        [Fact]
        public void Test1()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            FiniteElementsCreator.LoadFiniteElementTypes(assembly);
            
            string directory = "./input/MeshTest1";

            string[] filenames = ["Mesh", "Fragmentation", "Boundary"];
            filenames = filenames.Select(i => Path.Combine(directory,i)).ToArray();

            var builder = new RegularParallelepipedMeshBuilder();

            var mesh = builder.BuildMesh(Dimension.D3,GeometryType.Parallelepiped,BasisType.VectorHierarchical,2,filenames);

            DofsEnumerator.EnumerateMeshDofs(mesh);

        }
    }
}