using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MKE_complex.Problems.Materials;
public struct MaterialFileInfo
{
    public string Name { get; init; }
    public Dictionary<string, string> Functions { get; init; }
    public MaterialType MaterialType { get; init; }
}