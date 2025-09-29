using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class Vector2D : VectorBase
{
    public double X => components![0];
    public double Y => components![1];
    public Vector2D(double X, double Y) => components = [X, Y];
    protected override VectorBase CreateVector(params double[] components)
    {
        return new Vector2D(components[0], components[1]);
    }
}
