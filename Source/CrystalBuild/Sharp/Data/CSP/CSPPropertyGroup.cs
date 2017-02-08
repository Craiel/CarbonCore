namespace CarbonCore.CrystalBuild.Sharp.Data.CSP
{
    using System.Xml;

    public class CSPPropertyGroup
    {
        private readonly XmlElement node;

        private XmlAttribute conditionAttribute;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        internal CSPPropertyGroup(CSPFile parent)
        {
            this.Parent = parent;
            
            this.node = parent.Document.CreateElement("PropertyGroup", SharpConstants.ProjectFileNamespace);
            parent.Root.AppendChild(this.node);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CSPFile Parent { get; }

        public string Condition
        {
            get { return this.node.GetAttribute("Condition"); }
            set { this.node.SetAttribute("Condition", value); }
        }

        public void AddProperty(string type, string contents)
        {
            var propertyNode = this.Parent.Document.CreateElement(type, SharpConstants.ProjectFileNamespace);
            propertyNode.InnerText = contents;
            this.node.AppendChild(propertyNode);
        }
    }
}
