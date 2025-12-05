using MKE_complex.Problems.Materials.MaterialsClasses.Elliptic.Scalar;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Problems.Materials;

public class SpecificMaterials  //тестовый, потом возможно можно будет убрать
{
    public SolidMaterialForScalarEllipticProblem<Vector2D> Iron = new SolidMaterialForScalarEllipticProblem<Vector2D>("Iron","80.4","5","2*x + y",CoordinateSystem.Cartesian);
}
