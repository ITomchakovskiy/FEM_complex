using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh.MeshBuilder;

//public record PseudoRegularMeshData(
//    Vector2D[,] lines,
//    double[]? z_lines,
//    string[] materials,
//    int[,] areaBorders,

//    int[] x_intervals,
//    int[] y_intervals,
//    int[]? z_intervals,

//    double[] x_stretch,
//    double[] y_stretch,
//    double[]? z_stretch);

public class PseudoRegularMeshBuilder : IMeshBuilder
{

    private Vector2D[,]? lines;
    private double[]? z_lines;
    private string[]? materials;
    private int[,]? areaBorders;

    private int[]? x_intervals;
    private int[]? y_intervals;
    private int[]? z_intervals;

    private double[]? x_stretch;
    private double[]? y_stretch;
    private double[]? z_stretch;

    private Dimension dimension;
    public IFiniteElementMesh<VectorT> BuildMesh<VectorT>(Dimension dimension, GeometryType meshType, BasisType basisType, int order, string[] fileNames) where VectorT : IVector
    {
        this.dimension = dimension;
        ReadFile(fileNames[0], fileNames[1], dimension);

        if (lines == null || materials == null || areaBorders == null || x_intervals == null || y_intervals == null || x_stretch == null || y_stretch == null)
            throw new Exception("Error in reading mesh file");

        //List<IFiniteElementGeometry<IVector>> elementsGeometry = new();

        List<VectorT> vertices = new();

        List<IFiniteElement<VectorT>> elements = new();

        //List<IFiniteElement<Vector3D>> elements3d = new();

        //add points on coordinate lines
        switch (dimension)
        {
            case Dimension.D2:
                {
                    for(int i = 0; i < lines.GetLength(0); ++i)
                    {
                        for (int j = 0; j < lines.GetLength(1); ++j)
                            if(vertices is List<Vector2D> vertices2d)
                                vertices2d.Add(lines[i, j]);
                    }
                    break;
                }
            case Dimension.D3:
                {
                    foreach(double z in z_lines!)
                    {
                        for(int j = 0; j < lines.GetLength(0); ++j)
                        {
                            for(int k = 0; k < lines.GetLength(1);++k)
                            {
                                Vector2D xy = lines[j, k];
                                Vector3D vector = new(xy.X, xy.Y, z);
                                if (vertices is List<Vector3D> vertices3d)
                                    vertices3d.Add(vector);
                            }
                        }
                    }
                    break;
                }
            default: throw new NotImplementedException();
        }

        //number of points on the subdomain borders
        Dictionary<(int y, int z, int x_left, int x_right), int> vertices_on_x_lines = new();
        Dictionary<(int x, int z, int y_left, int y_right), int> vertices_on_y_lines = new();
        Dictionary<(int x, int y, int z_left, int z_right), int> vertices_on_z_lines = new();

        for (int w = 0; w < materials.Length; ++w)
        {
            string material = materials[w];

            int x0 = areaBorders[w, 0];
            int x1 = areaBorders[w, 1];

            int y0 = areaBorders[w, 2];
            int y1 = areaBorders[w, 3];

            int z0 = areaBorders[w, 4];
            int z1 = areaBorders[w, 5];

            for (int x_line = x0; x_line < x1; ++x_line)
            {
                //addition of points on subdomain borders
                //y const,z const borders
                int n = x_intervals[x_line];
                double k = x_stretch[x_line];

                //z = z0; y = y0
                FillBorder("x", vertices, vertices_on_x_lines, x_line, y0, z0, n, k);
                //z = z0; y = y1
                FillBorder("x", vertices, vertices_on_x_lines, x_line, y1, z0, n, k);
                //z = z1; y = y0
                FillBorder("x", vertices, vertices_on_x_lines, x_line, y0, z1, n, k);
                //z = z1; y = y1
                FillBorder("x", vertices, vertices_on_x_lines, x_line, y1, z1, n, k);
            }
            for (int y_line = y0; y_line < y1; ++y_line)
            {
                //addition of points on subdomain borders
                //x const,z const borders
                int n = y_intervals[y_line];
                double k = y_stretch[y_line];

                //z = z0; x = x0
                FillBorder("y", vertices, vertices_on_y_lines, x0, y_line, z0, n, k);
                //z = z0; x = x1                               
                FillBorder("y", vertices, vertices_on_y_lines, x1, y_line, z0, n, k);
                //z = z1; x = x0                                  
                FillBorder("y", vertices, vertices_on_y_lines, x0, y_line, z1, n, k);
                //z = z1; x = x1                                   
                FillBorder("y", vertices, vertices_on_y_lines, x1, y_line, z1, n, k);
            }
            if(dimension is Dimension.D3)
            {
                for (int z_line = z0; z_line < z1; ++z_line)
                {
                    //addition of points on subdomain borders
                    //x const,y const borders
                    int n = z_intervals![z_line];
                    double k = z_stretch![z_line];

                    //y = y0; x = x0
                    FillBorder("z", vertices, vertices_on_z_lines, x0, y0, z_line, n, k);
                    //z = y0; x = x1                               
                    FillBorder("z", vertices, vertices_on_z_lines, x1, y0, z_line, n, k);
                    //z = y1; x = x0                                  
                    FillBorder("z", vertices, vertices_on_z_lines, x0, y1, z_line, n, k);
                    //z = y1; x = x1                                   
                    FillBorder("z", vertices, vertices_on_z_lines, x1, y1, z_line, n, k);
                }
            }

            //addition of subdomain inner vertices
            int inner_index_start = vertices.Count; //start index for inner vertices

            switch (dimension)
            {
                case Dimension.D2:
                    {
                        for (int y_line = y0; y_line < y1; ++y_line)
                        {
                            int y_ind_start = y_line == y0 ? 1 : 0;
                            for (int y_ind = y_ind_start; y_ind < y_intervals[y_line]; ++y_ind)
                            {
                                for (int x_line = x0; x_line < x1; ++x_line)
                                {
                                    int x_ind_start = x_line == x0 ? 1 : 0;
                                    for (int x_ind = x_ind_start; x_ind < x_intervals[x_line]; ++x_ind)
                                    {
                                        Vector2D vertex = PointOnLine(lines[y_line, x_line], lines[y_line + 1, x_line + 1], x_intervals[x_line], x_stretch[x_line], y_intervals[y_line], y_stretch[y_line], x_ind, y_ind);
                                        if(vertices is List<Vector2D> vertices2d)
                                            vertices2d.Add(vertex);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case Dimension.D3:
                    {
                        for (int z_line = z0; z_line < z1; ++z_line)
                        {
                            int z_ind_start = z_line == z0 ? 1 : 0;
                            for (int z_ind = z_ind_start; z_ind < z_intervals![z_line]; ++z_ind)
                            {
                                for (int y_line = y0; y_line < y1; ++y_line)
                                {
                                    int y_ind_start = y_line == y0 ? 1 : 0;
                                    for (int y_ind = y_ind_start; y_ind < y_intervals[y_line]; ++y_ind)
                                    {
                                        for (int x_line = x0; x_line < x1; ++x_line)
                                        {
                                            int x_ind_start = x_line == x0 ? 1 : 0;
                                            for (int x_ind = x_ind_start; x_ind < x_intervals[x_line]; ++x_ind)
                                            {
                                                Vector3D p1 = new(lines[y_line, x_line].X, lines[y_line, x_line].Y, z_lines![z_line]);
                                                Vector3D p2 = new(lines[y_line + 1, x_line + 1].X, lines[y_line + 1, x_line + 1].Y, z_lines![z_line + 1]);
                                                Vector3D vertex = PointOnLine(p1, p2, x_intervals[x_line], x_stretch[x_line], y_intervals[y_line], y_stretch[y_line], z_intervals[z_line], z_stretch![z_line], x_ind, y_ind, z_ind);
                                                if(vertices is List<Vector3D> vertices3d)
                                                    vertices3d.Add(vertex);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
            }
            //forming of Quadrangles or Hexagons

            List<IFiniteElementGeometry<VectorT>> elementsGeometry = new();

            switch (dimension)
            {
                case Dimension.D2:
                    {
                        int n_x = x_intervals.Sum() + 1;
                        int n_y = y_intervals.Sum() + 1;

                        int x_interval_points = 0;
                        int y_interval_points = 0;

                        for (int y_line = y0; y_line < y1; y_interval_points += y_intervals[y_line],++y_line)
                        {
                            for (int y_ind = 0; y_ind < y_intervals[y_line]; ++y_ind)
                            {
                                for (int x_line = x0; x_line < x1;x_interval_points += x_intervals[x_line], ++x_line)
                                {
                                    for (int x_ind = 0; x_ind < x_intervals[x_line]; ++x_ind)
                                    {
                                        (int x, int y) local_index_start = (x_interval_points + x_ind, y_interval_points + y_ind);
                                        (int x, int y)[] quadrangle_local_indices = [local_index_start,(local_index_start.x, local_index_start.y + 1),
                                                                                                       (local_index_start.x + 1, local_index_start.y + 1),
                                                                                                       (local_index_start.x + 1, local_index_start.y)];
                                        int[] quadrangle_vertex_numbers = new int[quadrangle_local_indices.Length];
                                        for (int i = 0; i < quadrangle_vertex_numbers.Length; ++i)
                                        {
                                            (int x, int y) local_index = quadrangle_local_indices[i];

                                            if ((local_index.x == 0 || local_index.x == n_x - 1) && (local_index.y == 0 || local_index.y == n_y - 1)) //vertices of subdomain
                                            {
                                                int shift_x = local_index.x == 0 ? 0 : 1;
                                                int shift_y = local_index.y == 0 ? 0 : 1;
                                                quadrangle_vertex_numbers[i] = lines.GetLength(1) * (y_line + shift_y) + x_line + shift_x;
                                            }
                                            else if ((local_index.x == 0 || local_index.x == n_x - 1))  //x0 and x1 border
                                            {
                                                int shift = local_index.x == 0 ? 0 : 1;
                                                quadrangle_vertex_numbers[i] = vertices_on_y_lines[(x_line + shift, 0, y_line, y_line + 1)] + y_ind - 1;
                                            }
                                            else if ((local_index.y == 0 || local_index.y == n_y - 1)) //y0 and y1 border
                                            {
                                                int shift = local_index.y == 0 ? 0 : 1;
                                                quadrangle_vertex_numbers[i] = vertices_on_x_lines[(y_line + shift, 0, x_line, x_line + 1)] + x_ind - 1;
                                            }
                                            else  //inner vertices
                                                quadrangle_vertex_numbers[i] = inner_index_start + (n_x - 2) * (local_index.y - 1) + local_index.x - 1;
                                        }
                                        Quadrangle quadrangle = new(quadrangle_vertex_numbers);
                                        if (elementsGeometry is List<IFiniteElementGeometry<Vector2D>> elementsGeometry2d)
                                        {
                                            switch (meshType)
                                            {
                                                case GeometryType.Triangle:
                                                    elementsGeometry2d.AddRange(quadrangle.ToTriangles());
                                                    break;
                                                case GeometryType.Quadrangle:
                                                    elementsGeometry2d.Add(quadrangle);
                                                    break;
                                                default:
                                                    throw new NotSupportedException();
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        break;
                    }
                case Dimension.D3:
                    {
                        int n_x = x_intervals.Sum() + 1;
                        int n_y = y_intervals.Sum() + 1;
                        int n_z = z_intervals!.Sum() - 1;

                        int x_interval_points = 0;
                        int y_interval_points = 0;
                        int z_interval_points = 0;
                        for (int z_line = z0; z_line < z1;z_interval_points += z_intervals![z_line],++z_line)
                        {
                            for (int z_ind = 0; z_ind < z_intervals![z_line]; ++z_ind)
                            {
                                for (int y_line = y0; y_line < y1; y_interval_points += y_intervals[y_line], ++y_line)
                                {
                                    for (int y_ind = 0; y_ind < y_intervals[y_line]; ++y_ind)
                                    {
                                        for (int x_line = x0; x_line < x1; x_interval_points += x_intervals[x_line], ++x_line)
                                        {
                                            for (int x_ind = 0; x_ind < x_intervals[x_line]; ++x_ind)
                                            {
                                                (int x, int y, int z) local_index_start = (x_interval_points + x_ind, y_interval_points + y_ind, z_interval_points + z_ind);
                                                (int x, int y, int z)[] hexagon_local_indices = [local_index_start,(local_index_start.x, local_index_start.y + 1, local_index_start.z),
                                                                                                                      (local_index_start.x + 1, local_index_start.y + 1, local_index_start.z),
                                                                                                                      (local_index_start.x + 1, local_index_start.y, local_index_start.z),
                                                                                                                      (local_index_start.x, local_index_start.y, local_index_start.z + 1),
                                                                                                                      (local_index_start.x, local_index_start.y + 1, local_index_start.z + 1),
                                                                                                                      (local_index_start.x + 1, local_index_start.y + 1, local_index_start.z + 1),
                                                                                                                      (local_index_start.x + 1, local_index_start.y, local_index_start.z + 1)];
                                                int[] hexagon_vertex_numbers = new int[hexagon_local_indices.Length];

                                                for (int i = 0; i < hexagon_vertex_numbers.Length; ++i)
                                                {
                                                    (int x, int y, int z) local_index = hexagon_local_indices[i];

                                                    if ((local_index.x == 0 || local_index.x == n_x - 1) && (local_index.y == 0 || local_index.y == n_y - 1) && (local_index.z == 0 || local_index.z == n_z - 1)) //vertices of subdomain
                                                    {
                                                        int shift_x = local_index.x == 0 ? 0 : 1;
                                                        int shift_y = local_index.y == 0 ? 0 : 1;
                                                        int shift_z = local_index.z == 0 ? 0 : 1;
                                                        hexagon_vertex_numbers[i] = lines.GetLength(0)* lines.GetLength(1) * (z_line + shift_z) + lines.GetLength(1) * (y_line + shift_y) + x_line + shift_x;
                                                    }
                                                    else if ((local_index.y == 0 || local_index.y == n_y - 1) && (local_index.z == 0 || local_index.z == n_z - 1)) //y const and z const borders
                                                    {
                                                        int shift_y = local_index.y == 0 ? 0 : 1;
                                                        int shift_z = local_index.z == 0 ? 0 : 1;
                                                        hexagon_vertex_numbers[i] = vertices_on_x_lines[(y_line + shift_y, z_line + shift_z, x_line, x_line + 1)] + x_ind - 1;
                                                    }
                                                    else if ((local_index.x == 0 || local_index.x == n_x - 1) && (local_index.z == 0 || local_index.z == n_z - 1)) //x const and z const borders
                                                    {
                                                        int shift_x = local_index.x == 0 ? 0 : 1;
                                                        int shift_z = local_index.z == 0 ? 0 : 1;
                                                        hexagon_vertex_numbers[i] = vertices_on_y_lines[(x_line + shift_x, z_line + shift_z, y_line, y_line + 1)] + y_ind - 1;
                                                    }
                                                    else if ((local_index.x == 0 || local_index.x == n_x - 1) && (local_index.y == 0 || local_index.y == n_y - 1)) //x const and y const borders
                                                    {
                                                        int shift_x = local_index.x == 0 ? 0 : 1;
                                                        int shift_y = local_index.y == 0 ? 0 : 1;
                                                        hexagon_vertex_numbers[i] = vertices_on_y_lines[(x_line + shift_x, y_line + shift_y, z_line, z_line + 1)] + z_ind - 1;
                                                    }
                                                    else
                                                        hexagon_vertex_numbers[i] = inner_index_start + (n_x - 2) * (n_y - 2) * (local_index.z - 1) + (n_x - 2) * (local_index.y - 1) + local_index.x - 1;
                                                }
                                                Hexagon hexagon = new(hexagon_vertex_numbers);
                                                if(elementsGeometry is List<IFiniteElementGeometry<Vector3D>> elementsGeometry3d)
                                                switch(meshType)
                                                {
                                                    case GeometryType.Hexagon:
                                                        elementsGeometry3d.Add(hexagon);
                                                        break;
                                                    default:
                                                        throw new NotSupportedException();
                                                }
                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
            }
            foreach(var geometry in elementsGeometry)
                elements.Add(FiniteElementsCreator.CreateFiniteElement(meshType, basisType, order, material, geometry));
        }

        return new FiniteElementMesh<VectorT>(vertices, elements);
    }

    private void FillBorder<VectorT>(string coordinate, List<VectorT> vertices, Dictionary<(int, int , int , int ),int> borderDictionary, int x, int y, int z,int n, double k) where VectorT : IVector
    {
        switch(dimension)
        {
            case Dimension.D2:
                {
                    switch(coordinate)
                    {
                        case "x":
                            {
                                var key = (y, 0, x, x + 1);
                                if (!borderDictionary.ContainsKey(key))
                                {
                                    int index = vertices.Count;
                                    borderDictionary[key] = index;
                                    for (int x_ind = 1; x_ind < n; ++x_ind)
                                    {
                                        Vector2D vector = PointOnLine(lines![y, x],
                                                                      lines[y, x + 1], n, k, 0, 0, x_ind, 0);
                                        if(vertices is List<Vector2D> vertices2d)
                                            vertices2d.Add(vector);
                                    }
                                }
                                break;
                            }
                        case "y":
                            {
                                var key = (x, 0, y, y + 1);
                                if (!borderDictionary.ContainsKey(key))
                                {
                                    int index = vertices.Count;
                                    borderDictionary[key] = index;
                                    for (int y_ind = 1; y_ind < n; ++y_ind)
                                    {
                                        Vector2D vector = PointOnLine(lines![y, x], lines[y + 1, x],0,0, n, k, 0, y_ind);
                                        if (vertices is List<Vector2D> vertices2d)
                                            vertices2d.Add(vector);
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
            case Dimension.D3:
                {
                    switch (coordinate)
                    {
                        case "x":
                            {
                                var key = (y, z, x, x + 1);
                                if (!borderDictionary.ContainsKey(key))
                                {
                                    int index = vertices.Count;
                                    borderDictionary[key] = index;
                                    for (int x_ind = 1; x_ind < n; ++x_ind)
                                    {
                                        Vector3D p1 = new(lines![y, x].X,
                                                          lines[y, x].Y, z_lines![z]);
                                        Vector3D p2 = new(lines[y, x + 1].X,
                                                          lines[y, x + 1].Y, z_lines![z]);
                                        Vector3D vector = PointOnLine(p1, p2, n, k, 0, 0, 0, 0, x_ind, 0, 0);

                                        if (vertices is List<Vector3D> vertices3d)
                                            vertices3d.Add(vector);
                                        //vertices.Add(vector);
                                    }
                                }
                                break;
                            }
                        case "y":
                            {
                                var key = (x, z, y, y + 1);
                                if (!borderDictionary.ContainsKey(key))
                                {
                                    int index = vertices.Count;
                                    borderDictionary[key] = index;
                                    for (int y_ind = 1; y_ind < n; ++y_ind)
                                    {
                                        Vector3D p1 = new(lines![y, x].X,
                                                          lines[y, x].Y, z_lines![z]);
                                        Vector3D p2 = new(lines[y + 1, x].X,
                                                          lines[y + 1, x].Y, z_lines![z]);
                                        Vector3D vector = PointOnLine(p1, p2, 0, 0, n, k, 0, 0, 0, y_ind, 0);

                                        if (vertices is List<Vector3D> vertices3d)
                                            vertices3d.Add(vector);
                                    }
                                }
                                break;
                            }
                        case "z":
                            {
                                var key = (x, y, z, z + 1);
                                if (!borderDictionary.ContainsKey(key))
                                {
                                    int index = vertices.Count;
                                    borderDictionary[key] = index;
                                    for (int z_ind = 1; z_ind < n; ++z_ind)
                                    {
                                        Vector3D p1 = new(lines![y, x].X,
                                                          lines[y, x].Y, z_lines![z]);
                                        Vector3D p2 = new(lines[y, x].X,
                                                          lines[y, x].Y, z_lines![z + 1]);
                                        Vector3D vector = PointOnLine(p1, p2, 0, 0, 0, 0, n, k, 0, 0, z_ind);

                                        if (vertices is List<Vector3D> vertices3d)
                                            vertices3d.Add(vector);
                                    }
                                }
                                break;
                            }
                    }
                    break;
                }
        }
    }

    private void ReadFile(string MeshFileName, string MeshFragmentationFileName, Dimension dimension)
    {
        //PseudoRegularMeshData data = new();

        string meshPath = Path.Combine(AppContext.BaseDirectory,"input/" + MeshFileName);

        //string? line;
        try
        {
            StreamReader srMesh = new StreamReader(meshPath);
            string[] strings = srMesh.ReadLine()!.Split(' ');

            //if (strings.Length != 2) throw new Exception("Wrong parameter count");

            //coordinate lines reading

            int n_x = int.Parse(strings[0]);

            int n_y = int.Parse(strings[1]);

            lines = new Vector2D[n_y,n_x] ;

            for (int i = 0; i < n_y; ++i)
            {
                strings = srMesh.ReadLine()!.Split(' ');
                for(int j = 0; j < n_x; ++j)
                {
                    double x = double.Parse(strings[2 * j]);
                    double y = double.Parse(strings[2 * j + 1]);
                    lines[i, j] = new Vector2D(x, y);
                }
            }

            //z_coordinate lines reading

            z_lines = null;

            int n_z = 0;

            if(dimension is Dimension.D3)
            {
                strings = srMesh.ReadLine()!.Split(' ');

                n_z = int.Parse(strings[0]);

                z_lines = new double[n_z];

                strings = srMesh.ReadLine()!.Split(' ');

                for (int i = 0; i < n_z; ++i)
                {
                    z_lines[i] = double.Parse(strings[i]);
                }
            }

            //areas
            strings = srMesh.ReadLine()!.Split(' ');

            int n_w = int.Parse(strings[0]);

            materials = new string[n_w];

            int n_borders = 0;

            switch(dimension)
            {
                case Dimension.D2:
                    n_borders = 4;
                    break;
                case Dimension.D3:
                    n_borders = 6;
                    break;
                default:
                    throw new Exception("");
            }

            areaBorders = new int[n_w, 6];

            strings = srMesh.ReadLine()!.Split(' ');

            for (int i = 0; i < n_w; ++i)
            {
                materials[i] = strings[0];
                for(int j = 0; j < n_borders; ++j)
                    areaBorders[i, j] = int.Parse(strings[j + 1]);
                for(int j = n_borders; j < 6; ++j)
                    areaBorders[i, j] = 0;
            }

            srMesh.Close(); //close mesh file

            string fragmentPath = Path.Combine(AppContext.BaseDirectory, "input/" + MeshFragmentationFileName);

            StreamReader srFragment = new StreamReader(fragmentPath); //open mesh fragmentation file

            x_intervals = new int[n_x - 1];
            x_stretch = new double[n_x - 1];

            y_intervals = new int[n_y - 1];
            y_stretch = new double[n_y - 1];

            z_intervals = n_z > 0 ? new int[n_z - 1] : null;
            z_stretch =  n_z > 0 ? new double[n_z - 1] : null;

            strings = srFragment.ReadLine()!.Split(' ');

            for (int i = 0; i < x_intervals.Length; ++i)
            {
                x_intervals[i] = int.Parse(strings[2*i]);
                x_stretch[i] = double.Parse(strings[2*i + 1]);
            }

            strings = srFragment.ReadLine()!.Split(' ');

            for (int i = 0; i < y_intervals.Length; ++i)
            {
                y_intervals[i] = int.Parse(strings[2 * i]);
                y_stretch[i] = double.Parse(strings[2 * i + 1]);
            }

            if(dimension is Dimension.D3)
            {
                strings = srFragment.ReadLine()!.Split(' ');

                for (int i = 0; i < z_intervals!.Length; ++i)
                {
                    z_intervals[i] = int.Parse(strings[2 * i]);
                    z_stretch![i] = double.Parse(strings[2 * i + 1]);
                }
            }

            srFragment.Close(); //close mesh fragmentation file



            //return new(lines, z_lines, materials, areaBorders, x_intervals, y_intervals, z_intervals, x_stretch, y_stretch, z_stretch);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

        //return null;
    }

    private double PointOnLine(double p1, double p2, int n, double k, int ind)
    {
        if (ind == 0) return p1;
        if (ind == n) return p2;
        double l = p2 - p1;
        if (Math.Abs(k - 1d) < 1.0E-13)
            return p1 + l / n *ind;

        double l_ind = l * (1d - Math.Pow(k, ind)) / (1d - Math.Pow(k, n));
        l_ind = k > 0 ? l_ind : l - l_ind;
        return p1 + l_ind;
    }

    private Vector2D PointOnLine(Vector2D p1, Vector2D p2,int n_x, double k_x, int n_y, double k_y, int x_ind, int y_ind)
    {
        return new Vector2D(PointOnLine(p1.X, p2.X, n_x, k_x, x_ind),
                   PointOnLine(p1.Y, p2.Y, n_y, k_y, y_ind));
    }

    private Vector3D PointOnLine(Vector3D p1, Vector3D p2, int n_x, double k_x, int n_y, double k_y, int n_z, double k_z, int x_ind, int y_ind, int z_ind)
    {

        return new(PointOnLine(p1.X, p2.X, n_x, k_x, x_ind),
                   PointOnLine(p1.Y, p2.Y, n_y, k_y, y_ind),
                   PointOnLine(p1.Z, p2.Z, n_z, k_z, z_ind));
    }
}
