using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh.MeshBuilder;

struct PseudoRegularMeshData
{
    Vector2D[,] lines;
    double[] z_lines;
    string[] materials;
    int[,] areaBorders;

    int[] x_intervals;
    int[] y_intervals;
    int[] z_intervals;

    double[] x_stretch;
    double[] y_stretch;
    double[] z_stretch;
}

public class PseudoRegularMeshBuilder
{
    private PseudoRegularMeshData ReadFile(string MeshFileName, Dimension dimension)
    {
        //PseudoRegularMeshData data = new();

        string path = Path.Combine(AppContext.BaseDirectory, MeshFileName);

        //string? line;
        try
        {
            StreamReader sr = new StreamReader(path);
            string[] strings = sr.ReadLine()!.Split(' ');

            //if (strings.Length != 2) throw new Exception("Wrong parameter count");

            //coordinate lines reading

            int n_x = int.Parse(strings[0]);

            int n_y = int.Parse(strings[1]);

            Vector2D[,] lines = new Vector2D[n_y,n_x] ;

            for (int i = 0; i < n_y; ++i)
            {
                strings = sr.ReadLine()!.Split(' ');
                for(int j = 0; j < n_x; ++j)
                {
                    double x = double.Parse(strings[2 * j]);
                    double y = double.Parse(strings[2 * j + 1]);
                    lines[i, j] = new Vector2D(x, y);
                }
            }

            //z_coordinate lines reading

            if(dimension is Dimension.D3)
            {
                strings = sr.ReadLine()!.Split(' ');

                int n_z = int.Parse(strings[0]);

                strings = sr.ReadLine()!.Split(' ');

                for (int i = 0; i < n_z; ++i)
                {

                }
            }

            

            while (line is not null)
            {
                string[] strings = line.Split(' ');
                //Read the next line
                line = sr.ReadLine();
            }
            //close the file
            sr.Close();
            Console.ReadLine();
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Executing finally block.");
        }
    }
}
