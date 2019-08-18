using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FilmLister.Service
{
    public class ChoicesRemainingService
    {
        private Dictionary<int, int> choices = new Dictionary<int, int>();
        
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
