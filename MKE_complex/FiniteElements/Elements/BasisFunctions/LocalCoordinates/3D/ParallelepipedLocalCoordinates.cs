using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._1D;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._3D;
public static class ParallelepipedLocalCoordinates
{
    public static (double xi, double eta, double zeta) CalcLocalCoordinates(ReadOnlySpan<Vector3D> vertices, Vector3D point)
    {
        var opposingVerticesTuple = Hexahedron.OpposingVertices(vertices);

        Vector3D[] opposingVertices = [opposingVerticesTuple.A,
                                       opposingVerticesTuple.B];
        
        var x = opposingVertices.Select(i => i.X).ToArray();
        var y = opposingVertices.Select(i => i.Y).ToArray();
        var z = opposingVertices.Select(i => i.Z).ToArray();
        double xi = LineLocalCoordinates.Xi(x, point.X);
        double eta = LineLocalCoordinates.Xi(y, point.Y);
        double zeta = LineLocalCoordinates.Xi(y, point.Z);
        return (xi, eta, zeta);
    }

    public static double Xi(ReadOnlySpan<Vector3D> vertices, Vector3D point)
    {
        var opposingVerticesTuple = Hexahedron.OpposingVertices(vertices);
        Vector3D[] opposingVertices = [opposingVerticesTuple.A,
                                       opposingVerticesTuple.B];
        
        double[] x = opposingVertices.Select(i => i.X).ToArray();
        double xi = LineLocalCoordinates.Xi(x, point.X);
        return xi;
    }

    public static double Eta(ReadOnlySpan<Vector3D> vertices, Vector3D point)
    {
        var opposingVerticesTuple = Hexahedron.OpposingVertices(vertices);
        Vector3D[] opposingVertices = [opposingVerticesTuple.A,
                                       opposingVerticesTuple.B];
        
        double[] y = opposingVertices.Select(i => i.Y).ToArray();
        double eta = LineLocalCoordinates.Xi(y, point.Y);
        return eta;
    }

    public static double Zeta(ReadOnlySpan<Vector3D> vertices, Vector3D point)
    {
        var opposingVerticesTuple = Hexahedron.OpposingVertices(vertices);
        Vector3D[] opposingVertices = [opposingVerticesTuple.A,
                                       opposingVerticesTuple.B];
        
        double[] z = opposingVertices.Select(i => i.Z).ToArray();
        double zeta = LineLocalCoordinates.Xi(z, point.Z);
        return zeta;
    }

    public static Vector3D LocalCoordinatesToGlobal(ReadOnlySpan<Vector3D> vertices, (double xi, double eta, double zeta) localCoordinates)
    {
        return vertices[0] + (vertices[1] - vertices[0]) * localCoordinates.xi + 
                             (vertices[2] - vertices[0]) * localCoordinates.eta +
                             (vertices[4] - vertices[0]) * localCoordinates.zeta;
    }
}