using HtmlAgilityPack;
using System.Globalization;
using System.Text;

namespace Webcrawler
{
    public class Program
    {
        // Defining a custom class to store the scraped data 
        public class Text
        {
            public string? Description { get; set; }
        }

        public static void Main()
        {
            // Creating the list that will keep the scraped data 
            var products = new List<Text>();
            var descriptionsSet = new HashSet<string>();

            // Creating the HAP object 
            var web = new HtmlWeb();
            web.OverrideEncoding = Encoding.UTF8;

            // Base URL without page parameter
            string baseUrl = "https://flatfy.ua/uk/%D0%BF%D1%80%D0%BE%D0%B4%D0%B0%D0%B6-%D0%BA%D0%B2%D0%B0%D1%80%D1%82%D0%B8%D1%80-%D0%BB%D1%8C%D0%B2%D1%96%D0%B2";


            for (int page = 1; page <= 40; page++)
            {
                // Construct the URL for the current page
                string url = $"{baseUrl}?page={page}";

                // Visiting the target web page 
                var document = web.Load(url);

                // Getting the list of HTML product nodes using XPath 
                var productHTMLElements = document.DocumentNode.SelectNodes("//div[contains(@class, 'feed-layout__item')]");

                if (productHTMLElements == null || productHTMLElements.Count == 0)
                {
                    // If no more products found, exit the loop
                    break;
                }

                // Iterating over the list of product HTML elements 
                foreach (var productHTMLElement in productHTMLElements)
                {
                    var descriptionNode = productHTMLElement.SelectSingleNode(".//p");

                    if (descriptionNode != null)
                    {
                        // Remove extra whitespace and new lines
                        var description = descriptionNode.InnerText
                            .Replace("\r", " ")  // Remove carriage return
                            .Replace("\n", " ")  // Remove newline
                            .Replace("  ", " "); // Remove double spaces 

                        // Check if the description is unique before adding
                        if (!descriptionsSet.Contains(description))
                        {
                            descriptionsSet.Add(description);
                            var product = new Text() { Description = description };
                            products.Add(product);
                        }
                    }
                }
            }

            // Creating the TXT output file 
            using (var writer = new StreamWriter("sentences.txt"))
            {
                foreach (var product in products)
                {
                    // Write each description in a single line
                    writer.WriteLine(product.Description);
                }
            }
        }
    }
}