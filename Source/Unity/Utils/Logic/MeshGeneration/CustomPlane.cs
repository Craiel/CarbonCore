namespace CarbonCore.Unity.Utils.Logic.MeshGeneration
{
    using UnityEngine;

    public class CustomPlane : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public MeshFilter MeshFilter { get; private set; }

        public MeshRenderer Renderer { get; private set; }

        public Mesh Mesh { get; private set; }

        public void Awake()
        {
            this.MeshFilter = this.GetComponent<MeshFilter>();
            this.Renderer = this.GetComponent<MeshRenderer>();
            this.Mesh = this.MeshFilter.mesh;
        }
    }
}
