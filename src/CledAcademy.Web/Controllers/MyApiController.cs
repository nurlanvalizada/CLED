using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Models;
using CledAcademy.Web.Models.VideoStreaming;

namespace CledAcademy.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class MyApiController : Controller
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyApiController(
            IUnitOfWorkManager unitOfWorkManager,
            IHostingEnvironment hostingEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("[action]")]
        public FileStreamResult Get([FromQuery] int lessonId)
        {
            FileStream fs;
            var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(lessonId);
            if(lesson == null)
            {
                ViewBag.GetFileResult = GetFileResult.Image;
                fs = new FileStream(_hostingEnvironment.WebRootPath + "//images//404.jpg", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return new FileStreamResult(fs, "image/jpeg");
            }

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            if(!_unitOfWorkManager.Repository<StudentLesson>().Any(so => so.StudentId == student.Id && so.LessonId == lessonId))
            {
                ViewBag.GetFileResult = GetFileResult.Image;
                fs = new FileStream(_hostingEnvironment.WebRootPath + "//images//forbidden.jpg", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return new FileStreamResult(fs, "image/jpeg");
            }

            ViewBag.GetFileResult = GetFileResult.Video;

            fs = new FileStream(lesson.VideoUrl, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new VideoStreamResult(fs, "video/mp4");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("[action]")]
        public FileStreamResult GetFree([FromQuery] int courseId)
        {
            var course = _unitOfWorkManager.Repository<Course>().GetSingle(courseId);
            if(course == null) return null;

            var fs = new FileStream(course.VideoUrl, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new VideoStreamResult(fs, "video/mp4");
        }

        [HttpPost]
        [Route("[action]")]
        public IEnumerable<TestCorrectness> CheckTestResults(IFormCollection collection)
        {
            var closedTestAnswers = collection.Where(x => x.Key.Contains("optionsRadios"))
                                              .Select(ta => new ClosedTestAnswer
                                              {
                                                  TestId = int.Parse(ta.Key.Replace("optionsRadios", "")),
                                                  AnswerId = int.Parse(ta.Value)
                                              }).ToList();

            var openTestAnswers = collection.Where(x => x.Key.Contains("openValue"))
                                            .Select(ta => new OpenTestAnswer
                                            {
                                                TestId = int.Parse(ta.Key.Replace("openValue", "")),
                                                AnswerText = ta.Value
                                            }).ToList();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            if(!_unitOfWorkManager.Repository<StudentTestAnswer>().Any(sta => sta.StudentId == student.Id))
            {
                foreach(var testAnswer in closedTestAnswers)
                    _unitOfWorkManager.Repository<StudentTestAnswer>().Add(new StudentTestAnswer
                    {
                        StudentId = student.Id,
                        AnswerId = testAnswer.AnswerId
                    });

                foreach(var testAnswer in openTestAnswers)
                    _unitOfWorkManager.Repository<StudentOpenTestAnswer>().Add(new StudentOpenTestAnswer
                    {
                        StudentId = student.Id,
                        AnswerText = testAnswer.AnswerText
                    });
            }
            else
                return null;

            var results = _unitOfWorkManager.TestAnswerRepository.CheckTestAnswers(closedTestAnswers, openTestAnswers).ToList();
            _unitOfWorkManager.Commit();

            return results;
        }

        [HttpPost]
        [Produces("application/json")]
        [Route("[action]")]
        public int DeleteItem(IFormCollection collection, int id)
        {
            var item = _unitOfWorkManager.Repository<ShoppingCard>().GetSingle(id);
            if(item != null)
            {
                _unitOfWorkManager.Repository<ShoppingCard>().Delete(item);
                _unitOfWorkManager.Commit();
            }

            return 0;
        }
    }
}