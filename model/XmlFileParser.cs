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
        private string?                _file;
        private string?                _pathToFile;
        private XElement?              _xmlDocument;
        private IEnumerable<XElement>? _xmlElements;
        private SaleCasesList          _saleCasesList;
        private List< string >         _warningMessagesList;
        private List< string >         _errorMessagesList;
        private Dictionary< int, string > _processStatusToElement 
            = new Dictionary< int, string >() { { 1, "Model" },{ 2, "sellDate" }
                                               ,{ 4, "price" },{ 8, "vatRate" } };

        public XmlFileParser( string pathToFile, SaleCasesList casesList, List< string > _errMsgs, List< string > _warnMsgs )
        {
            _saleCasesList = casesList;
            _errorMessagesList = _errMsgs;
            _warningMessagesList = _warnMsgs;

                _pathToFile = pathToFile;
                _xmlDocument = XElement.Load(pathToFile);
                _xmlElements = _xmlDocument.Descendants();

                processXmlFile();
        }

        private void processXmlFile()
        {
            const byte processStartBit = 0b10000; // start bit definition
            byte processStatus = processStartBit; // proces is starting
            const byte controlStatus = 0b1111;          // this is what it should look like - all fields were be initialised
            string saleCaseNumber = "1";
            SaleCase sc = new SaleCase();         // initialisation
            foreach (XElement element in _xmlElements)
            {
                if (element.Name == "sellingCase")
                {
                    if (processStatus == processStartBit)
                    {
                        processStatus = 0b0000;   // ok - we have completed the first pass - no more is needed.
                        continue;                 // first pass - there is nothing to solve
                    }
                    processStatusEvaluate( controlStatus, ref processStatus, saleCaseNumber, ref sc );
                    try
                    {
                        saleCaseNumber = element.Attribute("um").Value;  // the value is used in the next cycle
                    }
                    catch( NullReferenceException ex )
                    {
                        saleCaseNumber = "undefined";
                    }
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

                    _warningMessagesList.Add($"XmlFileParser: undefined element <{element.Name}>.");
                }
            }
            processStatusEvaluate(controlStatus, ref processStatus, saleCaseNumber, ref sc);
        }
        private void processStatusEvaluate( byte controlStatus, ref byte processStatus, string saleCaseNumber, ref SaleCase sc )
        {
            if (processStatus == controlStatus)    // ok - we have initialised all needed fields.
            {
                sc.processPriceWithVat();
                _saleCasesList.saleCaseAdd(sc);
                sc = new SaleCase();
                processStatus = 0b0000;
            }
            else
            {
                byte absentElement = (byte) (processStatus ^ controlStatus);
                string? elementName;
                byte[] states = { 1, 2, 4, 8 };
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
                            elementName = "neznámý typ";
                            _warningMessagesList.Add( ex.Data.Values.ToString() );
                        }
                        _warningMessagesList.Add($"XmlFileParser: Incomplete saling case omitted: number <{saleCaseNumber}> element <{elementName}>.");
                    }
                }
                processStatus = 0b0000;
                sc = new SaleCase();
            }
        }
    }
}
