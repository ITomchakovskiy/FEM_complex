// See https://aka.ms/new-console-template for more information
using MKE_complex;
using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Matrix;
using MKE_complex.Mesh;
using MKE_complex.Mesh.MeshBuilder;
using MKE_complex.Problems.Materials;
using MKE_complex.Vector;
using System.Globalization;
using System.Reflection;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

Assembly assembly = Assembly.GetExecutingAssembly();

FiniteElementsCreator.LoadFiniteElementTypes(assembly);

var GeometryTypesForDimension = new Dictionary<Dimension, GeometryType[]>()
{
    {Dimension.D2, new GeometryType[] {GeometryType.Triangle,GeometryType.Quadrangle} },
    {Dimension.D3, new GeometryType[] {GeometryType.Hexagon,GeometryType.Tetrahedron} },
};

Console.WriteLine("Choose dimension");

//foreach(Dimension d in Enum.GetValues(typeof(Dimension)))
//{
//    Console.WriteLine($"{d} : {(int)d}");
//}
Dimension dimension = Dimension.D2; //(Dimension)int.Parse(Console.ReadLine()!);

Console.WriteLine("Choose mesh type");

//foreach (GeometryType g in GeometryTypesForDimension[dimension])
//{
//    Console.WriteLine($"{g} : {(int)g}");
//}

GeometryType mesh_type = GeometryType.Triangle;      //(GeometryType)int.Parse(Console.ReadLine()!);

//foreach (BasisType b in Enum.GetValues(typeof(BasisType)))
//{
//    Console.WriteLine($"{b} : {(int)b}");
//}

BasisType basis = BasisType.Lagrangian; //(BasisType)int.Parse(Console.ReadLine()!);

//Console.WriteLine($"Choose basis order");

int order = 4; //int.Parse(Console.ReadLine()!);

if (order < 1) throw new Exception();

//Console.WriteLine("Type file names for mesh building");

string[] fileNames = ["Mesh.txt", "MeshFragmentation.txt", "Edges.txt"]; //Console.ReadLine()!.Split(' '); 

PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();

IFiniteElementMesh<Vector2D> mesh = builder.BuildMesh<Vector2D>(dimension,mesh_type,basis,order,fileNames); //костыль

DofsEnumerator.EnumerateMeshDofs(mesh);

var matrix = MatrixProfileBuilder.BuildMatrixProfile<double, Vector2D>(mesh);

if (mesh is FiniteElementMesh<Vector2D> mesh2d)
    mesh2d.SaveMeshGeometry("input_points", "input_triangles", "input_dofs", "input_edges", "input_edgeDofs");

Console.WriteLine("Done");
//
Console.WriteLine(new SpecificMaterials().Iron.Lambda(new Vector2D(0d,0d)));
Console.WriteLine(new SpecificMaterials().Iron.Gamma(new Vector2D(0d, 0d)));
Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(0d, 0d)));
Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(1d, 0d)));
Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(0d, 2d)));

