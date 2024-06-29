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
        public dynamic weekendSaleQuery()
        {
            return from saleCase in _saleCasesList where saleCase.WeekendSale == true select new { saleCase }; //linqQuery;
        }
        public List<SaleCase> SaleCasesListInit { set => _saleCasesList = value; }
    }
}
