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
    private static Dictionary<(GeometryType, BasisType), Type> finiteElementType = new();

    private static Dictionary<(GeometryType, BasisType), Type> finiteElementEdgeType = new();

    public static void LoadFiniteElementTypes(Assembly assembly)
    {
        var elementsTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IFiniteElement<>)) && !t.IsInterface);

        foreach (var type in elementsTypes)
        {
            var attrs = Attribute.GetCustomAttributes(type).Where(t => t is FiniteElementAttribute);

            FiniteElementAttribute? attribute = (FiniteElementAttribute?)attrs.FirstOrDefault();

            if (attribute is null)
                throw new NotSupportedException("");

            finiteElementType[(attribute.GeometryType, attribute.BasisType)] = type;
        }

        var edgeTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IBoundaryCondition<>)) && !t.IsInterface);

        foreach (var type in edgeTypes)
        {
            var attrs = Attribute.GetCustomAttributes(type).Where(t => t is FiniteElementAttribute);

            FiniteElementAttribute? attribute = (FiniteElementAttribute?)attrs.FirstOrDefault();

            if (attribute is null)
                throw new NotSupportedException("");

            finiteElementEdgeType[(attribute.GeometryType, attribute.BasisType)] = type;
        }
    }

    public static IFiniteElement<VectorT> CreateFiniteElement<VectorT>(GeometryType geometryType, BasisType basis, int order, string material, IFiniteElementGeometry<VectorT> geometry) where VectorT : VectorBase<double, VectorT>
    {
        Type elementType;
        if (finiteElementType.TryGetValue((geometryType, basis), out elementType!))
        {
            Type[] types = [typeof(string), geometry.GetType(), typeof(int)];
            var constructor = elementType.GetConstructor(types);
            if (constructor is null)
                throw new NotSupportedException();
            
            object[] arguments = [material, geometry, order];
            return (IFiniteElement<VectorT>)constructor!.Invoke(arguments);
        }
        else throw new NotSupportedException();
    }

    public static IFiniteElement<VectorT> CreateFiniteElement<VectorT>(GeometryType geometryType, BasisType basis, int order, string material, IFiniteElementGeometry<VectorT> geometry, int[] DOFs) where VectorT : VectorBase<double, VectorT>
    {
        Type elementType;
        if (finiteElementType.TryGetValue((geometryType, basis), out elementType!))
        {
            Type[] types = [typeof(string), geometry.GetType(), typeof(int), typeof(int[])];
            var constructor = elementType.GetConstructor(types);
            if (constructor is null)
                throw new NotSupportedException();
            
            object[] arguments = [material, geometry, order, DOFs];
            return (IFiniteElement<VectorT>)constructor!.Invoke(arguments);
        }
        else throw new NotSupportedException();
    }

    public static IBoundaryCondition<VectorT> CreateBoundaryCondition<VectorT>(GeometryType geometryType, BasisType basis, int order, string material, IFiniteElementGeometry<VectorT> geometry) where VectorT : VectorBase<double, VectorT>
    {
        Type edgeType;
        if (finiteElementEdgeType.TryGetValue((geometryType, basis), out edgeType!))
        {
            Type[] types = [typeof(string), geometry.GetType(), typeof(int)];
            var constructor = edgeType.GetConstructor(types);
            if (constructor is null)
                throw new NotSupportedException();

            object[] arguments = [material, geometry, order];
            return (IBoundaryCondition<VectorT>)constructor!.Invoke(arguments);
        }
        else throw new NotSupportedException();
    }

    public static IBoundaryCondition<VectorT> CreateBoundaryCondition<VectorT>(GeometryType geometryType, BasisType basis, int order, string material, IFiniteElementGeometry<VectorT> geometry, int[] DOFs) where VectorT : VectorBase<double, VectorT>
    {
        Type edgeType;
        if (finiteElementEdgeType.TryGetValue((geometryType, basis), out edgeType!))
        {
            Type[] types = [typeof(string), geometry.GetType(), typeof(int), typeof(int[])];
            var constructor = edgeType.GetConstructor(types);
            if (constructor is null)
                throw new NotSupportedException();

            object[] arguments = [material, geometry, order, DOFs];
            return (IBoundaryCondition<VectorT>)constructor!.Invoke(arguments);
        }
        else throw new NotSupportedException();
    }
}
