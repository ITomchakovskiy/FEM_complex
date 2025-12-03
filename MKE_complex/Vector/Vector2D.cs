using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class Vector2D : VectorBase<double, Vector2D>
{
    public double X => components![0];
    public double Y => components![1];
    public Vector2D(double X, double Y) => components = [X, Y];

    protected override Vector2D CreateVector(params double[] components) => new Vector2D(components[0], components[1]);


    //protected override VectorBase<double> CreateVector(params double[] components)
    //{
    //    return new Vector2D(components[0], components[1]);
    //}
}
