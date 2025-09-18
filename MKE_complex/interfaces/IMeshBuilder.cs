using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.interfaces;

public enum MeshBuildMethod {Regular, PseudoRegular }

public interface IMeshBuilder
{
    IFiniteElementMesh<IVector> BuildMesh(Dimension dimension, MeshType meshType, string[] fileNames);
}
