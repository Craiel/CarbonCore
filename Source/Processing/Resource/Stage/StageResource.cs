namespace CarbonCore.Processing.Resource.Stage
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Processing.Resource;

    public class StageResource : ProtocolResource
    {
        internal const int Version = 1;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<StageCameraElement> Cameras { get; set; }
        public IList<StageLightElement> Lights { get; set; }
        public IList<StageModelElement> Models { get; set; }

        public IList<string> References { get; set; } 

        public override void Load(Stream source)
        {
            /*Protocol.Resource.Stage entry = Protocol.Resource.Stage.ParseFrom(source);

            if (entry.Version != Version)
            {
                throw new InvalidOperationException("Stage version is not correct: " + entry.Version);
            }

            if (entry.ReferencesCount > 0)
            {
                this.References = new List<string>();
                foreach (string reference in entry.ReferencesList)
                {
                    this.References.Add(reference);
                }
            }

            if (entry.CamerasCount > 0)
            {
                this.Cameras = new List<StageCameraElement>(entry.CamerasCount);
                foreach (Protocol.Resource.StageCamera cameraEntry in entry.CamerasList)
                {
                    this.Cameras.Add(new StageCameraElement(cameraEntry));
                }
            }

            if (entry.LightsCount > 0)
            {
                this.Lights = new List<StageLightElement>(entry.LightsCount);
                foreach (Protocol.Resource.StageLight lightEntry in entry.LightsList)
                {
                    this.Lights.Add(new StageLightElement(lightEntry));
                }
            }

            if (entry.ModelsCount > 0)
            {
                this.Models = new List<StageModelElement>(entry.ModelsCount);
                foreach (Protocol.Resource.StageModel modelEntry in entry.ModelsList)
                {
                    this.Models.Add(new StageModelElement(modelEntry));
                }
            }*/
        }

        public override long Save(Stream target)
        {
            // TODO:
            return 0;

            /*var builder = new Protocol.Resource.Stage.Builder { Version = Version };
            if (this.Cameras != null)
            {
                foreach (StageCameraElement camera in this.Cameras)
                {
                    builder.AddCameras(camera.GetBuilder());
                }
            }

            if (this.Lights != null)
            {
                foreach (StageLightElement light in this.Lights)
                {
                    builder.AddLights(light.GetBuilder());
                }
            }

            if (this.Models != null)
            {
                foreach (StageModelElement model in this.Models)
                {
                    builder.AddModels(model.GetBuilder());
                }
            }

            if (this.References != null)
            {
                foreach (string reference in this.References)
                {
                    builder.AddReferences(reference);
                }
            }

            Protocol.Resource.Stage entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;*/
        }
    }
}
