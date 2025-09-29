using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public abstract class VectorBase
{
    public VectorBase(params double[] components) => this.components = components;

    protected double[]? components { get; init; }

    protected abstract VectorBase CreateVector(params double[] components);

    public static VectorBase operator +(VectorBase A, VectorBase B)
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        int n = A.components.Length;
        double[] new_components = new double[n];
        for(int i = 0; i < n; ++i)
            new_components[i] = A.components[i] + B.components[i];

        return A.CreateVector(new_components);
    }

    public static VectorBase operator -(VectorBase A, VectorBase B)
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        int n = A.components.Length;
        double[] new_components = new double[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] - B.components[i];

        return A.CreateVector(new_components);
    }

    public static VectorBase operator *(VectorBase A, double k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        double[] new_components = new double[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] * k;

        return A.CreateVector(new_components);
    }

    public static VectorBase operator *(double k, VectorBase A)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        double[] new_components = new double[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] * k;

        return A.CreateVector(new_components);
    }

    public static VectorBase operator /(VectorBase A, double k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        double[] new_components = new double[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] / k;

        return A.CreateVector(new_components);
    }

    public double Norm()
    {
        if (components is null)
            throw new ArgumentException();
        double square_sum = 0;
        foreach(double x in components)
            square_sum += x * x;
        return Math.Sqrt(square_sum);
    }

    public VectorBase Nornmalize()
    {
        double norm = Norm();

        return this / norm;
    }
    public static VectorBase PointOnLine(VectorBase A, VectorBase B, int n, double k, int ind) //for mesh initialization
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        if (ind == 0) return A;
        if (ind == n) return B;
        VectorBase r = B - A;
        double l = r.Norm();
        if (Math.Abs(k - 1d) < 1.0E-13)
            return A + r / n * ind;

        double l_ind = l * (1d - Math.Pow(Math.Abs(k), ind)) / (1d - Math.Pow(Math.Abs(k), n));

        l_ind = k > 0 ? l_ind : l - l_ind;
        return A + l_ind / l * r;
    }
   //public static IVector operator +(IVector v1, IVector v2);
}
