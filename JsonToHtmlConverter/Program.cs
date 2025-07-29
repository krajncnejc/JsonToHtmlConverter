using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonToHtmlConverter
{
    internal class Program
    {
        private static ConvertController controller;

        static void Main(string[] args)
        {
            controller = new ConvertController();
            string jsonFileOutput1 = File.ReadAllText("helloWorld.json"); //Najdemo json in ga preberemo
            string jsonFileOutput2 = File.ReadAllText("pageNotFound.json");  //vse datoteke se nahajajo lokalno znotraj JsonToHtmlConverter\JsonToHtmlConverter\bin\Debug
            string jsonFileOutput3 = File.ReadAllText("pageNotFoundV2.json");

            var jsonObject1 = JsonConvert.DeserializeObject<HtmlDocument>(jsonFileOutput1); //Newtonsoft pomaga da spremeni v HtmlDocument tip
            var jsonObject2 = JsonConvert.DeserializeObject<HtmlDocument>(jsonFileOutput2); 
            var jsonObject3 = JsonConvert.DeserializeObject<HtmlDocument>(jsonFileOutput3); 

            string htmlOutput1 = controller.GenerateHtml(jsonObject1);
            string htmlOutput2 = controller.GenerateHtml(jsonObject2);
            string htmlOutput3 = controller.GenerateHtml(jsonObject3);

            Console.WriteLine(htmlOutput1);
            Console.WriteLine();
            Console.WriteLine(htmlOutput2);
            Console.WriteLine();
            Console.WriteLine(htmlOutput3);

            //Shranimo Html datoteko v BIN od projekta se shrani
            string htmlFileName1 = "outputHtml1.html";
            string htmlFileName2 = "outputHtml2.html";
            string htmlFileName3 = "outputHtml3.html";
            File.WriteAllText(htmlFileName1, htmlOutput1);
            File.WriteAllText(htmlFileName2, htmlOutput2);
            File.WriteAllText(htmlFileName3, htmlOutput3);
        }  
    }
}
