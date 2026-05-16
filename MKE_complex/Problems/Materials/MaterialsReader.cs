using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.Problems.Materials;
public static class MaterialsReader
{
    public static Dictionary<string, IMaterial<VectorT>> ReadMaterials<VectorT>(string filename, PDE_Type pdeType, FieldType fieldType, CoordinateSystem coordinateSystem) where VectorT : VectorBase<double, VectorT>
    {
        var info = File.ReadAllText(Path.Combine("input", filename));

        var options = new JsonSerializerOptions()
        {
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        var materials = JsonSerializer.Deserialize<MaterialFileInfo[]>(info, options)?.Select(i => MaterialCreator.CreateMaterial<VectorT>(pdeType,fieldType, i, coordinateSystem));

        return materials?.ToDictionary(i => i.Name, i => i) ?? new Dictionary<string, IMaterial<VectorT>>();
    }
}