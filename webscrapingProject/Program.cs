using CsvHelper;
using Newtonsoft.Json;
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

namespace webscrapingProject

{
    public class VideoResult  // klasse aanmaken om later te gebruiken voor het omzetten naar csv en json files
        // we maken een attribuut title,views,uploader en link aan
    {
        public string Title { get; set; }
        public string Views { get; set; }
        public string Uploader { get; set; }
        public string Link { get; set; }
    }

    
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the YouTube search query: "); // we vragen een zoekterm input om in youtube op te zoeken
            string youtubeZoek = Console.ReadLine(); // we slaan deze zoekterm op

            
            using (var driver = new ChromeDriver()) // we maken een nieuwe chromedriver aan zodat we kunnen scrapen
            {
               
                driver.Navigate().GoToUrl("https://www.youtube.com"); // we navigeren naar youtube
                // we gaan een popup om cookies te accepteren krijgen, daardoor zoeken we de accepteren knop en gaan we hierop klikken
                var acceptCookiesButton = driver.FindElement(By.CssSelector("ytd-button-renderer.style-scope.ytd-consent-bump-v2-lightbox " +
                    "button[aria-label='Akkoord gaan met het gebruik van cookies en andere gegevens voor de beschreven doeleinden']"));
                acceptCookiesButton.Click();
                System.Threading.Thread.Sleep(3000); // we wachten 3 seconden zodat de pagina fatsoenlijk kan herladen


               
                IWebElement zoekInput = driver.FindElement(By.Name("search_query")); // we slaan de input locatie van de zoekbalk op
                zoekInput.SendKeys(youtubeZoek); // we typen de input die we van de gebruiker hadden gevraagd in de zoekbalk en drukken op enter
                zoekInput.Submit();

          
                System.Threading.Thread.Sleep(3000); // we wachten 3 seconden zodat de zoekresultaten kunnen geladen worden

                var filterButton = driver.FindElement(By.CssSelector("#filter-button")); // we zoeken naar de filterknop
                filterButton.Click(); // we klikken op de filterknop
                System.Threading.Thread.Sleep(2000); // we wachten 2 seconden
                // we zoeken naar de filteroptie om op uploaddatum te sorteren
                var sortByDateOption = driver.FindElement(By.CssSelector("[title='Sorteren op uploaddatum']"));
                sortByDateOption.Click();
                // nu hebben we onze meest recente zoekresultaten
                System.Threading.Thread.Sleep(2000);

                // we zoeken de videos met de informatie
                var videoElementen = driver.FindElements(By.CssSelector(".text-wrapper.style-scope.ytd-video-renderer"));

                var youtubeVideos = new List<VideoResult>(); // we maken een lijst aan die we later zullen gebruiken voor het omzetten naar json en csv

                for (int i = 0; i < 5; i++) // we nemen enkel de eerste 5 meest recente resultaten
                {
                    // we slaan de titel, views,uploader en link op door de juiste elementen te vinden
                    var title = videoElementen[i].FindElement(By.CssSelector("#video-title")).Text;
                    var views = videoElementen[i].FindElement(By.CssSelector(".style-scope.ytd-video-meta-block span:first-of-type")).Text;
                    var uploader = videoElementen[i].FindElement(By.CssSelector(".style-scope.ytd-channel-name yt-formatted-string a")).Text;
                    var link = videoElementen[i].FindElement(By.CssSelector("#video-title")).GetAttribute("href");

                    // we printen op onze console deze resultaten af zodat we kunnen zien of we juist bezig zijn
                    Console.WriteLine($"Title: {title}\nViews: {views}\nUploader: {uploader}\nLink: {link}\n");
                    // we voegen onze resultaten toe aan de eerder aangemaakte lijst
                    youtubeVideos.Add(new VideoResult { Title = title, Views = views, Uploader = uploader, Link = link });



                }
                string csvFilePath = "\\users\\thoma\\DevOps\\Project\\webscrapingProject\\youtubeResults.csv"; // we definiëren een pad
                using (var writer = new StreamWriter(csvFilePath)) // we maken een csv file aan met het gedefinieerde pad
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) // we gaan nu aan deze csv file de resultaten van de lijst toevoegen
                {
                    csv.WriteRecords(youtubeVideos);
                }

                // we zetten onze lijst met resultaten om in een json file
                string jsonResults = System.Text.Json.JsonSerializer.Serialize(youtubeVideos, new JsonSerializerOptions { WriteIndented = true });

                // we schrijven onze json resulaten over in de volgen file met het gedefinieerde pad
                File.WriteAllText("\\users\\thoma\\DevOps\\Project\\webscrapingProject\\YouTubeResults.json", jsonResults);

               




            }
        }
  

    }
}
