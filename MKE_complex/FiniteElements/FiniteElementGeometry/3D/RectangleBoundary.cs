using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.Vector;

namespace MKE_complex.FiniteElements.FiniteElementGeometry._3D;

public record RectangleBoundary(int[] VertexNumber) : Rectangle<Vector3D>(VertexNumber), IFiniteElementGeometry3D
{
    public int FacesCount => 1;

    public int[] GlobalFace(int faceNumber)
    {
        return LocalFace(faceNumber).Select(i => VertexNumber[i]).ToArray();
    }

    public int[] LocalFace(int faceNumber)
    {
        return faceNumber switch
        {
            0 => [0, 1, 2, 3],
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
