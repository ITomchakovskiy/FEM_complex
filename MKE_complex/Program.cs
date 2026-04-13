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
using MKE_complex.Problems;
using MKE_complex.Problems.Materials;
using MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;
using MKE_complex.Vector;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;


Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

Assembly assembly = Assembly.GetExecutingAssembly();

FiniteElementsCreator.LoadFiniteElementTypes(assembly);

MaterialCreator.LoadMaterialsAssemblyInfo(assembly);

Console.WriteLine("Choose dimension");

foreach(Dimension d in Enum.GetValues(typeof(Dimension)))
           Console.WriteLine($"{d} : {(int)d}");
        
Dimension dimension = (Dimension)int.Parse(Console.ReadLine()!);

object problem;

switch (dimension)
{
    case Dimension.D1:
        problem = new ScalarEllipticProblem<Vector1D>();
        break;
    case Dimension.D2:
        problem = new ScalarEllipticProblem<Vector2D>();
        break;
    case Dimension.D3:
        problem = new ScalarEllipticProblem<Vector3D>();
        break;
    default:
        throw new Exception();
}

var problemType = problem.GetType();

if (problemType.IsGenericType && 
    problemType.GetGenericTypeDefinition() == typeof(ScalarEllipticProblem<>))
{
    
    problemType.GetMethod("Solve")!.Invoke(problem, null);
}
