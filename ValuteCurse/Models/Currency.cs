using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ValuteCurse.Models
{
    public class Currency
    {
        //Буквенный код валюты
        public string CurrencyStringCode { get; set; }

        // Количество единиц
        public int Nominal { get; set; }

        //Название валюты
        public string CurrencyName { get; set; }

        //Курс
        public string ExchangeRate { get; set; }
    }

    public class CurrencyFromDate
    {
        //Название валюты
        public string CurrencyName { get; set; }

        //Цифровой код валюты
        public string CurrencyCode { get; set; }
        
        //Дата начала
        public DateTime StartDate { get; set; }
        
        //Дата окончания
        public DateTime EndDate { get; set; }
    }

    public class DynamicCurrency
    {
        //дата курса
        public DateTime RatesDate { get; set; }

        //Количество единиц
        public int Nominal { get; set; }

        //Курс
        public string ExchangeRate { get; set; }
    }
}