using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Casasum.model
{
    sealed public class XmlFileParser
    {
        private string? _file;
        private string? _pathToFile;
        private XElement? _xmlDocument;
        private IEnumerable<XElement>? _xmlElements;
        private List<SaleCase>? _saleCasesList = new List<SaleCase>();

        public XmlFileParser(string pathToFile)
        {
            bool ok_state = true;
            try
            {
                _pathToFile = pathToFile;
                _xmlDocument = XElement.Load(pathToFile);
                _xmlElements = _xmlDocument.Descendants();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                ok_state = false;
            }

            if (ok_state)
            {
                processXmlFile();
            }
            // !!!else unvalid XML file 

        }


        private void processXmlFile()
        {
            const byte processStartBit = 0b10000;         // start bit definition
            byte processStatus = processStartBit; // proces is starting
            byte controlStatus = 0b1111;          // this is what it should look like - all fields were be initialised
            SaleCase sc = new SaleCase();  // initialisation
            foreach (XElement element in _xmlElements)
            {
                if (element.Name == "sellingCase")
                {
                    if (processStatus == controlStatus)    // ok - we have initialised all needed fields.
                    {
                        sc.processPriceWithVat();
                        _saleCasesList.Add(sc);
                        sc = new SaleCase();
                        processStatus = 0b0000;
                    }
                    else if (processStatus == processStartBit)
                    {
                        processStatus = 0b0000;                 // ok - we have completed the first pass - no more is needed.
                        continue;                               // there isn't something to solve - we need next element.
                    }
                    // else smth???
                }
                else if (element.Name == "model")
                {
                    sc.Model = element.Value;
                    processStatus |= 0b0001;
                }
                else if (element.Name == "sellDate")
                {
                    string[] dateItems = element.Value.Split('.');
                    int day = System.Int32.Parse(dateItems[0]);
                    int month = System.Int32.Parse(dateItems[1]);
                    int year = System.Int32.Parse(dateItems[2]);

                    DateTime date = new DateTime(year, month, day);
                    sc.Date = date;
                    processStatus |= 0b0010;
                }
                else if (element.Name == "price")
                {
                    sc.PriceWoVat = Convert.ToDouble(element.Value);
                    processStatus |= 0b0100;
                }
                else if (element.Name == "vatRate")
                {
                    sc.Vat = Convert.ToDouble(element.Value);
                    processStatus |= 0b1000;
                }
                else
                {
                    //MessageBox.Show("Undefined XML element in parser context");
                }
            }
        }

        public List<SaleCase> SaleCasesList { get => _saleCasesList; }
    }
}
