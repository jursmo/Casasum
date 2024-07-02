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
            private model.SaleCasesList _saleCasesList   = new();
            private List< string > _weekendSumPrintQueue = null;
            private List<string>? _workWeekSumPrintQueue = null;    // it could remain uninitialised - it is calculate only for demand.
            private List<string>? _allDaysSumPrintQueue  = null;    // it could remain uninitialised - it is calculate only for demand.

            public model.SaleCasesList SaleCasesList   { get => _saleCasesList;         set => _saleCasesList         = value; }
            public List< string > WeekendSumPrintQueue { get => _weekendSumPrintQueue;  set => _weekendSumPrintQueue  = value; }
            public List<string> WorkWeekSumPrintQueue  { get => _workWeekSumPrintQueue; set => _workWeekSumPrintQueue = value; }
            public List<string> AllDaysSumPrintQueue   { get => _allDaysSumPrintQueue;  set => _allDaysSumPrintQueue  = value; }

            public bool isItNull( Constants.SaleTime saleTime)
            {
                bool outputValue = false;

                if( saleTime == Constants.SaleTime.WorkWeekSale )
                {
                    if( _workWeekSumPrintQueue == null )
                    {
                        outputValue = true;
                    }
                }
                else if( saleTime == Constants.SaleTime.AllSales )
                {
                    if( _allDaysSumPrintQueue == null )
                    {
                        outputValue = true;
                    }
                }
                else if( saleTime == Constants.SaleTime.WeekendSale )
                {
                    if( _weekendSumPrintQueue == null )
                    {
                        outputValue = true;
                    }    
                }
                return outputValue;
            }
        }

        public SeparatorOut SeparatorOutput { get => separatorOutput; }

        public void processXmlFile( string pathToXml )
        {

            separatorOutput = new();
            model.XmlFileParser xmlFileParser = new( pathToXml );
            separatorOutput.SaleCasesList.SaleCasesListInit = xmlFileParser.SaleCasesList;
            summarize( /*(int)*/ Constants.SaleTime.WeekendSale );          //it is calculated in default - it is calculate automatically therefore

        }

        public void summarize( Constants.SaleTime saleTime )
        {
            if ( separatorOutput.isItNull(saleTime) )
            {
                if (saleTime == Constants.SaleTime.WeekendSale)       { separatorOutput.WeekendSumPrintQueue  = new(); }
                else if (saleTime == Constants.SaleTime.WorkWeekSale) { separatorOutput.WorkWeekSumPrintQueue = new(); }
                else if (saleTime == Constants.SaleTime.AllSales)     { separatorOutput.AllDaysSumPrintQueue  = new(); }
                Dictionary<string, Dictionary<string, double>> saleSummary = model.Summarizer.saleSum(separatorOutput.SaleCasesList.getSalesQuery(/*(int)*/saleTime));

                StringBuilder str = new();
                List<string> printQueue = new List<string>();
                foreach (var model in saleSummary.Keys)
                {
                    str.Append(model + "\n ");
                    string multipurposeString = saleSummary[model]["priceWithVat"].ToString("N0");
                    int space = 25 - multipurposeString.Length;
                    string spacebar = new(' ', space);
                    str.Append(multipurposeString + spacebar + saleSummary[model]["priceWoVat"].ToString("N0"));

                    if (saleTime == Constants.SaleTime.WeekendSale)
                    {
                        separatorOutput.WeekendSumPrintQueue.Add(str.ToString());
                    }
                    else if( saleTime == Constants.SaleTime.WorkWeekSale )
                    {
                        separatorOutput.WorkWeekSumPrintQueue.Add(str.ToString());
                    }
                    else if( saleTime == Constants.SaleTime.AllSales )
                    {
                        separatorOutput.AllDaysSumPrintQueue.Add(str.ToString());
                    }    
                    str = new();
                }
            }
        }
    }
}
