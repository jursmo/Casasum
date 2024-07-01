using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casasum.model
{
    sealed public class SaleCasesList
    {
        private List<SaleCase>? _saleCasesList;
        // bool initialised?

        public SaleCasesList() { }
        public SaleCasesList(List<SaleCase> salesList) { _saleCasesList = salesList; }

        public List<SaleCase> SaleCaseList { get => _saleCasesList; set => _saleCasesList = value; }
        public dynamic getSalesQuery( int saleTime )
        {
            if( saleTime == ( int ) controller.Constants.SaleTime.WeekendSale )
            {
                return from saleCase in _saleCasesList where saleCase.WeekendSale == true select new { saleCase };
            }
            else if( saleTime == ( int ) controller.Constants.SaleTime.WorkWeekSale)
            {
                return from saleCase in _saleCasesList where saleCase.WorkWeekSale == true select new { saleCase };
            }
            else if( saleTime == ( int ) controller.Constants.SaleTime.AllSales )
            {
                return from saleCase in _saleCasesList where saleCase.WorkWeekSale == true select new { saleCase };
            }
            // else throw something???

            return null;
        }
        public List<SaleCase> SaleCasesListInit { set => _saleCasesList = value; }
    }
}
