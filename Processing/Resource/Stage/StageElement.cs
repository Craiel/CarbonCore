namespace CarbonCore.Processing.Resource.Stage
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class StageElement
    {
        public string Id { get; set; }

        public IList<bool> LayerFlags { get; set; }
        public IList<StagePropertyElement> Properties { get; set; }

        protected void LoadProperties(IList<Protocol.Resource.StageProperty> propertiesList)
        {
            this.Properties = new List<StagePropertyElement>();
            foreach (Protocol.Resource.StageProperty property in propertiesList)
            {
                switch (property.Type)
                {
                    case Protocol.Resource.StageProperty.Types.StagePropertyType.String:
                        {
                            this.Properties.Add(new StagePropertyElementString(property));
                            break;   
                        }

                    case Protocol.Resource.StageProperty.Types.StagePropertyType.Float:
                        {
                            this.Properties.Add(new StagePropertyElementFloat(property));
                            break;
                        }

                    case Protocol.Resource.StageProperty.Types.StagePropertyType.Int:
                        {
                            this.Properties.Add(new StagePropertyElementInt(property));
                            break;
                        }

                    default:
                        {
                            Utils.Edge.Diagnostic.Internal.NotImplemented("Unsupported property type: " + property.Type);
                            break;
                        }
                }
            }
        }

        protected void LoadLayerData(int layerFlags)
        {
            this.LayerFlags = new List<bool>();
            for (int i = 1; i <= 32; i++)
            {
                int flagValue = 1 << i;
                this.LayerFlags.Add((layerFlags & flagValue) == flagValue);
            }
        }

        protected IEnumerable<Protocol.Resource.StageProperty> SaveProperties()
        {
            return this.Properties.Select(propertyElement => propertyElement.GetBuilder().Build()).ToList();
        }

        protected int SaveLayerData()
        {
            System.Diagnostics.Debug.Assert(this.LayerFlags != null && this.LayerFlags.Count <= 32, "Layer flags are not in valid state");
            int value = 0;
            for (int i = 0; i < 32; i++)
            {
                if (this.LayerFlags.Count <= i)
                {
                    break;
                }

                if (this.LayerFlags[i])
                {
                    value = value | (1 << i);
                }
            }

            return value;
        }

        /*private static int TranslateLayerFlags(int[] data)
        {
            int flags = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 1)
                {
                    flags = flags & 1 << i;
                }
            }

            return flags;
        }*/

        /*protected override void DoLoad(CarbonBinaryFormatter source)
        {
            this.LayerFlags = source.ReadInt();

            short count = source.ReadShort();
            if (count > 0)
            {
                this.Properties = new StagePropertyElement[count];
                for (int i = 0; i < count; i++)
                {
                    short type = source.ReadShort();
                    switch ((StagePropertyType)type)
                    {
                        case StagePropertyType.String:
                            {
                                this.Properties[i] = new StagePropertyElementString();
                                this.Properties[i].Load(source);
                                break;
                            }

                        case StagePropertyType.Float:
                            {
                                this.Properties[i] = new StagePropertyElementFloat();
                                this.Properties[i].Load(source);
                                break;
                            }

                        case StagePropertyType.Int:
                            {
                                this.Properties[i] = new StagePropertyElementInt();
                                this.Properties[i].Load(source);
                                break;
                            }
                    }
                }
            }

            this.Id = source.ReadString();
        }

        protected override void DoSave(CarbonBinaryFormatter target)
        {
            target.Write(this.LayerFlags);

            if (this.Properties == null)
            {
                target.Write((short)0);
            }
            else
            {
                target.Write(this.Properties.Length);
                for (int i = 0; i < this.Properties.Length; i++)
                {
                    target.Write((short)this.Properties[i].Type);
                    this.Properties[i].Save(target);
                }
            }

            target.Write(this.Id);
        }*/

        
    }
}
