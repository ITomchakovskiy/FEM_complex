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
    private static Dictionary<(GeometryType, BasisType, int), Type> finiteElementType = new();

    private static Dictionary<(GeometryType, BasisType, int), Type> finiteElementEdgeType = new();

    public static void LoadFiniteElementTypes(Assembly assembly)
    {
        var elementsTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IFiniteElement<>)));

        foreach (var type in elementsTypes)
        {
            var attrs = Attribute.GetCustomAttributes(type).Where(t => t is FiniteElementAttribute);

            FiniteElementAttribute? attribute = (FiniteElementAttribute?)attrs.FirstOrDefault();

            if (attribute is null)
                throw new NotSupportedException("");

            finiteElementType[(attribute.GeometryType, attribute.BasisType, attribute.Order)] = type;
        }

        var edgeTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IFiniteElementEdge<>)));

        foreach (var type in edgeTypes)
        {
            var attrs = Attribute.GetCustomAttributes(type).Where(t => t is FiniteElementAttribute);

            FiniteElementAttribute? attribute = (FiniteElementAttribute?)attrs.FirstOrDefault();

            if (attribute is null)
                throw new NotSupportedException("");

            finiteElementEdgeType[(attribute.GeometryType, attribute.BasisType, attribute.Order)] = type;
        }
    }

    public static IFiniteElement<VectorT> CreateFiniteElement<VectorT>(GeometryType geometryType, BasisType basis, int order, string material, IFiniteElementGeometry<VectorT> geometry) where VectorT : VectorBase
    {
        Type elementType;
        if (finiteElementType.TryGetValue((geometryType, basis, order), out elementType!))
        {
            Type[] types = [typeof(string), geometry.GetType()];
            var constructor = elementType.GetConstructor(types);
            if (constructor is null)
                throw new NotSupportedException();
            
            object[] arguments = [material, geometry];
            return (IFiniteElement<VectorT>)constructor!.Invoke(arguments);
        }
        else throw new NotSupportedException();
    }

    public static IFiniteElementEdge<VectorT> CreateFiniteElementEdge<VectorT>(GeometryType geometryType, BasisType basis, int order, string material, IFiniteElementGeometry<VectorT> geometry) where VectorT : VectorBase
    {
        Type edgeType;
        if (finiteElementEdgeType.TryGetValue((geometryType, basis, order), out edgeType!))
        {
            Type[] types = [typeof(string), geometry.GetType()];
            var constructor = edgeType.GetConstructor(types);
            if (constructor is null)
                throw new NotSupportedException();

            object[] arguments = [material, geometry];
            return (IFiniteElementEdge<VectorT>)constructor!.Invoke(arguments);
        }
        else throw new NotSupportedException();
    }
}
