using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar.Lagrangian;

public static class ParallelepipedScalarLagrangianBases
{
    public static int LocalXDofNum(int i , int order)  =>  i % (order + 1);
    public static int LocalYDofNum(int i, int order) => i / (order + 1) % (order + 1);
    public static int LocalZDofNum(int i, int order) => i / (order + 1) / (order + 1);

    public static Func<int, double, double, double, double> BasisFunctions(int order)
    {
        if(order < 1) throw new ArgumentException("wrong element order");
        return (int i, double xi, double eta, double zeta) => LineLagrangianBases.BasisFunctions(order)[LocalXDofNum(i, order)](xi) * 
                                                              LineLagrangianBases.BasisFunctions(order)[LocalYDofNum(i, order)](eta) *
                                                              LineLagrangianBases.BasisFunctions(order)[LocalZDofNum(i, order)](zeta);
    }
}