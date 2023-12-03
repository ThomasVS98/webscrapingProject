using CsvHelper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ictjobScraping
{
    public class ictJobResult  // klasse aanmaken om later te gebruiken voor het omzetten naar csv en json files
                               // we maken een attribuut titel,bedrijf,locatie,keywords en link aan
    {
        public string Titel { get; set; }
        public string Bedrijf { get; set; }
        public string Locatie { get; set; }
        public string Keywords { get; set; }
        public string Link { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the job search query: "); // we vragen een zoekterm input om in ictjob op te zoeken
            string jobZoek = Console.ReadLine(); // we slaan deze zoekterm op


            using (var driver = new ChromeDriver()) // we maken een chromedriver aan
            {
                driver.Navigate().GoToUrl("https://www.ictjob.be/nl/"); // we geven aan welke site we willen bezoeken
                IWebElement cookie = driver.FindElement(By.CssSelector(".cookie-layer-button")); // we zoeken de knop om een cookie te accepteren
                cookie.Click(); // we klikken op de accepteren knop

                IWebElement zoekInput = driver.FindElement(By.CssSelector("#keywords-input")); // we zoeken naar de zoekbalk
                zoekInput.SendKeys(jobZoek); // we geven daar onze gevraagde input in
                zoekInput.Submit(); // we klikken enter

                System.Threading.Thread.Sleep(20000); // we wachten even 20s zodat de site kan laden
                              
                var jobList = new List<ictJobResult>(); // we maken een job lijst aan die we later zullen gebruiken voor het omzetten naar json en csv

                IWebElement datumSort = driver.FindElement(By.CssSelector("#sort-by-date"));
                datumSort.Click();
                System.Threading.Thread.Sleep(10000); // we wachten 10s

                var jobElementen = driver.FindElements(By.CssSelector(".search-item.clearfix")); // we zoeken nu de lijst met jobs


                System.Threading.Thread.Sleep(5000); // we wachten 5s

                for (int i = 0; i < 5; i++) // voor de eerste 5 jobs printen we de titel,bedrijf,locatie,link en keywords
                {
            
                    string title = jobElementen[i].FindElement(By.CssSelector("h2")).Text;
                    string bedrijf = jobElementen[i].FindElement(By.CssSelector(".job-company")).Text;
                    string locatie = jobElementen[i].FindElement(By.CssSelector("[itemprop='addressLocality']")).Text;
                    string link = jobElementen[i].FindElement(By.CssSelector(".search-item-link")).GetAttribute("href");
                    IWebElement keyword = null; // we zetten eerst keyword op null aangezien we soms geen keywords kunnen hebben bij een job
                    try // we proberen keywords te zoeken
                    {
                        keyword = jobElementen[i].FindElement(By.CssSelector(".job-keywords"));
                    }
                    catch
                    {

                    }
                    // als er keywords zijn dan nemen we die op, anderes is het een lege string
                    string keywords = keyword != null ? keyword.Text : "";


                    // we voegen alles toe aan de lijst
                    jobList.Add(new ictJobResult { Titel = title, Bedrijf = bedrijf,Locatie=locatie,Keywords=keywords,Link = link });
                }

                string csvFilePath = "\\users\\thoma\\DevOps\\Project\\ictjobScraping\\jobResults.csv"; // we definiëren een pad
                using (var writer = new StreamWriter(csvFilePath)) // we maken een csv file aan met het gedefinieerde pad
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) // we gaan nu aan deze csv file de resultaten van de lijst toevoegen
                {
                    csv.WriteRecords(jobList);
                }

                // we zetten onze lijst met resultaten om in een json file
                string jsonResults = System.Text.Json.JsonSerializer.Serialize(jobList, new JsonSerializerOptions { WriteIndented = true });

                // we schrijven onze json resulaten over in de volgen file met het gedefinieerde pad
                File.WriteAllText("\\users\\thoma\\DevOps\\Project\\ictjobScraping\\jobResults.json", jsonResults);





            }
        }
    }
}
