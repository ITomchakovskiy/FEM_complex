// See https://aka.ms/new-console-template for more information
using MKE_complex;
using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.Matrix;
using MKE_complex.Mesh;
using MKE_complex.Mesh.MeshBuilder;
using MKE_complex.Problems;
using MKE_complex.Problems.Materials;
using MKE_complex.Vector;
using System.Globalization;
using System.Reflection;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

Assembly assembly = Assembly.GetExecutingAssembly();

FiniteElementsCreator.LoadFiniteElementTypes(assembly);

//var GeometryTypesForDimension = new Dictionary<Dimension, GeometryType[]>()
//{
//    {Dimension.D2, new GeometryType[] {GeometryType.Triangle,GeometryType.Quadrangle} },
//    {Dimension.D3, new GeometryType[] {GeometryType.Hexagon,GeometryType.Tetrahedron} },
//};

//Console.WriteLine("Choose dimension");

////foreach(Dimension d in Enum.GetValues(typeof(Dimension)))
////{
////    Console.WriteLine($"{d} : {(int)d}");
////}
//Dimension dimension = Dimension.D2; //(Dimension)int.Parse(Console.ReadLine()!);

//Console.WriteLine("Choose mesh type");

////foreach (GeometryType g in GeometryTypesForDimension[dimension])
////{
////    Console.WriteLine($"{g} : {(int)g}");
////}

//GeometryType mesh_type = GeometryType.Triangle;      //(GeometryType)int.Parse(Console.ReadLine()!);

////foreach (BasisType b in Enum.GetValues(typeof(BasisType)))
////{
////    Console.WriteLine($"{b} : {(int)b}");
////}

//BasisType basis = BasisType.Lagrangian; //(BasisType)int.Parse(Console.ReadLine()!);

////Console.WriteLine($"Choose basis order");

////int order = 3; //int.Parse(Console.ReadLine()!);
//int order = 2;

//if (order < 1) throw new Exception();

////Console.WriteLine("Type file names for mesh building");

//string[] fileNames = ["Mesh.txt", "MeshFragmentation.txt", "Edges.txt"]; //Console.ReadLine()!.Split(' '); 

//PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();

//IFiniteElementMesh<Vector2D> mesh = builder.BuildMesh<Vector2D>(dimension, mesh_type, basis, order, fileNames); //костыль

//DofsEnumerator.EnumerateMeshDofs(mesh);

//var matrix = MatrixProfileBuilder.BuildMatrixProfile<double, Vector2D>(mesh);

//if (mesh is FiniteElementMesh<Vector2D> mesh2d)
//    mesh2d.SaveMeshGeometry("input_points", "input_triangles", "input_dofs", "input_edges", "input_edgeDofs");

//Console.WriteLine("Done");

////
//Console.WriteLine(new SpecificMaterials().Iron.Lambda(new Vector2D(0d, 0d)));
//Console.WriteLine(new SpecificMaterials().Iron.Gamma(new Vector2D(0d, 0d)));
//Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(0d, 0d)));
//Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(1d, 0d)));
//Console.WriteLine(new SpecificMaterials().Iron.F(new Vector2D(0d, 2d)));

var problem = new ScalarEllipticProblem<Vector2D>();
problem.Solve();
Vector2D[] vertices = [];

double x_min = 0.5;
double x_max = 7.6;
double y_min = 0.5;
double y_max = 3.6;

//double x_min = 0.5;
//double x_max = 3.6;
//double y_min = 5.5;
//double y_max = 9.6;

double dx = 0.5;
double dy = 0.5;

for(double x = x_min; x <= x_max; x += dx)
    for(double y = y_min; y <= y_max; y += dy)
        vertices = vertices.Append(new Vector2D(x, y)).ToArray();

Func<Vector2D, double> u = (Vector2D v) => 5d * v.X + 10d * v.Y + 10;

//Func<Vector2D, double> u = (Vector2D v) => v.X + 6d * v.Y - 2d;

double discrepancy = problem.EvaluateDiscrepancy(vertices, u);
Console.WriteLine($"Discrepancy: {discrepancy:E3}");
