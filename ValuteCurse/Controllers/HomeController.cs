using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using ValuteCurse.Models;
using ValuteCurse.RatesCBR; //добавлена ссылка на службу https://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx?WSDL

namespace ValuteCurse.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Получить список кодов валют
        /// </summary>
        /// <returns></returns>
    public List<CurrencyFromDate> GetEnumCurrencies()
        {
            try
            {
                List<CurrencyFromDate> result = new List<CurrencyFromDate>();
                List<string> vName = new List<string>();
                List<string> vCode = new List<string>();
                DailyInfoSoapClient RatesCBR_Data = new DailyInfoSoapClient();
                XmlNode data = RatesCBR_Data.EnumValutesXML(false); //получаем данные с веб-сервиса http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx
                foreach (XmlNode valute in data)
                {
                    result.Add(new CurrencyFromDate()
                    {
                        CurrencyName = valute["Vname"].InnerText.TrimEnd(),
                        CurrencyCode = valute["Vcode"].InnerText.TrimEnd(),
                    });
                }
                return result;
            }
            catch (Exception e)
            {
                string MethodName = MethodBase.GetCurrentMethod().ToString();
                WriteLog(e.Message, MethodName);
                return null;
            }
        }

        /// <summary>
        /// Получить курс валют на сегодняшний день
        /// </summary>
        /// <returns></returns>
        public List<Currency> GetCursOnDate()
        {
            try
            {
                List<Currency> result = new List<Currency>();
                DailyInfoSoapClient RatesCBR_Data = new DailyInfoSoapClient();
                XmlNode data = RatesCBR_Data.GetCursOnDateXML(DateTime.Now); //получаем данные с веб-сервиса http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx
                foreach (XmlNode valute in data)
                {
                    result.Add(new Currency() //заполняем список данными из запроса
                    {
                        CurrencyName = valute["Vname"].InnerText,
                        CurrencyStringCode = valute["VchCode"].InnerText,
                        Nominal = Convert.ToInt32(valute["Vnom"].InnerText),
                        ExchangeRate = valute["Vcurs"].InnerText,
                    });
                }
                return result;
            }
            catch (Exception e)
            {
                string MethodName = MethodBase.GetCurrentMethod().ToString();
                WriteLog(e.Message, MethodName);
                return null;
            }
        }

        /// <summary>
        /// Получить динамику выбранной валюты по дате
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endTime"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public List<DynamicCurrency> GetCursDynamic(DateTime startDate, DateTime endDate, string currencyCode)
        {
            try
            {
                List<DynamicCurrency> result = new List<DynamicCurrency>();
                DailyInfoSoapClient RatesCBR_Data = new DailyInfoSoapClient();
                XmlNode data = RatesCBR_Data.GetCursDynamicXML(startDate, endDate, currencyCode); //получаем данные с веб-сервиса http://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx
                foreach (XmlNode valute in data)
                {
                    result.Add(new DynamicCurrency()
                    {
                        RatesDate = Convert.ToDateTime(valute["CursDate"].InnerText),
                        Nominal = Convert.ToInt32(valute["Vnom"].InnerText),
                        ExchangeRate = valute["Vcurs"].InnerText,
                    });
                }
                return result;
            }
            catch (Exception e)
            {
                string MethodName = MethodBase.GetCurrentMethod().ToString();
                WriteLog(e.Message, MethodName);
                return null;
            }
        }

        /// <summary>
        /// Домашняя страница
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try
            {
                ViewBag.Currency = GetCursOnDate(); //записываем список с курсами за сегодняшний день
                return View();
            }
            catch (Exception e)
            {
                string MethodName = MethodBase.GetCurrentMethod().ToString();
                WriteLog(e.Message, MethodName);
                return HttpNotFound();
            }
        }

        /// <summary>
        /// Динамика котировок
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Rates()
        {
            try
            {
                ViewBag.CurrencyFromDate = GetEnumCurrencies(); //Получаем коды валют
                return View();
            }
            catch (Exception e)
            {
                string MethodName = MethodBase.GetCurrentMethod().ToString();
                WriteLog(e.Message, MethodName);
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Rates(CurrencyFromDate currency)
        {
             try
             {
                 ViewBag.CurrencyFromDate = GetEnumCurrencies();//Получаем коды валют
                 ViewBag.DynamicCurrency = GetCursDynamic(currency.StartDate, currency.EndDate, currency.CurrencyCode);//Получаем динамику курса выбранной валюты за определенный период 
                 if (Request.IsAjaxRequest())//если ajax запрос
                 {
                    if (ViewBag.DynamicCurrency.Count != 0)//есть ли данные о курсе
                        return PartialView("RatesView");
                    else
                        return PartialView("RatesNoneView");
                 }
                 else
                     return View();
             }
            catch (Exception e)
            {
                string MethodName = MethodBase.GetCurrentMethod().ToString();
                WriteLog(e.Message, MethodName);
                return HttpNotFound();
             }
            
        }

        //логирование ошибок
        static void WriteLog(string exeption, string MethodName)
        {
            StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\error.log", true, System.Text.Encoding.Default);
            sw.WriteLine(DateTime.Now + " - Ошибка в методе: " + MethodName + ": " + exeption + Environment.NewLine);
            sw.Dispose();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}