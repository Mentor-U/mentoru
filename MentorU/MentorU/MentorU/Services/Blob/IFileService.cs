using System.IO;

namespace MentorU.Services
{
    public interface IFileService
    {
        string SavePicture(string name, Stream data, string location = "temp");
        string SavePDF(string name, byte[] data, string location = "temp");
    }
}
