using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Vector;

public class Vector<T> : VectorBase<T, Vector<T>> where T : INumber<T>
{
    public Vector(params T[] components) => this.components = components;
    public override Vector<T> CreateVector(params T[] components) => new Vector<T>(components);
}
