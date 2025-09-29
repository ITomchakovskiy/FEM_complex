using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public record Vector2D (double X, double Y): IVector
{
    public static Vector2D operator +(Vector2D A, Vector2D B) => new(A.X + B.X, A.Y + B.Y);
    public static Vector2D operator -(Vector2D A, Vector2D B) => new(A.X - B.X, A.Y - B.Y);
    public static Vector2D operator /(Vector2D A, double l) => new(A.X / l, A.Y / l);
    public static Vector2D operator *(Vector2D A, double l) => new(A.X * l, A.Y * l);
    public static Vector2D operator *(double l, Vector2D A) => new(A.X * l, A.Y * l);

    public double Norm() => Math.Sqrt(X * X + Y * Y);
    //public double X;//{ get; init; }
    // public double Y; //{ get; init; }

    //public Vector2D(double x, double y) { X = x; Y = y; }
}
