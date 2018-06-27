 class Program
    {
        static void Main(string[] args)
        {
            Task.Run(DoLoop);
            Console.ReadLine();

        }

        private static async Task DoLoop()
        {
            while (true)
            {
                Console.Clear();
                Console.ResetColor();

                HttpClient client = new HttpClient();
                var page = await client.GetAsync(
                    "http://outlet.euro.dell.com/Online/InventorySearch.aspx?brandid=7&c=uk&cs=ukdfh1&l=en&s=dfh&frid=144&~ck=mn");

                var content = await page.Content.ReadAsStringAsync();
                //
                var matches = Regex.Matches(content,
                    "<div class=\"fl-config-desc-container\\d\">(.|\\n)*?\\r\\s*</div>", RegexOptions.Multiline);

                var newXpsList = matches.Where(m => m.Value.Contains("9370")).Select(m => m.Value);

                Console.WriteLine($"{newXpsList.Count()} Matches ({matches.Count} Total) @ {DateTime.Now}");

                Console.WriteLine();

                foreach (var newXps in newXpsList)
                {

                    var details = Regex.Match(newXps,
                        ">(XPS.*?)<.*?(\\d+\\s?GB).*?(\\d+\\s?GB).*?(....\\(\\d+\\s?x\\s?\\d+\\).*?)</div>", RegexOptions.Singleline);

                    if (details.Groups[4].Value.Contains("1080"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.WriteLine(
                        $"{details.Groups[1]} | {details.Groups[2]} | {details.Groups[3]} | {details.Groups[4]}");

                }

                await Task.Delay(60000);
            }



            //"\"fl - config - desc - container0\">"
        }
    }