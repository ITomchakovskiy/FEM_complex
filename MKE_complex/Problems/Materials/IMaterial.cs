using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Problems.Materials;

public enum MaterialType
{
    Solid,
    DirichletCondition,
    NeumannCondition,
    RobinCondition
}

public interface IMaterial<VectorT> where VectorT : VectorBase<double, VectorT>
{
    string Name { get; }
}
