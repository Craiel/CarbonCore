namespace CarbonCore.Processing.Source.Collada
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    using CarbonCore.Processing.Source.Collada.Effect;
    using CarbonCore.Processing.Source.Collada.General;
    using CarbonCore.Processing.Source.Collada.Geometry;
    using CarbonCore.Processing.Source.Collada.Scene;

    [Serializable]
    [XmlRoot(ElementName = "COLLADA", Namespace = "http://www.collada.org/2005/11/COLLADASchema", IsNullable = false)]
    public class ColladaModel
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ColladaModel));

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement(ElementName = "library_images")]
        public ColladaImageLibrary ImageLibrary { get; set; }

        [XmlElement(ElementName = "library_effects")]
        public ColladaEffectLibrary EffectLibrary { get; set; }

        [XmlElement(ElementName = "library_materials")]
        public ColladaMaterialLibrary MaterialLibrary { get; set; }

        [XmlElement(ElementName = "library_geometries")]
        public ColladaGeometryLibrary GeometryLibrary { get; set; }

        [XmlElement(ElementName = "library_visual_scenes")]
        public ColladaSceneLibrary SceneLibrary { get; set; }

        public static ColladaModel Load(byte[] data)
        {
            using (var dataStream = new MemoryStream(data))
            {
                return Serializer.Deserialize(dataStream) as ColladaModel;
            }
        }

        public static ColladaModel Load(Stream source)
        {
            return Serializer.Deserialize(source) as ColladaModel;
        }
    }
}
