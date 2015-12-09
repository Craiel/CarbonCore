namespace CarbonCore.Processing.Source.Collada
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CarbonCore.Processing.Logic;
    using CarbonCore.Processing.Resource.Model;
    using CarbonCore.Processing.Source.Collada.Data;
    using CarbonCore.Processing.Source.Collada.Geometry;
    using CarbonCore.Processing.Source.Collada.Scene;
    using CarbonCore.Processing.Source.Generic.Data;
    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Edge.DirectX;

    using SharpDX;

    /* <summary>
     Todo:
     - Clean up the general process
     - First we get all the Model groups out of the collada since that is really all we care for here
     - then either group them all together if nothing specific was requested, otherwise return the one that was asked for
     - !! Get rid of the offset and scaling entirely here, we will do that at the actual scene definition !!
    */
    public static class ColladaProcessor
    {
        private static readonly IDictionary<string, ModelResourceGroup> MeshLibrary = new Dictionary<string, ModelResourceGroup>();

        private static string targetElement;
        private static CarbonDirectory texturePath;

        private static ColladaInput[] currentInputs;
        private static ColladaSource[] currentSources;

        private static IDictionary<uint, uint[]>[] polygonData;

        private static int[] vertexCount;
        private static int[] indexData;

        private static ColladaInput vertexInput;
        private static ColladaInput normalInput;
        private static ColladaInput textureInput;

        private static Vector3[] positionData;
        private static Vector3[] normalData;
        private static Vector2[] textureData;
        
        public static ModelResourceGroup Process(ColladaInfo info, string element, CarbonDirectory texPath)
        {
            ClearCache();
            texturePath = texPath;
            targetElement = element;
            using (var stream = info.Source.OpenRead())
            {
                var model = ColladaModel.Load(stream);

                BuildGeometryLibrary(info, model.GeometryLibrary);

                foreach (ColladaSceneNode sceneNode in model.SceneLibrary.VisualScene.Nodes)
                {
                    ApplyNodeTranslations(sceneNode);
                }

                if (!string.IsNullOrEmpty(element))
                {
                    return MeshLibrary[element];
                }

                if (MeshLibrary.Count <= 1)
                {
                    return MeshLibrary.Values.FirstOrDefault();
                }

                var joinedGroup = new ModelResourceGroup
                                      {
                                          Groups = MeshLibrary.Values.ToList(),
                                          Name = info.Source.FileName
                                      };
                return joinedGroup;
            }
        }

        private static void ClearCache()
        {
            currentInputs = null;
            currentSources = null;
            polygonData = null;

            vertexCount = null;
            indexData = null;

            vertexInput = null;
            normalInput = null;
            textureInput = null;

            positionData = null;
            normalData = null;
            textureData = null;
        }
        
        private static void BuildGeometryLibrary(ColladaInfo info, ColladaGeometryLibrary library)
        {
            MeshLibrary.Clear();

            foreach (ColladaGeometry colladaGeometry in library.Geometries)
            {
                ClearCache();

                if (!string.IsNullOrEmpty(targetElement) && !colladaGeometry.Id.Equals(targetElement))
                {
                    continue;
                }

                IList<ModelResource> parts = new List<ModelResource>();
                if (colladaGeometry.Mesh.Polygons != null && colladaGeometry.Mesh.Polygons.Polygons != null)
                {
                    // Polygons
                    polygonData = new IDictionary<uint, uint[]>[colladaGeometry.Mesh.Sources.Length];
                    polygonData = new IDictionary<uint, uint[]>[colladaGeometry.Mesh.Sources.Length];
                    for (int i = 0; i < colladaGeometry.Mesh.Sources.Length; i++)
                    {
                        ColladaSource source = colladaGeometry.Mesh.Sources[i];
                        ParseGeometry(i, colladaGeometry);
                        ModelResource part = TranslateGeometry(i, colladaGeometry.Name);

                        System.Diagnostics.Trace.TraceWarning(
                            "ColladaSource does not support materials yet! ({0})", source.Id);

                        parts.Add(part);
                    }
                }

                if(colladaGeometry.Mesh.PolyLists != null)
                {
                    // PolyLists
                    polygonData = new IDictionary<uint, uint[]>[colladaGeometry.Mesh.PolyLists.Length];
                    for (int i = 0; i < colladaGeometry.Mesh.PolyLists.Length; i++)
                    {
                        ParseGeometry(i, colladaGeometry);
                        ModelResource part = TranslateGeometry(i, colladaGeometry.Name);

                        ColladaPolyList polyList = colladaGeometry.Mesh.PolyLists[i];
                        ModelMaterialElement material = GetMaterialForPolyList(info, polyList);
                        if (material != null)
                        {
                            if (part.Materials == null)
                            {
                                part.Materials = new List<ModelMaterialElement>();
                            }

                            part.Materials.Add(material);
                        }

                        parts.Add(part);
                    }
                }

                MeshLibrary.Add(colladaGeometry.Id, new ModelResourceGroup { Models = parts, Name = colladaGeometry.Id });
            }
        }

        private static ModelMaterialElement GetMaterialForPolyList(ColladaInfo info, ColladaPolyList polyList)
        {
            if (polyList.Material != null && info.MaterialInfos.ContainsKey(polyList.Material))
            {
                ModelMaterialElement material = info.MaterialInfos[polyList.Material].Clone();
                if (material.DiffuseTexture != null && texturePath != null)
                {
                    material.DiffuseTexture = HashUtils.BuildResourceHash(texturePath.ToFile(Uri.UnescapeDataString(material.DiffuseTexture)).ToString());
                }

                if (material.NormalTexture != null && texturePath != null)
                {
                    material.NormalTexture = HashUtils.BuildResourceHash(texturePath.ToFile(Uri.UnescapeDataString(material.NormalTexture)).ToString());
                }

                if (material.SpecularTexture != null && texturePath != null)
                {
                    material.SpecularTexture = HashUtils.BuildResourceHash(texturePath.ToFile(Uri.UnescapeDataString(material.SpecularTexture)).ToString());
                }

                if (material.AlphaTexture != null && texturePath != null)
                {
                    material.AlphaTexture = HashUtils.BuildResourceHash(texturePath.ToFile(Uri.UnescapeDataString(material.AlphaTexture)).ToString());
                }

                return material;
            }

            return null;
        }

        private static void ParseGeometry(int index, ColladaGeometry geometry)
        {
            if (geometry.Mesh == null || geometry.Mesh.Vertices == null)
            {
                throw new InvalidOperationException("ConvertGeometry failed, no Mesh or Vertex data found for " + geometry.Name);
            }

            if (geometry.Mesh.Polygons != null && geometry.Mesh.PolyLists != null)
            {
                Utils.Edge.Diagnostic.Internal.NotImplemented("Polygons and Polylists at the same time are untested!");
            }

            currentSources = geometry.Mesh.Sources;
            if (geometry.Mesh.Polygons != null && geometry.Mesh.Polygons.Polygons != null)
            {
                System.Diagnostics.Trace.Assert(geometry.Mesh.Polygons != null, "Must have polygons if we are not working with Poly Lists!");

                currentInputs = geometry.Mesh.Polygons.Inputs;
                vertexInput = FindInput("VERTEX");
                normalInput = FindInput("NORMAL");
                textureInput = FindInput("TEXCOORD");
                if (!geometry.Mesh.Vertices.Id.Equals(vertexInput.Source.TrimStart('#')))
                {
                    throw new InvalidDataException("Vertex source does not match position source!");
                }

                LoadPolygonData(index, geometry.Mesh.Polygons.Polygons);

                // Process the Vertex Data
                ColladaSource positionSource = FindSource(geometry.Mesh.Vertices.Input.Source);
                positionData = DataConversion.ToVector3(positionSource.FloatArray.Data);

                // Now load the Normals
                if (normalInput != null)
                {
                    ColladaSource source = FindSource(normalInput.Source);
                    normalData = DataConversion.ToVector3(source.FloatArray.Data);
                }
            }

            if (geometry.Mesh.PolyLists != null)
            {
                // Get the Layout information
                currentInputs = geometry.Mesh.PolyLists[index].Inputs;
                vertexInput = FindInput("VERTEX");
                normalInput = FindInput("NORMAL");
                textureInput = FindInput("TEXCOORD");
                if (!geometry.Mesh.Vertices.Id.Equals(vertexInput.Source.TrimStart('#')))
                {
                    throw new InvalidDataException("Vertex source does not match position source!");
                }

                vertexCount = geometry.Mesh.PolyLists[index].VertexCount.Data;
                indexData = geometry.Mesh.PolyLists[index].P.Data;
                LoadPolygonData(index, indexData);

                // Process the Vertex Data
                ColladaSource positionSource = FindSource(geometry.Mesh.Vertices.Input.Source);
                positionData = DataConversion.ToVector3(positionSource.FloatArray.Data);

                // Now load the Normals and UV's
                if (normalInput != null)
                {
                    ColladaSource source = FindSource(normalInput.Source);
                    normalData = DataConversion.ToVector3(source.FloatArray.Data);
                }

                if (textureInput != null)
                {
                    ColladaSource source = FindSource(textureInput.Source);
                    textureData = DataConversion.ToVector2(source.FloatArray.Data);
                }
            }

            // Enable only if needed, causes massive slowness for larger meshes
            // TraceConversionInfo(geometry);
        }

        /*private static void TraceConversionInfo(ColladaGeometry geometry)
        {
            Trace.TraceInformation("Converting Collada Geometry {0}", geometry.Name);
            Trace.TraceInformation("  -> {0} Polygons", vertexCount.Length);
            Trace.TraceInformation("  -> {0} IndexData", indexData.Length);
            Trace.TraceInformation("  -> {0} Vertices", positionData.Length);
            if (normalData != null)
            {
                Trace.TraceInformation("  -> {0} Normals", normalData.Length);
            }

            if (textureData != null)
            {
                Trace.TraceInformation("  -> {0} UV's", textureData.Length);
            }
        }*/

        private static ModelResource TranslateGeometry(int polyIndex, string name)
        {
            var builder = new ModelBuilder(name);

            // All data is set now, Build the polygons into our mesh
            int indexPosition = 0;
            for (int index = 0; index < vertexCount.Length; index++)
            {
                int count = vertexCount[index];
                builder.BeginPolygon();
                for (int i = 0; i < count; i++)
                {
                    Vector3 position = positionData[polygonData[polyIndex][(uint)vertexInput.Offset][indexPosition]];
                    position = BlenderUtils.AdjustCoordinateSystem(position);

                    Vector3 normal = Vector3.Zero;
                    Vector2 texture = Vector2.Zero;

                    if (normalData != null)
                    {
                        normal = normalData[polygonData[polyIndex][(uint)normalInput.Offset][indexPosition]];
                        normal = BlenderUtils.AdjustCoordinateSystem(normal);
                    }

                    if (textureData != null)
                    {
                        texture = textureData[polygonData[polyIndex][(uint)textureInput.Offset][indexPosition]];
                    }

                    builder.AddVertex(position, normal, texture);
                    indexPosition++;
                }

                // For collada we assume clockwise since we flip everything by -1z
                builder.EndPolygon(cw: true);
            }

            return builder.ToResource();
        }

        private static ColladaInput FindInput(string semantic)
        {
            return currentInputs.FirstOrDefault(colladaInput => colladaInput.Semantic.Equals(semantic));
        }

        private static ColladaSource FindSource(string id)
        {
            id = id.TrimStart('#');
            return currentSources.FirstOrDefault(source => source.Id.Equals(id));
        }

        private static void LoadPolygonData(int index, int[] data)
        {
            polygonData[index] = new Dictionary<uint, uint[]>();
            uint highestOffset = 0;
            foreach (ColladaInput input in currentInputs)
            {
                if (input.Offset > highestOffset)
                {
                    highestOffset = (uint)input.Offset;
                }
            }

            foreach (ColladaInput input in currentInputs)
            {
                if (polygonData[index].ContainsKey((uint)input.Offset))
                {
                    // Todo: This currently happens for normal map texture mapping, the same offset is defined twice with different material source
                    // throw new InvalidDataException("Multiple inputs defined with the same offset");
                    continue;
                }

                polygonData[index].Add((uint)input.Offset, new uint[data.Length / (highestOffset + 1)]);
            }

            uint offset = 0;
            uint element = 0;
            while (offset < data.Length)
            {
                uint currentOffset = 0;
                while (currentOffset <= highestOffset)
                {
                    polygonData[index][currentOffset][element] = (uint)data[offset + currentOffset];
                    currentOffset++;
                }

                offset += highestOffset + 1;
                element++;
            }
        }

        private static void LoadPolygonData(int index, IntArrayType[] polygons)
        {
            polygonData[index] = new Dictionary<uint, uint[]>();
            uint highestOffset = 0;
            foreach (ColladaInput input in currentInputs)
            {
                if (input.Offset > highestOffset)
                {
                    highestOffset = (uint)input.Offset;
                }
            }

            vertexCount = new int[polygons.Length];
            foreach (ColladaInput input in currentInputs)
            {
                if (polygonData[index].ContainsKey((uint)input.Offset))
                {
                    // Todo: This currently happens for normal map texture mapping, the same offset is defined twice with different material source
                    // throw new InvalidDataException("Multiple inputs defined with the same offset");
                    continue;
                }

                polygonData[index].Add((uint)input.Offset, new uint[polygons.Length * 3]);
            }

            uint polygon = 0;
            uint element = 0;
            foreach (IntArrayType data in polygons)
            {
                System.Diagnostics.Trace.Assert(data.Data.Length == (highestOffset + 1) * 3, "Polygon data must be vertex count 3 right now, otherwise we have to adjust the VertexCount array!");

                vertexCount[polygon++] = 3;
                uint dataElementOffset = 0;
                while (dataElementOffset < data.Data.Length - 1)
                {
                    uint currentOffset = 0;
                    while (currentOffset <= highestOffset)
                    {
                        polygonData[index][currentOffset][element] = (uint)data.Data[dataElementOffset + currentOffset];
                        currentOffset++;
                    }

                    dataElementOffset += highestOffset + 1;
                    element++;
                }
            }

            /*foreach (ColladaInput input in currentInputs)
            {
                if (polygonData[index].ContainsKey((uint)input.Offset))
                {
                    // Todo: This currently happens for normal map texture mapping, the same offset is defined twice with different material source
                    // throw new InvalidDataException("Multiple inputs defined with the same offset");
                    continue;
                }

                polygonData[index].Add((uint)input.Offset, new uint[data.Length / (highestOffset + 1)]);
            }

            uint offset = 0;
            uint element = 0;
            while (offset < data.Length)
            {
                uint currentOffset = 0;
                while (currentOffset <= highestOffset)
                {
                    polygonData[index][currentOffset][element] = (uint)data[offset + currentOffset];
                    currentOffset++;
                }

                offset += highestOffset + 1;
                element++;
            }*/
        }

        private static void ApplyNodeTranslations(ColladaSceneNode sceneNode)
        {
            if (sceneNode.InstanceGeometry == null)
            {
                return;
            }

            string targetNode = ColladaInfo.GetUrlValue(sceneNode.InstanceGeometry.Url);
            if (!MeshLibrary.ContainsKey(targetNode))
            {
                return;
            }

            if (sceneNode.Translation != null)
            {
                Vector3 translation = DataConversion.ToVector3(sceneNode.Translation.Data)[0];
                translation = BlenderUtils.AdjustCoordinateSystem(translation);

                MeshLibrary[targetNode].Offset = translation;
            }

            MeshLibrary[targetNode].Scale = sceneNode.Scale != null ? DataConversion.ToVector3(sceneNode.Scale.Data)[0] : new Vector3(1);

            if (sceneNode.Rotations != null)
            {
                MeshLibrary[targetNode].Rotation = Quaternion.Invert(GetNodeRotation(sceneNode));
            }

            // Create a default transformation matrix to rotate the object into place
            // Todo: should do this statically so we don't have to do an extra matrix calculation in the engine
            MeshLibrary[targetNode].Transformations = new List<Matrix>
                                                          {
                                                              Matrix.RotationX(MathUtil.DegreesToRadians(90))
                                                          };

            // Process additional matrices now
            if (sceneNode.Matrices != null)
            {
                foreach (ColladaMatrix matrixEntry in sceneNode.Matrices)
                {
                    Matrix matrix = DataConversion.ToMatrix(matrixEntry.Data);
                    MeshLibrary[targetNode].Transformations.Add(matrix);
                }
            }
        }

        private static Quaternion GetNodeRotation(ColladaSceneNode node)
        {
            float rotationX = 0;
            float rotationY = 0;
            float rotationZ = 0;

            foreach (ColladaRotate rotation in node.Rotations)
            {
                if (rotation.Sid.EndsWith("X"))
                {
                    rotationX = MathExtension.DegreesToRadians(DataConversion.ToVector4(rotation.Data)[0][3]);
                }
                else if (rotation.Sid.EndsWith("Y"))
                {
                    rotationY = MathExtension.DegreesToRadians(DataConversion.ToVector4(rotation.Data)[0][3]);
                }
                else if (rotation.Sid.EndsWith("Z"))
                {
                    rotationZ = MathExtension.DegreesToRadians(DataConversion.ToVector4(rotation.Data)[0][3]);
                }
            }

            return Quaternion.RotationYawPitchRoll(rotationX, rotationY, rotationZ);
        }
    }
}
