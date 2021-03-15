﻿using MentorU.iOS.Services;
using MentorU.Services;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileService))]
namespace MentorU.iOS.Services
{
    public class FileService: IFileService
    {
        public string SavePicture(string name, Stream data, string location = "temp")
        {

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            documentsPath = Path.Combine(documentsPath, "Orders", location);
            Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, name);

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
    }
}