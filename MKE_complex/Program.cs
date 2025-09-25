// See https://aka.ms/new-console-template for more information
using MKE_complex;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.Mesh;
using MKE_complex.Mesh.MeshBuilder;
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

//Console.Write($"Choose dimension\n" +
//              $"1D : {Dimension.D1}\n" +
//              $"2D : {Dimension.D2}\n" +
//              $"3D : {Dimension.D3}\n");
Console.WriteLine("Choose dimension");

foreach(Dimension d in Enum.GetValues(typeof(Dimension)))
{
    Console.WriteLine($"{d} : {(int)d}");
}
Dimension dimension = (Dimension)int.Parse(Console.ReadLine()!);

Console.WriteLine("Choose mesh type");

foreach (GeometryType g in GeometryTypesForDimension[dimension])
{
    Console.WriteLine($"{g} : {(int)g}");
}

GeometryType mesh_type = (GeometryType)int.Parse(Console.ReadLine()!);

foreach (BasisType b in Enum.GetValues(typeof(BasisType)))
{
    Console.WriteLine($"{b} : {(int)b}");
}

BasisType basis = (BasisType)int.Parse(Console.ReadLine()!);

Console.WriteLine($"Choose basis order");

int order = int.Parse(Console.ReadLine()!);

if (order < 1) throw new Exception();

Console.WriteLine("Type file names for mesh building");

string[] fileNames = Console.ReadLine()!.Split(' ');

PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();

IFiniteElementMesh<Vector2D> mesh = builder.BuildMesh<Vector2D>(dimension,mesh_type,basis,order,fileNames); //костыль

