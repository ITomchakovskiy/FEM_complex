using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class VectorRZ : VectorBase<double, VectorRZ>
{
    public double R => components![0];
    public double Z => components![1];
    public VectorRZ(double R, double Z) => components = [R, Z];
    public VectorRZ(Vector2D V) => components = [V.X, V.Y];
    public override VectorRZ CreateVector(params double[] components) => new VectorRZ(components[0], components[1]);
}
