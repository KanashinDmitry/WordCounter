using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Hosting;

namespace WcfService
{
    public class Service1 : IService1
    {
        public Dictionary<string, int> CountUniqueWords(IEnumerable<string> text)
        {
            object result;

            try
            {
                var curDir = HostingEnvironment.ApplicationPhysicalPath;
                string projectDirectory = Directory.GetParent(curDir).Parent.FullName;
                var asmPath = projectDirectory + "WordCounter\\bin\\Debug\\WordCounter.dll";

                Assembly asm = Assembly.LoadFrom(asmPath);

                Type t = asm.GetType("WordCounter.WordCounter", true, true);

                object instance = Activator.CreateInstance(t);

                MethodInfo countWordsParallel = t.GetMethod("CountUniqueWordsParallel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

                result = countWordsParallel.Invoke(instance, new object[] { text });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Dictionary<string, int>();
            }

            return (Dictionary<string, int>)result;
        }
    }
}
