using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LanShanPRMS.ViewModel;
using LanShanPRMS.Service;
using LanShanPRMS.Attribute;
using LanShanPRMS.Common.Helpers;

namespace LanShanPRMS.ControllerBase
{
    [Login]
    public class BaseUpLoadController : Controller
    {       
        public JsonResult UpLoad(string dirname, HttpPostedFileBase file)
        {
            var urlPath = "UpFiles/" + dirname + "/";
            var localPath = ConfigHelper.ServerPath + urlPath;
            if (Request.Files.Count == 0)
                return Json(new UpLoadViewModel() { Message = "上传失败！" });
            var ex = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ex))
                return Json(new UpLoadViewModel() { Message = "未找到文件后缀名！！请重新选择文件" });

            if (!System.IO.Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);
            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ex;
            var filepath = Path.Combine(localPath, filename);
            file.SaveAs(filepath);
            return Json(new UpLoadViewModel()
            {
                jsonrpc = "2.0",
                FileName = file.FileName,
                FilePath = urlPath + filename,
                State = 1
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpLoad(string dirname, string username, HttpPostedFileBase file)
        {
            var urlPath = "UpFiles/" + dirname + "/" + username + "/";
            var localPath = ConfigHelper.ServerPath + urlPath;
            if (Request.Files.Count == 0)
                return Json(new UpLoadViewModel() { Message = "上传失败！" });
            var ex = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ex))
                return Json(new UpLoadViewModel() { Message = "上传失败！请重新选择文件" });

            if (!System.IO.Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);
            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ex;
            var filepath = Path.Combine(localPath, filename);
            file.SaveAs(filepath);
            return Json(new UpLoadViewModel()
            {
                jsonrpc = "2.0",
                FileName = file.FileName,
                FilePath = urlPath + filename,
                State = 1
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(string filePathName)
        {
            var urlPath = "";
            if (string.IsNullOrEmpty(filePathName))
            {
                return Content("no");
            }
            //检查路径
            if (!filePathName.StartsWith(urlPath) ||
                filePathName.Substring(6, filePathName.Length - 7).Contains("../"))
            {
                return Content("no!");
            }
            var localFilePathName = this.Server.MapPath(filePathName);

            try
            {
                System.IO.File.Delete(localFilePathName);
                return Content("ok");
            }
            catch
            {
                return Content("no");
            }
        }
        private static readonly Dictionary<Guid, Tuple<string, string>> DicPathId = new Dictionary<Guid, Tuple<string, string>>();

        public ActionResult GetPathId(string filePathName, string fileName)
        {
            var urlPath = "";
            if (string.IsNullOrEmpty(filePathName))
            {
                return Content("no");
            }
            //检查路径
            if (!filePathName.StartsWith(urlPath) ||
                filePathName.Substring(6, filePathName.Length - 7).Contains("../"))
            {
                return Content("no!");
            }

            var localFilePathName = this.Server.MapPath("~/" + filePathName.Replace("../", ""));

            try
            {
                var id = Guid.NewGuid();
                var fileinfo = new Tuple<string, string>(localFilePathName, fileName);
                DicPathId.Add(id, fileinfo);
                return Content(id.ToString("N"));
            }
            catch
            {
                return Content("no");
            }
        }
        /// <summary>
        /// KindEditor的内置上传方法
        /// </summary>
        /// <returns></returns>
        public JsonResult KindImage()
        {
            const string urlPath = "/UpFiles/KindEditor/Image/";
            var localPath = ConfigHelper.ServerPath + urlPath;
            const string fileTypes = "gif,jpg,jpeg,png,bmp";
            const int maxSize = 1000000;
            var file = Request.Files["imgFile"];
            if (file == null)
                return Json(new { error = 1, message = "请选择文件！" }, JsonRequestBehavior.AllowGet);
            if (file.InputStream == null || file.InputStream.Length > maxSize)
                return Json(new { error = 1, message = "上传文件大小超过限制！" }, JsonRequestBehavior.AllowGet);

            var ex = Path.GetExtension(file.FileName)?.ToLower();
            if (string.IsNullOrEmpty(ex) || Array.IndexOf(fileTypes.Split(','), ex.Substring(1).ToLower()) == -1)
                return Json(new { error = 1, message = "上传文件扩展名是不允许的扩展名！" }, JsonRequestBehavior.AllowGet);
            if (!System.IO.Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);
            var filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ex;
            var filepath = Path.Combine(localPath, filename);
            file.SaveAs(filepath);
            return Json(new { error = 0, url = urlPath + filename }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Download(Guid pathid)
        {
            var fileinfo = DicPathId[pathid];
            var path = fileinfo.Item1;
            var filename = fileinfo.Item2;
            DicPathId.Remove(pathid);
            return File(path, "application/octet-stream", HttpUtility.UrlEncode(Path.GetFileName(filename)));
        }

        #region WebUploader 文件分片
        [HttpPost]
        public JsonResult UpLoadChucks(HttpPostedFileBase file, int? chunk, int? chunks, string guid)
        {
            var model = new WebUploaderResult();
            try
            {
                //如果进行了分片
                if (chunks > 0 && !string.IsNullOrWhiteSpace(guid))
                {
                    //根据GUID创建用该GUID命名的临时文件夹
                    var folder = "/UpFiles/File/" + DateTime.Now.ToString("yyyyMMdd") + "/";
                    var guidfolder = ConfigHelper.ServerPath + folder + guid + "/";
                    var path = guidfolder + chunk;

                    //建立临时传输文件夹
                    if (!Directory.Exists(Path.GetDirectoryName(guidfolder)))
                        Directory.CreateDirectory(guidfolder);

                    var addFile = new FileStream(path, FileMode.Append, FileAccess.Write);
                    var AddWriter = new BinaryWriter(addFile);
                    //获得上传的分片数据流
                    var stream = file.InputStream;
                    var TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                    model.State = 1;
                    model.Message = "保存成功！";
                    model.FileName = file.FileName;
                    model.FilePath = folder;
                    model.Guid = guid;
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                else//没有分片直接保存
                    return UpLoad("File", file);

            }
            catch (Exception ex)
            {
                model.State = 0;
                model.Message = "上传失败！";
                model.FilePath = ex.Message;
                return Json(model, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult MergeFiles(WebUploaderResult model)
        {
            var sourcePath = ConfigHelper.ServerPath + model.FilePath;
            sourcePath = sourcePath + model.Guid;//源数据文件夹
            var targetPath = model.FilePath + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(model.FileName);//合并后的文件

            var dicInfo = new DirectoryInfo(sourcePath);
            if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
            {
                var files = dicInfo.GetFiles();
                foreach (var file in files.OrderBy(f => int.Parse(f.Name)))
                {
                    var addFile = new FileStream(ConfigHelper.ServerPath + targetPath, FileMode.Append, FileAccess.Write);
                    var AddWriter = new BinaryWriter(addFile);

                    //获得上传的分片数据流
                    Stream stream = file.Open(FileMode.Open);
                    var TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                }
                DeleteFolder(sourcePath);
                return Json(new WebUploaderResult() { FileName = model.FileName, FilePath = targetPath, State = 1, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
                //context.Response.Write("{\"hasError\" : false}");
            }
            else
                return Json(new WebUploaderResult() { FileName = model.FileName, FilePath = targetPath, Message = "上传失败，请刷新后重试！" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dirpath"></param>
        private static void DeleteFolder(string dirpath)
        {
            //删除这个目录下的所有子目录
            if (Directory.GetDirectories(dirpath).Length > 0)
            {
                foreach (var fl in Directory.GetDirectories(dirpath))
                {
                    Directory.Delete(fl, true);
                }
            }
            //删除这个目录下的所有文件
            if (Directory.GetFiles(dirpath).Length > 0)
            {
                foreach (var f in Directory.GetFiles(dirpath))
                {
                    System.IO.File.Delete(f);
                }
            }
            Directory.Delete(dirpath, true);
        }
        #endregion

        #region FineUploader 文件分片
        [HttpPost]
        public JsonResult Chucks(HttpPostedFileBase qqfile, int? qqpartindex, string qquuid)
        {
            var re = Request.Form.ToString();

            var model = new FineUploaderResult();
            try
            {

                //根据GUID创建用该GUID命名的临时文件夹
                var folder = "/UpFiles/File/" + DateTime.Now.ToString("yyyyMMdd") + "/";
                var guidfolder = ConfigHelper.ServerPath + folder + qquuid + "/";
                var path = guidfolder + qqpartindex;

                //建立临时传输文件夹
                if (!Directory.Exists(Path.GetDirectoryName(guidfolder)))
                    Directory.CreateDirectory(guidfolder);

                var addFile = new FileStream(path, FileMode.Append, FileAccess.Write);
                var AddWriter = new BinaryWriter(addFile);
                //获得上传的分片数据流
                var stream = qqfile.InputStream;
                var TempReader = new BinaryReader(stream);
                //将上传的分片追加到临时文件末尾
                AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                //关闭BinaryReader文件阅读器
                TempReader.Close();
                stream.Close();
                AddWriter.Close();
                addFile.Close();

                TempReader.Dispose();
                stream.Dispose();
                AddWriter.Dispose();
                addFile.Dispose();
                model.success = true;
                model.error = "保存成功！";
                model.qqfilename = qqfile.FileName;
                model.filepath = folder;
                model.qquuid = qquuid;
                return Json(model, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                model.success = false;
                model.error = "上传失败！";
                model.filepath = ex.Message;
                model.qquuid = re;
                return Json(model, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]

        public JsonResult ChunkComplete(FineUploaderResult model)
        {
            var folder = "/UpFiles/File/" + DateTime.Now.ToString("yyyyMMdd") + "/";
            var sourcePath = ConfigHelper.ServerPath + folder + model.qquuid + "/";
            var targetPath = folder + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(model.qqfilename);//合并后的文件

            var dicInfo = new DirectoryInfo(sourcePath);
            if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
            {
                var files = dicInfo.GetFiles();
                foreach (var file in files.OrderBy(f => int.Parse(f.Name)))
                {
                    var addFile = new FileStream(ConfigHelper.ServerPath + targetPath, FileMode.Append, FileAccess.Write);
                    var AddWriter = new BinaryWriter(addFile);

                    //获得上传的分片数据流
                    Stream stream = file.Open(FileMode.Open);
                    var TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                }
                DeleteFolder(sourcePath);
                return Json(new FineUploaderResult() { qqfilename = model.qqfilename, filepath = targetPath, success = true, error = "保存成功！" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new FineUploaderResult() { qqfilename = model.qqfilename, filepath = targetPath, error = "上传失败，请刷新后重试！" }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 假装执行了文件删除操作
        /// </summary>
        /// <param name="qqfile"></param>
        /// <returns></returns>
        public JsonResult Delete(HttpPostedFileBase qqfile)
        {
            return Json(new FineUploaderResult()
            {
                success = true
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}
