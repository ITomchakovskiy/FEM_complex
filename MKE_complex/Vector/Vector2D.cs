using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public record Vector2D (double X, double Y): IVector
{
    //public double X;//{ get; init; }
   // public double Y; //{ get; init; }

    //public Vector2D(double x, double y) { X = x; Y = y; }
}
