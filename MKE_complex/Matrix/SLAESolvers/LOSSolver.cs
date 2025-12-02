using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MKE_complex.Matrix.SLAESolvers;

public enum Preconditioning {None, Diagonal, LU }

public class LOSSolver
{
    public int Maxiter { get; init; }
    public double MaxDiscrepancy { get; init; }

    public LOSSolver(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "input/" + fileName);
        var reader = new StreamReader(filePath);
        string[]? lines = (reader.ReadLine()?.Split(" ").Where(s => s != ""))?.ToArray();
        if (lines?.Count() != 2) throw new ArgumentException();
        Maxiter = int.Parse(lines[0]);
        MaxDiscrepancy = double.Parse(lines[1]);
        reader.Close();
    }

    public VectorBase<T> Solve<T>(Preconditioning mode, SparseMatrix<T> A, VectorBase<T> b) where T : INumber<T>
    {
        switch(mode)
        {
            case Preconditioning.None:
                return SolveWithoutPrecodintion(A, b);
            default:
                throw new NotImplementedException();
        }
    }



    private VectorBase<T> SolveWithoutPrecodintion<T>(SparseMatrix<T> A, VectorBase<T> pr) where T : INumber<T>
    {
        if(A.N != pr.N) throw new ArgumentException();
        int N = A.N;
        var x0 = new T[N];
        var x = new T[N];
        var xV = new VectorBase<T>(x);

        var r = pr - (A * xV); //r = f - A*x0
        var z = new VectorBase<T>(new T[N]);
        Array.Copy(r.components, z.components, N); //z = r
        var p = A * z;   //p = A*z
        double discrepancy = r.Norm(); discrepancy *= discrepancy;  //квадрат нормы невязки
        int k = 1;
        if (discrepancy < MaxDiscrepancy)
        {
            Console.WriteLine("Метод LOS сошелся: %.3le", discrepancy);
            return xV;
        }
        for (; k < Maxiter + 1; k++)
        {
            double norm2_p = p.Norm(); norm2_p *= norm2_p; //(p,p)
            double a = double.CreateChecked(VectorBase<T>.Scalar(p, r)) / norm2_p; //a = (p,r)/(p,p)
            xV = xV + a * z; //x = x + a*z
            r = r - a * p; //r = r - a*p
            var Ar = A * r; //Ar = A*r
            double b = -double.CreateChecked(VectorBase<T>.Scalar(p, Ar)) / norm2_p; //b = -(p,Ar)/(p,p)
            z = r + b * z; //z = r + b*z
            p = Ar + b * p; //p = Ar + b*p
            discrepancy = r.Norm(); discrepancy *= discrepancy; //квадрат нормы невязки
            if (discrepancy < MaxDiscrepancy)
            {
                Console.WriteLine("Метод LOS сошелся: %.3le", discrepancy);
                return xV;
            }
        }
        if (k == Maxiter + 1)
            Console.WriteLine("Достигнуто максимальное число итераций: %.3le", discrepancy);
        return xV;
    }
}
