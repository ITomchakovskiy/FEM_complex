using Flee.PublicTypes;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;

[MaterialAttribute(PDE_Type.Elliptic, MaterialType.DirichletCondition,FieldType.Scalar)]

public class DirichletConditionForScalarEllipticProblem<VectorT> : IMaterial<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public string Name { get; init; }

    private IGenericExpression<double> ugExp;

    private string[] coordinates;

    private ExpressionContext context;

    private double EvaluateExpression(IGenericExpression<double> expression, VectorT point)
    {
        int dim = point.N;
        for (int i = 0; i < dim; ++i)
            context.Variables[coordinates[i]] = point.components[i];
        return expression.Evaluate();
    }

    public double Ug(VectorT point) => EvaluateExpression(ugExp, point);

    public DirichletConditionForScalarEllipticProblem(string name, string ug, string[] coordinates)
    {
        Name = name;

        context = new ExpressionContext();
        context.Imports.AddType(typeof(Math));

        this.coordinates = coordinates;

        context.Variables[coordinates[0]] = 0d;
        context.Variables[coordinates[1]] = 0d;
        context.Variables[coordinates[2]] = 0d;

        ugExp = context.CompileGeneric<double>(ug);
    }

    public DirichletConditionForScalarEllipticProblem(MaterialFileInfo fileInfo, string[] coordinates) : 
    this(fileInfo.Name, fileInfo.Functions["Ug"], coordinates){}
}
