using Flee.PublicTypes;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;

[Material(PDE_Type.Elliptic, MaterialType.Solid, FieldType.Scalar)]

public class SolidMaterialForScalarEllipticProblem<VectorT> : IMaterial<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public string Name { get; init; }

    private IGenericExpression<double> lambdaExp;

    private IGenericExpression<double> gammaExp;

    private IGenericExpression<double> fExp;

    private string[] coordinates;

    private ExpressionContext context;

    private double EvaluateExpression(IGenericExpression<double> expression, VectorT point)
    {
        int dim = point.N;
        for(int i = 0;i<dim;++i)
            context.Variables[coordinates[i]] = point.components[i];
        return expression.Evaluate();
    }

    public double Lambda(VectorT point) => EvaluateExpression(lambdaExp, point);

    public double Gamma(VectorT point) => EvaluateExpression(gammaExp, point);

    public double F(VectorT point) => EvaluateExpression(fExp, point);

    public SolidMaterialForScalarEllipticProblem(string name, string lambda, string gamma, string f, CoordinateSystem system)
    {
        Name = name;

        context = new ExpressionContext();
        context.Imports.AddType(typeof(Math));

        switch (system)
        {
            case CoordinateSystem.Cartesian:
                coordinates = ["x", "y", "z" ];
                break;
            case CoordinateSystem.Cylindrical:
                coordinates = ["r", "z", "phi" ];
                break;
            case CoordinateSystem.Spherical:
                coordinates = ["r", "phi", "psi"];
                break;
            default:
                throw new NotImplementedException();
        }

        context.Variables[coordinates[0]] = 0d;
        context.Variables[coordinates[1]] = 0d;
        context.Variables[coordinates[2]] = 0d;

        lambdaExp = context.CompileGeneric<double>(lambda);

        gammaExp = context.CompileGeneric<double>(gamma);

        fExp = context.CompileGeneric<double>(f);
    }
}
