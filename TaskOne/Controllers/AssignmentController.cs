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
            if (assignment != null)
            {
                if (assignment.Files != null && assignment.Files.Count > 0)
                {
                    var pathitems = new List<FilePath>();
                    var assignmentitems = new List<Assignments>();
                    assignment.FilePaths = new List<string>();

                    foreach (var file in assignment.Files)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = file.FileName;
                            var fileExtension = Path.GetExtension(fileName);

                            var validationAttribute = new FileValidationAttribute(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" }, 2 * 1024 * 1024);
                            var validationResult = validationAttribute.GetValidationResult(assignment.Files, new ValidationContext(assignment));

                            if (validationResult != ValidationResult.Success)
                            {
                                ModelState.AddModelError("Files", validationResult.ErrorMessage);
                                return View(assignment);
                            }


                            // Saving to Folder and KeepRecord in Database
                            var JMMFilePath = Path.Combine(Env.WebRootPath, "img", Guid.NewGuid() + Path.GetExtension(file.FileName));
                            using (var stream = new FileStream(JMMFilePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            // Store the file path in the list
                            assignment.FilePaths.Add(JMMFilePath);
                            //assignmentRepo.CreateAssignment(assignment);
                            assignmentitems.Add(new Assignments { AssignmentTitle = assignment.AssignmentTitle});
                            pathitems.Add(new FilePath { Path = JMMFilePath, AssignmentId = assignment.Id });
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

            return View(assignment);
        }
        public IActionResult GetFiles(int id)
        {
            var filepaths = assignmentRepo.GetFilesbyAssignmentId(id);
            if(filepaths.Count() > 0)
            {
                return View(filepaths);
            }
            return View();
        }
    }
}
