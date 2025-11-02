using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Matrix;

public class SparseMatrix<ElementType>
{
    private int[] ia { get; set; }
    private int[] ja { get; set; }
    private ElementType[] al { get; set; }
    private ElementType[] au { get; set; }
    private ElementType[] di { get; set; }

    public int N => ia.Length - 1;
    public int ElementsCount => ja.Length;
    public bool IsSymmetric => au.Length == 0;
    public bool HaveDiagonal => di.Length == 0;

    public ElementType? GetElement(int i, int j)
    {
        if(i >= N || j >= N) throw new ArgumentOutOfRangeException();
        if(i == j)
        {
            if (!HaveDiagonal) return default;
            return di[i];
        }

        int row_index = i > j ? i : j;

        int start_column = ia[row_index];

        int end_column = ia[row_index + 1] - 1;

        int element_index = 0;

        int column = 0;

        if (ja[start_column] > j || ja[end_column] < j) return default;

        do                //dichotomy
        {
            element_index = (end_column + start_column) / 2;
            column = ja[element_index];
            if (j < column)
                end_column = element_index;
            else if (j > column)
                start_column = element_index;
            else
            {
                if (i > j || IsSymmetric) return al[element_index];

                return au[element_index];
            }
        } while (end_column - start_column > 1);

        return default;
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, ReadOnlySpan<ElementType> al)
    {
        this.ia = ia.ToArray();
        this.ja = ja.ToArray();
        this.al = al.ToArray();

        if (ElementsCount != al.Length) throw new ArgumentOutOfRangeException();

        this.di = [];
        this.au = [];
    }

    public SparseMatrix(int[] ia, int[] ja, ElementType[] al)
    {
        this.ia = ia;
        this.ja = ja;
        this.al = al;

        if (ElementsCount != al.Length) throw new ArgumentOutOfRangeException();

        this.di = [];
        this.au = [];
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, ReadOnlySpan<ElementType> al, ReadOnlySpan<ElementType> au_or_di, bool isSymmetric)
        : this(ia, ja, al)
    {
        if(isSymmetric)
        {
            this.di = au_or_di.ToArray();
            if (HaveDiagonal && di.Length != N) throw new ArgumentOutOfRangeException();
        }
        else
        {
            this.au = au_or_di.ToArray();
            if (!IsSymmetric && au.Length != ElementsCount) throw new ArgumentOutOfRangeException();
        }
    }

    public SparseMatrix(int[] ia, int[] ja, ElementType[] al, ElementType[] au_or_di, bool isSymmetric)
        : this(ia, ja, al)
    {
        if (isSymmetric)
        {
            this.di = au_or_di;
            if (HaveDiagonal && di.Length != N) throw new ArgumentOutOfRangeException();
        }
        else
        {
            this.au = au_or_di;
            if (!IsSymmetric && au.Length != ElementsCount) throw new ArgumentOutOfRangeException();
        }
    }

    public SparseMatrix(ReadOnlySpan<int> ia, ReadOnlySpan<int> ja, ReadOnlySpan<ElementType> al, ReadOnlySpan<ElementType> au, ReadOnlySpan<ElementType> di) 
        : this(ia,ja,al)
    {
        this.di = di.ToArray();
        if (HaveDiagonal && di.Length != N) throw new ArgumentOutOfRangeException();

        this.au = au.ToArray();
        if(!IsSymmetric && au.Length != ElementsCount) throw new ArgumentOutOfRangeException();
    }

    public SparseMatrix(int[] ia, int[] ja, ElementType[] al, ElementType[] au, ElementType[] di)
        : this(ia, ja, al)
    {
        this.di = di;
        if (HaveDiagonal && di.Length != N) throw new ArgumentOutOfRangeException();

        this.au = au;
        if (!IsSymmetric && au.Length != ElementsCount) throw new ArgumentOutOfRangeException();
    }
}
