using MKE_complex.FiniteElements;
using MKE_complex.Mesh;
using MKE_complex.Tests;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Matrix;

public class MatrixProfileBuilder
{
    public static SparseMatrix<T> BuildMatrixProfile<T,VectorT>(IFiniteElementMesh<VectorT> mesh, int N) where T : INumber<T> where VectorT : VectorBase<double, VectorT>
    {
        if (mesh.DofsCount is null)
            throw new InvalidOperationException("Mesh DOFs are not set.");
        var DofsConnections = new HashSet<int>[(int)mesh.DofsCount];
        for(int i = 0; i < mesh.DofsCount; i++)
            DofsConnections[i] = new HashSet<int>();

        foreach (var element in mesh.Elements)
        {
            var dofs = element.SortedDofs;
            for (int i = 1; i < dofs.Length; ++i)
            {
                int dof_i = dofs[i];
                for (int j = 0; j < i; ++j)
                {
                    int dof_j = dofs[j];
                    if (!element.IsDofsConnected(dof_i, dof_j)) continue;

                    DofsConnections[dof_i].Add(dof_j);
                }
            }
        }

        var ia = new int[N+1];

        var jaList = new List<int>();

        for(int i = 0; i < N; ++i)
        {
            var set = DofsConnections[i];
            ia[i + 1] = ia[i] + set.Count();
            jaList.AddRange(set.OrderBy(x => x));
        }
            
        return new SparseMatrix<T>(ia, CollectionsMarshal.AsSpan(jaList), true);
    }

    public static SparseMatrix<T> BuildMatrixProfile<T,VectorT>(IFiniteElementMesh<VectorT> mesh) where T : INumber<T> where VectorT : VectorBase<double, VectorT>
    {
        if (mesh.DofsCount is null)
            throw new InvalidOperationException("Mesh DOFs are not set.");
        return BuildMatrixProfile<T,VectorT>(mesh, (int)mesh.DofsCount);
    }

    public static SparseMatrix<T> BuildBoundariesMatrixProfile<T,VectorT>(ReadOnlySpan<IBoundaryCondition<VectorT>> boundaries, int N) where T : INumber<T> where VectorT : VectorBase<double, VectorT>
    {
        var DofsConnections = new HashSet<int>[N];
        for(int i = 0; i < N; i++)
            DofsConnections[i] = new HashSet<int>();

        foreach (var boundary in boundaries)
        {
            var dofs = boundary.SortedDofs;
            for (int i = 1; i < dofs.Length; ++i)
            {
                int dof_i = dofs[i];
                for (int j = 0; j < i; ++j)
                {
                    int dof_j = dofs[j];
                    // if (!boundary.IsDofsConnected(dof_i, dof_j)) continue;

                    DofsConnections[dof_i].Add(dof_j);
                }
            }
        }

        var ia = new int[N+1];

        var jaList = new List<int>();

        for(int i = 0; i < N; ++i)
        {
            var set = DofsConnections[i];
            ia[i + 1] = ia[i] + set.Count();
            jaList.AddRange(set.OrderBy(x => x));
        }
            
        return new SparseMatrix<T>(ia, CollectionsMarshal.AsSpan(jaList), true);
    }
}
