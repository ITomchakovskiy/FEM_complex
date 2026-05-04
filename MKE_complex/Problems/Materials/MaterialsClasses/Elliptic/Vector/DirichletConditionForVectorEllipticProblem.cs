using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using Flee.PublicTypes;
using MKE_complex.Vector;

namespace MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Vector;

[Material(PDE_Type.Elliptic, MaterialType.DirichletCondition, FieldType.Vector)]
public class DirichletConditionForVectorEllipticProblem<VectorT> : IMaterial<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public string Name { get; init; }
    private IGenericExpression<double> agExpX;
    private IGenericExpression<double> agExpY;
    private IGenericExpression<double> agExpZ;
    private string[] coordinates;
    private ExpressionContext context;

    private double EvaluateExpression(IGenericExpression<double> expression, VectorT point)
    {
        int dim = point.N;
        for (int i = 0; i < dim; ++i)
            context.Variables[coordinates[i]] = point.components[i];
        return expression.Evaluate();
    }

    public double AgX(VectorT point) => EvaluateExpression(agExpX, point);
    public double AgY(VectorT point) => EvaluateExpression(agExpY, point);
    public double AgZ(VectorT point) => EvaluateExpression(agExpZ, point);
    public VectorT Ag(VectorT point)
    {
        double[] components;
        //VectorT res = point.CreateVector()
        
        components = point switch
        {
            Vector2D => [AgX(point), AgY(point)],
            Vector3D => [AgX(point), AgY(point), AgZ(point)],
            _ => throw new NotSupportedException()
        };

        return point.CreateVector(components);
    }
    

    public DirichletConditionForVectorEllipticProblem(string name, string ugX, string ugY, string ugZ, string[] coordinates)
    {
        Name = name;

        context = new ExpressionContext();
        context.Imports.AddType(typeof(Math));

        this.coordinates = coordinates;

        context.Variables[coordinates[0]] = 0d;
        context.Variables[coordinates[1]] = 0d;
        context.Variables[coordinates[2]] = 0d;

        agExpX = context.CompileGeneric<double>(ugX);
        agExpY = context.CompileGeneric<double>(ugY);
        agExpZ = context.CompileGeneric<double>(ugZ);
    }

    public DirichletConditionForVectorEllipticProblem(MaterialFileInfo fileInfo, string[] coordinates) : 
    this(fileInfo.Name, fileInfo.Functions["AgX"],fileInfo.Functions["AgY"],fileInfo.Functions["AgZ"], coordinates){}
}