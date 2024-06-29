using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casasum.model
{
    public static class Summarizer
    {
                public static Dictionary<string, Dictionary<string, double>>  saleSum( dynamic linqQuery )
        {
            Dictionary< string, Dictionary< string, double > > _saleSummary = new Dictionary< string, Dictionary< string, double > >();
            var items = linqQuery;
            foreach ( var item in items )
            {
                if ( _saleSummary.ContainsKey( item.saleCase.Model ) )
                {
                    _saleSummary[item.saleCase.Model]["priceWoVat"] += item.saleCase.PriceWoVat;
                    _saleSummary[item.saleCase.Model]["priceWithVat"] += item.saleCase.PriceWithVat;
                }
                else
                {
                    _saleSummary.Add( item.saleCase.Model, new Dictionary<string, double>() );
                    _saleSummary[ item.saleCase.Model ].Add( "priceWoVat",   item.saleCase.PriceWoVat   );
                    _saleSummary[ item.saleCase.Model ].Add( "priceWithVat", item.saleCase.PriceWithVat );
                }
            }
            return _saleSummary;
        }
    }
}
