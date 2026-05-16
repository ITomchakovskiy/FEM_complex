using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MKE_complex.Vector;

namespace MKE_complex.Problems.Materials;
public static class MaterialCreator
{
    private static Dictionary<(PDE_Type, MaterialType, FieldType), Type> materialTypesDictionary = new();

    public static void LoadMaterialsAssemblyInfo(Assembly assembly)
    {
        var materialTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType && 
                                                                                 i.GetGenericTypeDefinition() == typeof(IMaterial<>)));
                                                                                
        foreach (var type in materialTypes)
        {
            var attr = (MaterialAttribute?)type.GetCustomAttributes(typeof(MaterialAttribute)).FirstOrDefault();
            if(attr is null)
                throw new NotSupportedException();

            materialTypesDictionary[(attr.PDE_Type, attr.MaterialType, attr.FieldType)] = type;
        }
    }

    public static IMaterial<VectorT> CreateMaterial<VectorT>(PDE_Type pdeType, FieldType fieldType, MaterialFileInfo fileInfo, CoordinateSystem coordinateSystem) where VectorT : VectorBase<double, VectorT>
    {
        Type type;
        if (materialTypesDictionary.TryGetValue((pdeType, fileInfo.MaterialType, fieldType), out type!))
        {
            type = type.MakeGenericType(typeof(VectorT));
            Type[] types = [typeof(MaterialFileInfo), typeof(string[])];
            var constructor = type.GetConstructor(types);
            if (constructor is null)
                throw new NotSupportedException();

            string[] coordinates;

            switch(coordinateSystem)
            {
            case CoordinateSystem.Cartesian:
                coordinates = ["x", "y", "z" ];
                break;
            case CoordinateSystem.Cylindrical:
                coordinates = ["r", "z", "phi" ];
                break;
            case CoordinateSystem.Spherical:
                coordinates = ["r", "phi", "psi"];
                break;
            default:
                throw new NotImplementedException();
            }

            object[] arguments = [fileInfo, coordinates];
            return (IMaterial<VectorT>)constructor.Invoke(arguments);
        }
        else
            throw new NotSupportedException();
    }
}