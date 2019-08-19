using FilmLister.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmLister.Service
{
    public class ChoicesRemainingService
    {
        private readonly OrderService orderService;

        private Dictionary<int, int> choices = new Dictionary<int, int>();

        public ChoicesRemainingService(OrderService orderService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async Task LoadChoicesFromJson(string jsonFilePath)
        {
            string json = await File.ReadAllTextAsync(jsonFilePath);
            var choiceArray = JsonConvert.DeserializeObject<int[]>(json);

            choices = new Dictionary<int, int>();
            for (int i = 0; i < choiceArray.Length; i++)
            {
                choices.Add(i, choiceArray[i]);
            }
        }

        public int? FindChoicesRemaining(OrderedFilmRank orderedFilmRank)
        {
            int choicesRemaining = 0;
            var sw = new Stopwatch();
            sw.Start();
            if (!orderedFilmRank.Completed)
            {
                var cloned = orderedFilmRank.Clone();
                var random = new Random();

                var sortResult = orderService.OrderFilms(orderedFilmRank.SortedFilms.Where(f => !f.Ignore));
                do
                {
                    if (random.Next() % 2 == 0)
                    {
                        sortResult.LeftSort.AddHigherRankedObject(sortResult.RightSort);
                    }
                    else
                    {
                        sortResult.RightSort.AddHigherRankedObject(sortResult.LeftSort);
                    }
                    sortResult = orderService.OrderFilms(sortResult.SortedResults.Where(f => !f.Ignore));
                    choicesRemaining++;
                } while (!sortResult.Completed);
            }
            sw.Stop();
            Console.WriteLine($"Choices {choicesRemaining} | Seconds {sw.Elapsed.TotalSeconds}");
            return choicesRemaining;
        }


        [Obsolete]
        public int? FindChoicesRemaining(int listLength)
        {
            int? result = null;

            int foundChoice;
            if (choices.TryGetValue(listLength, out foundChoice))
            {
                result = foundChoice;
            }

            return result;
        }
    }
}
