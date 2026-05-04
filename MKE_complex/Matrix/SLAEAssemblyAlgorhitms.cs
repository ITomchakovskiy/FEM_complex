using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Matrix;

public static class SLAEAssemblyAlgorhitms
{
    public static void AddLocalMatrix<T>(SparseMatrix<T> matrix, double[][] localMatrix, ReadOnlySpan<int> dofs, ReadOnlySpan<int> dofsSortedIndices) where T : INumber<T>
    {
        for(int i = 0; i < dofs.Length; ++i)
            matrix.Di[dofs[i]] += T.CreateChecked(localMatrix[i][i]);
        for(int i = 0; i < dofs.Length; ++i) //
        {
            int dof_i_local = dofsSortedIndices[i];
            int dof_i_global = dofs[dof_i_local];
            int istart = matrix.Ia[dof_i_global];
            int iend = matrix.Ia[dof_i_global + 1];
            for (int j = 0; j < i; ++j)
            {
                int dof_j_local = dofsSortedIndices[j];
                int dof_j_global = dofs[dof_j_local];
                istart = matrix.GetOffDiagonalElementIndex(dof_j_global, istart, iend);
                if(dof_i_local > dof_j_local)
                    matrix.Al[istart] += T.CreateChecked(localMatrix[dof_i_local][dof_j_local]);
                else
                    matrix.Al[istart] += T.CreateChecked(localMatrix[dof_j_local][dof_i_local]);
            }
        }
    }

    public static void AddLocalMatrix<T>(SparseMatrix<T> matrix, double[][] localMatrix, ReadOnlySpan<int> dofs, ReadOnlySpan<int> dofsSortedIndices, Vector.Vector<T> rs, ReadOnlySpan<T> Solution) where T : INumber<T>
    {
        for(int i = 0; i < dofs.Length; ++i)
            if(dofs[i] < matrix.N)
                matrix.Di[dofs[i]] += T.CreateChecked(localMatrix[i][i]);
        for(int i = 1; i < dofs.Length; ++i) //
        {
            int dof_i_local = dofsSortedIndices[i];
            int dof_i_global = dofs[dof_i_local];
            int istart = matrix.Ia[dof_i_global];
            int iend = matrix.Ia[dof_i_global + 1];
            for (int j = 0; j < i; ++j)
            {
                int dof_j_local = dofsSortedIndices[j];
                int dof_j_global = dofs[dof_j_local];
                istart = matrix.GetOffDiagonalElementIndex(dof_j_global, istart, iend);
                if(dof_i_local > dof_j_local)
                    matrix.Al[istart] += T.CreateChecked(localMatrix[dof_i_local][dof_j_local]);
                else
                    matrix.Al[istart] += T.CreateChecked(localMatrix[dof_j_local][dof_i_local]);
            }
        }
    }

    public static void AddLocalRightPart<T>(Vector.Vector<T> pr, double[] localRightPart, ReadOnlySpan<int> dofs) where T : INumber<T>
    {
        for (int i = 0; i < localRightPart.Length; ++i)
            pr.components[dofs[i]] += T.CreateChecked(localRightPart[i]);
    }

    public static void ApplyDirichletConditions<T>(SparseMatrix<T> matrix, Vector.Vector<T> pr, double[] localRightPart, ReadOnlySpan<int> dofs) where T : INumber<T>
    {
        for(int i = 0; i < dofs.Length; ++i)
        {
            int dof = dofs[i];

            pr.components[dof] = T.CreateChecked(localRightPart[i]);
            matrix.Di[dof] = T.CreateChecked(1d);
            //nullify elements with j < dof
            int istart = matrix.Ia[dof];
            int iend = matrix.Ia[dof + 1];

            for(int num = istart; num < iend; ++num)
            {
                int j = matrix.Ja[num];
                pr.components[j] -= matrix.Al[num] * pr.components[dof]; //changes right part
                matrix.Al[num] = T.Zero;                                 //nullify elements with j < dof
            }
            //nullify elements with j > dof

            for(int row = dof + 1; row < matrix.N; ++row)
            {
                istart = matrix.Ia[row];
                iend = matrix.Ia[row + 1];
                int num = Array.BinarySearch(matrix.Ja, istart, iend - istart, dof);
                if(num >= 0)
                {
                    pr.components[row] -= matrix.Al[num] * pr.components[dof]; //changes right part
                    matrix.Al[num] = T.Zero;                                 //nullify elements with j > dof
                }
            }
        }
    }
}