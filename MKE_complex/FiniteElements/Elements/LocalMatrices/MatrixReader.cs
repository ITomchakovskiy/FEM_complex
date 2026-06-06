using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices;
public static class MatrixReader
{
    public static double[][] ReadMatrixFromFile(string filename)
    {
        var path = Path.Join(Directory, filename);
        var reader = new StreamReader(path);

        List<double[]> matrix = [];

        var line = reader.ReadLine();
        
        while(line is not null)
        {
            //reader.Read();
            var strings = line.Split(", ");
            //strings[^1] = strings[^1].Remove(strings[^1].Length-1);
            double[] values = strings.Select(double.Parse).ToArray();
            matrix.Add(values);
            line = reader.ReadLine();
        }
        reader.Close();
        return matrix.ToArray();
    }

    private static string Directory = "./LocalMatricesFiles";
}