using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public record Vector1D(double X) : IVector
{
    public static Vector1D operator +(Vector1D A, Vector1D B) => new(A.X + B.X);

    public static Vector1D operator -(Vector1D A, Vector1D B) => new(A.X - B.X);

    public IVector PointOnLine(IVector A, IVector B, int n, double k, int ind)
    {
        if (ind == 0) return A;
        if (ind == n) return B;
        double l = B - A;
        if (Math.Abs(k - 1d) < 1.0E-13)
            return p1 + l / n * ind;

        double l_ind = l * (1d - Math.Pow(Math.Abs(k), ind)) / (1d - Math.Pow(Math.Abs(k), n));
        l_ind = k > 0 ? l_ind : l - l_ind;
        return p1 + l_ind;
    }
}
