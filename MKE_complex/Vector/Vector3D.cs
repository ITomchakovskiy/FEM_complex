using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public record Vector3D(double X, double Y, double Z) : IVector
{
    public static Vector3D operator +(Vector3D A, Vector3D B) => new Vector3D(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
    public static Vector3D operator -(Vector3D A, Vector3D B) => new Vector3D(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
    public static Vector3D operator /(Vector3D A, double l) => new(A.X / l, A.Y / l, A.Z / l);
    public static Vector3D operator *(Vector3D A, double l) => new(A.X * l, A.Y * l, A.Z * l);
    public static Vector3D operator *(double l, Vector3D A) => new(A.X * l, A.Y * l, A.Z * l);

    public double Norm() => Math.Sqrt(X * X + Y * Y + Z * Z);
    //public double X;//{ get; init; }
    //public double Y; //{ get; init; }
    //public double Z; //{ get; init; }
}
