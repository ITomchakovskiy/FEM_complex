using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MKE_complex.Matrix.SLAESolvers;

public enum Preconditioning {None, Diagonal, Cholesky, LU }

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

    public Vector.Vector<T> Solve<T>(Preconditioning mode, SparseMatrix<T> A, Vector.Vector<T> b) where T : INumber<T>
    {
        switch(mode)
        {
            case Preconditioning.None:
                return SolveWithoutPrecodintion(A, b);
            case Preconditioning.Diagonal:
                return SolveWithDiagonalPrecodintion(A, b);
            case Preconditioning.Cholesky:
                return SolveWithCholeskyPrecodintion(A, b);
            case Preconditioning.LU:
                return SolveWithLUPrecodintion(A,b);
            default:
                throw new NotImplementedException();
        }
    }

    private Vector.Vector<T> SolveWithoutPrecodintion<T>(SparseMatrix<T> A, Vector.Vector<T> pr) where T : INumber<T>
    {
        if(A.N != pr.N) throw new ArgumentException();
        int N = A.N;
        var x0 = new T[N];
        var x = new T[N];
        var xV = new Vector.Vector<T>(x);

        var r = pr - (A * xV); //r = f - A*x0
        var z = new Vector.Vector<T>(new T[N]);
        Array.Copy(r.components, z.components, N); //z = r
        var p = A * z;   //p = A*z
        double discrepancy = r.Norm(); discrepancy *= discrepancy;  //квадрат нормы невязки
        int k = 1;
        if (discrepancy < MaxDiscrepancy)
        {
            Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}", discrepancy);
            return xV;
        }
        for (; k < Maxiter + 1; k++)
        {
            double norm2_p = p.Norm(); norm2_p *= norm2_p; //(p,p)
            double a = double.CreateChecked(Vector.Vector<T>.Scalar(p, r)) / norm2_p; //a = (p,r)/(p,p)
            xV += a * z; //x = x + a*z
            r = r - a * p; //r = r - a*p
            var Ar = A * r; //Ar = A*r
            double b = -double.CreateChecked(Vector.Vector<T>.Scalar(p, Ar)) / norm2_p; //b = -(p,Ar)/(p,p)
            z = r + b * z; //z = r + b*z
            p = Ar + b * p; //p = Ar + b*p
            discrepancy = r.Norm(); discrepancy *= discrepancy; //квадрат нормы невязки
            if (discrepancy < MaxDiscrepancy)
            {
                Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}");
                return xV;
            }
        }
        if (k == Maxiter + 1)
            Console.WriteLine($"Достигнуто максимальное число итераций: {discrepancy:E3}");
        return xV;
    }

    private Vector.Vector<T> SolveWithDiagonalPrecodintion<T>(SparseMatrix<T> A, Vector.Vector<T> pr) where T : INumber<T>
    {
        if(A.N != pr.N) throw new ArgumentException();
        int N = A.N;
        var x = new T[N];
        var Xv = new Vector.Vector<T>(x);
        var Ar1 = A * Xv;        //Ar = A*x
        Ar1 = pr - Ar1;           //Ar = f - A * x0
        var DPreconditioner = new Vector.Vector<T>([.. A.Di]).Sqrt();
        var r = Ar1.Division(DPreconditioner);   //r = D^(-1)*(f-A*x0)
        var z = r.Division(DPreconditioner);   //z = D^(-1)*r
        var p = (A * z).Division(DPreconditioner); //p = D^(-1) * A*z
        double discrepancy = r.Norm(); discrepancy *= discrepancy;  //квадрат нормы невязки
        int k = 1;
        if (discrepancy < MaxDiscrepancy)
        {
            Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}", discrepancy);
            return Xv;
        }
        for (; k < Maxiter + 1; k++)
        {
            double norm2_p = p.Norm(); norm2_p *= norm2_p; //(p,p)
            double a = double.CreateChecked(p.Scalar(r)) / norm2_p;        //a = (p,r)/(p,p)
            Xv += a * z;                                                    //x = x+a*z
            r -= a * p;            //r = r -a*p
            Ar1 = r.Division(DPreconditioner);      //Ar = D^(-1)*r
            var Ar2 = A * Ar1;                         //Ar_2 = A*U^(-1)*r
            Ar1 = Ar2.Division(DPreconditioner);        //Ar = D^(-1)*A*U^(-1)*r                             
            double b = -double.CreateChecked(p.Scalar(Ar1)) / norm2_p;       //b = -(p,Ar)/(p,p)
            Ar2 = DPreconditioner.Multiply(z);                                 //Ar_2 = D*z
            z = r + b * Ar2;                                         //z = r + b*U*z    
            z = z.Division(DPreconditioner);                         //z = D^(-1)*(r+b*D*z)
            p = Ar1 + b * p;                                           //p = L^(-1)*A*U^(-1)*r + b*p
            discrepancy = r.Norm(); discrepancy *= discrepancy; //квадрат нормы невязки
            if (discrepancy < MaxDiscrepancy)
            {
                Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}");
                return Xv;
            }
        }
   
        if (k == Maxiter + 1)
            Console.WriteLine($"Достигнуто максимальное число итераций: {discrepancy:E3}");
        return Xv;
    }

    private Vector.Vector<T> SolveWithCholeskyPrecodintion<T>(SparseMatrix<T> A, Vector.Vector<T> pr) where T : INumber<T>
    {
        var LLt = Preconditioners.CholeskySparseDecomposition(A);
        if(A.N != pr.N) throw new ArgumentException();
        int N = A.N;
        var x = new T[N];
        var Xv = new Vector.Vector<T>(x);
        var Ar1 = A * Xv;        //Ar = A*x
        Ar1 = pr - Ar1;           //Ar = f - A * x0
        var r = Preconditioners.ForwardSubstitutionForLUSparseDecomposition(LLt, Ar1); //r = L^(-1)*(f-A*x0)
        var z = Preconditioners.BackSubstitutionForCholeskySparseDecomposition(LLt, r);   //z = U^(-1)*r
        var p = Preconditioners.ForwardSubstitutionForLUSparseDecomposition(LLt,A*z); //p = L^(-1) * A*z
        double discrepancy = r.Norm(); discrepancy *= discrepancy;  //квадрат нормы невязки
        int k = 1;
        if (discrepancy < MaxDiscrepancy)
        {
            Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}", discrepancy);
            return Xv;
        }
        for (; k < Maxiter + 1; k++)
        {
            double norm2_p = p.Norm(); norm2_p *= norm2_p; //(p,p)
            double a = double.CreateChecked(p.Scalar(r)) / norm2_p;        //a = (p,r)/(p,p)
            Xv += a * z;                                                    //x = x+a*z
            r -= a * p;            //r = r -a*p
            Ar1 = Preconditioners.BackSubstitutionForCholeskySparseDecomposition(LLt, r);      //Ar = U^(-1)*r
            var Ar2 = A * Ar1;                         //Ar_2 = A*U^(-1)*r
            Ar1 = Preconditioners.ForwardSubstitutionForLUSparseDecomposition(LLt, Ar2);        //Ar = L^(-1)*A*U^(-1)*r                             
            double b = -double.CreateChecked(p.Scalar(Ar1)) / norm2_p;       //b = -(p,Ar)/(p,p)
            Ar2 = Preconditioners.MultiplyUpperTriangleForCholeskySparseDecomposition(LLt,z);                                 //Ar_2 = U*z
            z = r + b * Ar2;                                         //z = r + b*U*z    
            z = Preconditioners.BackSubstitutionForCholeskySparseDecomposition(LLt, z);                         //z = U^(-1)*(r+b*D*z)
            p = Ar1 + b * p;                                           //p = L^(-1)*A*U^(-1)*r + b*p
            discrepancy = r.Norm(); discrepancy *= discrepancy; //квадрат нормы невязки
            if (discrepancy < MaxDiscrepancy)
            {
                Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}");
                return Xv;
            }
        }
   
        if (k == Maxiter + 1)
            Console.WriteLine($"Достигнуто максимальное число итераций: {discrepancy:E3}");
        return Xv;
    }

    private Vector.Vector<T> SolveWithLUPrecodintion<T>(SparseMatrix<T> A, Vector.Vector<T> pr) where T : INumber<T>
    {
        var LU = Preconditioners.LUSparseDecomposition(A);
        if(A.N != pr.N) throw new ArgumentException();
        int N = A.N;
        var x = new T[N];
        var Xv = new Vector.Vector<T>(x);
        var Ar1 = A * Xv;        //Ar = A*x
        Ar1 = pr - Ar1;           //Ar = f - A * x0
        var r = Preconditioners.ForwardSubstitutionForLUSparseDecomposition(LU, Ar1); //r = L^(-1)*(f-A*x0)
        var z = Preconditioners.BackSubstitutionForLUSparseDecomposition(LU, r);   //z = U^(-1)*r
        var p = Preconditioners.ForwardSubstitutionForLUSparseDecomposition(LU,A*z); //p = L^(-1) * A*z
        double discrepancy = r.Norm(); discrepancy *= discrepancy;  //квадрат нормы невязки
        int k = 1;
        if (discrepancy < MaxDiscrepancy)
        {
            Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}", discrepancy);
            return Xv;
        }
        for (; k < Maxiter + 1; k++)
        {
            double norm2_p = p.Norm(); norm2_p *= norm2_p; //(p,p)
            double a = double.CreateChecked(p.Scalar(r)) / norm2_p;        //a = (p,r)/(p,p)
            Xv += a * z;                                                    //x = x+a*z
            r -= a * p;            //r = r -a*p
            Ar1 = Preconditioners.BackSubstitutionForLUSparseDecomposition(LU, r);      //Ar = U^(-1)*r
            var Ar2 = A * Ar1;                         //Ar_2 = A*U^(-1)*r
            Ar1 = Preconditioners.ForwardSubstitutionForLUSparseDecomposition(LU, Ar2);        //Ar = L^(-1)*A*U^(-1)*r                             
            double b = -double.CreateChecked(p.Scalar(Ar1)) / norm2_p;       //b = -(p,Ar)/(p,p)
            Ar2 = Preconditioners.MultiplyUpperTriangleForLUSparseDecomposition(LU,z);                                 //Ar_2 = U*z
            z = r + b * Ar2;                                         //z = r + b*U*z    
            z = Preconditioners.BackSubstitutionForLUSparseDecomposition(LU, z);                         //z = U^(-1)*(r+b*D*z)
            p = Ar1 + b * p;                                           //p = L^(-1)*A*U^(-1)*r + b*p
            discrepancy = r.Norm(); discrepancy *= discrepancy; //квадрат нормы невязки
            if (discrepancy < MaxDiscrepancy)
            {
                Console.WriteLine($"Метод LOS сошелся: {discrepancy:E3}");
                return Xv;
            }
        }
   
        if (k == Maxiter + 1)
            Console.WriteLine($"Достигнуто максимальное число итераций: {discrepancy:E3}");
        return Xv;
    }
}
