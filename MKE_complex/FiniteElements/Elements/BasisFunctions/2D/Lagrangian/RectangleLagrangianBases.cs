using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._2D.Lagrangian;
public static class RectangleLagrangianBases
{
    public static int localXDofNum(int i , int order)  =>  i % (order + 1);

    public static int localYDofNum(int i, int order) => i / (order + 1);

    public static Func<int, double, double, double> BasisFunctions(int order)
    {
        if(order < 1) throw new ArgumentException("wrong element order");
        return (int i, double xi, double eta) => LineLagrangianBases.BasisFunctions(order)[localXDofNum(i, order)](xi) * 
                                                 LineLagrangianBases.BasisFunctions(order)[localYDofNum(i, order)](eta);
    }
}