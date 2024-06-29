namespace Casasum.model
{
    sealed public class SaleCase
    {
        private string   _model;
        private DateTime _date;
        private double   _vat;
        private double   _priceWoVat;
        private double   _priceWithVat;
        private bool     _weekendSale;
        private bool     _workWeekSale;

        public string Model { get => _model; set => _model = value; }
        public DateTime Date
        {
            get { return _date.Date; }
            set
            {
                _date = value;
                if (_date.DayOfWeek == DayOfWeek.Saturday || _date.DayOfWeek == DayOfWeek.Sunday) { _weekendSale = true; }
                else { _workWeekSale = true; }
            }
        }
        public double Vat { get => _vat; set => _vat = value; }
        public double PriceWoVat { get => _priceWoVat; set => _priceWoVat = value; }
        public double PriceWithVat { get => _priceWithVat; }
        public bool WeekendSale { get => _weekendSale; }
        public bool WorkWeekSale { get => _workWeekSale; }
        public void processPriceWithVat()
        {
            double vatValue = _priceWoVat * _vat / 100;
            _priceWithVat = _priceWoVat + vatValue;
        }
    }
}
