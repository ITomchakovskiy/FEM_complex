using MKE_complex.FiniteElements;
using MKE_complex.Matrix;
using MKE_complex.Mesh;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MKE_complex.DofsEnumerators;

public static class DofsEnumerator
{
    public static void EnumerateMeshDofs<VectorT>(IFiniteElementMesh<VectorT> mesh) where VectorT : VectorBase<double, VectorT>
    {
        //mesh.SortElementsByMinimumVertexNumber();
        var edgeList = EdgesListBuilding(mesh.Elements, mesh.Vertices.Length);
        int[] vertexList = new int[mesh.Vertices.Length];
        
        foreach(var element in mesh.Elements) //dofs count on vertex calculation
        {
            int dofsCount = element.DofsOnVertexCount;
            foreach(var vertex in element.Geometry.VertexNumber)
                vertexList[vertex] = Math.Max(vertexList[vertex], dofsCount);
        }

        int dofNumber = 0;
        int elementIndex = 0;
        for(int vertexNumber = 0;  vertexNumber < mesh.Vertices.Length; ++vertexNumber) //dofs enumeration
        {
            int vertexDofsCount = vertexList[vertexNumber];    //vertex dof enumeration
            vertexList[vertexNumber] = dofNumber; //rewrite count of dofs to a minimum dof number
            dofNumber += vertexDofsCount;

            var edgeDictionary = edgeList[vertexNumber];
            foreach (var edgeInfo in edgeDictionary)  //edges dofs enumeration
            {
                int edgeDofsCount = edgeInfo.Value;
                edgeDictionary[edgeInfo.Key] = dofNumber;
                dofNumber += edgeDofsCount;
            }

            var elements = mesh.Elements;
            for(; elementIndex < mesh.Elements.Length && elements[elementIndex].Geometry.VertexNumber.Min() == vertexNumber; ++elementIndex) //elementsdofsEnumeration
            {
                var element = elements[elementIndex];
                int elementDofsCount = element.DofsOnElementCount;
                element.SetElementDofs(dofNumber);
                dofNumber += elementDofsCount;
            }
        }

        mesh.DofsCount = dofNumber;

        foreach (var element in mesh.Elements) //setting dofs to elements
        {
            element.SetVericesDofs(element.Geometry.VertexNumber.Select(i => vertexList[i]).ToArray());
            for(int i = 0; i < element.Geometry.EdgesCount; ++i)
            {
                var edge = element.Geometry.LocalEdge(i);
                edge = (element.Geometry.VertexNumber[edge.Item1], element.Geometry.VertexNumber[edge.Item2]);
                edge = edge.Item1 < edge.Item2 ? edge : (edge.Item2, edge.Item1);
                int edgeDofNumber = edgeList[edge.Item1][edge.Item2];
                element.SetEdgeDofs(i,edgeDofNumber);
            }
        } //нужно сделать еще дофы для граней(3д)!!!

        foreach (var boundary in mesh.Boundaries) //setting dofs to boundary conditions
        {
            boundary.SetVericesDofs(boundary.Geometry.VertexNumber.Select(i => vertexList[i]).ToArray());
            for (int i = 0; i < boundary.Geometry.EdgesCount; ++i)
            {
                var edge = boundary.Geometry.LocalEdge(i);
                edge = (boundary.Geometry.VertexNumber[edge.Item1], boundary.Geometry.VertexNumber[edge.Item2]);
                edge = edge.Item1 < edge.Item2 ? edge : (edge.Item2, edge.Item1);
                int edgeDofNumber = edgeList[edge.Item1][edge.Item2];
                boundary.SetEdgeDofs(i, edgeDofNumber);
            }
        }
    }
    private static Dictionary<int, int>[] EdgesListBuilding<VectorT>(ReadOnlySpan<IFiniteElement<VectorT>> elements, int vertexCount) where VectorT : VectorBase<double, VectorT>
    {
        Dictionary<int,int>[] edgesList = new Dictionary<int, int>[vertexCount]; //key - second vertex number; value - dofs count on edge

        for(int i = 0; i < edgesList.Length; ++i) edgesList[i] = new Dictionary<int, int>();


        foreach (var element in elements)
        {
            for(int edgeNumber = 0; edgeNumber < element.Geometry.EdgesCount; ++edgeNumber)
            {
                var edge = element.Geometry.LocalEdge(edgeNumber);
                edge = (element.Geometry.VertexNumber[edge.Item1], element.Geometry.VertexNumber[edge.Item2]);
                edge = edge.Item1 < edge.Item2 ? edge : (edge.Item2, edge.Item1); //ascending order

                var dictionary = edgesList[edge.Item1];
                int value;
                if (dictionary.TryGetValue(edge.Item2,out value))
                    dictionary[edge.Item2] = Math.Max(value, element.DofsOnEdgeCount);
                else dictionary[edge.Item2] = element.DofsOnEdgeCount;
            }
        }
        return edgesList;
    }
}