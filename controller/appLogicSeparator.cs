using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casasum.controller
{
    public class appLogicSeparator
    {
        private SaleCasesList saleCasesList = new();

        public List<string> processXmlFile(string pathToXml)
        {
            XmlFileParser xmlFileParser = new(pathToXml);
            saleCasesList.SaleCasesListInit = xmlFileParser.SaleCasesList;
            Dictionary<string, Dictionary<string, double>> saleSummary = Summarizer.saleSum(saleCasesList.weekendSaleQuery());

            StringBuilder str = new();
            List<string> printQueue = new List<string>();
            foreach (var model in saleSummary.Keys)
            {
                str.Append(model + "\n ");
                string toSubtract = saleSummary[model]["priceWithVat"].ToString("C");
                int space = 25 - toSubtract.Length;
                string spacebar = new(' ', space);
                str.Append(saleSummary[model]["priceWithVat"].ToString("C") + spacebar + saleSummary[model]["priceWoVat"].ToString("C"));

                printQueue.Add(str.ToString());
                str = new();
            }
            return printQueue;
        }
    }
}
