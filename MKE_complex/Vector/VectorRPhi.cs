using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class VectorRPhi : VectorBase<double, VectorRPhi>
{
    public double R => components![0];
    public double Phi => components![1];
    public VectorRPhi(double R, double Phi) => components = [R, Phi];
    public VectorRPhi(Vector2D V) => components = [V.X, V.Y];
    protected override VectorRPhi CreateVector(params double[] components) => new VectorRPhi(components[0], components[1]);
}
