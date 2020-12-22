using Assessment.Models;
using Assessment.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Assessment.Controllers
{
    public class HomeController : Controller
    {
        //GLOBAL VARIABLES
        private readonly ILogger<HomeController> _logger;
        private readonly IToastNotification _toastNotification;

        //CONSTRUCTOR
        public HomeController(ILogger<HomeController> logger, IToastNotification toastNotification)
        {
            _logger = logger;
            _toastNotification = toastNotification;
        }

        //GET INDEX
        public IActionResult Index()
        {
            return View();
        }

        //GET SUCCESS
        [HttpGet]
        public IActionResult Success()
        {
            //IF STATEMENT TO PREVENT USERS FROM TYPING IN URL'S MANUALLY
            if (TempData["downloadUrl"] != null)
            {
                //DOWNLOAD SUCCESS
                _toastNotification.AddSuccessToastMessage("File Downloaded");
                Process.Start(new ProcessStartInfo(TempData["downloadUrl"].ToString()) { UseShellExecute = true });
                return View("Index");
            }else
            {
                //DOWNLOAD FAILURE
                _toastNotification.AddErrorToastMessage("Please Upload A File First");
                return View("Index");
            }
        }

        //POST INDEX
        [HttpPost]
        [Obsolete]
        public IActionResult Index(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
        {
            //CREATE A NEW GUID FOR THE FILE NAME
            string fileName = Guid.NewGuid().ToString() + "." + file.FileName.Split(".")[1];
            string path = $"{hostingEnvironment.WebRootPath}\\data\\{fileName}";

            Directory.CreateDirectory(hostingEnvironment.WebRootPath + "\\data");

            //COPY THE SELECTED FILE TO THE PROJECT FILES
            using (FileStream stream = System.IO.File.Create(path))
            {
                file.CopyTo(stream);
                stream.Flush();
            }

            //REWRITE THE FILE INTO THE REQUIRED FORMAT
            string filePath = DataManager.FormatFile(fileName);
            //SHOW A SUCCESS TOAST
            _toastNotification.AddSuccessToastMessage("File successfully converted");
            //PREPARE THE FILE FOR DOWNLOAD
            TempData["downloadUrl"] = DownloadManager.GetDownloadUrl(filePath, fileName);
            //SHOW AN UPLOAD SUCCESS TOAST
            _toastNotification.AddSuccessToastMessage("File upload complete");
            //DELETE THE LOCAL FILE FROM PROJECT
            System.IO.File.Delete(filePath);
            return View("Success");
        }

        //GET ERROR
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
