// See https://aka.ms/new-console-template for more information
using MKE_complex;
using MKE_complex.Mesh.MeshBuilder;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

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

foreach (GeometryType d in GeometryTypesForDimension[dimension])
{
    Console.WriteLine($"{d} : {(int)d}");
}

GeometryType mesh_type = (GeometryType)int.Parse(Console.ReadLine()!);

Console.WriteLine("Type file names for mesh building");

string[] fileNames = Console.ReadLine()!.Split(' ');

PseudoRegularMeshBuilder builder = new PseudoRegularMeshBuilder();

//builder.ReadFile(fileNames[0], fileNames[1], dimension);

