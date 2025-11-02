using MKE_complex.FiniteElements;
using MKE_complex.Matrix;
using MKE_complex.Mesh;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.DofsEnumerators;

public static class DofsEnumerator
{
    public static void EnumerateMeshDofs<VectorT>(IFiniteElementMesh<VectorT> mesh) where VectorT : VectorBase
    {
        mesh.SortElementsByMinimumVertexNumber();
        var edgeList = EdgesListBuilding(mesh.Elements, mesh.Vertices.Length);
        List<(int dofsCount, int dofNumber)> vertexList = new(mesh.Vertices.Length);
        for(int i =0;i < mesh.Vertices.Length; ++i) vertexList.Add((0,0));

        foreach(var element in mesh.Elements) //dofs count on vertex calculation
        {
            int dofsCount = element.DofsOnVertexCount;
            foreach(var vertex in element.Geometry.VertexNumber)
                vertexList[vertex] = (Math.Max(vertexList[vertex].dofsCount, dofsCount),0);
        }

        int dofNumber = 0;
        int elementIndex = 0;
        for(int vertexNumber = 0;  vertexNumber < mesh.Vertices.Length; ++vertexNumber) //dofs enumeration
        {
            int vertexDofsCount = vertexList[vertexNumber].dofsCount; //vertex dof enumeration
            vertexList[vertexNumber] = (vertexDofsCount, dofNumber);
            dofNumber += vertexDofsCount;

            var edgeDictionary = edgeList[vertexNumber];
            foreach (var edgeInfo in edgeDictionary)  //edges dofs enumeration
            {
                int edgeDofsCount = edgeInfo.Value.dofsCount;
                edgeDictionary[edgeInfo.Key] = (edgeDofsCount, dofNumber);
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

        foreach(var element in mesh.Elements) //setting dofs to elements
        {
            element.SetVericesDofs(element.Geometry.VertexNumber.Select(i => vertexList[i].dofNumber));
            for(int i = 0; i < element.Geometry.EdgesCount; ++i)
            {
                var edge = element.Geometry.Edge(i);
                edge = edge.Item1 < edge.Item2 ? edge : (edge.Item2, edge.Item1);
                int edgeDofNumber = edgeList[edge.Item1][edge.Item2].dofNumber;
                element.SetEdgeDofs(i,edgeDofNumber);
            }
        } //нужно сделать еще дофы для граней(3д)!!!
    }
    private static List<Dictionary<int, (int dofsCount, int dofNumber)>> EdgesListBuilding<VectorT>(ReadOnlySpan<IFiniteElement<VectorT>> elements, int vertexCount) where VectorT : VectorBase
    {
        List<Dictionary<int, (int dofsCount, int dofNumber)>> edgesList = new(vertexCount); //key - second vertex number; value - dofs count on edge
        for (int i = 0; i < vertexCount; ++i) edgesList.Add(new());

        foreach (var element in elements)
        {
            for(int edgeNumber = 0; edgeNumber < element.Geometry.EdgesCount; ++edgeNumber)
            {
                var edge = element.Geometry.Edge(edgeNumber);
                edge = edge.Item1 < edge.Item2 ? edge : (edge.Item2, edge.Item1); //ascending order

                var dictionary = edgesList[edge.Item1];
                (int dofsCount, int dofNumber) value;
                if (dictionary.TryGetValue(edge.Item2,out value))
                    dictionary[edge.Item2] = (Math.Max(value.dofsCount, element.DofsOnEdgeCount),0);
                else dictionary[edge.Item2] = (element.DofsOnEdgeCount,0);
            }
        }
        return edgesList;
    }
}