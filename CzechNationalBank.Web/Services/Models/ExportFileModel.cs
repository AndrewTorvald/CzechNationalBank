using System.IO;

namespace CzechNationalBank.Web.Services.Models
{
    /// <summary>
    /// Модель эспорта файла
    /// </summary>
    public class ExportFileModel
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
    }
}