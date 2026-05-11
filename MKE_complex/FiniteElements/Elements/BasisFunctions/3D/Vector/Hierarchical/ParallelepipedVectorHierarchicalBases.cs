using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions._3D.Vector.Hierarchical;
public static class ParallelepipedVectorHierarchicalBases
{

    public static Func<double, double, double, Vector3D>[] BasisFunctions(int order)
    {
        return order switch
        {
            1 => Psi.AsSpan(0,12).ToArray(),
            2 => Psi,
            _ => throw new ArgumentException("wrong element order")
        };
    }
    private static int[] nonZeroBasisComponentsIndices = [0,0,0,0,1,1,1,1,2,2,2,2,0,0,0,0,1,1,1,1,2,2,2,2,1,1,2,2,1,1,2,2,0,0,2,2,0,0,2,2,0,0,1,1,0,0,1,1,0,0,1,1,2,2];

    public static int[] GetNonZeroBasisComponentsIndices(int order)
    {
        return order switch
        {
            1 =>nonZeroBasisComponentsIndices.Take(12).ToArray(),
            2 => nonZeroBasisComponentsIndices,
            _ => throw new ArgumentException("wrong element order")
        };
    }

    private static Func<double, double, double, Vector3D>[] Psi = BuildPsi();

    private static Func<double, double, double, Vector3D>[] BuildPsi()
    {

        var Psi = new List<Func<double, double, double, Vector3D>>(54);

        Func<double, double, double, Vector3D>[] Psi1 =
        [
            (xi, eta, zeta) => new(PhiL(eta) * PhiL(zeta), 0d, 0d),
            (xi, eta, zeta) => new(PhiR(eta) * PhiL(zeta), 0d, 0d),
            (xi, eta, zeta) => new(PhiL(eta) * PhiR(zeta), 0d, 0d),
            (xi, eta, zeta) => new(PhiR(eta) * PhiR(zeta), 0d, 0d),
            (xi, eta, zeta) => new(0d, PhiL(xi) * PhiL(zeta), 0d),
            (xi, eta, zeta) => new(0d, PhiR(xi) * PhiL(zeta), 0d),
            (xi, eta, zeta) => new(0d, PhiL(xi) * PhiR(zeta), 0d),
            (xi, eta, zeta) => new(0d, PhiR(xi) * PhiR(zeta), 0d),
            (xi, eta, zeta) => new(0d, 0d, PhiL(xi) * PhiL(eta)),
            (xi, eta, zeta) => new(0d, 0d, PhiR(xi) * PhiL(eta)),
            (xi, eta, zeta) => new(0d, 0d, PhiL(xi) * PhiR(eta)),
            (xi, eta, zeta) => new(0d, 0d, PhiR(xi) * PhiR(eta)),
        ];

        Psi.AddRange(Psi1);
        
        for(int i = 0; i < 4; ++i)
        {
            int index = i;
            Psi.Add((xi, eta, zeta) => LocalCoordinateTransformation(xi) * Psi1[index](xi, eta, zeta)); 
        }
            
        for(int i = 4; i < 8; ++i)
        {
            int index = i;
            Psi.Add((xi, eta, zeta) => LocalCoordinateTransformation(eta) * Psi1[index](xi, eta, zeta));
        }
            
        for(int i = 8; i < 12; ++i)
        {
            int index = i;
            Psi.Add((xi, eta, zeta) => LocalCoordinateTransformation(zeta) * Psi1[index](xi, eta, zeta));
        }
            

        for(int i = 24; i < 54;++i)
            Psi.Add((xi, eta, zeta) => new(0d,0d,0d));


        int[][] numbers_for_faces_and_element = 
        [
            [32, 36, 40, 44, 48],
            [24, 28, 42, 46, 50],
            [26, 30, 34, 38, 52]
        ];

        Func<double, double, double>[] Psi2X =
        [
            (eta, zeta) => PhiP(zeta,2)*PhiL(eta),
            (eta, zeta) => PhiP(zeta,2)*PhiR(eta),
            (eta, zeta) => PhiP(eta,2)*PhiL(zeta),
            (eta, zeta) => PhiP(eta,2)*PhiR(zeta),
            (eta, zeta) => PhiP(eta,2)*PhiP(zeta,2)
        ];

        Func<double, double, double>[] Psi2Y =
        [
            (xi, zeta) => PhiP(zeta,2)*PhiL(xi),
            (xi, zeta) => PhiP(zeta,2)*PhiR(xi),
            (xi, zeta) => PhiP(xi,2)*PhiL(zeta),
            (xi, zeta) => PhiP(xi,2)*PhiR(zeta),
            (xi, zeta) => PhiP(xi,2)*PhiP(zeta,2)
        ];

        Func<double, double, double>[] Psi2Z =
        [
            (xi, eta) => PhiP(eta,2)*PhiL(xi),
            (xi, eta) => PhiP(eta,2)*PhiR(xi),
            (xi, eta) => PhiP(xi,2)*PhiL(eta),
            (xi, eta) => PhiP(xi,2)*PhiR(eta),
            (xi, eta) => PhiP(xi,2)*PhiP(eta,2)
        ];

        for(int i =0; i < numbers_for_faces_and_element[0].Length; ++i)
        {
            int index = i;
            Psi[numbers_for_faces_and_element[0][index]] = 
                (xi, eta, zeta) => 
                    new(Psi2X[index](eta, zeta),0,0);
            Psi[numbers_for_faces_and_element[1][index]] = 
                (xi, eta, zeta) => 
                    new(0,Psi2Y[index](xi, zeta),0);
            Psi[numbers_for_faces_and_element[2][index]] = 
                (xi, eta, zeta) => 
                    new(0,0,Psi2Z[index](xi, eta));
            Psi[numbers_for_faces_and_element[0][index]+1] = 
                (xi, eta, zeta) => LocalCoordinateTransformation(xi)*
                Psi[numbers_for_faces_and_element[0][index]](xi,eta,zeta);
            Psi[numbers_for_faces_and_element[1][index]+1] = 
                (xi, eta, zeta) => LocalCoordinateTransformation(eta)*
                Psi[numbers_for_faces_and_element[1][index]](xi,eta,zeta);
            Psi[numbers_for_faces_and_element[2][index]+1] = 
                (xi, eta, zeta) => LocalCoordinateTransformation(zeta)*
                Psi[numbers_for_faces_and_element[2][index]](xi,eta,zeta);
        }

        return Psi.ToArray();
    }
    
    private static double PhiL(double xi) => (1d - LocalCoordinateTransformation(xi))/2d;
    private static double PhiR(double xi) => (1d + LocalCoordinateTransformation(xi))/2d;
    private static double PhiP(double xi, double p)
    {
        double local = LocalCoordinateTransformation(xi);

        return Math.Pow(local, p - 2) * (1d - local * local);
    }
    private static double LocalCoordinateTransformation(double xi) => 2d * xi -1d;
}