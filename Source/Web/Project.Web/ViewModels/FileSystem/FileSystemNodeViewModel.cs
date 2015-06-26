namespace Project.Web.ViewModels.FileSystem
{
    using System.Collections.Generic;

    public class FileSystemNodeViewModel
    {
        public string id { get; set; }

        public string text { get; set; }

        public string type { get; set; }

        public ICollection<FileSystemNodeViewModel> children { get; set; }
    }
}