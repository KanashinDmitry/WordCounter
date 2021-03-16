using System;
using System.Collections.Generic;
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

            object result;

            try
            {
                var curDir = Directory.GetCurrentDirectory();
                string projectDirectory = Directory.GetParent(curDir).Parent.Parent.FullName;
                var asmPath = projectDirectory + "\\WordCounter\\bin\\Debug\\WordCounter.dll";

                Assembly asm = Assembly.LoadFrom(asmPath);

                Type t = asm.GetType("WordCounter.WordCounter", true, true);

                object instance = Activator.CreateInstance(t);

                MethodInfo method = t.GetMethod("CountUniqueWords", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);

                result = method.Invoke(instance, new object[] { text });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            var words_list = (List<KeyValuePair<string, int>>) result;

            using (StreamWriter sw = new StreamWriter(outputPath, false, System.Text.Encoding.Default))
            {
                foreach (var entry in words_list)
                {
                    sw.WriteLine($"{entry.Key} {entry.Value}");
                }

            }

            Console.WriteLine($"Numbers of unique words has written in {outputPath}");
            Console.ReadLine();
        }
    }
}
