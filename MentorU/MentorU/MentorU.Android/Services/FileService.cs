using Android.OS;
using MentorU.Droid.Services;
using MentorU.Services;
using System;
using System.IO;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(FileService))]
namespace MentorU.Droid.Services
{
    public class FileService : IFileService
    {
        public string SavePicture(string name, Stream data, string location = "temp")
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, "Orders", location);
            Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, name);

            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            byte[] bArray = new byte[data.Length];
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (data)
                {
                    data.Read(bArray, 0, (int)data.Length);
                }
                int length = bArray.Length;
                fs.Write(bArray, 0, length);
            }

            return filePath;
        }

        public string SavePDF(string name, byte[] data, string location = "temp")
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, "Orders", location);
            Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, name);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                int length = data.Length;
                fs.Write(data, 0, length);
            }

            return filePath;
        }
    }
}