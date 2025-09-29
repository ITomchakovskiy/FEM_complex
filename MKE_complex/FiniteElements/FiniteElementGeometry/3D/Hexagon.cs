using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record Hexagon(int[] VertexNumber) : IFiniteElementGeometry<Vector3D>
{
    public GeometryType GeometryType => GeometryType.Hexagon;

    public (int, int) Edge(int edgeNumber)
    {
        switch(edgeNumber)
        {
            case 0: return (0, 1);
            case 1: return (1, 2);
            case 2: return (2, 3);
            case 3: return (3, 0);
            case 4: return (4, 5);
            case 5: return (5, 6);
            case 6: return (6, 7);
            case 7: return (7, 4);
            case 8: return (0, 4);
            case 9: return (1, 5);
            case 10: return (2, 6);
            case 11: return (3, 7);
            default: throw new Exception("Wrong edge number");
        }
    }

    public static Vector3D PointOnHexagon(Vector3D[] vertices, int n_x, double k_x, int ind_x, int n_y, double k_y, int ind_y, int n_z, double k_z, int ind_z) //for mesh initialization
    {
        Vector3D A = (Vector3D)Vector3D.PointOnLine(vertices[0], vertices[3], n_x, k_x, ind_x);

        Vector3D B = (Vector3D)Vector3D.PointOnLine(vertices[1], vertices[2], n_x, k_x, ind_x);

        Vector3D C = (Vector3D)Vector3D.PointOnLine(vertices[4], vertices[7], n_x, k_x, ind_x);

        Vector3D D = (Vector3D)Vector3D.PointOnLine(vertices[5], vertices[6], n_x, k_x, ind_x);

        Vector3D MAB = (Vector3D)Vector3D.PointOnLine(A, B, n_y, k_y, ind_y);

        Vector3D MCD = (Vector3D)Vector3D.PointOnLine(C, D, n_y, k_y, ind_y);

        return (Vector3D)Vector3D.PointOnLine(MAB, MCD, n_y, k_y, ind_y);
    }
}
