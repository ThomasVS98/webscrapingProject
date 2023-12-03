using CsvHelper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace goodreadsScraping
{
    public class goodreadsResult  // klasse aanmaken om later te gebruiken voor het omzetten naar csv en json files
                               // we maken een attribuut titel,auteur,rating aan
    {
        public string Titel { get; set; }
        public string Auteur { get; set; }
        public string Rating { get; set; }
  
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the goodreads search: "); // we vragen een zoekterm input om in goodreads op te zoeken
            string boekSearch = Console.ReadLine(); // we slaan deze zoekterm op

            using (var driver = new ChromeDriver()) // we maken een chromedriver aan
            {
                driver.Navigate().GoToUrl("https://www.goodreads.com/"); // we geven aan welke site we willen bezoeken
                System.Threading.Thread.Sleep(3000); // we wachten 3 seconden zodat de site geladen is
                IWebElement zoekInput = driver.FindElement(By.Name("query")); // we zoeken naar de zoekbalk
                zoekInput.SendKeys(boekSearch); // we geven daar onze gevraagde input in
                zoekInput.Submit(); // we klikken enter

                System.Threading.Thread.Sleep(5000); // we wachten 5 seconden zodat de resultaten zichtbaar zijn voor we scrapen                                                    
              

                var bookkList = new List<goodreadsResult>(); // we maken een job lijst aan die we later zullen gebruiken voor het omzetten naar json en csv             
                var bookElements = driver.FindElements(By.CssSelector("[itemtype='http://schema.org/Book']")); // we zoeken nu de lijst met boek resultaten

                for (int i = 0; i < 3; i++) // voor de eerste 3 boeken printen we de titel,auteur,rating
                {
                    string title = bookElements[i].FindElement(By.CssSelector(".bookTitle")).Text;
                    string auteur = bookElements[i].FindElement(By.CssSelector(".authorName")).Text;
                    string rating = bookElements[i].FindElement(By.CssSelector(".minirating")).Text;
                    

                    // we voegen alles toe aan de lijst              
                    bookkList.Add(new goodreadsResult { Titel = title, Auteur = auteur, Rating = rating });
                }

                string csvFilePath = "\\users\\thoma\\DevOps\\Project\\goodreadsScraping\\bookResults.csv"; // we definiëren een pad
                using (var writer = new StreamWriter(csvFilePath)) // we maken een csv file aan met het gedefinieerde pad
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) // we gaan nu aan deze csv file de resultaten van de lijst toevoegen
                {
                    csv.WriteRecords(bookkList);
                }

                // we zetten onze lijst met resultaten om in een json file
                string jsonResults = System.Text.Json.JsonSerializer.Serialize(bookkList, new JsonSerializerOptions { WriteIndented = true });

                // we schrijven onze json resulaten over in de volgen file met het gedefinieerde pad
                File.WriteAllText("\\users\\thoma\\DevOps\\Project\\goodreadsScraping\\bookResults.json", jsonResults);

            }

            }
    }
}
