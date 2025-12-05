using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Problems;

public enum PhysicsType
{
    Heat,
    Electrostatics,
}

public enum PDE_Type
{
    Elliptic,
    Parabolic,
}

public enum CoordinateSystem
{
    Cartesian,
    Cylindrical,
    Spherical,
}

public enum FieldType
{
    Scalar,
    Vector,
}
