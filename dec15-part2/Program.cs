using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        long result = 0;

        OrderedDictionary[] dict_label_focal = new OrderedDictionary[256];
        for (int b = 0; b < 256; b++)
        {
            dict_label_focal[b] = [];
        }

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            string[] inputs = line.Split(',').ToArray();

            foreach (string input in inputs)
            {
                string label = (input.Last() == '-') ? input.Remove(input.Length - 1) : input.Remove(input.Length - 2);
                int boxId = GetHashCode(label);

                // remove
                if (input.Last() == '-')
                {
                    if (dict_label_focal[boxId].Contains(label))
                    {
                        dict_label_focal[boxId].Remove(label);
                    }
                }
                else
                {
                    int focal = input.Last() - '0';

                    if (dict_label_focal[boxId].Contains(label))
                    {
                        dict_label_focal[boxId][label] = focal;
                    }
                    else
                    {
                        dict_label_focal[boxId].Add(label, focal);
                    }
                }

                //result += hash;
                //Console.WriteLine($"{boxId}");
            }
        }

        for (int b = 0; b < 256; b++)
        {
            long value = 0;

            int slot = 0;
            OrderedDictionary dict = dict_label_focal[b];
            foreach (DictionaryEntry label_focal in dict)
            {
                ++slot;

                value += (1 + b) * slot * (int)label_focal.Value;
            }

            result += value;
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int GetHashCode(string input)
    {
        int value = 0;
        foreach (char c in input)
        {
            value += (int)c;
            value *= 17;
            value %= 256;
        }

        return value;
    }
}