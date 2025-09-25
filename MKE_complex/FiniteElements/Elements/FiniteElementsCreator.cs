using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MKE_complex.FiniteElements.Elements;

public static class FiniteElementsCreator
{
    private static Dictionary<(GeometryType, BasisType, int), Type> AttributeToType = new();

    public static void LoadFiniteElementTypes(Assembly assembly)
    {
        var finiteElementType = typeof(IFiniteElement<IVector>);
        //var types = assembly.GetTypes();
        //bool re = TriangleLagrangianLinearFiniteElement is typeof(IFiniteElement<IVector>);
        var elementsTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IFiniteElement<>)));

        foreach (var type in elementsTypes)
        {
            var attrs = Attribute.GetCustomAttributes(type).Where(t => t is FiniteElementAttribute);

            FiniteElementAttribute? attribute = (FiniteElementAttribute?)attrs.FirstOrDefault();

            if (attribute is null)
                throw new NotSupportedException("");

            AttributeToType[(attribute.GeometryType, attribute.BasisType, attribute.Order)] = type;
        }
    }

    public static IFiniteElement<IVector> CreateFiniteElement(GeometryType geometryType, BasisType basis, int order, string material, IFiniteElementGeometry<IVector> geometry)
    {
        Type elementType;
        if (AttributeToType.TryGetValue((geometryType, basis, order), out elementType!))
        {
            Type[] types = [typeof(string), typeof(IFiniteElementGeometry<IVector>)];
            var constructorInfo = elementType.GetConstructor(types);
            if (constructorInfo is null)
                throw new NotSupportedException("");

            object[] arguments = { material, geometry };
            return (IFiniteElement<IVector>)constructorInfo!.Invoke(arguments);
        }
        else throw new NotSupportedException("");
    }
}
