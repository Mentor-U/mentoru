using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MentorU.Services.DatabaseServices
{
    public class ClassList
    {
        public List<string> classList { get; set; }
        public ClassList()
        {
            classList = new List<string>();
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(LoadResourceText)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("MentorU.Services.DatabaseServices.dep_abvs.csv");
            using (var reader = new StreamReader(stream))
            {
                string line;
                while((line=reader.ReadLine())!= null)
                { 
                    classList.Add(reader.ReadLine());
                }

            }
            classList.Sort();
            classList[0] = "None";
        }

        public bool Contains(string val)
        {
            return classList.Contains(val);
        }

        // Dont ask why but its needed
        private class LoadResourceText
        {
        }

        // TODO: add edit distance checker here
    }
}
