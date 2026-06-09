using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex;
public static class TetrahedronQuadratures
{
    private static double[] p11 = [ 1.0 / 4.0, 1.0 / 2.0, 1.0 / 6.0, 1.0 / 6.0, 1.0 / 6.0 ];
    private static double[] p12 = [ 1.0 / 4.0, 1.0 / 6.0, 1.0 / 2.0, 1.0 / 6.0, 1.0 / 6.0 ];
    private static double[] p13 = [ 1.0 / 4.0, 1.0 / 6.0, 1.0 / 6.0, 1.0 / 2.0, 1.0 / 6.0 ];
    private static double[] p14;
    private static double[] w1 = [ -2.0 / 15.0, 3.0 / 40.0, 3.0 / 40.0, 3.0 / 40.0, 3.0 / 40.0 ];
                        
    private static double[] p21 = [1d/4d, 0, 1d/3d, 1d/3d, 1d/3d, 8d/11d, 1d/11d, 1d/11d, 1d/11d, 0.066550153573664, 0.066550153573664, 0.433449846426336, 0.433449846426336, 0.066550153573664, 0.433449846426336];
    private static double[] p22 = [1d/4d, 1d/3d, 0, 1d/3d, 1d/3d, 1d/11d, 8d/11d, 1d/11d, 1d/11d, 0.066550153573664, 0.433449846426336, 0.433449846426336, 0.066550153573664, 0.433449846426336, 0.066550153573664];
    private static double[] p23 = [1d/4d, 1d/3d, 1d/3d, 0, 1d/3d, 1d/11d, 1d/11d, 8d/11d, 1d/11d, 0.433449846426336, 0.433449846426336, 0.066550153573664, 0.066550153573664, 0.066550153573664, 0.433449846426336];

    private static double[] p24;

    private static double[] w2 = [ 0.030283678097089,
                         0.006026785714286, 0.006026785714286, 0.006026785714286, 0.006026785714286,
                         0.011645249086029, 0.011645249086029, 0.011645249086029, 0.011645249086029,
                         0.010949141561386, 0.010949141561386, 0.010949141561386, 0.010949141561386, 0.010949141561386, 0.010949141561386 ];
    
    private static double[] p31;
    private static double[] p32;
    private static double[] p33;
    private static double[] p34;
    private static double[] w3; 

    static TetrahedronQuadratures()
    {
        const double w1el = 0.665379170969464506e-2;
        const double w2el = 0.167953517588677620e-2;
        const double w3el = 0.922619692394239843e-2;
        const double w4el = 0.803571428571428248e-2;

        const double x1a = 0.214602871259151684;
        const double x1b = 0.356191386222544953;

        const double x2a = 0.406739585346113397e-1;
        const double x2b = 0.877978124396165982;

        const double x3a = 0.322337890142275646;
        const double x3b = 0.329863295731730594e-1;

        const double x4a = 0.636610018750175299e-1;
        const double x4b = 0.269672331458315867;
        const double x4c = 0.603005664791649076;

        p31 = [ x1a, x1a, x1a, x1b, x2a, x2a, x2a, x2b, x3a, x3a, x3a, x3b, x4a, x4a, x4a, x4a, x4b, x4c, x4a, x4a, x4b, x4b, x4c, x4c ];
        p32 = [ x1a, x1a, x1b, x1a, x2a, x2a, x2b, x2a, x3a, x3a, x3b, x3a, x4a, x4a, x4b, x4c, x4a, x4a, x4b, x4c, x4a, x4c, x4a, x4b ];
        p33 = [ x1a, x1b, x1a, x1a, x2a, x2b, x2a, x2a, x3a, x3b, x3a, x3a, x4b, x4c, x4a, x4a, x4a, x4a, x4c, x4b, x4c, x4a, x4b, x4a ];
        w3 = [ w1el, w1el, w1el, w1el, w2el, w2el, w2el, w2el, w3el, w3el, w3el, w3el, w4el, w4el, w4el, w4el, w4el, w4el, w4el, w4el, w4el, w4el, w4el, w4el ];

        p14 = new double[w1.Length];
        for(int i = 0; i < w1.Length; ++i)
            p14[i] = 1d - p11[i] - p12[i] - p13[i];
        p24 = new double[w2.Length];
        for(int i = 0; i < w2.Length; ++i)
            p24[i] = 1d - p21[i] - p22[i] - p23[i];
        p34 = new double[w3.Length];
        for(int i = 0; i < w3.Length; ++i)
            p34[i] = 1d - p31[i] - p32[i] - p33[i];
    }

    public static (double[][] LocalPoints, double[] Weights) GetQuadrature(int SchemeNumber)
    {
        double[] p1;
        double[] p2;
        double[] p3;
        double[] p4;
        double[] w;
        switch(SchemeNumber)
        {
            case 1:
                {
                    p1 = p11;
                    p2 = p12;
                    p3 = p13;
                    p4 = p14;
                    w = w1;
                    break;
                }
            case 2:
                {
                    p1 = p21;
                    p2 = p22;
                    p3 = p23;
                    p4 = p24;
                    w = w2;
                    break;
                }
            case 3:
                {
                    p1 = p31;
                    p2 = p32;
                    p3 = p33;
                    p4 = p34;
                    w = w3;
                    break;
                }    
            default: throw new NotImplementedException();
        }

        int N = w.Length;
        if(N != p1.Length || N != p2.Length || N != p3.Length || N != p4.Length) throw new Exception();
        double[][] localpoints = new double[N][];
        for(int i = 0; i < N; ++i)
            localpoints[i] = [p1[i], p2[i], p3[i], p4[i]];

        return (localpoints, w);
    }
                        
}