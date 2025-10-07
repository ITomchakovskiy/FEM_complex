using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh;

public class FiniteElementMesh<VectorT>(IReadOnlyList<VectorT> vertices, IReadOnlyList<IFiniteElement<VectorT>> elements, IReadOnlyList<IFiniteElementEdge<VectorT>> edges) : IFiniteElementMesh<VectorT> where VectorT : VectorBase
{
    public List<VectorT> Vertices { get; init; } = (List<VectorT>)vertices;
    IReadOnlyList<VectorT> IFiniteElementMesh<VectorT>.Vertices => Vertices;
    public List<IFiniteElement<VectorT>> Elements { get; init; } = (List<IFiniteElement<VectorT>>)elements;
    IReadOnlyList<IFiniteElement<VectorT>> IFiniteElementMesh<VectorT>.Elements => Elements;
    public List<IFiniteElementEdge<VectorT>> Edges { get; init; } = (List<IFiniteElementEdge<VectorT>>)edges;
    IReadOnlyList<IFiniteElementEdge<VectorT>> IFiniteElementMesh<VectorT>.Edges => Edges;

    public void SaveMeshGeometry(string VertexFileName, string ElementsFileName) //функция для тестов треугольных и тетраэдральных сеток
    {
        string vertexPath = Path.Combine(AppContext.BaseDirectory, VertexFileName);

        //string? line;
        try
        {
            StreamWriter srVertex = new(vertexPath);

            srVertex.WriteLine(Vertices.Count);
            foreach (var vertex in Vertices)
            {
                if (vertex is Vector2D vec2)
                    srVertex.WriteLine($"{vec2.X} {vec2.Y}");
                else if (vertex is Vector3D vec3)
                    srVertex.WriteLine($"{vec3.X} {vec3.Y} {vec3.Z}");
            }
            srVertex.Close();

            string elementsPath = Path.Combine(AppContext.BaseDirectory, ElementsFileName);

            StreamWriter srElements = new(elementsPath);

            srElements.WriteLine(Elements.Count);
            foreach (var element in Elements)
            {
                var geometry = element.Geometry;
                if (geometry is Triangle)
                {
                    for (int i = 0; i < geometry.VertexNumber.Length; ++i)
                        srElements.Write($"{geometry.VertexNumber[i]} ");
                    srElements.Write("\n");
                }
                else throw new NotImplementedException();
            }
            srElements.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
