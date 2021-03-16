using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
            string outputPath = inputTextInfo.DirectoryName + "\\Output.txt";

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

            object result = null;
            object resultParallel = null;
            long sequentResultTime = 0;
            long parallelResultTime = 0;

            try
            {
                var curDir = Directory.GetCurrentDirectory();
                string projectDirectory = Directory.GetParent(curDir).Parent.Parent.FullName;
                var asmPath = projectDirectory + "\\WordCounter\\bin\\Debug\\WordCounter.dll";

                Assembly asm = Assembly.LoadFrom(asmPath);

                Type t = asm.GetType("WordCounter.WordCounter", true, true);

                object instance = Activator.CreateInstance(t);

                MethodInfo countWords = t.GetMethod("CountUniqueWords", BindingFlags.Instance | BindingFlags.NonPublic 
                                                                        | BindingFlags.Static);
                MethodInfo countWordsParallel = t.GetMethod("CountUniqueWordsParallel", BindingFlags.Instance 
                                                                                        | BindingFlags.Public | BindingFlags.Static);

                Stopwatch timer;

                var numberTimes = 20;

                for (int i = 0; i < numberTimes; i++)
                {
                    timer = Stopwatch.StartNew();
                    result = countWords.Invoke(instance, new object[] { text });
                    timer.Stop();
                    sequentResultTime += timer.ElapsedMilliseconds;

                    timer = Stopwatch.StartNew();
                    resultParallel = countWordsParallel.Invoke(instance, new object[] { text });
                    timer.Stop();
                    parallelResultTime += timer.ElapsedMilliseconds;
                }
                
                sequentResultTime /= numberTimes;
                parallelResultTime /= numberTimes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            var wordsList = (Dictionary<string, int>) result;
            
            Console.WriteLine($"{sequentResultTime} {parallelResultTime}");
            
            var parallelList = (Dictionary<string, int>) resultParallel;

            using (StreamWriter sw = new StreamWriter(outputPath, false, System.Text.Encoding.Default))
            {
                foreach (var entry in parallelList)
                {
                    sw.WriteLine($"{entry.Key} {entry.Value}");
                }

            }

            Console.WriteLine($"Numbers of unique words has written in {outputPath}");
            Console.ReadLine();
        }
    }
}
