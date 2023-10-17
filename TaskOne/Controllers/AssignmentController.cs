using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml.XPath;
using TaskOne.Models;
using TaskOne.Repository;

namespace TaskOne.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly IAssignmentRepository assignmentRepo;
        private readonly IWebHostEnvironment Env;

        public AssignmentController(IAssignmentRepository _assignmentRepo, IWebHostEnvironment env)
        {
            assignmentRepo = _assignmentRepo;
            Env = env;
        }

        public IActionResult Index()
        {
            var assignments = assignmentRepo.GetAllAssignments();
            return View(assignments);
        }
        [HttpGet]
        public IActionResult UploadAssignment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadAssignment(Assignments assignment)
        {
            if (ModelState.IsValid)
            {
                if (assignment != null)
                {
                    if (assignment.Files != null && assignment.Files.Count > 0)
                    {
                        var pathitems = new List<FilePath>();
                        var assignmentitems = new List<Assignments>();
                        foreach (var file in assignment.Files)
                        {
                            if (file.Length > 0)
                            {
                                // Saving to Folder and KeepRecord in Database
                                var GuidId = Guid.NewGuid();
                                var JMMFilePath = Path.Combine(Env.WebRootPath, "img", GuidId + Path.GetExtension(file.FileName));
                                var pathtoSaveinDB = Path.Combine("\\" + "img", GuidId + Path.GetExtension(file.FileName));
                                using (var stream = new FileStream(JMMFilePath, FileMode.Create))
                                {
                                    file.CopyTo(stream);
                                }

                                // Store the file path in the list
                                assignmentitems.Add(new Assignments { AssignmentTitle = assignment.AssignmentTitle });
                                pathitems.Add(new FilePath { Path = pathtoSaveinDB, AssignmentId = assignment.Id });
                            }
                        }
                        //Save in Db
                        var SavedAssignment = assignmentRepo.CreateAssignment(assignment);
                        foreach (var filePath in pathitems)
                        {
                            filePath.AssignmentId = SavedAssignment.Id;
                        }
                        //Save Assignment Files Paths
                        assignmentRepo.SaveFilePaths(pathitems);
                        ViewBag.ResultMessage = "Files Added Successfully";
                        ModelState.Clear();
                        return View();
                    }
                }
            }
            return View(assignment);
        }
        public IActionResult GetFiles(int id)
        {
            var filepaths = assignmentRepo.GetFilesbyAssignmentId(id);
            if (filepaths.Count() > 0)
            {
                return View(filepaths);
            }
            return View();
        }
        [HttpGet]
        public IActionResult DownloadFile(string FilePath)
        {
            string GetFileName = Path.GetFileName(FilePath);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", GetFileName);
            var mimeType = "application/octet-stream";

            return File(System.IO.File.OpenRead(filePath), mimeType, GetFileName);
        }
    }
}
