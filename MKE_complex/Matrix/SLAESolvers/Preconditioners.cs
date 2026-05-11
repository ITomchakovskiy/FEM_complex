using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace MKE_complex.Matrix.SLAESolvers;

public static class Preconditioners
{
    public static SparseMatrix<T> CholeskySparseDecomposition<T>(SparseMatrix<T> A) where T : INumber<T>
    {
        if(!A.IsSymmetric) throw new ArgumentException();

        T[] al = new T[A.Al.Length];
        T[] di = new T[A.Di.Length];
        
        for(int i = 0; i < A.N; ++i)
        {
            T DiSum = T.Zero;
            int i0 = A.Ia[i];
            int i1 = A.Ia[i+1];
            for(int el = i0; el < i1; ++el)
            {
                int j = A.Ja[el];
                int j0 = A.Ia[j];
                int j1 = A.Ia[j+1];
                T AlSum = T.Zero;
                for(int Iel = i0, Jel = j0; Iel < el && Jel < j1; )
                {
                    if(A.Ja[Iel] > A.Ja[Jel])
                        ++Jel;
                    else if(A.Ja[Iel] < A.Ja[Jel])
                        ++Iel;
                    else
                    {
                        AlSum += al[Iel] * al[Jel];
                        ++Iel;
                        ++Jel;
                    }
                }
                al[el] = (A.Al[el] - AlSum) / di[j];
                DiSum += al[el] * al[el];
            }
            var diExpr = A.Di[i] - DiSum;
            if(diExpr is Complex compExpr && di is Complex[] compDi)
                compDi[i] = Complex.Sqrt(compExpr);
            else
            {
                if(double.CreateChecked(diExpr) < 0d) throw new ArgumentException("Matrix is not positive definite");
                di[i] = T.CreateChecked(Math.Sqrt(double.CreateChecked(diExpr)));
            }
                
        }

        return new SparseMatrix<T>(A.Ia,A.Ja,di,al);
    }

    public static Vector.Vector<T> BackSubstitutionForCholeskySparseDecomposition<T>(SparseMatrix<T> A, Vector.Vector<T> rs) where T : INumber<T>
    {
        var res = new T[rs.N];
        Array.Fill(res,T.Zero);
        for(int i = rs.N - 1; i >= 0; --i)
        {
            res[i] = (rs.components[i] - res[i]) / A.Di[i];
            int i0 = A.Ia[i];
            int i1 = A.Ia[i+1];
            for(int el = i1 - 1; el >= i0; --el)
                res[A.Ja[el]] += A.Al[el] * res[i];
        }

        return new(res);
    }

    public static Vector.Vector<T> MultiplyUpperTriangleForCholeskySparseDecomposition<T>(SparseMatrix<T> A, Vector.Vector<T> b) where T : INumber<T>
    {
        var res = new T[b.N];
        Array.Fill(res,T.Zero);
        for(int i = 0; i < res.Length; ++i)
        {
            for(int el = A.Ia[i]; el < A.Ia[i+1]; ++el)
            {
                int j = A.Ja[el];
                res[j] += A.Al[el] * b.components[i];
            }
            res[i] += A.Di[i] * b.components[i];
        }

        return new(res);
    }

    public static SparseMatrix<T> LUSparseDecomposition<T>(SparseMatrix<T> A) where T : INumber<T>
    {
        T[] al = new T[A.Al.Length];
        T[] au = new T[A.Al.Length];
        T[] di = new T[A.Di.Length];
        
        for(int i = 0; i < A.N; ++i)
        {
            T DiSum = T.Zero;
            int i0 = A.Ia[i];
            int i1 = A.Ia[i+1];
            for(int el = i0; el < i1; ++el)
            {
                int j = A.Ja[el];
                int j0 = A.Ia[j];
                int j1 = A.Ia[j+1];
                T AlSum = T.Zero;
                T AuSum = T.Zero;
                for(int Iel = i0, Jel = j0; Iel < el && Jel < j1; )
                {
                    if(A.Ja[Iel] > A.Ja[Jel])
                        ++Jel;
                    else if(A.Ja[Iel] < A.Ja[Jel])
                        ++Iel;
                    else
                    {
                        AlSum += al[Iel] * au[Jel];
                        AuSum += au[Iel] * al[Jel];
                        
                        ++Iel;
                        ++Jel;
                    }
                }
                al[el] = A.Al[el] - AlSum;
                au[el] = A.IsSymmetric ? (A.Al[el] - AuSum) / di[j] : (A.Au[el] - AuSum) / di[j];
                DiSum += al[el] * au[el];
            }
            di[i] = A.Di[i] - DiSum;
            
        }

        return new SparseMatrix<T>(A.Ia,A.Ja,di,al,au);
    }

    public static Vector.Vector<T> ForwardSubstitutionForLUSparseDecomposition<T>(SparseMatrix<T> A, Vector.Vector<T> rs) where T : INumber<T>
    {
        var res = new T[rs.N];
        for(int i = 0; i < rs.N; ++i)
        {
            T sum = T.Zero;
            int i0 = A.Ia[i];
            int i1 = A.Ia[i+1];
            for(int el = i0; el < i1; ++el)
                sum += A.Al[el] * res[A.Ja[el]];
            res[i] = (rs.components[i] - sum) / A.Di[i];
        }

        return new(res);
    }

    public static Vector.Vector<T> BackSubstitutionForLUSparseDecomposition<T>(SparseMatrix<T> A, Vector.Vector<T> rs) where T : INumber<T>
    {
        var res = new T[rs.N];
        Array.Fill(res,T.Zero);
        for(int i = rs.N - 1; i >= 0; --i)
        {
            res[i] = rs.components[i] - res[i];
            int i0 = A.Ia[i];
            int i1 = A.Ia[i+1];
            for(int el = i1 - 1; el >= i0; --el)
                res[A.Ja[el]] += A.Au[el] * res[i];
        }

        return new(res);
    }

    public static Vector.Vector<T> MultiplyUpperTriangleForLUSparseDecomposition<T>(SparseMatrix<T> A, Vector.Vector<T> b) where T : INumber<T>
    {
        var res = new T[b.N];
        Array.Fill(res,T.Zero);
        for(int i = 0; i < res.Length; ++i)
        {
            for(int el = A.Ia[i]; el < A.Ia[i+1]; ++el)
            {
                int j = A.Ja[el];
                res[j] += A.Au[el] * b.components[i];
            }
            res[i] += b.components[i];
        }

        return new(res);
    }
}