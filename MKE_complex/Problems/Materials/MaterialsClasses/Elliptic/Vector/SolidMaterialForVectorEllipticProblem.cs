using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flee.PublicTypes;
using MKE_complex.Vector;

namespace MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Vector;

[Material(PDE_Type.Elliptic, MaterialType.Solid, FieldType.Vector)]
public class SolidMaterialForVectorEllipticProblem<VectorT> : IMaterial<VectorT> where VectorT : VectorBase<double, VectorT>
{
    public string Name { get; init; }
    private IGenericExpression<double> muExp;
    private IGenericExpression<double> gammaExp;
    private IGenericExpression<double> fxExp;
    private IGenericExpression<double> fyExp;
    private IGenericExpression<double> fzExp;

    private string[] coordinates;

    private ExpressionContext context;

    private double EvaluateExpression(IGenericExpression<double> expression, VectorT point)
    {
        int dim = point.N;
        for(int i = 0;i<dim;++i)
            context.Variables[coordinates[i]] = point.components[i];
        return expression.Evaluate();
    }

    public double Mu(VectorT point) => EvaluateExpression(muExp, point);

    public double Gamma(VectorT point) => EvaluateExpression(gammaExp, point);

    public double Fx(VectorT point) => EvaluateExpression(fxExp, point); //!could return Vector
    public double Fy(VectorT point) => EvaluateExpression(fyExp, point);
    public double Fz(VectorT point) => EvaluateExpression(fzExp, point);

    public SolidMaterialForVectorEllipticProblem(string name, string lambda, string gamma, string fx,string fy, string fz, string[] coordinates)
    {
        Name = name;

        context = new ExpressionContext();
        context.Imports.AddType(typeof(Math));

        this.coordinates = coordinates;

        context.Variables[coordinates[0]] = 0d;
        context.Variables[coordinates[1]] = 0d;
        context.Variables[coordinates[2]] = 0d;

        muExp = context.CompileGeneric<double>(lambda);

        gammaExp = context.CompileGeneric<double>(gamma);

        fxExp = context.CompileGeneric<double>(fx);
        fyExp = context.CompileGeneric<double>(fy);
        fzExp = context.CompileGeneric<double>(fz);
    }

    public SolidMaterialForVectorEllipticProblem(MaterialFileInfo fileInfo, string[] coordinates) : 
    this(fileInfo.Name, fileInfo.Functions["Mu"], fileInfo.Functions["Gamma"], fileInfo.Functions["Fx"],
                                                                                   fileInfo.Functions["Fy"],
                                                                                   fileInfo.Functions["Fz"], coordinates) {}
}