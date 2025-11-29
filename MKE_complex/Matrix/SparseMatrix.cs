using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Matrix;

public class SparseMatrix<ElementType>
{
    public int[] Ia { get; init; }
    public int[] Ja { get; init; }
    public ElementType[] Al { get; init; }
    public ElementType[] Au { get; init; }
    public ElementType[] Di { get; init; }
    public int N => Ia.Length - 1;
    public int ElementsCount => Ja.Length;
    public bool IsSymmetric => Au.Length == 0;

    public ElementType? GetElement(int i, int j)
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

        Di = new ElementType[N];
        Al = new ElementType[ElementsCount];

        Au = isSymmetric ? [] : new ElementType[ElementsCount];
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, ReadOnlySpan<ElementType> di, ReadOnlySpan<ElementType> al)
    {
        Ia = ia.ToArray();
        Ja = ja.ToArray();

        if (di.Length != N) throw new ArgumentOutOfRangeException();
        Di = di.ToArray();
        if(al.Length != ElementsCount) throw new ArgumentOutOfRangeException();
        Al = al.ToArray();
        Au = [];
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, ReadOnlySpan<ElementType> di, ReadOnlySpan<ElementType> al, ReadOnlySpan<ElementType> au)
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
}
