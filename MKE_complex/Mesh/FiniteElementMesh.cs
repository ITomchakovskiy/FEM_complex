using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public class FiniteElementMesh<VectorT>(IReadOnlyList<VectorT> vertices, IReadOnlyList<IFiniteElement<VectorT>> elements, IReadOnlyList<IBoundaryCondition<VectorT>> edges) : IFiniteElementMesh<VectorT> where VectorT : VectorBase
{
    private List<VectorT> vertices { get; init; } = (List<VectorT>)vertices;
    ReadOnlySpan<VectorT> IFiniteElementMesh<VectorT>.Vertices => CollectionsMarshal.AsSpan(vertices);
    private List<IFiniteElement<VectorT>> elements { get; init; } = (List<IFiniteElement<VectorT>>)elements;
    ReadOnlySpan<IFiniteElement<VectorT>> IFiniteElementMesh<VectorT>.Elements => CollectionsMarshal.AsSpan(elements);
    public List<IBoundaryCondition<VectorT>> boundaries { get; init; } = (List<IBoundaryCondition<VectorT>>)edges;
    ReadOnlySpan<IBoundaryCondition<VectorT>> IFiniteElementMesh<VectorT>.Boundaries => CollectionsMarshal.AsSpan(boundaries);

    public void SaveMeshGeometry(string VertexFileName, string ElementsFileName, string DofsFileName ,string EdgesFileName, string EdgeDofsFileName) //функция для тестов треугольных и тетраэдральных сеток
    {
        string vertexPath = Path.Combine(AppContext.BaseDirectory, VertexFileName);

        //string? line;
        try
        {
            StreamWriter swVertex = new(vertexPath);

            //srVertex.WriteLine(Vertices.Count);
            foreach (var vertex in vertices)
            {
                if (vertex is Vector2D vec2)
                    swVertex.Write($"{vec2.X} ");
            }
            swVertex.Write("\n");
            foreach (var vertex in vertices)
            {
                if (vertex is Vector2D vec2)
                    swVertex.Write($"{vec2.Y} ");
            }
            swVertex.Write("\n");
            foreach (var vertex in vertices)
                swVertex.Write("0 ");
            swVertex.Close();

            string elementsPath = Path.Combine(AppContext.BaseDirectory, ElementsFileName);

            StreamWriter swElements = new(elementsPath);

            //srElements.WriteLine(Elements.Count);
            foreach (var element in elements)
            {
                var geometry = element.Geometry;
                if (geometry is Triangle)
                {
                    for (int i = 0; i < geometry.VertexNumber.Length; ++i)
                        swElements.Write($"{geometry.VertexNumber[i]} ");
                    swElements.Write("\n");
                }
                else throw new NotImplementedException();
            }
            
            swElements.Close();

            string edgesPath = Path.Combine(AppContext.BaseDirectory, EdgesFileName);

            StreamWriter swEdges = new(edgesPath);

            foreach (var edge in boundaries)
            {
                var geometry = edge.Geometry;
                if (geometry is Line)
                {
                    for (int i = 0; i < geometry.VertexNumber.Length; ++i)
                        swEdges.Write($"{geometry.VertexNumber[i]} ");
                    swEdges.Write($"{geometry.VertexNumber[0]} ");
                    swEdges.Write("\n");
                }
                else throw new NotImplementedException();
            }

            swEdges.Close();

            string DofsPath = Path.Combine(AppContext.BaseDirectory, DofsFileName);

            StreamWriter swDofs = new(DofsPath);

            List<double> x = new();
            List<double> y = new();
            List<int> dofs = new();

            foreach (var element in elements)
            {
                if(element is TriangleLagrangianCubicFiniteElement cube && vertices is List<Vector2D> ver2)
                {
                    var info = cube.ReturnDofs(CollectionsMarshal.AsSpan(ver2));
                    x.AddRange(info.x);
                    y.AddRange(info.y);
                    dofs.AddRange(info.dofs);
                }
            }

            for(int i = 0; i < x.Count; ++i)
                swDofs.Write($"{x[i]} ");
            swDofs.Write("\n");
            for (int i = 0; i < x.Count; ++i)
                swDofs.Write($"{y[i]} ");
            swDofs.Write("\n");
            for (int i = 0; i < x.Count; ++i)
                swDofs.Write($"{dofs[i]} ");
            swDofs.Close();

            string EdgeDofsPath = Path.Combine(AppContext.BaseDirectory, EdgeDofsFileName);

            StreamWriter swEdgeDofs = new(EdgeDofsPath);

            x = new();
            y = new();
            dofs = new();

            foreach (var edge in edges)
            {
                if (edge is LagrangianCubicEdgeCondition cube && vertices is List<Vector2D> ver2)
                {
                    var info = cube.ReturnDofs(CollectionsMarshal.AsSpan(ver2));
                    x.AddRange(info.x);
                    y.AddRange(info.y);
                    dofs.AddRange(info.dofs);
                }
            }

            for (int i = 0; i < x.Count; ++i)
                swEdgeDofs.Write($"{x[i]} ");
            swEdgeDofs.Write("\n");
            for (int i = 0; i < x.Count; ++i)
                swEdgeDofs.Write($"{y[i]} ");
            swEdgeDofs.Write("\n");
            for (int i = 0; i < x.Count; ++i)
                swEdgeDofs.Write($"{dofs[i]} ");
            swEdgeDofs.Close();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private class ElementComparer : Comparer<IFiniteElement<VectorT>>
    {
        public override int Compare(IFiniteElement<VectorT>? x, IFiniteElement<VectorT>? y)
        {
            if(x == null || y == null) throw new ArgumentNullException();
            int minVertexNumberX = x.Geometry.VertexNumber.Min();
            int minVertexNumberY = y.Geometry.VertexNumber.Min();
            return minVertexNumberX - minVertexNumberY;
        }
    }

    public void SortElementsByMinimumVertexNumber()
    {
        var comparer = new ElementComparer();
        elements.Sort(comparer);
    }
}
