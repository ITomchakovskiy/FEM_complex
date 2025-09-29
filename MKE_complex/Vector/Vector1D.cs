using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class Vector1D : VectorBase
{
    public double X => components![0];
    public Vector1D(double x) => components = [x];
    protected override VectorBase CreateVector(params double[] components) => new Vector1D(components[0]);
}
