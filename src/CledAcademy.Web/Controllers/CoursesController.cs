using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Models;
using CledAcademy.Web.Models.ViewModels;
using CledAcademy.Web.Services.Abstract;

namespace CledAcademy.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataDictionaryService _dataDictionaryService;

        public CoursesController(IUnitOfWorkManager unitOfWorkManager, UserManager<ApplicationUser> userManager,
                                 IDataDictionaryService dataDictionaryService)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
            _dataDictionaryService = dataDictionaryService;
        }

        public IActionResult Index()
        {
            var courses = _unitOfWorkManager.Repository<Course>().GetAll().ToList();
            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var courseBuyDiscount = _dataDictionaryService.GetSingle("CourseBuyDiscount");

            var courseViewModels = (from course in courses
                                    let lessonsDetails =
                                        _unitOfWorkManager.Repository<Lesson>().FindBy(l => course.Id == l.Section.CourseId).Select(
                                            l => new { l.Id, l.Price }).ToList()
                                    let isBoughtAnyLesson =
                                        _unitOfWorkManager.Repository<StudentLesson>().Any(
                                            sl => sl.StudentId == student.Id && lessonsDetails.Select(l => l.Id).Contains(sl.LessonId))
                                    select
                                        new CourseViewModel
                                        {
                                            Course = course,
                                            Price = lessonsDetails.Sum(l => l.Price) * double.Parse(courseBuyDiscount.Value),
                                            IsBoughtAnyLesson = isBoughtAnyLesson
                                        }).ToList();

            return View(courseViewModels);
        }

        [AllowAnonymous]
        public IActionResult FreeVideos(int? id)
        {
            if(id != null)
            {
                var course = _unitOfWorkManager.Repository<Course>().GetSingle(c => c.Id == id);
                if(course == null) return BadRequest();
                ViewBag.CurrentCourse = course;
            }

            var courses = _unitOfWorkManager.Repository<Course>().GetAll().ToList();

            return View(courses);
        }

        public IActionResult Details(int? id, string result)
        {
            if(id == null) return BadRequest();

            if(result != null) ViewBag.Message = this.GetMessage(result);

            ViewBag.Course = _unitOfWorkManager.Repository<Course>().GetSingle(id.Value, c => c.Teacher);
            var sections = _unitOfWorkManager.Repository<Section>().FindBy(t => t.CourseId == id.Value).ToList();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var sectionBuyDiscount = _dataDictionaryService.GetSingle("SectionBuyDiscount");

            var sectionViewModels = (from section in sections
                                     let lessonsDetails =
                                         _unitOfWorkManager.Repository<Lesson>().FindBy(l => section.Id == l.SectionId).Select(l => new { l.Id, l.Price })
                                                           .ToList()
                                     let isBoughtAnyLesson =
                                         _unitOfWorkManager.Repository<StudentLesson>().Any(
                                             sl => sl.StudentId == student.Id && lessonsDetails.Select(l => l.Id).Contains(sl.LessonId))
                                     select
                                         new SectionViewModel
                                         {
                                             Section = section,
                                             Price = lessonsDetails.Sum(l => l.Price) * double.Parse(sectionBuyDiscount.Value),
                                             IsBoughtAnyLesson = isBoughtAnyLesson,
                                             LessonCount = lessonsDetails.Count
                                         }).ToList();

            return View(sectionViewModels);
        }

        public IActionResult TakeLesson(int? id, int? sectionId, string result)
        {
            if(sectionId == null) return NotFound();
            var section = _unitOfWorkManager.Repository<Section>().GetSingle(sectionId.Value);
            if(section == null) return NotFound();
            if(result != null) ViewBag.Message = this.GetMessage(result);

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var lessons = _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == sectionId).ToList();

            if(!lessons.Any()) return NotFound();

            var takenLessons =
                _unitOfWorkManager.Repository<StudentLesson>().FindBy(
                    sl => sl.StudentId == student.Id && sl.Lesson.SectionId == sectionId.Value, sl => sl.Lesson).Select(
                    sl => sl.Lesson).ToList();

            var lessonViewModels =
                lessons.Select(
                    lesson =>
                        new LessonViewModel
                        {
                            Name = lesson.Name,
                            Id = lesson.Id,
                            VideoUrl = lesson.VideoUrl,
                            SectionId = lesson.SectionId,
                            IsBought = takenLessons.Any(tl => tl.Id == lesson.Id)
                        }).ToList();

            ViewBag.ActiveLessonId = id ?? 0;
            ViewBag.Section = _unitOfWorkManager.Repository<Section>().GetSingle(sectionId.Value, s => s.Course);

            if(id.HasValue && id.Value != 0)
            {
                var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(id.Value);
                if(lesson == null) return BadRequest();

                var studentLesson =
                    _unitOfWorkManager.Repository<StudentLesson>().GetSingle(sl => sl.StudentId == student.Id && sl.LessonId == id);

                ViewBag.GetFileResult = studentLesson == null ? GetFileResult.Image : GetFileResult.Video;

                if(studentLesson != null)
                {
                    studentLesson.LastVisitDateTime = DateTime.Now;
                    studentLesson.VisitCounts++;
                    _unitOfWorkManager.Repository<StudentLesson>().Update(studentLesson);
                    _unitOfWorkManager.Commit();
                }
            }

            return View(lessonViewModels);
        }

        public IActionResult AddLessonToCard(int? id)
        {
            if(id == null) return BadRequest();

            var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(id.Value, l => l.Section);
            if(lesson == null) return BadRequest();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            if(_unitOfWorkManager.Repository<ShoppingCard>().Any(
                   sc =>
                       sc.StudentId == student.Id &&
                       (sc.OrderType == OrderType.Lesson && sc.OrderId == id.Value ||
                        sc.OrderType == OrderType.Section && sc.OrderId == lesson.SectionId ||
                        sc.OrderType == OrderType.Course && sc.OrderId == lesson.Section.CourseId)))
                return RedirectToAction("ShoppingCard", "Account",
                    new { result = CheckoutResult.AlreadyAddedShoppingCardLesson.ToString() });

            if(_unitOfWorkManager.Repository<StudentLesson>().Any(sl => sl.StudentId == student.Id && sl.LessonId == lesson.Id))
                return RedirectToAction("ShoppingCard", "Account", new { result = CheckoutResult.AlreadyBoughtLesson.ToString() });

            _unitOfWorkManager.Repository<ShoppingCard>().Add(new ShoppingCard
            {
                OrderId = id.Value,
                OrderType = OrderType.Lesson,
                StudentId = student.Id
            });
            _unitOfWorkManager.Commit();
            return RedirectToAction("ShoppingCard", "Account");
        }

        public IActionResult AddSectionToCard(int? id)
        {
            if(id == null) return BadRequest();
            var section = _unitOfWorkManager.Repository<Section>().GetSingle(id.Value);
            if(section == null) return BadRequest();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var lessonIds =
                _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == section.Id).Select(l => l.Id).ToList();

            if(_unitOfWorkManager.Repository<ShoppingCard>().Any(
                   sc =>
                       sc.StudentId == student.Id &&
                       (sc.OrderType == OrderType.Section && sc.OrderId == id.Value ||
                        sc.OrderType == OrderType.Course && sc.OrderId == section.CourseId ||
                        sc.OrderType == OrderType.Lesson && lessonIds.Contains(sc.OrderId))))
                return RedirectToAction("ShoppingCard", "Account",
                    new { result = CheckoutResult.AlreadyAddedShoppingCardSection.ToString() });

            if(_unitOfWorkManager.Repository<StudentLesson>().Any(
                   sl => sl.StudentId == student.Id && lessonIds.Contains(sl.LessonId)))
                return RedirectToAction("ShoppingCard", "Account", new { result = CheckoutResult.AlreadyBoughtSection.ToString() });

            _unitOfWorkManager.Repository<ShoppingCard>().Add(new ShoppingCard
            {
                OrderId = id.Value,
                OrderType = OrderType.Section,
                StudentId = student.Id
            });
            _unitOfWorkManager.Commit();
            return RedirectToAction("ShoppingCard", "Account");
        }

        public IActionResult AddCourseToCard(int? id)
        {
            if(id == null) return BadRequest();
            var course = _unitOfWorkManager.Repository<Course>().GetSingle(id.Value);
            if(course == null) return BadRequest();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var sectionIds =
                _unitOfWorkManager.Repository<Section>().FindBy(s => s.CourseId == course.Id).Select(s => s.Id).ToList();
            var lessonIds =
                _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.Section.CourseId == course.Id).Select(l => l.Id).ToList();

            if(_unitOfWorkManager.Repository<ShoppingCard>().Any(
                   sc =>
                       sc.StudentId == student.Id &&
                       (sc.OrderType == OrderType.Course && sc.OrderId == id.Value ||
                        sc.OrderType == OrderType.Section && sectionIds.Contains(sc.OrderId) ||
                        sc.OrderType == OrderType.Lesson && lessonIds.Contains(sc.OrderId))))
                return RedirectToAction("ShoppingCard", "Account",
                    new { result = CheckoutResult.AlreadyAddedShoppingCardCourse.ToString() });

            if(_unitOfWorkManager.Repository<ShoppingCard>().Any(
                   sc => sc.StudentId == student.Id && sc.OrderType == OrderType.Section && sectionIds.Contains(sc.OrderId)))
                return RedirectToAction("ShoppingCard", "Account",
                    new { result = CheckoutResult.AlreadyAddedShoppingCardCourse.ToString() });

            if(_unitOfWorkManager.Repository<StudentLesson>().Any(
                   sl => sl.StudentId == student.Id && lessonIds.Contains(sl.LessonId)))
                return RedirectToAction("ShoppingCard", "Account", new { result = CheckoutResult.AlreadyBoughtCourse.ToString() });
            _unitOfWorkManager.Repository<ShoppingCard>().Add(new ShoppingCard
            {
                OrderId = id.Value,
                OrderType = OrderType.Course,
                StudentId = student.Id
            });
            _unitOfWorkManager.Commit();
            return RedirectToAction("ShoppingCard", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuyLesson(int? id)
        {
            if(id == null) return BadRequest();
            var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(id.Value, l => l.Section);
            if(lesson == null) return BadRequest();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            if(_unitOfWorkManager.Repository<StudentLesson>().Any(sl => sl.StudentId == student.Id && sl.LessonId == lesson.Id))
                return RedirectToAction("TakeLesson",
                    new { id, sectionId = lesson.SectionId, result = CheckoutResult.AlreadyBoughtLesson.ToString() });

            CheckoutResult buyResult;

            if(student.Balance < lesson.Price) buyResult = CheckoutResult.InSufficentBalance;
            else
                try
                {
                    _unitOfWorkManager.Repository<StudentOrder>().Add(new StudentOrder
                    {
                        DateTime = DateTime.Now,
                        Fee = -lesson.Price,
                        OrderId = lesson.Id,
                        OrderType = OrderType.Lesson,
                        StudentId = student.Id
                    });
                    _unitOfWorkManager.Repository<StudentLesson>().Add(new StudentLesson
                    {
                        StudentId = student.Id,
                        StartDate = DateTime.Now,
                        LessonId = lesson.Id
                    });
                    student.Balance -= lesson.Price;
                    _unitOfWorkManager.Repository<Student>().Update(student);
                    buyResult = CheckoutResult.Success;
                }
                catch(Exception)
                {
                    buyResult = CheckoutResult.Failure;
                }

            if(buyResult == CheckoutResult.Success)
            {
                _unitOfWorkManager.Repository<ShoppingCard>().DeleteWhere(
                    sc => sc.StudentId == student.Id && sc.OrderType == OrderType.Lesson && sc.OrderId == lesson.Id);

                _unitOfWorkManager.Repository<ShoppingCard>().DeleteWhere(
                    sc =>
                        sc.StudentId == student.Id &&
                        (sc.OrderType == OrderType.Lesson && sc.OrderId == lesson.Id ||
                         sc.OrderType == OrderType.Section && sc.OrderId == lesson.SectionId ||
                         sc.OrderType == OrderType.Course && sc.OrderId == lesson.Section.CourseId));

                _unitOfWorkManager.Commit();
                return RedirectToAction("TakeLesson", new { id, sectionId = lesson.SectionId, result = buyResult.ToString() });
            }

            _unitOfWorkManager.Commit();
            return RedirectToAction("TakeLesson", new { sectionId = lesson.SectionId, result = buyResult.ToString() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuySection(int? id)
        {
            if(id == null) return BadRequest();
            var section = _unitOfWorkManager.Repository<Section>().GetSingle(id.Value);
            if(section == null) return BadRequest();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var lessonIds =
                _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == section.Id).Select(l => l.Id).ToList();

            if(_unitOfWorkManager.Repository<StudentLesson>().Any(
                   sl => sl.StudentId == student.Id && lessonIds.Contains(sl.LessonId)))
                return RedirectToAction("Details",
                    new { id = section.CourseId, result = CheckoutResult.AlreadyBoughtSection.ToString() });

            CheckoutResult buyResult;

            var sectionBuyDiscount = _dataDictionaryService.GetSingle("SectionBuyDiscount");

            var sectionLessons = _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == section.Id).ToList();
            var totalPrice = sectionLessons.Sum(l => l.Price) * double.Parse(sectionBuyDiscount.Value);
            if(student.Balance < totalPrice) buyResult = CheckoutResult.InSufficentBalance;
            else
                try
                {
                    _unitOfWorkManager.Repository<StudentOrder>().Add(new StudentOrder
                    {
                        DateTime = DateTime.Now,
                        Fee = -totalPrice,
                        OrderId = section.Id,
                        OrderType = OrderType.Section,
                        StudentId = student.Id
                    });

                    foreach(var lesson in sectionLessons)
                        _unitOfWorkManager.Repository<StudentLesson>().Add(new StudentLesson
                        {
                            StudentId = student.Id,
                            StartDate = DateTime.Now,
                            LessonId = lesson.Id
                        });
                    student.Balance -= totalPrice;
                    _unitOfWorkManager.Repository<Student>().Update(student);
                    buyResult = CheckoutResult.Success;
                }
                catch(Exception)
                {
                    buyResult = CheckoutResult.Failure;
                }

            if(buyResult == CheckoutResult.Success)
            {
                _unitOfWorkManager.Repository<ShoppingCard>().DeleteWhere(
                    sc =>
                        sc.StudentId == student.Id &&
                        (sc.OrderType == OrderType.Section && sc.OrderId == section.Id ||
                         sc.OrderType == OrderType.Course && sc.OrderId == section.CourseId ||
                         sc.OrderType == OrderType.Lesson && lessonIds.Contains(sc.OrderId)));

                _unitOfWorkManager.Commit();
                return RedirectToAction("TakeLesson", new { sectionId = section.Id, result = buyResult.ToString() });
            }

            _unitOfWorkManager.Commit();
            return RedirectToAction("Details", new { id = section.CourseId, result = buyResult.ToString() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuyCourse(int? id)
        {
            if(id == null) return BadRequest();
            var course = _unitOfWorkManager.Repository<Course>().GetSingle(id.Value);
            if(course == null) return BadRequest();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var lessonIds =
                _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.Section.CourseId == course.Id).Select(l => l.Id).ToList();

            if(
                _unitOfWorkManager.Repository<StudentLesson>().Any(
                    sl => sl.StudentId == student.Id && lessonIds.Contains(sl.LessonId)))
                return RedirectToAction("Details", new { id, result = CheckoutResult.AlreadyBoughtCourse.ToString() });

            CheckoutResult buyResult;

            var courseBuyDiscount = _dataDictionaryService.GetSingle("CourseBuyDiscount");

            var courseLessons = _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.Section.CourseId == course.Id).ToList();
            var totalPrice = courseLessons.Sum(l => l.Price) * double.Parse(courseBuyDiscount.Value);
            if(student.Balance < totalPrice) buyResult = CheckoutResult.InSufficentBalance;
            else
                try
                {
                    _unitOfWorkManager.Repository<StudentOrder>().Add(new StudentOrder
                    {
                        DateTime = DateTime.Now,
                        Fee = -totalPrice,
                        OrderId = course.Id,
                        OrderType = OrderType.Course,
                        StudentId = student.Id
                    });

                    foreach(var lesson in courseLessons)
                        _unitOfWorkManager.Repository<StudentLesson>().Add(new StudentLesson
                        {
                            StudentId = student.Id,
                            StartDate = DateTime.Now,
                            LessonId = lesson.Id
                        });
                    student.Balance -= totalPrice;
                    _unitOfWorkManager.Repository<Student>().Update(student);
                    buyResult = CheckoutResult.Success;
                }
                catch(Exception)
                {
                    buyResult = CheckoutResult.Failure;
                }

            if(buyResult == CheckoutResult.Success)
            {
                var sectionIds =
                    _unitOfWorkManager.Repository<Section>().FindBy(s => s.CourseId == course.Id).Select(s => s.Id).ToList();
                _unitOfWorkManager.Repository<ShoppingCard>().DeleteWhere(
                    sc =>
                        sc.StudentId == student.Id &&
                        (sc.OrderType == OrderType.Course && sc.OrderId == course.Id ||
                         sc.OrderType == OrderType.Section && sectionIds.Contains(sc.OrderId) ||
                         sc.OrderType == OrderType.Lesson && lessonIds.Contains(sc.OrderId)));
                _unitOfWorkManager.Commit();
            }

            _unitOfWorkManager.Commit();
            return RedirectToAction("Details", new { id, result = buyResult.ToString() });
        }
    }
}