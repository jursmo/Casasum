using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casasum.controller
{
    sealed public class AppLogicSeparator
    {
        //private model.SaleCasesList saleCasesList = new();
        private SeparatorOut separatorOutput;

        public class SeparatorOut
        {
            private model.SaleCasesList saleCasesList = new();
            private List< string > sumPrintQueue = new();

            public model.SaleCasesList SaleCasesList { get => saleCasesList; set => saleCasesList = value; }
            public List< string > SumPrintQueue { get => sumPrintQueue; set => sumPrintQueue = value; }
        }

        public SeparatorOut SeparatorOutput { get => separatorOutput; }

        public void processXmlFile( string pathToXml )
        {

            separatorOutput = new();
            model.XmlFileParser xmlFileParser = new( pathToXml );
            separatorOutput.SaleCasesList.SaleCasesListInit = xmlFileParser.SaleCasesList;
            summarize( (int) Constants.SaleTime.WeekendSale );

        }

        public void summarize( Constants.SaleTime saleTime )
        {
            Dictionary< string, Dictionary<string, double >> saleSummary = model.Summarizer.saleSum( separatorOutput.SaleCasesList.getSalesQuery( ( int ) saleTime ));

            StringBuilder str = new();
            List< string > printQueue = new List< string >();
            foreach( var model in saleSummary.Keys )
            {
                str.Append( model + "\n " );
                string multipurposeString = saleSummary[ model ][ "priceWithVat" ].ToString( "N0" );
                int space = 25 - multipurposeString.Length;
                string spacebar = new( ' ', space );
                str.Append( multipurposeString + spacebar + saleSummary[ model ][ "priceWoVat" ].ToString( "N0" ));

                separatorOutput.SumPrintQueue.Add( str.ToString() );
                str = new();
            }
        }
    }
}
