using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.Problems.Materials;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class MaterialAttribute(PDE_Type pDE, MaterialType material, FieldType fieldType) : Attribute
{
    public PDE_Type PDE_Type { get; init; } = pDE;
    public MaterialType MaterialType { get; init; } = material;
    public FieldType FieldType { get; init; } = fieldType;
}
