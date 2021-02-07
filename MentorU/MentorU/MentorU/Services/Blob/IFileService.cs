using System.IO;

namespace MentorU.Services
{
    public interface IFileService
    {
        string SavePicture(string name, Stream data, string location = "temp");
    }
}
