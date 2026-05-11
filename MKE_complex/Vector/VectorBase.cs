using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public abstract class VectorBase<T, Tself>(params T[] components) where T : INumber<T>
                                           where Tself : VectorBase<T, Tself>
{
    public int N => components!.Length;

    public T[] components { get; init; } = components;

    public abstract Tself CreateVector(params T[] components);

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

    public T Scalar(Tself Other)
    {
        T result = T.Zero;
        if(N != Other.N) throw new ArgumentException();
        if(components is Complex[] ac && Other.components is Complex[] bc && result is Complex cr)
        {
            for (int i = 0; i < N; ++i)
                cr += ac[i] * new Complex(bc[i].Real, -bc[i].Imaginary);
        }
        else
        {
            for (int i = 0; i < N; ++i)
                result += components[i] * Other.components[i];
        }
        
        return result;
    }

    public static T Scalar(Tself A, Tself B) => A.Scalar(B);

    public Tself Multiply(Tself Other)
    {
        if(N != Other.N) throw new ArgumentException();
        T[] new_components = new T[N];
        for(int i = 0; i < N; ++i)
            new_components[i] = components[i] * Other.components[i];

        var result = CreateVector(new_components);
        
        return result;
    }

    public static Tself Multiply(Tself A, Tself B) => A.Multiply(B);

    public Tself Division(Tself other)
    {
        if(N != other.N) throw new ArgumentException();
        T[] new_components = new T[N];
        for(int i = 0; i < N; ++i)
            new_components[i] = components[i] / other.components[i];

        var result = CreateVector(new_components);
        
        return result;
    }

    public static Tself Division(Tself A, Tself B) => A.Division(B);

    public Tself Sqrt()
    {
        T[] new_components = new T[N];
        if(components is Complex[] complex && new_components is Complex[] new_complex)
        {
            for(int i = 0; i < N; ++i)
                new_complex[i] = Complex.Sqrt(complex[i]);
        }
        else
        {
            for(int i = 0; i < N; ++i)
                new_components[i] = T.CreateChecked(Math.Sqrt(double.CreateChecked(components[i])));
        }
        return CreateVector(new_components);
    }

    public static Tself Sqrt(Tself A)
    {
        return A.Sqrt();
    }

    public Tself Nornmalize()
    {
        double norm = Norm();

        return this / norm;
    }
    
   //public static IVector operator +(IVector v1, IVector v2);
}
