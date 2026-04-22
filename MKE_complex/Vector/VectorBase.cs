using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public abstract class VectorBase<T, Tself> where T : INumber<T>
                                           where Tself : VectorBase<T, Tself>
{
    public VectorBase(params T[] components) => this.components = components;

    public int N => components!.Length;

    public T[] components { get; init; }

    protected abstract Tself CreateVector(params T[] components);

    public static Tself operator +(VectorBase<T,Tself> A, Tself B)
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for(int i = 0; i < n; ++i)
            new_components[i] = A.components[i] + B.components[i];

        return A.CreateVector(new_components);
    }

    public string AsString(string format, string separator)
    {
        string[] stringComponents = new string[components.Length];
        for(int i = 0; i < components.Length; ++i)
        {
            var x = components[i];
            stringComponents[i] = $"{x.ToString(format, null)}{separator}";
        }
        return string.Join(separator, stringComponents);
    }

    public static Tself operator -(VectorBase<T, Tself> A, VectorBase<T, Tself> B)
    {
        if (A.components is null || B.components is null || A.components.Length != B.components.Length)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] - B.components[i];

        return A.CreateVector(new_components);
        //return new VectorBase<T>(new_components);
    }

    public static Tself operator *(VectorBase<T, Tself> A, T k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = A.components[i] * k;

        return A.CreateVector(new_components);
        //return new VectorBase<T>(new_components);
    }

    public static Tself operator *(VectorBase<T, Tself> A, double k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        for (int i = 0; i < n; ++i)
            new_components[i] = T.CreateChecked(double.CreateChecked(A.components[i]) * k);

        return A.CreateVector(new_components);
        //return new VectorBase<T>(new_components);
    }

    public static Tself operator *(T k, VectorBase<T, Tself> A)
    {
        return A * k;
    }

    public static Tself operator *(double k, VectorBase<T, Tself> A)
    {
        return A * k;
    }

    public static Tself operator /(VectorBase<T, Tself> A, double k)
    {
        if (A.components is null)
            throw new ArgumentException();
        int n = A.components.Length;
        var new_components = new T[n];
        
        for (int i = 0; i < n; ++i)
            new_components[i] = T.CreateChecked(double.CreateChecked(A.components[i]) / k);


        return A.CreateVector(new_components);
        //return new VectorBase<T>(new_components);
    }

    public static double Length(Tself A, Tself B)
    {
        return (B - A).Norm();
    }

    public double Length(Tself other)
    {
        return (other - this).Norm();
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

    public static T Scalar(Tself A, Tself B)
    {
        T result = T.Zero;
        if(A.N != B.N) throw new ArgumentException();
        int N = A.N;
        if(A.components is Complex[] ac && B.components is Complex[] bc && result is Complex cr)
        {
            for (int i = 0; i < N; ++i)
                cr += ac[i] * new Complex(bc[i].Real, -bc[i].Imaginary);
        }
        else
        {
            for (int i = 0; i < N; ++i)
                result += A.components[i] * B.components[i];
        }
        
        return result;
    }

    public Tself Nornmalize()
    {
        double norm = Norm();

        return this / norm;
    }
    
   //public static IVector operator +(IVector v1, IVector v2);
}
