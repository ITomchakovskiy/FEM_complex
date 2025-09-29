using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public interface IVector
{
    IVector PointOnLine(IVector A, IVector B, int n, double k, int ind);
   //public static IVector operator +(IVector v1, IVector v2);
}
