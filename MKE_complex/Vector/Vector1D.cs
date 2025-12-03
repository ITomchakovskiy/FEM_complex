using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class Vector1D : VectorBase<double, Vector1D>
{
    public double X => components![0];
    public Vector1D(double x) => components = [x];

    protected override Vector1D CreateVector(params double[] components) => new Vector1D(components[0]);

    // protected override VectorBase<double> CreateVector(params double[] components) => new Vector1D(components[0]);
}
