using MKE_complex.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public record Vector3D(double X, double Y, double Z) : IVector
{
    //public double X;//{ get; init; }
    //public double Y; //{ get; init; }
    //public double Z; //{ get; init; }
}
