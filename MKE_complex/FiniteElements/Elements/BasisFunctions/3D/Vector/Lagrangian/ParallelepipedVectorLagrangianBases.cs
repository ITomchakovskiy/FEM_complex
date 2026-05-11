using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions._1D.Lagrangian;
using MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Scalar.Lagrangian;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Vector.Lagrangian;
public static class ParallelepipedVectorLagrangianBases
{
    public static Func<int, double, double, double, Vector3D> BasisFunctions(int order)
    {
        if(order < 1) throw new ArgumentException("wrong element order");
        return (i, xi, eta, zeta) => 
        {
            var basisValue = ParallelepipedScalarLagrangianBases.BasisFunctions(order)(i/3,xi,eta,zeta);
            return (i % 3) switch
            {
                0 => new(basisValue, 0d, 0d),
                1 => new(0d, basisValue, 0d),
                2 => new(0d, 0d, basisValue),
                _ => throw new ArgumentException(),
            };
        };
    }
}