using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonToHtmlConverter
{
internal class ConvertController
    {
        public string tab(int level) => new string(' ', level * 4); 
        public string GenerateHtml(HtmlDocument jsonObject)
        {
            var stringBuilderGen = new StringBuilder();
            stringBuilderGen.AppendLine($"<!DOCTYPE {jsonObject.Doctype ?? "html"}>"); // naredi default če nima vlkjučeno v izvorni datoteki
            stringBuilderGen.AppendLine(($"<html lang='{jsonObject.Language ?? "en"}'>"));
            if (jsonObject.Head != null)
            {
                stringBuilderGen.Append(GenerateHtmlElement("head", jsonObject.Head, 1));
            }

            if (jsonObject.Body != null)
            {
                stringBuilderGen.Append(GenerateHtmlElement("body", jsonObject.Body, 1));
            }
            stringBuilderGen.AppendLine("</html>");

            return stringBuilderGen.ToString();
        }
        public string GenerateHtmlElement(string mainTag, HtmlElement mainElement, int tabLVL)
        {
            var stringBuilderGen = new StringBuilder();
            string[] selfEndingTags = { "meta", "link", "br", "hr", "img" }; //ob potrebi se še dodajo kateri so pač tagi ko ne rabijo <nekaj></nekaj>

            string htmlAttributes = "";
            if (mainElement.Attributes != null)//Vsi atributi ki spadajo poleg tega tag-a npr: <p id="myId",background-color:#000000 ipd...>
            {
                var attributeParts = new List<string>();
                foreach (var attribute in mainElement.Attributes) 
                {
                    if (attribute.Key == "style" && attribute.Value.Type == JTokenType.Object) //style dodamo posebej kot atribut da se vsi skupaj držijo
                    {
                        var attributeStyleParts = ((JObject)attribute.Value).Properties().Select(p => $"{p.Name}:{p.Value}");
                        attributeParts.Add($"style=\"{string.Join("; ", attributeStyleParts)}\"");
                    }
                    else
                    {
                        attributeParts.Add($"{attribute.Key}=\"{attribute.Value}\"");
                    }
                }
                if (attributeParts.Count > 0)
                {
                    htmlAttributes = " " + string.Join(" ", attributeParts);
                }
            }

            //preverimo če je Tag ko se sam zaključi drugače delamo normalno
            if (selfEndingTags.Contains(mainTag.ToLower()))
            {
                if (mainElement.Children != null)
                {
                    foreach (var child in mainElement.Children) //Namenjeno za MetaData atribute ki jih je težko dinamično brat brez da bi si jih vnaprej predefinirali
                    {
                        if (child.Value is JObject jObject)
                        {
                            string contentValue = string.Join(", ", jObject.Properties().Select(kv => $"{kv.Name} = {kv.Value}"));
                            htmlAttributes += $" name=\"{child.Key}\" content=\"{contentValue}\"";
                        }
                        else
                        {
                            htmlAttributes += $" {child.Key}=\"{child.Value}\"";
                        }
                    }
                }
                stringBuilderGen.AppendLine($"{tab(tabLVL)}<{mainTag}{htmlAttributes}/>");
            }
            else //navadni torej recimo <p></p>
            {
                stringBuilderGen.AppendLine($"{tab(tabLVL)}<{mainTag}{htmlAttributes}>");
                if (mainElement.Children != null)
                {
                    foreach (var child in mainElement.Children) //gremo skozi nested stvari(children)
                    {
                        if (child.Value.Type == JTokenType.String) //odvisno kakšen child je ga na različen način generiramo
                        {
                            stringBuilderGen.AppendLine($"{tab(tabLVL+1)}<{child.Key}> {child.Value}</{child.Key}>");
                        }
                        else if (child.Value.Type == JTokenType.Object)
                        {
                            var childElement = child.Value.ToObject<HtmlElement>();
                            stringBuilderGen.AppendLine(GenerateHtmlElement(child.Key, childElement, tabLVL + 1));
                        }
                        else if (child.Value.Type == JTokenType.Array)
                        {
                            foreach (var childItem in (JArray)child.Value)
                            {
                                var itemElement = childItem.ToObject<HtmlElement>();
                                stringBuilderGen.AppendLine(GenerateHtmlElement(child.Key, itemElement, tabLVL + 1));
                            }
                        }
                    }
                }
                if (mainTag != "html")
                    stringBuilderGen.AppendLine($"{tab(tabLVL)}</{mainTag}>");
            }
            return stringBuilderGen.ToString();
        }
    }
}
