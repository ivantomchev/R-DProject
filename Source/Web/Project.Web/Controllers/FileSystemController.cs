namespace Project.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    using Project.Common;
    using Project.Web.ViewModels.FileSystem;

    public class FileSystemController : Controller
    {
        private const string MainDirectory = GlobalConstants.MainDirectory;


        [HttpGet]
        public ActionResult UploadFile()
        {
            var model = new FileUploadFormInputModel();

            return PartialView("_UploadFileFormPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadFile(FileUploadFormInputModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var path = Path.Combine(model.Directory, model.FileToUpload.FileName);

                if (System.IO.File.Exists(path))
                {
                    path = GenerateFileName(model.Directory, model.FileToUpload);
                }

                model.FileToUpload.SaveAs(path);

                return PartialView("_UploadFileFormPartial",new FileUploadFormInputModel());
            }

            return PartialView("_UploadFileFormPartial", model);
        }

        [HttpPost]
        public ActionResult DeleteFolder(string dir)
        {

            try
            {
                Directory.Delete(dir, true);
                Directory.Delete(dir);
            }
            catch (Exception)
            {

                return Json(false);
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult DeleteFile(string dir)
        {
            var file = new FileInfo(dir);

            try
            {
                file.Delete();
            }
            catch (Exception)
            {

                return Json(false);
            }

            return Json(true);
        }

        [HttpGet]
        public ActionResult PopulateData()
        {
            var dir = Server.MapPath(MainDirectory);

            var main = new DirectoryInfo(dir);

            var result = new List<FileSystemNodeViewModel>();

            foreach (var item in main.GetDirectories())
            {
                var node = new FileSystemNodeViewModel();
                node.text = item.Name;
                node.id = item.FullName;
                node.type = "root";
                PopulateTree(node.id, node);
                result.Add(node);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void PopulateTree(string dir, FileSystemNodeViewModel node)
        {
            if (node.children == null)
            {
                node.children = new List<FileSystemNodeViewModel>();
            }

            DirectoryInfo directory = new DirectoryInfo(dir);

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                FileSystemNodeViewModel childNode = new FileSystemNodeViewModel();
                childNode.id = subDirectory.FullName;
                childNode.text = subDirectory.Name.ToString();
                childNode.type = "root";

                PopulateTree(subDirectory.FullName, childNode);
                node.children.Add(childNode);
            }

            foreach (FileInfo file in directory.GetFiles())
            {
                FileSystemNodeViewModel childNode = new FileSystemNodeViewModel();
                childNode.id = file.FullName;
                childNode.text = file.Name.ToString();
                childNode.type = "file";
                node.children.Add(childNode);
            }
        }

        private string GenerateFileName(string directory, HttpPostedFileBase file)
        {
            string newPath = Path.Combine(directory, file.FileName);
            int count = 1;

            var fileName = Path.GetFileNameWithoutExtension(newPath);
            var fileExtension = Path.GetExtension(newPath);

            while (System.IO.File.Exists(newPath))
            {
                string tempFileName = string.Format("{0} ({1})", fileName, count++);
                newPath = Path.Combine(directory, tempFileName + fileExtension);
            }

            return newPath;
        }
    }
}
