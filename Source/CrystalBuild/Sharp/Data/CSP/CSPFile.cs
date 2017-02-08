namespace CarbonCore.CrystalBuild.Sharp.Data.CSP
{
    using System.Collections.Generic;
    using System.Xml;
    using Utils;
    using Utils.IO;

    public class CSPFile
    {
        private readonly IDictionary<string, XmlElement> itemGroups;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        internal CSPFile()
        {
            this.Document = new XmlDocument();

            XmlNode docNode = this.Document.CreateXmlDeclaration("1.0", "utf-8", null);
            this.Document.AppendChild(docNode);

            this.Root = this.Document.CreateElement("Project", SharpConstants.ProjectFileNamespace);
            this.Root.SetAttribute("ToolsVersion", "4.0");
            this.Root.SetAttribute("DefaultTargets", "Build");

            this.Document.AppendChild(this.Root);

            this.itemGroups = new Dictionary<string, XmlElement>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        internal XmlDocument Document { get; private set; }

        internal XmlElement Root { get; private set; }

        public static CSPFile Create()
        {
            return new CSPFile();
        }

        public CSPPropertyGroup AddPropertyGroup()
        {
            var group = new CSPPropertyGroup(this);
            return group;
        }

        public void AddItem(string type, string content)
        {
            this.DoAddItem(type, content);
        }

        public void Save(CarbonFile file)
        {
            this.Document.Save(file.GetPath());
        }

        public void AddImport(string import)
        {
            XmlElement importNode = this.Document.CreateElement("Import", SharpConstants.ProjectFileNamespace);
            importNode.SetAttribute("Project", import);
            this.Root.AppendChild(importNode);
        }

        public void AddReference(BuildReference reference)
        {
            XmlElement node = this.DoAddItem("Reference", reference.Name);
            if (!string.IsNullOrEmpty(reference.HintPath))
            {
                XmlElement hintNode = this.Document.CreateElement("HintPath", SharpConstants.ProjectFileNamespace);
                hintNode.InnerText = reference.HintPath.GetAgnosticPath();
                node.AppendChild(hintNode);
            }
        }

        public void AddProjectReference(BuildProjectReference reference)
        {
            string referencePath = string.Concat(reference.Path, System.IO.Path.DirectorySeparatorChar, reference.Namespace, SharpConstants.ProjectFileExtension);
            
            XmlElement node = this.DoAddItem("ProjectReference", referencePath.GetAgnosticPath());

            XmlElement nameNode = this.Document.CreateElement("Name", SharpConstants.ProjectFileNamespace);
            nameNode.InnerText = reference.Namespace;
            node.AppendChild(nameNode);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private XmlElement DoAddItem(string type, string content)
        {
            XmlElement groupParent;
            if (!this.itemGroups.TryGetValue(type, out groupParent))
            {
                groupParent = this.Document.CreateElement("ItemGroup", SharpConstants.ProjectFileNamespace);
                this.Root.AppendChild(groupParent);
                this.itemGroups.Add(type, groupParent);
            }

            var itemNode = this.Document.CreateElement(type, SharpConstants.ProjectFileNamespace);
            itemNode.SetAttribute("Include", content);
            groupParent.AppendChild(itemNode);
            return itemNode;
        }

        public void AddTT(CarbonFile ttFile)
        {
            CarbonFile targetFile = ttFile.ChangeExtension(SharpConstants.ExtensionCS);

            // The compile for the generated code
            XmlElement element = this.DoAddItem("Compile", targetFile.GetPath());
            XmlElement autoGenElement = this.Document.CreateElement("AutoGen", SharpConstants.ProjectFileNamespace);
            XmlElement designTimeElement = this.Document.CreateElement("DesignTime", SharpConstants.ProjectFileNamespace);
            XmlElement dependElement = this.Document.CreateElement("DependentUpon", SharpConstants.ProjectFileNamespace);
            autoGenElement.InnerText = "True";
            designTimeElement.InnerText = "True";
            dependElement.InnerText = ttFile.FileName;

            element.AppendChild(autoGenElement);
            element.AppendChild(designTimeElement);
            element.AppendChild(dependElement);

            // Create the TT Content section
            element = this.DoAddItem("Content", ttFile.GetPath());
            XmlElement generatorElement = this.Document.CreateElement("Generator", SharpConstants.ProjectFileNamespace);
            XmlElement lastOutputElement = this.Document.CreateElement("LastGenOutput", SharpConstants.ProjectFileNamespace);

            generatorElement.InnerText = "TextTemplatingFileGenerator";
            lastOutputElement.InnerText = targetFile.FileName;

            element.AppendChild(generatorElement);
            element.AppendChild(lastOutputElement);
            /*<ItemGroup>
    <Content Include="Logic\Enums\EnumValues.tt">
      <Generator>TextTemplatingFileGenerator</Generator>
      <LastGenOutput>EnumValues.cs</LastGenOutput>
    </Content>
  </ItemGroup>*/
        }
    }
}
