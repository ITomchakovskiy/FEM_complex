using MKE_complex.FiniteElements.Elements.BasisFunctions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements.LocalMatrices;

public static class RectangleLagrangianLocalMatrices
{
    public static class QuadraticMatrices
    {
        public static double[][] GetStiffnessMatrix(double hx, double hy)
        {
            var Gx = LineLagrangianLocalMatrices.QuadraticMatrices.GetStiffnessMatrix(hx);

            var Gy = LineLagrangianLocalMatrices.QuadraticMatrices.GetStiffnessMatrix(hy);

            var Mx = LineLagrangianLocalMatrices.QuadraticMatrices.GetMassMatrix(hx);

            var My = LineLagrangianLocalMatrices.QuadraticMatrices.GetMassMatrix(hy);

            var result = new double[9][];

            for(int i = 0; i < 9; ++i)
            {
                result[i] = new double[i + 1];
                for(int j = 0; j <= i; ++j)
                {
                    (int i, int j) Mu = (RectangleLagrangianBases.localXDofNum(i, 2), 
                                         RectangleLagrangianBases.localXDofNum(j, 2));
                    Mu = Mu.j < Mu.i ? Mu : (Mu.j, Mu.i);

                    (int i, int j) Nu = (RectangleLagrangianBases.localYDofNum(i, 2),
                                         RectangleLagrangianBases.localYDofNum(j, 2));
                    Nu = Nu.j < Nu.i ? Nu : (Nu.j, Nu.i);

                    result[i][j] = Gx[Mu.i][Mu.j] * My[Nu.i][Nu.j] + Mx[Mu.i][Mu.j] * Gy[Nu.i][Nu.j];
                }
            }
            return result;
        }

        public static double[][] GetMassMatrix(double hx, double hy)
        {
            var Mx = LineLagrangianLocalMatrices.QuadraticMatrices.GetMassMatrix(hx);

            var My = LineLagrangianLocalMatrices.QuadraticMatrices.GetMassMatrix(hy);

            var result = new double[9][];

            for (int i = 0; i < 9; ++i)
            {
                result[i] = new double[i + 1];
                for (int j = 0; j <= i; ++j)
                {
                    (int i, int j) Mu = (RectangleLagrangianBases.localXDofNum(i, 2),
                                         RectangleLagrangianBases.localXDofNum(j, 2));
                    Mu = Mu.j < Mu.i ? Mu : (Mu.j, Mu.i);

                    (int i, int j) Nu = (RectangleLagrangianBases.localYDofNum(i, 2),
                                         RectangleLagrangianBases.localYDofNum(j, 2));
                    Nu = Nu.j < Nu.i ? Nu : (Nu.j, Nu.i);

                    result[i][j] = Mx[Mu.i][Mu.j] * My[Nu.i][Nu.j];
                }
            }
            return result;
        }
    }
}
