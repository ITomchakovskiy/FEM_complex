using MKE_complex.interfaces;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh.MeshBuilder;

public record PseudoRegularMeshData(
    Vector2D[,] lines,
    double[]? z_lines,
    string[] materials,
    int[,] areaBorders,

    int[] x_intervals,
    int[] y_intervals,
    int[]? z_intervals,

    double[] x_stretch,
    double[] y_stretch,
    double[]? z_stretch);

public class PseudoRegularMeshBuilder : IMeshBuilder
{
    public IFiniteElementMesh<IVector> BuildMesh(Dimension dimension, MeshType meshType, string[] fileNames)
    {
        PseudoRegularMeshData? fragmentData = ReadFile(fileNames[0], fileNames[1], dimension);

        if (fragmentData == null)
            throw new Exception("Error in reading mesh file");

        for(int w = 0; w < fragmentData.materials.Length; w++)
        {
            string material = fragmentData.materials[w];
            for (int x_line = fragmentData.areaBorders[w, 0]; x_line < fragmentData.areaBorders[w, 1]; ++x_line)
            {
                for(int y_line = fragmentData.areaBorders[w, 2]; x_line < fragmentData.areaBorders[w, 3]; ++y_line)
                {

                }
            }
        }
        //return new General2DMesh();
    }

    private PseudoRegularMeshData? ReadFile(string MeshFileName, string MeshFragmentationFileName, Dimension dimension)
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

            Vector2D[,] lines = new Vector2D[n_y,n_x] ;

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

            double[]? z_lines = null;

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

            string[] materials = new string[n_w];

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

            int[,] areaBorders = new int[n_w, n_borders];

            strings = srMesh.ReadLine()!.Split(' ');

            for (int i = 0; i < n_w; ++i)
            {
                materials[i] = strings[0];
                for(int j = 0; j < n_borders; ++j)
                    areaBorders[i,j] = int.Parse(strings[j + 1]);
            }

            srMesh.Close(); //close mesh file

            string fragmentPath = Path.Combine(AppContext.BaseDirectory, "input/" + MeshFragmentationFileName);

            StreamReader srFragment = new StreamReader(fragmentPath); //open mesh fragmentation file

            int[] x_intervals = new int[n_x - 1];
            double[] x_stretch = new double[n_x - 1];

            int[] y_intervals = new int[n_y - 1];
            double[] y_stretch = new double[n_y - 1];

            int[]? z_intervals = n_z > 0 ? new int[n_z - 1] : null;
            double[]? z_stretch =  n_z > 0 ? new double[n_z - 1] : null;

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

            return new(lines, z_lines, materials, areaBorders, x_intervals, y_intervals, z_intervals, x_stretch, y_stretch, z_stretch);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

        return null;
    }

    private double PointOnLine(double p1, double p2, int n, double k, int ind)
    {
        double l = p2 - p1;
        double l_ind = l * (1d - Math.Pow(k, ind + 1d)) / (1d - Math.Pow(k, n));
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
