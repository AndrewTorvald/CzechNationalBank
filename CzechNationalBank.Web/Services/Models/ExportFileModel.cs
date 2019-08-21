using System.IO;

namespace CzechNationalBank.Web.Services.Models
{
    public class ExportFileModel
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
    }
}