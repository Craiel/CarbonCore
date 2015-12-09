namespace CarbonCore.Utils.Unity.Logic.MeshGeneration
{
    using System.Diagnostics.CodeAnalysis;

    using CarbonCore.Utils.Unity.Logic.Enums;

    using UnityEngine;

    // Based on http://wiki.unity3d.com/index.php?title=CreatePlane
    public static class PlaneGeneration
    {
        private static readonly Vector3 Tangent = new Vector4(1f, 0f, 0f, -1f);

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static CustomPlane CreateMeshObject(PlaneOptions options = null, bool addCollider = false, GameObject customObject = null)
        {
            if (options == null)
            {
                // Create the default set of options
                options = PlaneOptions.Default;
            }

            GameObject result = customObject ?? new GameObject(options.Name);
            
            MeshFilter meshFilter = result.GetOrAddComponent<MeshFilter>();
            result.GetOrAddComponent<MeshRenderer>();

            meshFilter.sharedMesh = CreateMesh(options);
            meshFilter.sharedMesh.RecalculateBounds();

            if (addCollider)
            {
                result.GetOrAddComponent<BoxCollider>();
            }

            return result.GetOrAddComponent<CustomPlane>();
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public static Mesh CreateMesh(PlaneOptions options = null)
        {
            if (options == null)
            {
                // Create the default set of options
                options = PlaneOptions.Default;
            }

            var mesh = new Mesh { name = options.Name };

            // Determine the triangles we have to make
            int triangleCount = options.WidthSegments * options.HeightSegments * 6;
            if (options.TwoSided)
            {
                triangleCount *= 2;
            }

            // Determine vertex count
            int horizontalVertexCount = options.WidthSegments + 1;
            int verticalVertexCount = options.HeightSegments + 1;
            
            int vertexCount = horizontalVertexCount * verticalVertexCount;

            int[] triangles = new int[triangleCount];
            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertexCount];
            Vector4[] tangents = new Vector4[vertexCount];

            Vector3 anchorOffset = AnchorUtils.GetAnchorVector(options.Anchor, options.Width, options.Height);
            int index = 0;
            float uvFactorX = 1.0f / options.WidthSegments;
            float uvFactorY = 1.0f / options.HeightSegments;
            float scaleX = options.Width / options.WidthSegments;
            float scaleY = options.Height / options.HeightSegments;
            for (float y = 0.0f; y < verticalVertexCount; y++)
            {
                for (float x = 0.0f; x < horizontalVertexCount; x++)
                {
                    float vertexX = (x * scaleX) - (options.Width / 2f) - anchorOffset.x;
                    float vertexY = (y * scaleY) - (options.Height / 2f) - anchorOffset.y;

                    if (options.Orientation == Orientation.Horizontal)
                    {
                        vertices[index] = new Vector3(vertexX, 0.0f, vertexY);
                    }
                    else
                    {
                        vertices[index] = new Vector3(vertexX, vertexY, 0.0f);
                    }

                    tangents[index] = Tangent;
                    uvs[index++] = new Vector2(x * uvFactorX, y * uvFactorY);
                }
            }

            index = 0;
            for (int y = 0; y < options.HeightSegments; y++)
            {
                for (int x = 0; x < options.WidthSegments; x++)
                {
                    triangles[index] = (y * horizontalVertexCount) + x;
                    triangles[index + 1] = ((y + 1) * horizontalVertexCount) + x;
                    triangles[index + 2] = (y * horizontalVertexCount) + x + 1;

                    triangles[index + 3] = ((y + 1) * horizontalVertexCount) + x;
                    triangles[index + 4] = ((y + 1) * horizontalVertexCount) + x + 1;
                    triangles[index + 5] = (y * horizontalVertexCount) + x + 1;
                    index += 6;
                }

                if (options.TwoSided)
                {
                    // Same tri vertices with order reversed, so normals point in the opposite direction
                    for (int x = 0; x < options.WidthSegments; x++)
                    {
                        triangles[index] = (y * horizontalVertexCount) + x;
                        triangles[index + 1] = (y * horizontalVertexCount) + x + 1;
                        triangles[index + 2] = ((y + 1) * horizontalVertexCount) + x;

                        triangles[index + 3] = ((y + 1) * horizontalVertexCount) + x;
                        triangles[index + 4] = (y * horizontalVertexCount) + x + 1;
                        triangles[index + 5] = ((y + 1) * horizontalVertexCount) + x + 1;
                        index += 6;
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.tangents = tangents;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
