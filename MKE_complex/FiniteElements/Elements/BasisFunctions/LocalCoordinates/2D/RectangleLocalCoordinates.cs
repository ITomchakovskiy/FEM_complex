using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._1D;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.Elements.BasisFunctions.LocalCoordinates._2D;
public class RectangleLocalCoordinates
{
    public static (double xi, double eta) XiEta(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] x = opposingVertices.Select(i => i.X).ToArray();
        double[] y = opposingVertices.Select(i => i.Y).ToArray();
        double xi = LineLocalCoordinates.Xi(x, point.X);
        double eta = LineLocalCoordinates.Xi(y, point.Y);
        return (xi, eta);
    }

    public static (double xi, double eta) XiEta(ReadOnlySpan<Vector3D> vertices, Vector3D point, out string projectionPlane)
    {
        var vertices2D = GeometricMethods._2DProjection(vertices, out projectionPlane);

        var point2D = point.ProjectionToPlane(projectionPlane);

        return XiEta(vertices2D, point2D);
    }

    public static double Xi(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] x = opposingVertices.Select(i => i.X).ToArray();
        double xi = LineLocalCoordinates.Xi(x, point.X);
        return xi;
    }

    public static double Xi(ReadOnlySpan<Vector3D> vertices, Vector3D point, out string projectionPlane)
    {
        var vertices2D = GeometricMethods._2DProjection(vertices, out projectionPlane);

        var point2D = point.ProjectionToPlane(projectionPlane);

        return Xi(vertices2D, point2D);
    }

    public static double Eta(ReadOnlySpan<Vector2D> vertices, Vector2D point)
    {
        Vector2D[] opposingVertices = [vertices[0], vertices[2]];
        Array.Sort(opposingVertices, (a, b) => a.X.CompareTo(b.X));
        double[] y = opposingVertices.Select(i => i.Y).ToArray();
        double eta = LineLocalCoordinates.Xi(y, point.Y);
        return eta;
    }

    public static double Eta(ReadOnlySpan<Vector3D> vertices, Vector3D point, out string projectionPlane)
    {
        var vertices2D = GeometricMethods._2DProjection(vertices, out projectionPlane);

        var point2D = point.ProjectionToPlane(projectionPlane);

        return Eta(vertices2D, point2D);
    }

    public static VectorT LocalCoordinatesToGlobal<VectorT>(ReadOnlySpan<VectorT> vertices, (double xi, double eta) localCoordinates) where VectorT : VectorBase<double, VectorT>
    {
        // double x = LineLocalCoordinates.LocalCoordinatesToGlobal([vertices[0].X, vertices[2].X], localCoordinates.xi);
        // double y = LineLocalCoordinates.LocalCoordinatesToGlobal([vertices[0].Y, vertices[2].Y], localCoordinates.eta);

        return vertices[0] + (vertices[3] - vertices[0]) * localCoordinates.xi + (vertices[1] - vertices[0]) * localCoordinates.eta;
    }
}