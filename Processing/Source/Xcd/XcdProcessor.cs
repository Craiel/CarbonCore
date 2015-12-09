namespace CarbonCore.Processing.Source.Xcd
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CarbonCore.Processing.Resource.Stage;
    using CarbonCore.Processing.Source.Generic.Data;
    using CarbonCore.Processing.Source.Xcd.Scene;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Edge.DirectX;
    using SharpDX;

    public static class XcdProcessor
    {
        private static readonly IList<StageCameraElement> CameraElements;
        private static readonly IList<StageLightElement> LightElements;
        private static readonly IList<StageModelElement> ModelElements;
        private static readonly IList<string> ReferenceDictionary;
        
        private static XcdProcessingOptions currentOptions;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static XcdProcessor()
        {
            CameraElements = new List<StageCameraElement>();
            LightElements = new List<StageLightElement>();
            ModelElements = new List<StageModelElement>();
            ReferenceDictionary = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static StageResource Process(CarbonFile file, XcdProcessingOptions options)
        {
            ClearCache();

            currentOptions = options;

            using (var stream = file.OpenRead())
            {
                var stage = Xcd.Load(stream);
                var resource = new StageResource();
                if (stage.Scene == null)
                {
                    throw new InvalidDataException("XCD File contains no scene information!");
                }

                TranslateScene(stage.Scene);
                if (CameraElements.Count > 0)
                {
                    resource.Cameras = CameraElements.ToArray();
                }

                if (LightElements.Count > 0)
                {
                    resource.Lights = LightElements.ToArray();
                }

                if (ModelElements.Count > 0)
                {
                    resource.Models = ModelElements.ToArray();
                }

                resource.References = ReferenceDictionary;

                return resource;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void ClearCache()
        {
            CameraElements.Clear();
            LightElements.Clear();
            ModelElements.Clear();
            ReferenceDictionary.Clear();
        }

        private static void TranslateScene(XcdScene scene)
        {
            if (scene.Cameras != null)
            {
                foreach (XcdCamera camera in scene.Cameras)
                {
                    TranslateCamera(camera);
                }
            }

            if (scene.Lights != null)
            {
                foreach (XcdLight light in scene.Lights)
                {
                    TranslateLight(light);
                }
            }

            if (scene.Elements != null)
            {
                foreach (XcdElement element in scene.Elements)
                {
                    TranslateElement(element, null);
                }
            }
        }

        private static void TranslateCamera(XcdCamera camera)
        {
            Vector3 position = DataConversion.ToVector3(camera.Position.Data)[0];
            position = BlenderUtils.AdjustCoordinateSystem(position);

            Quaternion rotation = Quaternion.Invert(GetRotation(camera.Rotation.Data));
            var element = new StageCameraElement
                       {
                           Id = camera.Id,
                           FieldOfView = camera.FieldOfView,
                           Rotation = rotation,
                           Position = position,
                           Properties = TranslateProperties(camera.CustomProperties)
                       };

            if (camera.LayerInfo != null)
            {
                element.LayerFlags = new List<bool>(camera.LayerInfo.Data);
            }

            CameraElements.Add(element);
        }

        private static void TranslateLight(XcdLight light)
        {
            Vector3? direction = null;
            Vector3? location = null;
            Vector3? color = null;

            if (light.Direction != null)
            {
                direction = DataConversion.ToVector3(light.Direction.Data)[0];
                direction = BlenderUtils.AdjustCoordinateSystem(direction.Value);
            }

            if (light.Location != null)
            {
                location = DataConversion.ToVector3(light.Location.Data)[0];
                location = BlenderUtils.AdjustCoordinateSystem(location.Value);
            }

            if (light.Color != null)
            {
                color = DataConversion.ToVector3(light.Color.Data)[0];
            }

            var type = (Protocol.Resource.StageLight.Types.StageLightType)Enum.Parse(typeof(Protocol.Resource.StageLight.Types.StageLightType), light.Type);
            var element = new StageLightElement
            {
                Id = light.Id,
                Type = type,
                Direction = direction,
                Location = location,
                Color = color,
                Intensity = light.Intensity,
                AmbientIntensity = light.AmbientIntensity,
                SpotSize = light.SpotSize,
                Angle = light.Angle,
                Radius = light.Radius,
                Properties = TranslateProperties(light.CustomProperties)
            };

            if (light.LayerInfo != null)
            {
                element.LayerFlags = new List<bool>(light.LayerInfo.Data);
            }

            LightElements.Add(element);
        }

        private static void TranslateElement(XcdElement element, StageModelElement parent)
        {
            Vector3 translation = DataConversion.ToVector3(element.Translation.Data)[0];
            translation = BlenderUtils.AdjustCoordinateSystem(translation);

            // Todo: Check if we need to compensate these...
            Quaternion rotation = GetRotation(element.Rotation.Data);
            Vector3 scale = DataConversion.ToVector3(element.Scale.Data)[0];
            
            var modelElement = new StageModelElement
            {
                Id = element.Id,
                Translation = translation,
                Rotation = rotation,
                Scale = scale,
                Properties = TranslateProperties(element.CustomProperties)
            };

            if (!string.IsNullOrEmpty(element.Reference))
            {
                modelElement.ReferenceId = GetReferenceId(element.Reference);
            }

            if (element.LayerInfo != null)
            {
                modelElement.LayerFlags = new List<bool>(element.LayerInfo.Data);
            }

            if (element.Elements != null)
            {
                foreach (XcdElement xcdElement in element.Elements)
                {
                    TranslateElement(xcdElement, modelElement);
                }
            }

            if (parent != null)
            {
                if (parent.Children == null)
                {
                    parent.Children = new List<StageModelElement>();
                }

                parent.Children.Add(modelElement);
            }
            else
            {
                ModelElements.Add(modelElement);   
            }
        }
        
        private static Quaternion GetRotation(float[] data)
        {
            if (data.Length != 5)
            {
                throw new InvalidDataException("Rotation data is not in the expected format");
            }

            // Rotation is in floats 0-1 and in format wxyz so we re-order and convert
            var type = (XcdRotationType)data[0];
            switch (type)
            {
                case XcdRotationType.AxisAngle:
                    {
                        var angle = new Vector3(data[1], data[2], data[3]);
                        return Quaternion.RotationAxis(angle, data[4]);
                    }

                case XcdRotationType.EulerXYZ:
                    {
                        System.Diagnostics.Trace.TraceError("Rotation in EulerXYZ, not translating!");
                        return Quaternion.Identity;
                    }

                case XcdRotationType.Quaternion:
                    {
                        return new Quaternion(data[1], data[2], data[3], data[4]);
                    }

                default:
                    {
                        return Utils.Diagnostics.Internal.NotImplemented<Quaternion>("Unknown rotation mode: " + type);
                    }
            }
        }

        private static int GetReferenceId(string sourceReference)
        {
            string resolved = currentOptions.ReferenceResolver != null
                                             ? currentOptions.ReferenceResolver(sourceReference)
                                             : sourceReference;

            if (string.IsNullOrEmpty(resolved))
            {
                return -1;
            }

            if (ReferenceDictionary.Contains(resolved))
            {
                return ReferenceDictionary.IndexOf(resolved);
            }

            ReferenceDictionary.Add(resolved);
            return ReferenceDictionary.Count - 1;
        }

        private static StagePropertyElement[] TranslateProperties(XcdCustomProperties customProperties)
        {
            if (customProperties == null || customProperties.Properties == null || customProperties.Properties.Length <= 0)
            {
                return null;
            }

            var elements = new StagePropertyElement[customProperties.Properties.Length];
            for (int i = 0; i < customProperties.Properties.Length; i++)
            {
                var type = (Protocol.Resource.StageProperty.Types.StagePropertyType)Enum.Parse(typeof(Protocol.Resource.StageProperty.Types.StagePropertyType), customProperties.Properties[i].Type);
                switch (type)
                {
                    case Protocol.Resource.StageProperty.Types.StagePropertyType.String:
                        {
                            elements[i] = new StagePropertyElementString
                                              {
                                                  Id = customProperties.Properties[i].Id,
                                                  Value = customProperties.Properties[i].Value
                                              };
                            break;
                        }

                    case Protocol.Resource.StageProperty.Types.StagePropertyType.Float:
                        {
                            elements[i] = new StagePropertyElementFloat
                                              {
                                                  Id = customProperties.Properties[i].Id,
                                                  Value = float.Parse(customProperties.Properties[i].Value)
                                              };
                            break;
                        }

                    case Protocol.Resource.StageProperty.Types.StagePropertyType.Int:
                        {
                            elements[i] = new StagePropertyElementInt
                                              {
                                                  Id = customProperties.Properties[i].Id,
                                                  Value = int.Parse(customProperties.Properties[i].Value)
                                              };
                            break;
                        }

                    default:
                        {
                            return Utils.Diagnostics.Internal.NotImplemented<StagePropertyElement[]>();
                        }
                }
            }

            return elements;
        }
    }
}
