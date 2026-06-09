using MKE_complex.DofsEnumerators;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.EdgeConditions;
using MKE_complex.FiniteElements.Elements.ElementsClasses._2D.Lagrangian.TriangleElements;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public class FiniteElementMesh<VectorT>(IReadOnlyList<VectorT> vertices, IReadOnlyList<IFiniteElement<VectorT>> elements, IReadOnlyList<IBoundaryCondition<VectorT>> edges) : IFiniteElementMesh<VectorT>, IIntgrateMesh<VectorT> where VectorT : VectorBase<double, VectorT>
{
    private List<VectorT> vertices { get; init; } = (List<VectorT>)vertices;
    ReadOnlySpan<VectorT> IFiniteElementMesh<VectorT>.Vertices => CollectionsMarshal.AsSpan(vertices);
    private List<IFiniteElement<VectorT>> elements { get; init; } = (List<IFiniteElement<VectorT>>)elements;
    ReadOnlySpan<IFiniteElement<VectorT>> IFiniteElementMesh<VectorT>.Elements => CollectionsMarshal.AsSpan(elements);
    public List<IBoundaryCondition<VectorT>> boundaries { get; init; } = (List<IBoundaryCondition<VectorT>>)edges;
    ReadOnlySpan<IBoundaryCondition<VectorT>> IFiniteElementMesh<VectorT>.Boundaries => CollectionsMarshal.AsSpan(boundaries);

    private int? dofsCount;

    public int? DofsCount
    {
        get => dofsCount;
        set
        {
            if (dofsCount != null)
                throw new InvalidOperationException("Dofs count has already been set");
            dofsCount = value;
        }
    }

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
                if (geometry is Triangle<Vector2D>)
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
                if (geometry is Line<Vector2D>)
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
                if(element is TriangleLagrangianFiniteElement cube && vertices is List<Vector2D> ver2)
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
                if (edge is LagrangianEdgeCondition cube && vertices is List<Vector2D> ver2)
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

    public IFiniteElementMesh<VectorT> Refine() //Subject to changes!!!
    {
        //ДОБАВИТЬ СПИСОК ГРАНЕЙ ДЛЯ 3Д

        var EdgesList = DofsEnumerator.EdgesListBuilding(CollectionsMarshal.AsSpan(elements), vertices.Count);

        List<VectorT> NewVertexList = vertices.ToList();

        List<IFiniteElement<VectorT>> NewElementList = new();

        List<IBoundaryCondition<VectorT>> NewBoundaryList = new();

        for (int i = 0; i < EdgesList.Length; ++i) //adding points on edges
        {
            VectorT A = vertices[i];
            var dict = EdgesList[i];
            foreach(var val in dict)
            {
                VectorT B = vertices[val.Key];
                VectorT C = (A + B) / 2d;
                NewVertexList.Add(C);
                dict[val.Key] = NewVertexList.Count - 1;
            }
        }

        foreach(var element in elements) //elements refinement and adding element center if needed
        {
            bool isElementVertexNeeded = false;
            List<int> edgeVertices = new();
            for(int i = 0; i < element.Geometry.EdgesCount;++i)
            {
                var edge = element.Geometry.LocalEdge(i);
                (int i, int j) globalEdge = (element.Geometry.VertexNumber[edge.Item1],
                                             element.Geometry.VertexNumber[edge.Item2]);
                globalEdge = globalEdge.i < globalEdge.j ? globalEdge : (globalEdge.j, globalEdge.i);
                edgeVertices.Add(EdgesList[globalEdge.i][globalEdge.j]);
            }
            NewElementList.AddRange(element.Refine([], edgeVertices.ToArray(), NewVertexList.Count,out isElementVertexNeeded));

            // if(isElementVertexNeeded)
            // {
            //     var local_vertices = element.Geometry.VertexNumber.Select(i => vertices[i]).ToArray();
            //     //VectorT elementCenter = element.Geometry.CalculateCenterVertex(local_vertices);
            //     //NewVertexList.Add(elementCenter);
            // }
        }

        foreach(var boundary in boundaries) //refining boundary conditions
        {
            List<int> edgeVertices = new();
            for (int i = 0; i < boundary.Geometry.EdgesCount; ++i)
            {
                var edge = boundary.Geometry.LocalEdge(i);
                (int i, int j) globalEdge = (boundary.Geometry.VertexNumber[edge.Item1],
                                             boundary.Geometry.VertexNumber[edge.Item2]);
                globalEdge = globalEdge.i < globalEdge.j ? globalEdge : (globalEdge.j, globalEdge.i);
                edgeVertices.Add(EdgesList[globalEdge.i][globalEdge.j]);
            }
            NewBoundaryList.AddRange(boundary.Refine([], edgeVertices.ToArray()));
        }

        return new FiniteElementMesh<VectorT>(NewVertexList, NewElementList, NewBoundaryList);
    }

    public bool IsMeshConforming() //subject to changes!!! now only for 3d
    {
        Dictionary<int, Dictionary<int, int>>[] FaceList;
        var elements3D = elements.OfType<IFiniteElement3D>();
        FaceList = DofsEnumerator.FacesListBuilding(elements3D.ToArray(), vertices!.Count());

        foreach(var dict1 in FaceList) //nullify element count for faces
        {
            foreach(var dict2 in dict1.Values)
            {
                foreach(var elem in dict2.Keys)
                {
                    dict2[elem] = 0;
                }
            }
        }

        foreach(var elem in elements.OfType<IFiniteElement3D>())
        {
            for(int i = 0; i < elem.Geometry.FacesCount; ++i)
            {
                var faceGlobal = elem.Geometry.GlobalFace(i);
                var invariantFace = GeometricMethods.SimplifyFace(faceGlobal);
                FaceList[invariantFace[0]][invariantFace[1]][invariantFace[2]] += 1;
            }
        }

        foreach(var boundary in boundaries.OfType<IBoundaryCondition3D>())
        {
            var faceGlobal = boundary.Geometry.GlobalFace(0);
            var invariantFace = GeometricMethods.SimplifyFace(faceGlobal);
            FaceList[invariantFace[0]][invariantFace[1]][invariantFace[2]] += 1;
        }


        foreach(var dict1 in FaceList) //nullify element count for faces
        {
            foreach(var dict2 in dict1.Values)
            {
                foreach(var neighboursCount in dict2.Values)
                {
                    if(neighboursCount != 2)
                        return false;
                }
            }
        }

        return true;
    }

    public double Integrate(Func<VectorT, double> F, int scheme)
    {
        double res = 0d;
        foreach(var elem in elements.OfType<IIntegrationElement<VectorT>>())
        {
            if(elem is IFiniteElement<VectorT> felem)
            {
                var vertices = felem.Geometry.VertexNumber.Select(i => this.vertices[i]);
                res += elem.IntegrateElement(vertices.ToArray(), F, scheme);
            }
        }
        return res;
    }

    public double IntegrateDiscrepancy(Func<VectorT, double> F, ReadOnlySpan<double> Solution, int scheme)
    {
        double res = 0d;
        var SolutionCopy = Solution.ToArray();
        foreach(var elem in elements.OfType<IIntegrationElement<VectorT>>())
        {
            if(elem is IFiniteElement<VectorT> felem)
            {
                var vertices = felem.Geometry.VertexNumber.Select(i => this.vertices[i]);

                var localSolution = felem.DOFs.Select(i => SolutionCopy[i]).ToArray();
                double diff = elem.IntegrateDiscrepancy(vertices.ToArray(), F, localSolution, scheme);
                if(diff > 1d)
                {
                    int amogus = 0;
                    ++amogus;
                }
                res += diff;
            }
        }
        return Math.Sqrt(res);
    }
}
