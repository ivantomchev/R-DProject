namespace Project.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    using Project.Common;
    using Project.Common.Extensions;
    using Project.Web.ViewModels.FileSystem;

    public class FileSystemController : Controller
    {
        private const string MainDirectory = GlobalConstants.MainDirectory;

        [HttpPost]
        public ActionResult RenameFolder(string directory, string newName, string oldName)
        {
            if (oldName == newName)
            {
                return this.GridOperationAjaxRefreshData(true, null);
            }

            var path = directory.ReplaceLastOccurrence(oldName, newName);

            try
            {
                //TO DO
                //Check if folder with that name already exists
                Directory.Move(directory, path);
            }
            catch (Exception ex)
            {
                return this.GridOperationAjaxRefreshData(false, ex.Message);
            }

            return this.GridOperationAjaxRefreshData(true, null);
        }

        [HttpPost]
        public ActionResult RenameFile(string directory, string newName, string oldName)
        {
            if (oldName == newName)
            {
                return this.GridOperationAjaxRefreshData(true, null);
            }

            var path = directory.ReplaceLastOccurrence(oldName, newName);

            try
            {
                System.IO.File.Move(directory, path);
            }
            catch (Exception ex)
            {
                return this.GridOperationAjaxRefreshData(false, ex.Message);
            }

            return this.GridOperationAjaxRefreshData(true, null);
        }

        [HttpPost]
        public ActionResult DeleteFolder(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                return this.GridOperationAjaxRefreshData(false, ex.Message);
            }

            return this.GridOperationAjaxRefreshData(true, null);
        }

        [HttpPost]
        public ActionResult DeleteFile(string path)
        {
            var file = new FileInfo(path);

            try
            {
                file.Delete();
            }
            catch (Exception ex)
            {
                return this.GridOperationAjaxRefreshData(false, ex.Message);
            }

            return this.GridOperationAjaxRefreshData(true, null);
        }

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

                return PartialView("_UploadFileFormPartial", new FileUploadFormInputModel());
            }

            return PartialView("_UploadFileFormPartial", model);
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

        private JsonResult GridOperationAjaxRefreshData(bool isSuccess, string message)
        {
            return Json(new { success = isSuccess, message = message });
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
