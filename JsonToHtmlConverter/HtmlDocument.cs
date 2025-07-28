using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonToHtmlConverter
{
    public class HtmlDocument
    {
        //lahko dodamo [JsonProperty("doctype/Language/Head/Body")] samo ni pri nobenem potrebno ker imajo isto ime in newtonsoft zna povezat
        public string Doctype { get; set; }
        public string Language { get; set; }
        public HtmlElement Head{ get; set;}
        public HtmlElement Body {  get; set;}
    }

    public class HtmlElement
    {
        public Dictionary<string, JToken> Attributes { get; set; }

        [JsonExtensionData] //Edini ki nujno rabi, da lahko sprejme več podvrst/childov
        public Dictionary<string, JToken> Children { get; set; }
    }
}