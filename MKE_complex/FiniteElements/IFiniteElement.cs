using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements;

public interface IFiniteElement<VectorT> where VectorT : IVector
{
    IFiniteElement<VectorT> Geometry { get;}

    string Material { get; }
}
