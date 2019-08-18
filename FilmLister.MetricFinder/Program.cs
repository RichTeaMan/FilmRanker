using FilmLister.Service;
using FilmLister.Service.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RichTea.CommandLineParser;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.MetricFinder
{
    public class Program
    {
        private static int Main(string[] args)
        {
            return ParseCommand(args);
        }

        private static int ParseCommand(string[] args)
        {
            MethodInvoker command = null;
            try
            {
                command = new CommandLineParserInvoker().GetCommand(typeof(Program), args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing command:");
                Console.WriteLine(ex);
            }
            if (command != null)
            {
                try
                {
                    command.Invoke();
                    return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error running command:");
                    Console.WriteLine(ex);

                    var inner = ex.InnerException;
                    while (inner != null)
                    {
                        Console.WriteLine(inner);
                        Console.WriteLine();
                        inner = inner.InnerException;
                    }

                    Console.WriteLine(ex.StackTrace);

                    return 1;
                }
            }
            return -1;
        }

        [DefaultClCommand]
        private static void CalculateListChoiceCount([ClArgs("outpath", "o")]string outPath = "choices.json")
        {
            Console.WriteLine($"FilmList metric finder.");

            int maxSetLength = 200;
            int attempts = 100;

            Console.WriteLine($"Finding list decision numbers up to {maxSetLength}.");

            var results = new ConcurrentDictionary<int, int>();

            Parallel.ForEach(Enumerable.Range(0, maxSetLength), setLength =>
            {
                var setComparisions = new ConcurrentBag<int>();
                Parallel.ForEach(Enumerable.Range(0, attempts), s =>
                {

                    var orderService = new OrderService();

                    var integerComparableDictionary = new Dictionary<int, IntegerAbstractComparable>();
                    foreach (var i in Enumerable.Range(0, setLength))
                    {
                        integerComparableDictionary.Add(i, new IntegerAbstractComparable(i));
                    }

                    foreach (var kv in integerComparableDictionary)
                    {
                        if (integerComparableDictionary.TryGetValue(kv.Key + 1, out IntegerAbstractComparable greater))
                        {
                            kv.Value.AddHigherRankedObject(greater);
                        }
                    }

                    var integerComparableList = integerComparableDictionary.Values.ToArray();
                    var orderedIntegerComparableSortResult = orderService.OrderObjects(integerComparableList);

                    var expectedIntegerComparableList = integerComparableList.OrderBy(i => i.Value).ToArray();

                    // asserts
                    Assert.IsTrue(orderedIntegerComparableSortResult.Completed);
                    CollectionAssert.AreEquivalent(expectedIntegerComparableList, orderedIntegerComparableSortResult.SortedResults.ToArray());

                    int comparisons = orderedIntegerComparableSortResult.SortedResults.Sum(i => i.Comparisons);
                    setComparisions.Add(comparisons);
                });

                var roundedComparisons = (int)Math.Round(setComparisions.Average());
                results.TryAdd(setLength, roundedComparisons);
                
                if (results.Count % 10 == 0)
                {
                    Console.WriteLine($"{results.Count} lists completed.");
                }
            });

            var sortedResult = results.OrderBy(r => r.Key).Select(kv => kv.Value).ToArray();
            var json = JsonConvert.SerializeObject(sortedResult, Formatting.Indented);
            File.WriteAllText(outPath, json);

            Console.WriteLine($"Metric finding complete.");
        }
    }
}
