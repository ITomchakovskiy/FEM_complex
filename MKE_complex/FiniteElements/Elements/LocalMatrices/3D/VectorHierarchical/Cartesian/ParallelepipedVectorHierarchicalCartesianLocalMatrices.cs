using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices._3D.VectorHierarchical.Cartesian;
public static class ParallelepipedVectorHierarchicalCartesianLocalMatrices
{
    public static double[][] CalculateLocalMassMatrix(int order, double Coefficient, double hx, double hy, double hz)
    {
        double[][] baseMatrix = order switch
        {
            1 => M1,
            2 => M2,
            _ => throw new ArgumentException(),
        };

        double multiplier = Coefficient * hx * hy * hz;

        return baseMatrix.Select(i => i.Select(j => multiplier * j).ToArray()).ToArray();
    }

    public static double[][] CalculateLocalStiffnessMatrix(int order, double Coefficient, double hx, double hy, double hz)
    {
        double[][] baseMatrix = order switch
        {
            1 => CalcLocalBaseStiffnessMatrixLinear(hx,hy,hz),
            2 => CalcLocalBaseStiffnessMatrixQuadratic(hx,hy,hz),
            _ => throw new ArgumentException(),
        };

        return baseMatrix.Select(i => i.Select(j => j / Coefficient).ToArray()).ToArray();
    }

    private static readonly double[][] M1;

    private static readonly double[][] M2;

    static ParallelepipedVectorHierarchicalCartesianLocalMatrices()
    {
        M1 = CalcLocalBaseMassMatrixLinear();
        M2 = CalcLocalBaseMassMatrixQuadratic();
    }

    private static double[][] CalcLocalBaseMassMatrixLinear()
    {
        int N = 12;
        var res = new double[N][];
        for(int i = 0; i < res.Length; ++i)
            res[i] = new double[i+1];
        double[][] cell = [
            [4d],
            [2d,4d],
            [2d,1d,4d],
            [1d,2d,2d,4d]
        ];

        // for(int i = 0; i < cell.Length; ++i)
        // {
        //     for(int j = 0; j<=i;++j)
        //         cell[i][j] /= 36d;
        // }

        cell = cell.Select(i => i.Select(j => j/36d).ToArray()).ToArray();
        
        for(int step = 0; step < N; step += cell.Length) //cells
        {
            for(int i = step; i < step + cell.Length;++i)
            {
                for(int j = step; j <= i; ++j)
                    res[i][j] = cell[i-step][j-step];
            }
        }

        return res;
    }

    private static double[][] CalcLocalBaseMassMatrixQuadratic()
    {
        int N = 54;
        var res = new double[N][];
        for(int i = 0; i < res.Length; ++i)
            res[i] = new double[i+1];
        
        for(int i = 0;i < M1.Length; ++i)
        {
            for(int j = 0; j <= i; ++j)
            {
                res[i][j] = M1[i][j];
                //M1[i][j] *= 36d/108d;
            }  
        }
        for(int i = 0;i < M1.Length; ++i)
        {
            for(int j = 0; j <= i; ++j)
                res[i+M1.Length][j+M1.Length] = 36d/108d * M1[i][j];
        }

        double[][] lines1 = [
                        [2d,1d,2d,1d],
                        [1d,2d,1d,2d],
                        [2d,2d,1d,1d],
                        [1d,1d,2d,2d],
                        [2d,2d,2d,2d]
                            ];
        var lines2 = lines1.Select(i => i.ToArray()).ToArray();
        for(int i = 0; i < lines1.Length; ++i)
        {
            for(int j=0; j<lines1[0].Length;++j)
            {
                lines1[i][j] /= 18d;
                lines2[i][j] /= 54d;
            }
        }

        int[][] rowsForLines = 
        [
            [32,36,40,44,48],
            [24,28,42,46,50],
            [26,30,34,38,52],
            [33,37,41,45,49],
            [25,29,43,47,51],
            [27,31,35,39,53],
        ];

        for(int j = 0; j < 12; ++j)
        {
            for(int rowNum = 0; rowNum < 5; ++rowNum)
            {
                res[rowsForLines[j/4][rowNum]][j] = lines1[rowNum][j%4];
                res[rowsForLines[j/4+3][rowNum]][j+12] = lines2[rowNum][j%4];
            }
        }

        //main diagonal
        double[][] diagonalElements = [[8d/45d,8d/135d],
                                       [4d/45d,4d/135d,4d/45d,4d/135d],
                                       [1d/9d,1d/27d],
                                       [64d/225d,64d/675d]];
        for(int i = 24; i < N - 6; ++i)
            res[i][i] = diagonalElements[0][i%2];
        for(int i = N-6; i < N; ++i)
            res[i][i] = diagonalElements[3][i%2];

        (int i, int j)[][] indicesForDiagonalElements = [
                                            [(50,24),(52,26),(50,28),(52,30),(48,32),(52,34),(48,36),(52,38),(48,40),(50,42),(48,44),(50,46)],
                                            [(28,24),(36,32),(44,40)],
                                            [(42,24),(46,24),(34,26),(38,26),(42,28),(46,28),(34,30),(38,30),(40,32),(44,32),(40,36),(44,36)]
                                            ];
        
        for(int ielems = 0 ; ielems < indicesForDiagonalElements.Length; ++ielems)
        {
            var indices = indicesForDiagonalElements[ielems];
            var elems = diagonalElements[ielems];
            foreach(var index in indices)
            {
                for(int di = 0; di < elems.Length;++di)
                    res[index.i+di][index.j+di] = elems[di];
            }
        }
        
        return res;
    }

    private static double[][] CalcLocalBaseStiffnessMatrixLinear(double hx, double hy, double hz)
    {
        int N = 12;
        var res = new double[N][];
        for(int i = 0; i < res.Length; ++i)
            res[i] = new double[i+1];

        double[][] G1 = [
                [2d,1d,-2d,-1d],
                [1d,2d,-1d,-2d],
                [-2d,-1d,2d,1d],
                [-1d,-2d,1d,2d]
        ];
        double[][] G2 = [
                [2d,-2d,1d,-1d],
                [-2d,2d,-1d,1d],
                [1d,-1d,2d,-2d],
                [-1d,1d,-2d,2d]
        ];
        double[][] G3 = [
                [-2d,-1d,2d,1d],
                [2d,1d,-2d,-1d],
                [-1d,-2d,1d,2d],
                [1d,2d,-1d,-2d]
        ];

        double[] coefs = [hx*hy/6d/hz,hx*hz/6d/hy,hy*hz/6d/hx,-hz/6d,hy/6d,-hx/6d];

        for(int i =0;i <G1.Length;++i)
        {
            for(int j=0; j<=i;++j)
            {
                res[i][j] = coefs[0]*G1[i][j]+
                            coefs[1]*G2[i][j];
                res[i+4][j+4] = coefs[0]*G1[i][j]+
                                coefs[2]*G2[i][j];
                res[i+8][j+8] = coefs[1]*G1[i][j]+
                                coefs[2]*G2[i][j];
                res[i+4][j] = coefs[3]*G2[i][j];
                res[i+8][j] = coefs[4]*G3[i][j];
                res[i+8][j+4] = coefs[5]*G1[i][j];
            }
            for(int j = i + 1; j < G1.Length; ++j)
            {
                res[i+4][j] = coefs[3]*G2[i][j];
                res[i+8][j] = coefs[4]*G3[i][j];
                res[i+8][j+4] = coefs[5]*G1[i][j];
            }
        }
        return res;
    }

    private static double[][] CalcLocalBaseStiffnessMatrixQuadratic(double hx, double hy, double hz)
    {
        int N = 54;
        var res = new double[N][];
        for(int i = 0; i < res.Length; ++i)
            res[i] = new double[i+1];

        var G1 = CalcLocalBaseStiffnessMatrixLinear(hx,hy,hz);

        for(int i = 0; i < G1.Length; ++i)
        {
            for(int j = 0; j <= i; ++j)
                res[i][j] = G1[i][j];
        }

        for(int i = 0; i < 4; ++i)
        {
            for(int j = 0 ; j<=i;++j)
            {
                res[i+12][j+12] = G1[i][j]/3d;
                res[i+16][j+16] = G1[i+4][j+4]/3d;
                res[i+20][j+20] = G1[i+8][j+8]/3d;
            }  
        }

        double[][] sequencePatterns = [
            [-1d,1d,-1d,1d],
            [1d,-1d,1d,-1d],
            [-1d,-1d,1d,1d],
            [1d,1d,-1d,-1d],
            [-2d,-1d,2d,1d],
            [-1d,-2d,1d,2d],
            [-2d,2d,-1d,1d],
            [-1d,1d,-2d,2d]
        ];

        double[] sequenceCoefficients = [
            hx/3d,hy/3d,hz/3d,
            hy*hz/hx/3d,hx*hz/hy/3d,hx*hy/hz/3d,
            2d*hx/9d,2d*hy/9d,2d*hz/9d,
            4d*hx/9d,4d*hy/9d,4d*hz/9d,
            hy*hz/hx/9d,hx*hz/hy/9d,hx*hy/hz/9d
        ];

        (int i, int pat, int coef)[][] sequences = [
            [(24,0,2), (26,2,1), (28,1,2), (30,3,1), (32,1,4), (36,0,4),( 40,3,5), (44,2,5)],
            [(24,1,3), (28,0,3), (32,0,2), (34,2,0), (36,1,2), (38,3,0), (42,3,5), (46,2,5)],
            [(26,1,3), (30,0,3), (34,3,4), (38,2,4), (40,0,1), (42,2,0), (44,1,1), (46,3,0)],
            [(33,1,13), (34,4,7), (37,0,13), (38,5,7), (41,3,14), (42,6,8), (45,2,14), (46,7,8), (50,0,11), (52,2,10)],
            [(25,1,12),(26,4,6),(29,0,12),(30,5,6),(40,6,8),(43,3,14),(44,7,8),(47,2,14),(48,0,11),(52,2,9)],
            [(24,4,6),(27,1,12),(28,5,6),(31,0,12),(32,6,7),(35,3,13),(36,7,7),(39,2,13),(48,0,10),(50,2,9)]
        ];

        int sequenceLength = 4;

        for(int isequenceColumn = 0; isequenceColumn < sequences.Length; ++isequenceColumn)
        {
            int j = isequenceColumn * sequenceLength;

            var sequencesForColumn = sequences[isequenceColumn];

            foreach(var seq in sequencesForColumn)
            {
                for(int jShift = 0 ; jShift < sequenceLength; ++jShift)
                    res[seq.i][j+jShift] = sequencePatterns[seq.pat][jShift] * 
                                           sequenceCoefficients[seq.coef];
            }
        }

        double[][] diagonalCoefficients = [     //diagonal
            [16d/9d*hy*hz/hx, 16d/9d*hx*hz/hy, 16d/9d*hx*hy/hz], 
            [16d/27d*hy*hz/hx, 16d/27d*hx*hz/hy, 16d/27d*hx*hy/hz],
            [8d/15d*hy*hz/hx, 8d/15d*hx*hz/hy, 8d/15d*hx*hy/hz],
            [8d/45d*hy*hz/hx, 8d/45d*hx*hz/hy, 8d/45d*hx*hy/hz]];

        for(int i = 24; i < 48; ++i) 
            res[i][i] = diagonalCoefficients[2 + i % 2][(i - 24)/8];

        int[] indicesXFaces = [2,1];
        int[] indicesYFaces = [2,0];
        int[] indicesZFaces = [1,0];

        for(int ishift = 0; ishift < 8; ++ishift)
        {
            res[24+ishift][24+ishift] += diagonalCoefficients[ishift % 2]
                                                             [indicesXFaces[(ishift/2) % 2]];
            res[24+8+ishift][24+8+ishift] += diagonalCoefficients[ishift % 2]
                                                                 [indicesYFaces[(ishift/2) % 2]];
            res[24+16+ishift][24+16+ishift] += diagonalCoefficients[ishift % 2]
                                                                   [indicesZFaces[(ishift/2) % 2]];
        }

        int[][] RowsPatterns = [[50,51,52,53],  //parts of elements similar to diagonal
                                [48,49, 52, 53],
                                [48,49,50,51]];

        for(int ishift = 0; ishift < 8; ++ishift)
        {
            res[RowsPatterns[0][ishift % 4]][24+ishift] = diagonalCoefficients[ishift % 2]
                                                             [indicesXFaces[(ishift/2) % 2]];
            res[RowsPatterns[1][ishift % 4]][24+8+ishift] = diagonalCoefficients[ishift % 2]
                                                                 [indicesYFaces[(ishift/2) % 2]];
            res[RowsPatterns[2][ishift % 4]][24+16+ishift] = diagonalCoefficients[ishift % 2]
                                                                   [indicesZFaces[(ishift/2) % 2]];
        }

        for(int istart = 24; istart < 48; istart += 8)
        {
            for(int ishift = 0; ishift < 4; ++ishift)
                res[istart+ishift+4][istart+ishift] = -diagonalCoefficients[2 + ishift % 2][(istart + ishift - 24)/8];
        }

        for(int ishift = 0; ishift < 4; ++ishift)
        {
            res[24+ishift+4][24+ishift] += diagonalCoefficients[ishift % 2]
                                                             [indicesXFaces[(ishift/2) % 2]] /2d;
            res[24+8+ishift+4][24+8+ishift] += diagonalCoefficients[ishift % 2]
                                                                 [indicesYFaces[(ishift/2) % 2]] /2d;
            res[24+16+ishift+4][24+16+ishift] += diagonalCoefficients[ishift % 2]
                                                                   [indicesZFaces[(ishift/2) % 2]] /2d;
        }
        
        double[] baseCoefficients =
        [
            4d/9d*hx,4d/9d*hy,4d/9d*hz,
            8d/27d*hx,8d/27d*hy,8d/27d*hz,
            8d/15d*hx,8d/15d*hy,8d/15d*hz,
            32d/45d*hx,32d/45d*hy,32d/45d*hz
        ];

        double[] multiplyers = [1d,-1d,2d,-2d];

        (int i, int coef, int mult)[][] elements =
        [
            [(32,8,1),(35,0,1),(36,8,0),(39,0,0) ],
            [(27,3,3),(31,3,1),(40,2,1),(44,2,1),(48,11,1),(53,3,3)],
            [(40,7,1),(43,0,1),(44,7,0),(47,0,0)],
            [(29,3,1),(32,1,1),(36,1,1),(48,10,1),(51,3,3)],
            [(32,8,0),(35,0,1),(36,8,1),(39,0,0)],
            [(31,3,3),(40,2,0),(44,2,0),(48,11,0),(53,3,3)],
            [(40,7,0),(43,0,1),(44,7,1),(47,0,0)],
            [(32,1,0),(36,1,0),(48,10,0),(51,3,3)],
            [],
            [(35,4,3),(39,4,1),(42,2,1),(46,2,1),(50,11,1),(53,4,3)],
            [(41,1,1),(42,6,1),(45,1,0),(46,6,0)],
            [(37,4,1),(49,4,3),(50,9,1)],
            [],
            [(39,4,3),(42,2,0),(46,2,0),(50,11,0),(53,4,3)],
            [(41,1,1),(42,6,0),(45,1,0),(46,6,1)],
            [(49,4,3),(50,9,0)],
            [],
            [(43,5,3),(47,5,1),(51,5,3),(52,10,1)],
            [],
            [(45,5,1),(49,5,3),(52,9,1)],
            [],
            [(47,5,3),(51,5,3),(52,10,0)],
            [],
            [(49,5,3),(52,9,0)]
        ];

        for(int ielements = 0; ielements < elements.Length; ++ielements)
        {
            int j = 24 + ielements;
            var elems = elements[ielements];
            foreach(var elem in elems)
                res[elem.i][j] = baseCoefficients[elem.coef] * 
                                 multiplyers[elem.mult];
        }

        double[] ElementDofsCoefs = [128d*hy*hz/hx, 128d*hx*hz/hy,128d*hx*hy/hz];

        res[48][48] = (ElementDofsCoefs[2]+ElementDofsCoefs[1])/45d;
        res[49][49] = (ElementDofsCoefs[2]+ElementDofsCoefs[1])/135d;
        res[50][50] = (ElementDofsCoefs[2]+ElementDofsCoefs[0])/45d;
        res[51][51] = (ElementDofsCoefs[2]+ElementDofsCoefs[0])/135d;
        res[52][52] = (ElementDofsCoefs[1]+ElementDofsCoefs[0])/45d;
        res[53][53] = (ElementDofsCoefs[1]+ElementDofsCoefs[0])/135d;

        res[51][49] = -128d*hz/135d;
        res[53][49] = -128d*hy/135d;
        res[53][51] = -128d*hx/135d;

        return res;
    }
}