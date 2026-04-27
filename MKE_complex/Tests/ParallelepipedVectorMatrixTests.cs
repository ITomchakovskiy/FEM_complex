using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.LocalMatrices._3D.VectorHierarchical.Cartesian;
using Xunit;

namespace MKE_complex.Tests
{
    public class ParallelepipedVectorMatrixTests
    {
        [Fact]
        public void Test1()
        {
            var mnum = ParallelepipedVectorHierarchicalCartesianLocalMatrices.CalculateLocalMassMatrix(2,1d,2d,3d,4d);
            var gnum = ParallelepipedVectorHierarchicalCartesianLocalMatrices.CalculateLocalStiffnessMatrix(2,1d,2d,3d,4d);

            void writeFile(string filename, double[][] matrix)
            {
                var writer = new StreamWriter(filename);
                for(int i = 0; i < matrix.Length; ++i)
                {
                    for(int j = 0; j <= i; ++j)
                        writer.Write(matrix[i][j] + ", ");
                    writer.Write('\n');
                }
                writer.Close();
            }

            double[][] readTheoreticalFile(string filename, int N)
            {
                var reader = new StreamReader(filename);

                double[][] matrix = new double[N][];
                for(int i = 0; i < N; ++i)
                {
                    matrix[i] = new double[i+1];
                    reader.Read();
                    var strings = reader.ReadLine()!.Split(", ");
                    strings[^1] = strings[^1].Remove(strings[^1].Length-1);
                    double[] values = strings.Select(double.Parse).ToArray();
                    for(int j = 0; j <= i; ++j)
                        matrix[i][j] = values[j];
                }
                reader.Close();
                return matrix;
            }

            double EvaluateDiscrepancy(double[][] m1, double[][] m2)
            {
                double discr = 0d;
                for(int i = 0; i < m1.Length; ++i)
                {
                    for(int j = 0; j <= i;++j)
                    {
                        if((m1[i][j] - m2[i][j]) * (m1[i][j] - m2[i][j]) > 0.01)
                            Console.WriteLine("i: " + i + " j: " + j);
                        discr += (m1[i][j] - m2[i][j]) * (m1[i][j] - m2[i][j]);
                    }
                        
                }
                return discr;
            }

            writeFile("./MTestNumerical",mnum);
            writeFile("./GTestNumerical",gnum);

            var mteo = readTheoreticalFile("./MTestTheo",54);
            var gteo = readTheoreticalFile("./GTestTheo",54);

            Console.WriteLine("M: " + EvaluateDiscrepancy(mnum,mteo));
            Console.WriteLine("G: " + EvaluateDiscrepancy(gnum,gteo));
        }
    }
}