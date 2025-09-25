using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements;

[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class FiniteElementAttribute(GeometryType geometry, BasisType basis, int order) : Attribute
{
    public GeometryType GeometryType { get; init; } = geometry;

    public BasisType BasisType { get; init; } = basis;

    public int Order { get; init; } = order;
}
