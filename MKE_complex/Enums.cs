using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex;

public enum Dimension {D1,D2,D3}

public enum GeometryType {Line, Triangle, Quadrangle, Hexagon, Tetrahedron, Mix}

public enum BasisType { Lagrangian, Hermitian}
