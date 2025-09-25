using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Mesh.MeshBuilder;

public enum MeshBuildMethod {Regular, PseudoRegular }

public interface IMeshBuilder
{
    IFiniteElementMesh<IVector> BuildMesh(Dimension dimension, GeometryType meshType, BasisType basisType,int order, string[] fileNames);
}
