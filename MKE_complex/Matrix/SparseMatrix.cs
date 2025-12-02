using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Matrix;

public class SparseMatrix<T> where T : INumber<T>
{
    public int[] Ia { get; init; }
    public int[] Ja { get; init; }
    public T[] Al { get; init; }
    public T[] Au { get; init; }
    public T[] Di { get; init; }
    public int N => Ia.Length - 1;
    public int ElementsCount => Ja.Length;
    public bool IsSymmetric => Au.Length == 0;

    public T? GetElement(int i, int j)
    {
        if(i >= N || j >= N) throw new ArgumentOutOfRangeException();
        if(i == j)
        {
            return Di[i];
        }

        int row_index = i > j ? i : j;

        int element_number = Array.BinarySearch(Ja, Ia[row_index], Ia[row_index + 1] - Ia[row_index], j);

        if(element_number >= 0)
        {
            if (!IsSymmetric) return i > j ? Al[element_number] : Au[element_number];
            return Al[row_index];
        }

        return default;
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, bool isSymmetric)
    {
        Ia = ia.ToArray();
        Ja = ja.ToArray();

        Di = new T[N];
        Al = new T[ElementsCount];

        Au = isSymmetric ? [] : new T[ElementsCount];
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, ReadOnlySpan<T> di, ReadOnlySpan<T> al)
    {
        Ia = ia.ToArray();
        Ja = ja.ToArray();

        if (di.Length != N) throw new ArgumentOutOfRangeException();
        Di = di.ToArray();
        if(al.Length != ElementsCount) throw new ArgumentOutOfRangeException();
        Al = al.ToArray();
        Au = [];
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, ReadOnlySpan<T> di, ReadOnlySpan<T> al, ReadOnlySpan<T> au)
    {
        Ia = ia.ToArray();
        Ja = ja.ToArray();

        if (di.Length != N) throw new ArgumentOutOfRangeException();
        Di = di.ToArray();
        if (al.Length != ElementsCount) throw new ArgumentOutOfRangeException();
        Al = al.ToArray();
        if (au.Length != ElementsCount) throw new ArgumentOutOfRangeException();
        Au = au.ToArray();
    }

    public static VectorBase<T> operator *(SparseMatrix<T> M, VectorBase<T> X)
    {
        if(M.N != X.N) throw new ArgumentOutOfRangeException();
        int N = M.N;
        var components = new T[N];

        var x = X.components;

        for (int i = 0; i < N; i++)
        {
            components[i] = M.Di[i] * x[i];
            int i0 = M.Ia[i];
            int i1 = M.Ia[i + 1];
            for (int i_gg = i0; i_gg < i1; i_gg++)
            {
                int j = M.Ja[i_gg];
                components[i] += M.Al[i_gg] * x[j];
                components[j] += (M.IsSymmetric ? M.Al[i_gg] : M.Au[i_gg]) * x[i];
            }
        }
        return new VectorBase<T>(components);
    }
}
