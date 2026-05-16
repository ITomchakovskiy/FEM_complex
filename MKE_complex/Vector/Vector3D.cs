using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class Vector3D : VectorBase<double, Vector3D>
{
    public double X => components![0];
    public double Y => components![1];
    public double Z => components![2];
    public Vector3D(double X, double Y, double Z) => components = [X, Y, Z];
    public Vector3D(Vector2D xy, double z) => components = [xy.X, xy.Y, z];

    public override Vector3D CreateVector(params double[] components) => new Vector3D(components[0], components[1], components[2]);

    public Vector3D CrossProduct(Vector3D other)
    {
        return new Vector3D(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X);
    }

    public Vector2D ProjectionToPlane(string plane)
    {
        return plane switch
        {
            "Z" => new Vector2D(X, Y),
            "Y" => new Vector2D(X, Z),
            "X" => new Vector2D(Y, Z),
            _ => throw new ArgumentException("Invalid projection plane")
        };
    }

    //protected override VectorBase<double> CreateVector(params double[] components)
    //{
    //    return new Vector3D(components[0], components[1], components[2]);
    //}
}
