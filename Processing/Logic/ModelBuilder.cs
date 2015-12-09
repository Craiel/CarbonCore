namespace CarbonCore.Processing.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Processing.Resource.Model;

    using SharpDX;

    /*
     Helper class to construct Mesh Structures from Vectors and indices
     Todo: 
      - Check vertex duplication
      - Generate Normals with MathExtension.CalculateSurfaceNormal()
    */
    public class ModelBuilder
    {
        private readonly IList<ModelResourceElement> pendingElements;
        private readonly IList<ModelResourceElement> elements;
        private readonly IList<uint> elementIndices;

        private readonly string name;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelBuilder(string name)
        {
            this.name = name;

            this.IsIndexed = false;

            this.pendingElements = new List<ModelResourceElement>();
            this.elements = new List<ModelResourceElement>();
            this.elementIndices = new List<uint>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool DoubleSided { get; set; }
        public bool IsIndexed { get; set; }
        
        public void BeginPolygon()
        {
            if (this.IsIndexed)
            {
                throw new InvalidOperationException("Don't use Begin/End Polygon in indexed mode");
            }

            if (this.pendingElements.Count > 0)
            {
                throw new InvalidOperationException("BeginPolygon called with data still present, call EndPolygon before starting a new one!");
            }
        }

        public void EndPolygon(bool cw = false)
        {
            if (this.IsIndexed)
            {
                throw new InvalidOperationException("Don't use Begin/End Polygon in indexed mode");
            }

            if (this.pendingElements.Count < 3)
            {
                throw new InvalidDataException("Polygon must have at least 3 vertices set!");
            }

            switch (this.pendingElements.Count)
            {
                case 3:
                    {
                        // Triangle
                        if (cw)
                        {
                            this.EndPolygonCCW3();
                        }
                        else
                        {
                            this.EndPolygonCW3();
                        }

                        break;
                    }

                case 4:
                    {
                        // Quad, gets divided into triangles
                        if (cw)
                        {
                            this.EndPolygonCW4();
                        }
                        else
                        {
                            this.EndPolygonCCW4();
                        }

                        break;
                    }

                default:
                    {
                        // N-gon
                        if (!cw)
                        {
                            Utils.Edge.Diagnostic.Internal.NotImplemented("N-gon does not support rotation clockwise change currently");
                        }

                        for (int i = 0; i < this.pendingElements.Count; i++)
                        {
                            this.elements.Add(this.pendingElements[i]);
                        }

                        break;
                    }
            }

            this.pendingElements.Clear();
        }

        public void DiscardCurrent()
        {
            this.pendingElements.Clear();
        }

        public void Clear()
        {
            this.pendingElements.Clear();
            this.elements.Clear();
        }
        
        public void AddVertex(Vector3 position, Vector3? normal = null, Vector2? texture = null, Vector4? color = null)
        {
            this.pendingElements.Add(new ModelResourceElement { Position = position, Normal = normal, Texture = texture });
        }

        public void AddIndices(uint[] indices)
        {
            if (!this.IsIndexed)
            {
                throw new InvalidOperationException("Mesh Builder is set to generate indices, can't add custom");
            }

            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] >= this.pendingElements.Count)
                {
                    throw new InvalidDataException("Index outside of element range " + indices[i]);
                }

                this.elementIndices.Add(indices[i]);
            }
        }

        public ModelResource ToResource()
        {
            uint[] indexData;

            if (this.IsIndexed)
            {
                for (int i = 0; i < this.pendingElements.Count; i++)
                {
                    this.elements.Add(this.pendingElements[i]);
                }

                indexData = new uint[this.elementIndices.Count];
                for (int i = 0; i < this.elementIndices.Count; i++)
                {
                    indexData[i] = this.elementIndices[i];
                }

                return new ModelResource { Elements = this.elements, Indices = indexData, Name = this.name };
            }

            if (this.pendingElements.Count > 0)
            {
                throw new InvalidOperationException("Elements are still pending, ToMesh call invalid");
            }

            indexData = new uint[this.elements.Count];
            for (uint i = 0; i < this.elements.Count; i++)
            {
                indexData[i] = i;
            }

            return new ModelResource { Elements = this.elements, Indices = indexData, Name = this.name };
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void EndPolygonCW3()
        {
            this.elements.Add(this.pendingElements[0]);
            this.elements.Add(this.pendingElements[1]);
            this.elements.Add(this.pendingElements[2]);

            if (this.DoubleSided)
            {
                this.elements.Add(this.pendingElements[2]);
                this.elements.Add(this.pendingElements[1]);
                this.elements.Add(this.pendingElements[0]);
            }
        }

        private void EndPolygonCCW3()
        {
            this.elements.Add(this.pendingElements[0]);
            this.elements.Add(this.pendingElements[2]);
            this.elements.Add(this.pendingElements[1]);

            if (this.DoubleSided)
            {
                this.elements.Add(this.pendingElements[2]);
                this.elements.Add(this.pendingElements[0]);
                this.elements.Add(this.pendingElements[1]);
            }
        }

        private void EndPolygonCW4()
        {
            this.elements.Add(this.pendingElements[0]);
            this.elements.Add(this.pendingElements[1]);
            this.elements.Add(this.pendingElements[3]);

            this.elements.Add(this.pendingElements[3]);
            this.elements.Add(this.pendingElements[1]);
            this.elements.Add(this.pendingElements[2]);

            if (this.DoubleSided)
            {
                this.elements.Add(this.pendingElements[3]);
                this.elements.Add(this.pendingElements[1]);
                this.elements.Add(this.pendingElements[0]);

                this.elements.Add(this.pendingElements[2]);
                this.elements.Add(this.pendingElements[1]);
                this.elements.Add(this.pendingElements[3]);
            }
        }

        private void EndPolygonCCW4()
        {
            this.elements.Add(this.pendingElements[0]);
            this.elements.Add(this.pendingElements[3]);
            this.elements.Add(this.pendingElements[1]);

            this.elements.Add(this.pendingElements[3]);
            this.elements.Add(this.pendingElements[2]);
            this.elements.Add(this.pendingElements[1]);

            if (this.DoubleSided)
            {
                this.elements.Add(this.pendingElements[3]);
                this.elements.Add(this.pendingElements[0]);
                this.elements.Add(this.pendingElements[1]);

                this.elements.Add(this.pendingElements[2]);
                this.elements.Add(this.pendingElements[3]);
                this.elements.Add(this.pendingElements[1]);
            }
        }
    }
}
