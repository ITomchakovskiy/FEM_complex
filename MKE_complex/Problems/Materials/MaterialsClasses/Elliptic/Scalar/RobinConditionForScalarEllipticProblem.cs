using Flee.PublicTypes;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;

[Material(PDE_Type.Elliptic, MaterialType.RobinCondition, FieldType.Scalar)]

public class RobinConditionForScalarEllipticProblem<VectorT> : IMaterial<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public string Name { get; init; }

    private IGenericExpression<double> betaExp;

    private IGenericExpression<double> uBetaExp;

    private string[] coordinates;

    private ExpressionContext context;

    private double EvaluateExpression(IGenericExpression<double> expression, VectorT point)
    {
        int dim = point.N;
        for (int i = 0; i < dim; ++i)
            context.Variables[coordinates[i]] = point.components[i];
        return expression.Evaluate();
    }

    public double Beta(VectorT point) => EvaluateExpression(betaExp, point);

    public double UBeta(VectorT point) => EvaluateExpression(uBetaExp, point);


    public RobinConditionForScalarEllipticProblem(string name, string beta, string uBeta, CoordinateSystem system)
    {
        Name = name;

        context = new ExpressionContext();

        switch (system)
        {
            case CoordinateSystem.Cartesian:
                coordinates = ["x", "y", "z"];
                break;
            case CoordinateSystem.Cylindrical:
                coordinates = ["r", "z", "phi"];
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

        betaExp = context.CompileGeneric<double>(beta);

        uBetaExp = context.CompileGeneric<double>(uBeta);

    }
}
