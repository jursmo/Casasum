using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Casasum.model
{
    sealed public class XmlFileParser
    {
        private string?                _file;
        private string?                _pathToFile;
        private XElement?              _xmlDocument;
        private IEnumerable<XElement>? _xmlElements;
        private SaleCasesList          _saleCasesList;
        private List< string >         _warningMessagesList;
        private List< string >         _errorMessagesList;
        private List< bool >            _dataToShow;
        private Dictionary< int, string > _processStatusToElement 
            = new Dictionary< int, string >() { { 1, "Model" },{ 2, "sellDate" }
                                               ,{ 4, "price" },{ 8, "vatRate" } };

        public XmlFileParser( string pathToFile, SaleCasesList casesList, List< string > _errMsgs, List< string > _warnMsgs, List< bool > dataToShow)
        {
            _saleCasesList       = casesList;
            _errorMessagesList   = _errMsgs;
            _warningMessagesList = _warnMsgs;
            _dataToShow = dataToShow;

            _pathToFile  = pathToFile;
            _xmlDocument = XElement.Load(pathToFile);
            _xmlElements = _xmlDocument.Descendants();

            processXmlFile();
        }

        private void processXmlFile()
        {
            const byte processStartBit = 0b10000; // start bit definition
            byte processStatus = processStartBit; // proces is starting
            const byte controlStatus = 0b1111;    // this is what it should look like - all fields were be initialised
            string saleCaseNumber = "1";
            SaleCase sc = new SaleCase();         // initialisation
            bool thereIsSomethingToProcess = false;
            _dataToShow[0] = false;
            foreach (XElement element in _xmlElements)
            {
                if (element.Name == "sellingCase")
                {
                    thereIsSomethingToProcess = true;
                    if (processStatus == processStartBit)
                    {
                        processStatus = 0b0000;   // ok - we have completed the first pass - no more is needed.
                        continue;                 // first pass - there is nothing to solve
                    }
                    processStatusEvaluate(controlStatus, ref processStatus, saleCaseNumber, ref sc);
                    try
                    {
                        saleCaseNumber = element.Attribute("id").Value;  // the value is used in the next cycle
                    }
                    catch (NullReferenceException ex)
                    {
                        _warningMessagesList.Add(ex.Message);
                        saleCaseNumber = "undefined";
                    }
                }
                else if (element.Name == "model")
                {
                    sc.Model = element.Value;
                    if( sc.Model.Equals("") )
                    {
                        _warningMessagesList.Add("Zpracování vstupního XML souboru: Nekorektní formát <" + element.Name + ">");
                        continue;
                    }
                    processStatus |= 0b0001;
                }
                else if (element.Name == "sellDate")
                {   
                    try
                    {
                        string[] dateItems = element.Value.Split('.');
                        int day = System.Int32.Parse(dateItems[0]);
                        int month = System.Int32.Parse(dateItems[1]);
                        int year = System.Int32.Parse(dateItems[2]);
                        DateTime date = new DateTime(year, month, day);
                        sc.Date = date;
                    }
                    catch (FormatException ex)
                    {
                        _warningMessagesList.Add("Zpracování vstupního XML souboru: Nekorektní formát <" + element.Name + ">");
                        continue;
                    }
                    processStatus |= 0b0010;
                }
                else if (element.Name == "price")
                {
                    try
                    {
                        sc.PriceWoVat = Convert.ToDouble(element.Value);
                    }
                    catch (FormatException ex)
                    {
                        _warningMessagesList.Add("Zpracování vstupního XML souboru: Nekorektní formát <" + element.Name + ">");
                        continue;
                    }
                processStatus |= 0b0100;
                }
                else if (element.Name == "vatRate")
                {
                    try
                    {
                        sc.Vat = Convert.ToDouble(element.Value);
                    }
                    catch (FormatException ex)
                    {
                        _warningMessagesList.Add("Zpracování vstupního XML souboru: Nekorektní formát <" + element.Name + ">");
                        continue;
                    }
                    processStatus |= 0b1000;
                }
                else
                {

                    _warningMessagesList.Add($"Zpracování vstupního XML souboru: nedefinovaný element <{element.Name}>.");
                }
            }
            if (thereIsSomethingToProcess)
            {
                processStatusEvaluate(controlStatus, ref processStatus, saleCaseNumber, ref sc);
            }
            else
            {
                _warningMessagesList.Add("Ve vstupním XML souboru nejsou žádna data.");
            }
        }
        private void processStatusEvaluate( byte controlStatus, ref byte processStatus, string saleCaseNumber, ref SaleCase sc )
        {
            if (processStatus == controlStatus)    // ok - we have initialised all needed fields.
            {
                sc.processPriceWithVat();
                _saleCasesList.saleCaseAdd(sc);
                sc = new SaleCase();
                processStatus = 0b0000;
                _dataToShow[0] = true;
            }
            else
            {
                byte absentElement = (byte) (processStatus ^ controlStatus);
                string? elementName;
                byte[] states = { 1, 2, 4, 8 };
                _warningMessagesList.Add($"Zpracování vstupního XML souboru: Neúplný záznam o prodeji vynecháh: id <{saleCaseNumber}>");
                foreach (var state in states)
                {
                    if( Convert.ToBoolean( state & absentElement ))
                    {
                        try
                        {
                            elementName = _processStatusToElement[ state ];
                        }
                        catch( KeyNotFoundException ex )
                        {
                            elementName = "neznámy element";
                            _warningMessagesList.Add( ex.Message );
                        }
                        _warningMessagesList.Add($"chybějící element <{elementName}>.");
                    }
                }
                processStatus = 0b0000;
                sc = new SaleCase();
            }
        }
    }
}
