using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions.Messages;
using MKE_complex.FiniteElements;
using MKE_complex.FiniteElements.Elements;
using MKE_complex.FiniteElements.FiniteElementGeometry;
using MKE_complex.FiniteElements.FiniteElementGeometry._2D;
using MKE_complex.FiniteElements.FiniteElementGeometry._3D;
using MKE_complex.Vector;

namespace MKE_complex.Mesh.MeshBuilder;

public class RegularParallelepipedMeshBuilder : IMeshBuilder
{
    private class Domain(string material, (int,int) xw, (int,int) yw, (int,int) zw)
    {
        public string Material = material;
        public (int X0, int X1) XW = xw;
        public (int Y0, int Y1) YW = yw;
        public (int Z0, int Z1) ZW = zw;
    }
    public IFiniteElementMesh<VectorT> BuildMesh<VectorT>(Dimension dimension, GeometryType meshType, BasisType basisType, int order, ReadOnlySpan<string> fileNames) where VectorT : VectorBase<double,VectorT>
    {
        if(fileNames.Length != 3)
            throw new ArgumentException();
        var MeshFile = fileNames[0];

        var meshReader = new StreamReader(MeshFile);

        double[]? XWValues = meshReader.ReadLine()?.Split(' ').Select(double.Parse).ToArray();
        double[]? YWValues = meshReader.ReadLine()?.Split(' ').Select(double.Parse).ToArray();
        double[]? ZWValues = meshReader.ReadLine()?.Split(' ').Select(double.Parse).ToArray();

        if(XWValues is null || YWValues is null || ZWValues is null) throw new FormatException("Wrong file format");

        var line = meshReader.ReadLine()?.Split(' ');

        List<Domain> domains = [];

        while(line is not null && line.Length == 7)
        {
            string material = line[0];
            var W = line.AsSpan(1,line.Length-1).ToArray().Select(int.Parse).ToArray();
            domains.Add(new(material,(W[0],W[1]),(W[2],W[3]),(W[4],W[5])));
            line = meshReader.ReadLine()?.Split(' ');
        }

        meshReader.Close();

        var fragmentationFile = fileNames[1];

        var fragmentationReader = new StreamReader(fragmentationFile);

        void ReadFragmentationFileLine(int length, out int[] n, out double[] k, out int[] fragmentation, StreamReader reader)
        {
            line = fragmentationReader.ReadLine()?.Split(' ');

            if(line is null || line.Length != 3 * length) throw new FormatException("Wrong file format");

            n = new int[length];
            k = new double[length];
            fragmentation = new int[length];

            for(int i = 0; i < length; ++i)
            {
                n[i] = int.Parse(line[3*i]);
                k[i] = double.Parse(line[3*i + 1]);
                fragmentation[i] = int.Parse(line[3*i + 2]);
            }
        }
        
        int[] FragmentationLength = [XWValues.Length - 1, 
                                     YWValues.Length - 1,
                                     ZWValues.Length - 1];
        ReadFragmentationFileLine(FragmentationLength[0],out int[] nx,out double[] kx, out int[] ax, fragmentationReader);
        ReadFragmentationFileLine(FragmentationLength[1],out int[] ny,out double[] ky, out int[] ay,fragmentationReader);
        ReadFragmentationFileLine(FragmentationLength[2],out int[] nz,out double[] kz, out int[] az,fragmentationReader);

        fragmentationReader.Close();

        // List<double> X = [XWValues[0]], Y = [YWValues[0]], Z = [ZWValues[0]];

        // int[] XW = new int[XWValues.Length], YW = new int[YWValues.Length], ZW = new int[ZWValues.Length];

        void InitializeCoordinates(double[] WValues, int[] n, double[] k, int[] a, out double[] Values,out int[] W)
        {
            W = new int[WValues.Length];

            List<double> values = [WValues[0]];

            for(int i = 0; i < n.Length; ++i)
            {
                // double len = WValues[i+1] - WValues[0];
                // double initialInterval = Math.Abs(k[i] - 1d) > 1.0E-5 ? len/
                //                                                         ((1d - Math.Pow(k[i],n[i]))/(1d - k[i])) : 
                //                                                         len / n[i];
                // double multiplier = 1d;
                if(a[i] == 0)
                {
                    for(int j = 1; j <= n[i]; ++j)
                    values.Add(GeometricMethods.PointOnLine(WValues[i],WValues[i+1],n[i],k[i],j));
                    W[i+1] = values.Count() - 1;
                }
                else
                {
                    int il = 1;
                    for(int p = 0; p < a[i]; ++p) il *= 2;
                    int N = n[i] * il; 
                    double[] localValues = new double[N];
                    double Klocal = k[i];
                    for(int j = 1; j <= n[i]; ++j)
                    {
                        int ind = j * il - 1;
                        localValues[ind] = GeometricMethods.PointOnLine(WValues[i],WValues[i+1],n[i],k[i],j);
                    }
                    int newValuesPerA = n[i];
                    for(int ia = a[i]; ia > 0; --ia)
                    {
                        Klocal = Klocal > 0d ? Math.Sqrt(Klocal) : -Math.Sqrt(-Klocal);
                        int new_il = il / 2;

                        int ind = il - new_il - 1;
                        localValues[ind] = GeometricMethods.PointOnLine(WValues[i],localValues[ind+new_il],2,Klocal,1);

                        for(int j = 2; j <= newValuesPerA; ++j)
                        {
                            ind = j * il - new_il - 1;
                            localValues[ind] = GeometricMethods.PointOnLine(localValues[ind-new_il],localValues[ind+new_il],2,Klocal,1);
                        }
                        newValuesPerA *= 2;
                        il = new_il;
                    }
                    values.AddRange(localValues);
                    W[i+1] = values.Count() - 1;
                }
                
            }

            Values = values.ToArray();
        }

        double[] X, Y, Z;
        int[] XW, YW, ZW;

        InitializeCoordinates(XWValues,nx,kx,ax,out X, out XW);
        InitializeCoordinates(YWValues,ny,ky,ay,out Y, out YW);
        InitializeCoordinates(ZWValues,nz,kz,az,out Z, out ZW);

        Vector3D VertexForIndex(int number)
        {
            return new(X[number % X.Length],
                       Y[(number / X.Length) % Y.Length],
                       Z[number/(X.Length*Y.Length)]);
        }

        int N = X.Length * Y.Length * Z.Length;

        bool[] IsVertexIndexInDomain = new bool[N];
        Array.Fill(IsVertexIndexInDomain, false);

        Dictionary<int,int> IndicesDictionary = [];

        foreach(var domain in domains)
        {
            for(int i = ZW[domain.ZW.Z0]; i <= ZW[domain.ZW.Z1]; ++i)
            {
                for(int j = YW[domain.YW.Y0]; j <= YW[domain.YW.Y1]; ++j )
                {
                    for(int p = XW[domain.XW.X0]; p <= XW[domain.XW.X1]; ++p)
                    {
                        int index = i * X.Length * Y.Length + j * X.Length + p;
                        IsVertexIndexInDomain[index] = true;
                    }
                }
            }
        }

        //List<Vector3D> vertices = [];

        List<VectorT> vertices = [];

        for(int i = 0, currentIndex = 0; i < N; ++i)
        {
            if(IsVertexIndexInDomain[i])
            {
                IndicesDictionary[i] = currentIndex;
                ++currentIndex;
                if(vertices is List<Vector3D> vertices3d)
                    vertices3d.Add(VertexForIndex(i));
            }
        }

        List<IFiniteElement<VectorT>> elements = [];

        foreach(var domain in domains)
        {
            for(int i = ZW[domain.ZW.Z0]; i < ZW[domain.ZW.Z1]; ++i)
            {
                for(int j = YW[domain.YW.Y0]; j < YW[domain.YW.Y1]; ++j )
                {
                    for(int p = XW[domain.XW.X0]; p < XW[domain.XW.X1]; ++p)
                    {
                        int index = i * X.Length * Y.Length + j * X.Length + p;
                        int[] indices = [index, index+1,index+X.Length,index+X.Length+1,
                                        index+X.Length*Y.Length, index+X.Length*Y.Length+1,
                                        index+X.Length*(Y.Length+1), index+X.Length*(Y.Length+1)+1];
                        if(indices.Select(i => IsVertexIndexInDomain[i]).All(i => i is true))
                        {
                            var VertexNumber = indices.Select(i => IndicesDictionary[i]);
                            var geometry = new Parallelepiped(VertexNumber.ToArray());
                            var element = FiniteElementsCreator.CreateFiniteElement(GeometryType.Parallelepiped,
                                                                                    basisType,order,domain.Material,geometry);
                            if(elements is List<IFiniteElement<Vector3D>> elements3d)
                                elements3d.Add(element);
                        }
                    }
                }
            }
        }

        var edgesFile = fileNames[2];

        var edgesReader = new StreamReader(edgesFile);

        line = edgesReader.ReadLine()?.Split(' ');

        List<Domain>[] edgeDomains = [[],[],[]];

        while(line is not null && line.Length == 7)
        {
            string material = line[0];
            var W = line.AsSpan(1,line.Length-1).ToArray().Select(int.Parse).ToArray();
            if(W[0] == W[1])
                edgeDomains[0].Add(new(material,(W[0],W[1]),(W[2],W[3]),(W[4],W[5])));
            else if(W[2] == W[3])
                edgeDomains[1].Add(new(material,(W[0],W[1]),(W[2],W[3]),(W[4],W[5])));
            else if(W[4] == W[5])
                edgeDomains[2].Add(new(material,(W[0],W[1]),(W[2],W[3]),(W[4],W[5])));
            line = edgesReader.ReadLine()?.Split(' ');
        }

        List<IBoundaryCondition<VectorT>> boundaries = [];

        foreach(var boundaryDomain in edgeDomains[0])
        {
            int p = XW[boundaryDomain.XW.X0];
            for(int i = ZW[boundaryDomain.ZW.Z0]; i < ZW[boundaryDomain.ZW.Z1]; ++i)
            {
                for(int j = YW[boundaryDomain.YW.Y0]; j < YW[boundaryDomain.YW.Y1]; ++j )
                {
                    int index = i * X.Length * Y.Length + j * X.Length + p;
                    //int[] indices = [index, index + X.Length, index + X.Length * (Y.Length + 1), index + X.Length * Y.Length];
                    int[] indices = [index, index + X.Length * Y.Length, index + X.Length * (Y.Length + 1), index + X.Length];
                    if(indices.Select(i => IsVertexIndexInDomain[i]).All(i => i is true))
                    {
                        var VertexNumber = indices.Select(i => IndicesDictionary[i]);
                        var geometry = new RectangleBoundary(VertexNumber.ToArray());
                        var boundary = FiniteElementsCreator.CreateBoundaryCondition(GeometryType.Rectangle, basisType, order,boundaryDomain.Material,geometry);
                        if(boundaries is List<IBoundaryCondition<Vector3D>> boundaries3d)
                            boundaries3d.Add(boundary);
                    }
                }
            }
        }

        foreach(var boundaryDomain in edgeDomains[1])
        {
            int j = YW[boundaryDomain.YW.Y0];
            for(int i = ZW[boundaryDomain.ZW.Z0]; i < ZW[boundaryDomain.ZW.Z1]; ++i)
            {
                for(int p = XW[boundaryDomain.XW.X0]; p < XW[boundaryDomain.XW.X1]; ++p )
                {
                    int index = i * X.Length * Y.Length + j * X.Length + p;
                    int[] indices = [index, index + X.Length*Y.Length, index + X.Length*Y.Length + 1, index + 1];
                    if(indices.Select(i => IsVertexIndexInDomain[i]).All(i => i is true))
                    {
                        var VertexNumber = indices.Select(i => IndicesDictionary[i]);
                        var geometry = new RectangleBoundary(VertexNumber.ToArray());
                        var boundary = FiniteElementsCreator.CreateBoundaryCondition(GeometryType.Rectangle, basisType, order,boundaryDomain.Material,geometry);

                        if(boundaries is List<IBoundaryCondition<Vector3D>> boundaries3d)
                            boundaries3d.Add(boundary);
                    }
                }
            }
        }

        foreach(var boundaryDomain in edgeDomains[2])
        {
            int i = ZW[boundaryDomain.ZW.Z0];
            for(int j = YW[boundaryDomain.YW.Y0]; j < YW[boundaryDomain.YW.Y1]; ++j)
            {
                for(int p = XW[boundaryDomain.XW.X0]; p < XW[boundaryDomain.XW.X1]; ++p)
                {
                    int index = i * X.Length * Y.Length + j * X.Length + p;
                    int[] indices = [index, index + X.Length, index + X.Length + 1, index + 1];
                    if(indices.Select(i => IsVertexIndexInDomain[i]).All(i => i is true))
                    {
                        var VertexNumber = indices.Select(i => IndicesDictionary[i]);
                        var geometry = new RectangleBoundary(VertexNumber.ToArray());
                        var boundary = FiniteElementsCreator.CreateBoundaryCondition(GeometryType.Rectangle, basisType, order,boundaryDomain.Material,geometry);

                        if(boundaries is List<IBoundaryCondition<Vector3D>> boundaries3d)
                            boundaries3d.Add(boundary);
                    }
                }
            }
        }

        return new FiniteElementMesh<VectorT>(vertices, elements, boundaries);
    }
}
