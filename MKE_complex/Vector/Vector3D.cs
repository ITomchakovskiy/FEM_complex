using System;
using System.Collections.Generic;
using System.Linq;
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

    protected override Vector3D CreateVector(params double[] components) => new Vector3D(components[0], components[1], components[2]);

    //protected override VectorBase<double> CreateVector(params double[] components)
    //{
    //    return new Vector3D(components[0], components[1], components[2]);
    //}
}
