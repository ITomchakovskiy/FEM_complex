using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._1D;
public static class LineLocalCoordinates
{
    public static double Xi(ReadOnlySpan<double> vertices, double point)
    {
        return (point - vertices[0]) / (vertices[1] - vertices[0]);
    }

    public static double LocalCoordinatesToGlobal(ReadOnlySpan<double> vertices, double xi)
    {
        double h = vertices[1] - vertices[0];
        return h * xi + vertices[0];
    }

    public static double Xi<VectorT>(ReadOnlySpan<VectorT> vertices, VectorT point) where VectorT : VectorBase<double, VectorT>
    {
        return vertices[0].Length(point) / 
               vertices[0].Length(vertices[1]);
    }

    public static VectorT LocalCoordinatesToGlobal<VectorT>(ReadOnlySpan<VectorT> vertices, double xi) where VectorT : VectorBase<double, VectorT>
    {
        var h = vertices[1] - vertices[0];
        return h * xi + vertices[0];
    }
}