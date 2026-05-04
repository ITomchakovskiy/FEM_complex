using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Lagrangian;
public static class RectangleScalarLagrangianBases
{
    public static int LocalXDofNum(int i , int order)  =>  i % (order + 1);

    public static int LocalYDofNum(int i, int order) => i / (order + 1);

    public static Func<int, double, double, double> BasisFunctions(int order)
    {
        if(order < 1) throw new ArgumentException("wrong element order");
        return (int i, double xi, double eta) => LineLagrangianBases.BasisFunctions(order)[LocalXDofNum(i, order)](xi) * 
                                                 LineLagrangianBases.BasisFunctions(order)[LocalYDofNum(i, order)](eta);
    }
}