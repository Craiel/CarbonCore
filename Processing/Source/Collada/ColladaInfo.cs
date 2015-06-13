namespace CarbonCore.Processing.Source.Collada
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using CarbonCore.Processing.Resource.Model;
    using CarbonCore.Processing.Source.Collada.Effect;
    using CarbonCore.Processing.Source.Collada.General;
    using CarbonCore.Processing.Source.Collada.Geometry;
    using CarbonCore.Protocol.Resource;
    using CarbonCore.Utils.Compat.IO;

    public struct ColladaMeshInfo
    {
        public string Name;
        
        public int Parts;
    }

    public class ColladaInfo
    {
        private readonly List<ColladaMeshInfo> meshInfos;
        private readonly IDictionary<string, ModelMaterialElement> materialInfo;
        private readonly IDictionary<string, string> imageInfo;
        private readonly List<string> normalImages;
        private readonly IDictionary<string, string> colorToNormalImages;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ColladaInfo(CarbonFile file)
        {
            if (!CarbonFile.FileExists(file))
            {
                throw new ArgumentException("Invalid file specified");
            }

            this.meshInfos = new List<ColladaMeshInfo>();
            this.materialInfo = new Dictionary<string, ModelMaterialElement>();
            this.imageInfo = new Dictionary<string, string>();
            this.normalImages = new List<string>();
            this.colorToNormalImages = new Dictionary<string, string>();

            this.Source = file;

            using (var stream = file.OpenRead())
            {
                var model = ColladaModel.Load(stream);
                this.BuildImageLibrary(model.ImageLibrary);
                this.BuildMaterialLibrary(model.EffectLibrary, model.MaterialLibrary);
                this.BuildMeshLibrary(model.GeometryLibrary);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonFile Source { get; private set; }

        public IReadOnlyCollection<ColladaMeshInfo> MeshInfos
        {
            get
            {
                return this.meshInfos.AsReadOnly();
            }
        }

        public IReadOnlyDictionary<string, ModelMaterialElement> MaterialInfos
        {
            get
            {
                return new ReadOnlyDictionary<string, ModelMaterialElement>(this.materialInfo);
            }
        }

        public IReadOnlyDictionary<string, string> ImageInfos
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(this.imageInfo);
            }
        }

        public IReadOnlyCollection<string> NormalImages
        {
            get
            {
                return this.normalImages.AsReadOnly();
            }
        }

        public IReadOnlyDictionary<string, string> ColorToNormalImages
        {
            get
            {
                return new ReadOnlyDictionary<string, string>(this.colorToNormalImages);
            }
        }

        public static string GetUrlValue(string url)
        {
            if (!url.StartsWith("#"))
            {
                return url;
            }

            return url.Substring(1, url.Length - 1);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void LoadEffectFromBlinn(EffectBlinn blinn, ref ModelMaterialElement target)
        {
            target.Type = ModelMaterial.Types.ModelMaterialType.Blinn;

            target.Shinyness = blinn.Shininess.Float.Value;
            target.Refraction = blinn.IndexOfRefraction.Float.Value;

            // Todo: Textures

            // Colors
            if (blinn.Diffuse.Color != null)
            {
                target.ColorDiffuse = blinn.Diffuse.Color.Value;
            }

            if (blinn.Specular.Color != null)
            {
                target.ColorSpecular = blinn.Specular.Color.Value;
            }

            if (blinn.Emission.Color != null)
            {
                target.ColorEmission = blinn.Emission.Color.Value;
            }

            if (blinn.Ambient.Color != null)
            {
                target.ColorAmbient = blinn.Ambient.Color.Value;
            }
        }

        private void BuildMeshLibrary(ColladaGeometryLibrary library)
        {
            this.meshInfos.Clear();

            foreach (ColladaGeometry colladaGeometry in library.Geometries)
            {
                var info = new ColladaMeshInfo
                {
                    Name = colladaGeometry.Id
                };

                if (colladaGeometry.Mesh.Polygons != null)
                {
                    info.Parts += colladaGeometry.Mesh.Sources.Length;
                }

                if (colladaGeometry.Mesh.PolyLists != null)
                {
                    info.Parts += colladaGeometry.Mesh.PolyLists.Length;
                }

                this.meshInfos.Add(info);
            }
        }

        private void BuildImageLibrary(ColladaImageLibrary images)
        {
            this.imageInfo.Clear();
            if (images == null || images.Images == null || images.Images.Length <= 0)
            {
                return;
            }

            foreach (ColladaImage image in images.Images)
            {
                if (string.IsNullOrEmpty(image.InitFrom.Source))
                {
                    System.Diagnostics.Trace.TraceError("Collada Image InitFrom value was null for " + image.Name);
                    continue;
                }

                this.imageInfo.Add(image.Id, image.InitFrom.Source);
            }
        }

        /*private string ResolveEffectTexture(ColladaEffect effect, string initFromValue)
        {
            foreach (EffectParameter parameter in effect.ProfileCommon.Parameter)
            {
                if (parameter.Sid.Equals(initFromValue, StringComparison.OrdinalIgnoreCase))
                {
                    string key;
                    if (parameter.Sampler2D != null)
                    {
                        return this.ResolveEffectTexture(effect, parameter.Sampler2D.Source.Content);
                    }

                    if (parameter.Surface != null)
                    {
                        key = parameter.Surface.InitFrom.Source;
                    }
                    else
                    {
                        return initFromValue;
                    }

                    if (this.imageInfo.ContainsKey(key))
                    {
                        return this.imageInfo[key];
                    }

                    break;
                }
            }

            return initFromValue;
        }*/

        private void BuildMaterialLibrary(ColladaEffectLibrary effectLibrary, ColladaMaterialLibrary materialLibrary)
        {
            this.materialInfo.Clear();

            // Read the effects out and sort them
            IDictionary<string, ModelMaterialElement> effectList = new Dictionary<string, ModelMaterialElement>();
            if (effectLibrary != null && effectLibrary.Effects != null && effectLibrary.Effects.Length > 0)
            {
                foreach (ColladaEffect effect in effectLibrary.Effects)
                {
                    var element = new ModelMaterialElement { Name = effect.Id };
                    if (effect.ProfileCommon.Technique.Blinn != null)
                    {
                        LoadEffectFromBlinn(effect.ProfileCommon.Technique.Blinn, ref element);
                    }
                    else if (effect.ProfileCommon.Technique.Phong != null)
                    {
                        this.LoadEffectFromPhong(effect.ProfileCommon.Technique.Phong, ref element);
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceWarning("Unhandled effect entry! Check source");
                        continue;
                    }

                    effectList.Add(effect.Id, element);
                }
            }

            foreach (ColladaMaterial material in materialLibrary.Materials)
            {
                string effectId = GetUrlValue(material.Effect.Url);
                if (effectList.ContainsKey(effectId))
                {
                    this.materialInfo.Add(material.Id, effectList[effectId]);
                }
            }
        }

        private void LoadEffectFromPhong(EffectPhong phong, ref ModelMaterialElement target)
        {
            target.Type = ModelMaterial.Types.ModelMaterialType.Blinn;

            if (phong.Shininess != null)
            {
                target.Shinyness = phong.Shininess.Float.Value;
            }

            if (phong.IndexOfRefraction != null)
            {
                target.Refraction = phong.IndexOfRefraction.Float.Value;
            }

            if (phong.Transparency != null && phong.Transparency.Float != null)
            {
                target.Transparency = phong.Transparency.Float.Value;
            }

            // Todo: Textures

            // Colors
            if (phong.Diffuse.Color != null)
            {
                target.ColorDiffuse = phong.Diffuse.Color.Value;
            }

            if (phong.Specular.Color != null)
            {
                target.ColorSpecular = phong.Specular.Color.Value;
            }

            if (phong.Emission.Color != null)
            {
                target.ColorEmission = phong.Emission.Color.Value;
            }

            if (phong.Ambient.Color != null)
            {
                target.ColorAmbient = phong.Ambient.Color.Value;
            }
        }

        /*private void BuildMaterialLibrary(ColladaMaterialLibrary materials, ColladaEffectLibrary effectLibrary)
        {
            this.materialInfo.Clear();
            if (materials == null || materials.Materials == null || materials.Materials.Length <= 0
                || effectLibrary == null || effectLibrary.Effects == null || effectLibrary.Effects.Length <= 0)
            {
                return;
            }

            IDictionary<string, string> materialEffectLookup = new Dictionary<string, string>();
            foreach (ColladaMaterial material in materials.Materials)
            {
                materialEffectLookup.Add(GetUrlValue(material.Effect.Url), material.Id);
            }

            foreach (ColladaEffect effect in effectLibrary.Effects)
            {
                EffectTechnique localTechnique = effect.ProfileCommon.Technique;
                string diffuseTexture = null;
                string normalTexture = null;
                string alphaTexture = null;

                if (localTechnique.Phong != null)
                {
                    if (localTechnique.Phong.Diffuse.Texture != null)
                    {
                        diffuseTexture = this.ResolveEffectTexture(effect, localTechnique.Phong.Diffuse.Texture.Texture);
                    }

                    if (localTechnique.Phong.Transparent != null)
                    {
                        alphaTexture = this.ResolveEffectTexture(effect, localTechnique.Phong.Transparent.Texture.Texture);
                    }
                }
                else if (localTechnique.Lambert != null)
                {
                    if (localTechnique.Lambert.Diffuse.Texture != null)
                    {
                        diffuseTexture = this.ResolveEffectTexture(effect, localTechnique.Lambert.Diffuse.Texture.Texture);
                    }
                }
                else if (localTechnique.Blinn != null)
                {
                    if (localTechnique.Blinn.Diffuse.Texture != null)
                    {
                        diffuseTexture = this.ResolveEffectTexture(effect, localTechnique.Blinn.Diffuse.Texture.Texture);
                    }
                }

                if (localTechnique.Extra != null && localTechnique.Extra.Technique != null &&
                        localTechnique.Extra.Technique.Profile == "FCOLLADA")
                {
                    normalTexture = this.ResolveEffectTexture(effect, localTechnique.Extra.Technique.Bump.Texture.Texture);
                    if (normalTexture != null)
                    {
                        if (normalTexture == diffuseTexture)
                        {
                            string normalName = string.Concat(Path.GetFileNameWithoutExtension(normalTexture), "_N", Path.GetExtension(normalTexture));
                            if (!this.colorToNormalImages.ContainsKey(normalName))
                            {
                                this.colorToNormalImages.Add(normalName, normalTexture);
                            }

                            normalTexture = normalName;
                        }
                        else
                        {
                            if (!this.normalImages.Contains(normalTexture))
                            {
                                this.normalImages.Add(normalTexture);
                            }
                        }
                    }
                }

                // Todo:
                if (diffuseTexture != null)
                {
                    var material = new ModelMaterialElement
                    {
                        Name = materialEffectLookup[effect.Id],
                        DiffuseTexture = diffuseTexture,
                        NormalTexture = normalTexture,
                        AlphaTexture = alphaTexture
                    };

                    this.materialInfo.Add(material.Name, material);
                }
            }
        }*/
    }
}
