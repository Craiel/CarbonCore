namespace CarbonCore.Processing.Processors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    using CarbonCore.Processing.Data;
    using CarbonCore.Processing.Resource;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;
    
    public class UserInterfaceProcessor
    {
        private const string NodeTypeImage = "image";
        private const string NodeTypeFrame = "frame";
        private const string NodeTypeConsole = "console";
        private const string NodeTypeSolidText = "solidtext";
        private const string NodeTypeDynamicText = "dynamictext";
        private const string NodeTypeComment = "#comment";
        private const string NodeTypePage = "page";

        private const string AttributeName = "name";
        private const string AttributeWidth = "width";
        private const string AttributeHeight = "height";
        private const string AttributeLayoutMode = "layoutmode";
        private const string AttributeText = "text";
        private const string AttributeMode = "mode";
        private const string AttributeSource = "source";
        private const string AttributeLeft = "left";
        private const string AttributeTop = "top";
        private const string AttributeRight = "right";
        private const string AttributeBottom = "bottom";
        private const string AttributeHorizontalAlignment = "horizontalalignment";
        private const string AttributeVerticalAlignment = "verticalalignment";

        private static readonly Regex CsamlFieldRegex = new Regex("{([a-z]+)[\\s]*([^\"]*)}", RegexOptions.IgnoreCase);

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static UserInterfaceResource Process(CarbonFile file, UserInterfaceProcessingOptions options)
        {
            if (!CarbonFile.FileExists(file))
            {
                throw new ArgumentException("Invalid Script Processing options");
            }

            CarbonFile scriptFile = file.ToFile(".lua");
            if (!CarbonFile.FileExists(scriptFile))
            {
                throw new InvalidOperationException("Script file was not found for User Interface");
            }

            var resource = new UserInterfaceResource();

            // Read the Csaml
            using (var stream = file.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    string data = reader.ReadToEnd();
                    data = CsamlFieldRegex.Replace(data, CsamlFieldEvaluator);
                    resource.CsamlData = BuildProtoEntry(data);
                }
            }

            // Read the Script
            resource.Script = ScriptProcessor.Process(scriptFile, options.ScriptOptions);

            return resource;
        }

        private static Csaml BuildProtoEntry(string data)
        {
            var document = new XmlDocument();
            document.LoadXml(data);
            
            // TODO:
            return null;

            /*var builder = new Csaml.Builder();
            builder.AddRangeNodes(BuildCsamlNodes(document.ChildNodes));

            if (builder.NodesCount != 1 || builder.NodesList[0].Type != CsamlNode.Types.CsamlNodeType.Page)
            {
                throw new InvalidOperationException("Csaml must have exactly one root of type page!");
            }

            return builder.Build();*/
        }

        private static IEnumerable<CsamlNode> BuildCsamlNodes(XmlNodeList nodes)
        {
            /*IList<CsamlNode> processedNodes = new List<CsamlNode>();
            foreach (XmlNode node in nodes)
            {
                var builder = new CsamlNode.Builder();
                string key = node.Name.ToLower();
                switch (key)
                {
                    case NodeTypeFrame:
                        {
                            builder.Type = CsamlNode.Types.CsamlNodeType.Frame;
                            break;
                        }

                    case NodeTypeImage:
                        {
                            builder.Type = CsamlNode.Types.CsamlNodeType.Image;
                            break;
                        }

                    case NodeTypeConsole:
                        {
                            builder.Type = CsamlNode.Types.CsamlNodeType.Console;
                            break;
                        }

                    case NodeTypeSolidText:
                        {
                            builder.Type = CsamlNode.Types.CsamlNodeType.SolidText;
                            break;
                        }

                    case NodeTypeDynamicText:
                        {
                            builder.Type = CsamlNode.Types.CsamlNodeType.DynamicText;
                            break;
                        }

                    case NodeTypePage:
                        {
                            builder.Type = CsamlNode.Types.CsamlNodeType.Page;
                            break;
                        }

                    case NodeTypeComment:
                        {
                            // Don't care about these
                            continue;
                        }

                    default:
                        {
                            throw new InvalidOperationException("Unknown Node Type: " + key);
                        }
                }

                builder.AddRangeAttributes(BuildCsamlAttribute(node.Attributes));

                if (node.HasChildNodes)
                {
                    builder.AddRangeChildren(BuildCsamlNodes(node.ChildNodes));
                }

                processedNodes.Add(builder.Build());
            }

            return processedNodes;*/

            // TODO
            return null;
        }

        private static IEnumerable<CsamlAttribute> BuildCsamlAttribute(XmlAttributeCollection attributes)
        {
            /*IList<CsamlAttribute> processedAttributes = new List<CsamlAttribute>();
            foreach (XmlAttribute attribute in attributes)
            {
                var builder = new CsamlAttribute.Builder();
                string key = attribute.Name.ToLower();
                switch (key)
                {
                    case AttributeName:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.ControlName; 
                            SetStringValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeWidth:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Width;
                            SetIntValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeHeight:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Height;
                            SetIntValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeLayoutMode:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.LayoutMode;
                            SetEnumValue(attribute.Value, builder, typeof(CsamlAttribute.Types.CsamlLayoutMode));
                            break;
                        }

                    case AttributeMode:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.TypeAttribute;
                            SetEnumValue(attribute.Value, builder, typeof(CsamlAttribute.Types.CsamlControlSizingMode));
                            break;
                        }

                    case AttributeSource:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Source;
                            SetStringValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeText:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Text;
                            SetStringValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeLeft:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Left;
                            SetIntValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeTop:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Top;
                            SetIntValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeRight:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Right;
                            SetIntValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeBottom:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.Bottom;
                            SetIntValue(attribute.Value, builder);
                            break;
                        }

                    case AttributeVerticalAlignment:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.VerticalAlignment;
                            SetEnumValue(attribute.Value, builder, typeof(CsamlAttribute.Types.CsamlVerticalAlignmentEnum));
                            break;
                        }

                    case AttributeHorizontalAlignment:
                        {
                            builder.Type = CsamlAttribute.Types.CsamlAttributeType.HorizontalAlignment;
                            SetEnumValue(attribute.Value, builder, typeof(CsamlAttribute.Types.CsamlHorizontalAlignmentEnum));
                            break;
                        }

                    default:
                        {
                            throw new InvalidOperationException("Unknown attribute Type: " + key);
                        }
                }

                processedAttributes.Add(builder.Build());
            }

            return processedAttributes;*/

            // TODO
            return null;
        }

        /*private static void SetStringValue(string source, CsamlAttribute.Builder target)
        {
            target.ValueType = CsamlAttribute.Types.CsamlAttributeValueType.String;
            target.ValueString = source;
        }

        private static void SetIntValue(string source, CsamlAttribute.Builder target)
        {
            try
            {
                target.ValueType = CsamlAttribute.Types.CsamlAttributeValueType.Int;
                target.ValueInt = int.Parse(source);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(string.Format("Format for Attribute {0} was invalid (int): {1}", target.Type, source), e);
            }
        }

        private static void SetEnumValue(string source, CsamlAttribute.Builder target, Type enumType)
        {
            try
            {
                target.ValueType = CsamlAttribute.Types.CsamlAttributeValueType.Int;
                target.ValueInt = (int)Enum.Parse(enumType, source);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(string.Format("Format for Attribute {0} was invalid ({1}): {2}", target.Type, enumType, source), e);
            }
        }*/

        private static string CsamlFieldEvaluator(Match match)
        {
            if (match.Captures.Count <= 0 || match.Groups.Count < 2)
            {
                Diagnostic.Warning("Could not evaluate Resource, no capture data");
                return "ERROR";
            }

            string fieldId = match.Groups[1].Value.ToLower();
            string fieldValue = match.Groups.Count > 2 ? match.Groups[2].Value : null;
            switch (fieldId)
            {
                case "resource":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            Diagnostic.Warning("Argument missing in resource Field");
                            return "ERROR";
                        }

                        return HashUtils.BuildResourceHash(fieldValue);
                    }
            }

            Diagnostic.Warning("Unknown Field in Script: " + match.Captures[0].Value);
            return "ERROR";
        }
    }
}
