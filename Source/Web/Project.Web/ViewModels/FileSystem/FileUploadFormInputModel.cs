namespace Project.Web.ViewModels.FileSystem
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public class FileUploadFormInputModel
    {
        [Required]
        public string Directory { get; set; }

        [UIHint("UploadFile")]
        [Required]
        public HttpPostedFileBase FileToUpload { get; set; }
    }
}