using System;
using System.Collections.Generic;
using System.Linq;

namespace WordCounter
{
    public class WordCounter
    {
        private static Dictionary<string, int> CountUniqueWords(IEnumerable<string> text)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            Dictionary<string, int> words_dict = new Dictionary<string, int>(comparer);

            foreach (string line in text)
            {
                var punctuation = line.Where(Char.IsPunctuation).Distinct().ToArray();
                var words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim(punctuation));

                foreach (string word in words)
                {
                    if (word == "" || int.TryParse(word, out _))
                        continue;

                    if (words_dict.ContainsKey(word))
                        words_dict[word]++;
                    else
                        words_dict.Add(word, 1);
                }
            }

            var words_list = words_dict.ToList();
            words_list.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            return words_list.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
    }
}
