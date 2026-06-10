using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex;
public static class TriangleQuadratures
{
    private static double[] p41;
    private static double[] p42;
    private static double[] p43;
    private static double[] w4; 

    static TriangleQuadratures()
    {
        const double x1a = 0.873821971016996;
        const double x1b = 0.063089014491502;
        const double x2a = 0.501426509658179;
        const double x2b = 0.249286745170910;
        const double x3a = 0.636502499121399;
        const double x3b = 0.310352451033785;
        const double x3c = 0.053145049844816;
        const double w1el = 0.050844906370207;
        const double w2el = 0.116786275726379;
        const double w3el = 0.082851075618374;
        p41 = [ x1a, x1b, x1b, x2a, x2b, x2b, x3a, x3b, x3a, x3c, x3b, x3c ];
        p42 = [ x1b, x1a, x1b, x2b, x2a, x2b, x3b, x3a, x3c, x3a, x3c, x3b ];
        w4 = [ w1el, w1el, w1el, w2el, w2el, w2el, w3el, w3el, w3el, w3el, w3el, w3el ];
        for (int i = 0; i < w4.Length; i++)
            w4[i] /= 2.0;

        p43 = new double[w4.Length];
        for(int i = 0; i < p43.Length; ++i)
            p43[i] = 1d - p41[i] - p42[i];
        
    }

    public static (double[][] LocalPoints, double[] Weights) GetQuadrature(int SchemeNumber)
    {
        double[] p1;
        double[] p2;
        double[] p3;
        double[] w;
        switch(SchemeNumber)
        {
            case 4:
                {
                    p1 = p41;
                    p2 = p42;
                    p3 = p43;
                    w = w4;
                    break;
                }
            
            default: throw new NotImplementedException();
        }

        int N = w.Length;
        if(N != p1.Length || N != p2.Length || N != p3.Length) throw new Exception();
        double[][] localpoints = new double[N][];
        for(int i = 0; i < N; ++i)
            localpoints[i] = [p1[i], p2[i], p3[i]];

        return (localpoints, w);
    }
}