using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class VectorBase<T> where T : INumber<T>
{
    public VectorBase(params T[] components) => this.components = components;

    public int N => components!.Length;

    public T[] components { get; init; }

    //protected abstract VectorBase<T> CreateVector(params T[] components) : base(components);

    public static VectorBase<T> operator +(VectorBase<T> A, VectorBase<T> B)
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for(int i = 0; i < n; ++i)
            new_components[i] = A.components[i] + B.components[i];

        //return A.CreateVector(new_components);
        return new VectorBase<T>(new_components);
    }

    public static VectorBase<T> operator -(VectorBase<T> A, VectorBase<T> B)
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] - B.components[i];

        //return A.CreateVector(new_components);
        return new VectorBase<T>(new_components);
    }

    public static VectorBase<T> operator *(VectorBase<T> A, T k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] * k;

        //return A.CreateVector(new_components);
        return new VectorBase<T>(new_components);
    }

    public static VectorBase<T> operator *(VectorBase<T> A, double k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = T.CreateChecked(double.CreateChecked(A.components[i]) * k);

        //return A.CreateVector(new_components);
        return new VectorBase<T>(new_components);
    }

    public static VectorBase<T> operator *(T k, VectorBase<T> A)
    {
        return A * k;
    }

    public static VectorBase<T> operator *(double k, VectorBase<T> A)
    {
        return A * k;
    }

    public static VectorBase<T> operator /(VectorBase<T> A, double k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        
        for (int i = 0; i < n; ++i)
            new_components[i] = T.CreateChecked(double.CreateChecked(A.components[i]) / k);


           // return A.CreateVector(new_components);
        return new VectorBase<T>(new_components);
    }

    public double Norm()
    {
        if (components is null)
            throw new ArgumentException();
        T square_sum = T.Zero;
        double? d_square_sum = 0;

        if (components is Complex[] c_components)
        {
            foreach (Complex x in c_components)
                d_square_sum += x.Magnitude * x.Magnitude;
            return Math.Sqrt((double)d_square_sum);
        }
        else 
        {
            foreach (T x in components)
                square_sum += x * x;
            return Math.Sqrt(double.CreateChecked(square_sum));
        }
    }

    public VectorBase<T> Nornmalize()
    {
        double norm = Norm();

        return this / norm;
    }
    public static VectorBase<T> PointOnLine(VectorBase<T> A, VectorBase<T> B, int n, double k, int ind) //for mesh initialization
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        if (ind == 0) return A;
        if (ind == n) return B;
        VectorBase<T> r = B - A;
        double l = r.Norm();
        if (Math.Abs(k - 1d) < 1.0E-13)
            return A + r / n * ind;

        double l_ind = l * (1d - Math.Pow(Math.Abs(k), ind)) / (1d - Math.Pow(Math.Abs(k), n));

        l_ind = k > 0 ? l_ind : l - l_ind;
        return A + l_ind / l * r;
    }
   //public static IVector operator +(IVector v1, IVector v2);
}
