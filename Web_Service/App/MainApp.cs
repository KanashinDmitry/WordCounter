using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App
{
    class MainApp
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path to the text file");
            string inputTextPath = Console.ReadLine();
            var inputTextInfo = new FileInfo(inputTextPath);
            string outputPath = inputTextInfo.DirectoryName + "\\Output_Tolstoy.txt";

            var text = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(inputTextPath, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        text.Add(line);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The file was not found:\n '{e}'");
                return;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine($"The directory was not found:\n '{e}'");
                return;
            }
            catch (IOException e)
            {
                Console.WriteLine($"The file could not be opened:\n '{e}'");
                return;
            }

            Dictionary<string, int> wordList = null;

            using (var client = new ServiceReference1.Service1Client())
            {
                wordList = client.CountUniqueWords(text.ToArray());
            }


            using (StreamWriter sw = new StreamWriter(outputPath, false, System.Text.Encoding.Default))
            {
                foreach (var entry in wordList)
                {
                    sw.WriteLine($"{entry.Key} {entry.Value}");
                }

            }

            Console.WriteLine($"Numbers of unique words has written in {outputPath}");
        }
    }
}
