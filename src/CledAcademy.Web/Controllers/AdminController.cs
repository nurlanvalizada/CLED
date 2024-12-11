using System;
using System.Linq;
using System.Threading.Tasks;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CledAcademy.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public AdminController(IUnitOfWorkManager unitOfWorkManager, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Students

        public IActionResult Students(string result)
        {
            var students = _unitOfWorkManager.Repository<Student>().GetAll(s => s.Person.ApplicationUser).ToList();
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.Message = result;
            }
            return View("Student/Students", students);
        }

        public IActionResult StudentAddPayment(int id)
        {
            var student = _unitOfWorkManager.Repository<Student>().GetSingle(s => s.Id == id, s => s.Person);

            if (student == null) return BadRequest();

            return View("Student/StudentAddPayment", student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentAddPayment(int id, double amount)
        {
            if (amount <= 0) return BadRequest();

            var user = _userManager.GetUserAsync(User).Result;
            if (user == null) return BadRequest();

            var studentObj = _unitOfWorkManager.Repository<Student>().GetSingle(s => s.Id == id);

            if (user.UserName == "cledacademy") // this user is going to be used to make offline payments
            {
                _unitOfWorkManager.Repository<AccountTopUp>().Add(new AccountTopUp
                {
                    StudentId = studentObj.Id,
                    Amount = amount,
                    DateTime = DateTime.Now,
                    PaymetOrigin = PaymentOrigin.CledAcademy,
                    PaymentKey = "",
                    StatusCode = "1-success"
                });

                var bonusAmount = BonusCalculator.GetBonusAmount(amount);

                var initialAmount = amount;

                amount += bonusAmount;

                if (bonusAmount > 0) // if gets bonus
                {
                    MakeBonusPayment(studentObj.Id, bonusAmount, "", PaymentOrigin.CledAcademyPaymentBonus);
                    var studentUser = await _userManager.Users.FirstAsync(u => u.PersonId == studentObj.PersonId);
                    await _emailSender.SendEmailAsync(studentUser.Email, "CLED ACADEMY - BONUS",
                                           "HÖRMƏTLİ İSTİFADƏÇİ! KOMPANİYAMIZIN İŞTİRAKÇISI OLDUĞUNUZ ÜÇÜN SİZƏ TƏŞƏKKÜR EDİRİK.",
                                           $"Kompaniyamız çərçivəsində siz son ödədiyiniz <b>{initialAmount} AZN</b> məbləğ qarşılığında <b>{bonusAmount} AZN</b> balansınıza bonus " +
                                           "qazandınız. Təbrik edirik! Daha çox ödəniş edin və daha çox bonuslar qazanın. " +
                                           "<br/><br/><b>Kompaniyalarımız haqqında daha ətraflı məlumat əldə etmək üçün saytımızın xəbərlər bölməsini mütəmadi olaraq izləyin</b> <br/> " +
                                           "<a href=\"http://cledacademy.com/Home/News/\" title=\"CLEDAcademy Xəbərlər\">CLEDAcademy Xəbərlər</a>");
                }

            }

            studentObj.Balance += amount;
            _unitOfWorkManager.Repository<Student>().Update(studentObj);

            _unitOfWorkManager.Commit();
            return RedirectToAction("Students");
        }

        private void MakeBonusPayment(int studentId, double bonusAmount, string paymentKey, PaymentOrigin paymentOrigin)
        {
            var bonusAccountTopUp = new AccountTopUp
            {
                StudentId = studentId,
                Amount = bonusAmount,
                DateTime = DateTime.Now,
                PaymentKey = paymentKey,
                PaymetOrigin = paymentOrigin,
                StatusCode = "1-success"
            };
            _unitOfWorkManager.Repository<AccountTopUp>().Add(bonusAccountTopUp);
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(int studentId)
        {
            var student = _unitOfWorkManager.Repository<Student>().GetSingle(studentId);
            var user = _userManager.Users.SingleOrDefault(u => u.PersonId == student.PersonId);
            string result = null;
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                result = "User already has admin role";
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                _unitOfWorkManager.Repository<Admin>().Add(new Admin
                {
                    PersonId = student.PersonId
                });
                _unitOfWorkManager.Commit();
            }
            return RedirectToAction("Students", new { result });
        }

        public IActionResult AccountTopUps()
        {
            var topUps = _unitOfWorkManager.Repository<AccountTopUp>().FindBy(at => at.StatusCode != null, at => at.Student.Person).ToList();
            return View("Student/AccountTopUps", topUps);
        }

        #endregion

        #region Teachers

        public IActionResult Teachers()
        {
            var teachers = _unitOfWorkManager.Repository<Teacher>().GetAll().ToList();
            return View("Teacher/Teachers", teachers);
        }

        public IActionResult TeacherEdit(int id)
        {
            var teacher = _unitOfWorkManager.Repository<Teacher>().GetSingle(s => s.Id == id);
            return View("Teacher/TeacherEdit", teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TeacherEdit(int id, Teacher teacher)
        {
            var teacherObj = _unitOfWorkManager.Repository<Teacher>().GetSingle(s => s.Id == id);
            teacherObj.FullName = teacher.FullName;
            teacherObj.ImageUrl = teacher.ImageUrl;
            teacherObj.Profession = teacher.Profession;
            teacherObj.About = teacher.About;
            teacherObj.FacebookProfile = teacher.FacebookProfile;
            teacherObj.TwitterProfile = teacher.TwitterProfile;
            teacherObj.SkypeProfile = teacher.SkypeProfile;
            _unitOfWorkManager.Repository<Teacher>().Update(teacherObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Teachers");
        }

        #endregion

        #region Faqs

        public IActionResult Faqs()
        {
            var faqs = _unitOfWorkManager.Repository<Faq>().GetAll().ToList();
            return View("Faq/Faqs", faqs);
        }

        public IActionResult FaqCreate()
        {
            return View("Faq/FaqCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FaqCreate(Faq faq)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkManager.Repository<Faq>().Add(faq);
                _unitOfWorkManager.Commit();
                return RedirectToAction("Faqs");
            }
            return View("Faq/FaqEdit");
        }

        public IActionResult FaqEdit(int id)
        {
            var faq = _unitOfWorkManager.Repository<Faq>().GetSingle(s => s.Id == id);
            return View("Faq/FaqEdit", faq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FaqEdit(int id, Faq faq)
        {
            var faqObj = _unitOfWorkManager.Repository<Faq>().GetSingle(s => s.Id == id);
            faqObj.Question = faq.Question;
            faqObj.Answer = faq.Answer;
            _unitOfWorkManager.Repository<Faq>().Update(faqObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Faqs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FaqDelete(int id)
        {
            var faq = _unitOfWorkManager.Repository<Faq>().GetSingle(s => s.Id == id);
            _unitOfWorkManager.Repository<Faq>().Delete(faq);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Faqs");
        }

        #endregion

        #region Course

        public IActionResult Courses()
        {
            var courses = _unitOfWorkManager.Repository<Course>().GetAll().ToList();
            return View("Course/Courses", courses);
        }

        public IActionResult CourseCreate()
        {
            return View("Course/CourseCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CourseCreate(Course course)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkManager.Repository<Course>().Add(course);
                _unitOfWorkManager.Commit();
                return RedirectToAction("Courses");
            }
            return View("Course/CourseCreate");
        }

        public IActionResult CourseEdit(int id)
        {
            var course = _unitOfWorkManager.Repository<Course>().GetSingle(s => s.Id == id);
            return View("Course/CourseEdit", course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CourseEdit(int id, Course course)
        {
            var courseObj = _unitOfWorkManager.Repository<Course>().GetSingle(s => s.Id == id);
            courseObj.Name = course.Name;
            courseObj.Description = course.Description;
            courseObj.ImageUrl = course.ImageUrl;
            courseObj.VideoUrl = course.VideoUrl;
            _unitOfWorkManager.Repository<Course>().Update(courseObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CourseDelete(int id)
        {
            var course = _unitOfWorkManager.Repository<Course>().GetSingle(s => s.Id == id);
            _unitOfWorkManager.Repository<Course>().Delete(course);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Courses");
        }

        #endregion

        #region Sections

        public IActionResult Sections(int courseId)
        {
            var sections = _unitOfWorkManager.Repository<Section>().FindBy(s => s.CourseId == courseId).ToList();
            return View("Section/Sections", sections);
        }

        public IActionResult SectionCreate(int courseId)
        {
            return View("Section/SectionCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SectionCreate(Section section)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkManager.Repository<Section>().Add(section);
                _unitOfWorkManager.Commit();
                return RedirectToAction("Sections", new { courseId = section.CourseId });
            }
            return View("Section/SectionCreate");
        }

        public IActionResult SectionEdit(int id)
        {
            var section = _unitOfWorkManager.Repository<Section>().GetSingle(s => s.Id == id);
            return View("Section/SectionEdit", section);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SectionEdit(int id, Section section)
        {
            var sectionObj = _unitOfWorkManager.Repository<Section>().GetSingle(s => s.Id == id);
            sectionObj.Name = section.Name;
            sectionObj.Description = section.Description;
            _unitOfWorkManager.Repository<Section>().Update(sectionObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Sections", new { courseId = sectionObj.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SectionDelete(int id)
        {
            var section = _unitOfWorkManager.Repository<Section>().GetSingle(s => s.Id == id);
            _unitOfWorkManager.Repository<Section>().Delete(section);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Sections", new { courseId = section.CourseId });
        }

        #endregion

        #region Lessons

        public IActionResult Lessons(int sectionId)
        {
            var lessons = _unitOfWorkManager.Repository<Lesson>().FindBy(s => s.SectionId == sectionId).ToList();
            return View("Lesson/Lessons", lessons);
        }

        public IActionResult LessonCreate(int sectionId)
        {
            return View("Lesson/LessonCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LessonCreate(Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkManager.Repository<Lesson>().Add(lesson);
                _unitOfWorkManager.Commit();
                return RedirectToAction("Lessons", new { sectionId = lesson.SectionId });
            }
            return View("Lesson/LessonCreate");
        }

        public IActionResult LessonEdit(int id)
        {
            var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(s => s.Id == id);
            return View("Lesson/LessonEdit", lesson);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LessonEdit(int id, Lesson lesson)
        {
            var lessonObj = _unitOfWorkManager.Repository<Lesson>().GetSingle(s => s.Id == id);
            lessonObj.Name = lesson.Name;
            lessonObj.VideoUrl = lesson.VideoUrl;
            lessonObj.Price = lesson.Price;
            _unitOfWorkManager.Repository<Lesson>().Update(lessonObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Lessons", new { sectionId = lessonObj.SectionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LessonDelete(int id)
        {
            var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(s => s.Id == id);
            _unitOfWorkManager.Repository<Lesson>().Delete(lesson);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Lessons", new { sectionId = lesson.SectionId });
        }

        #endregion

        #region News

        public IActionResult News()
        {
            var news = _unitOfWorkManager.Repository<News>().GetAll().ToList();
            return View("News/News", news);
        }

        public IActionResult NewsCreate()
        {
            return View("News/NewsCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewsCreate(News news)
        {
            if (ModelState.IsValid)
            {
                news.AdminId =
                    _unitOfWorkManager.Repository<Admin>()
                        .GetSingle(a => a.PersonId == _userManager.GetUserAsync(User).Result.PersonId).Id;
                news.PublishDate = DateTime.Now;
                _unitOfWorkManager.Repository<News>().Add(news);
                _unitOfWorkManager.Commit();
                return RedirectToAction("News");
            }
            return View("News/NewsCreate");
        }

        public IActionResult NewsEdit(int id)
        {
            var course = _unitOfWorkManager.Repository<News>().GetSingle(n => n.Id == id);
            return View("News/NewsEdit", course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewsEdit(int id, News news)
        {
            var newsObj = _unitOfWorkManager.Repository<News>().GetSingle(n => n.Id == id);
            newsObj.Title = news.Title;
            newsObj.ShortContent = news.ShortContent;
            newsObj.ImageUrl = news.ImageUrl;
            newsObj.Content = news.Content;
            _unitOfWorkManager.Repository<News>().Update(newsObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("News");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewsDelete(int id)
        {
            var news = _unitOfWorkManager.Repository<News>().GetSingle(n => n.Id == id);
            _unitOfWorkManager.Repository<News>().Delete(news);
            _unitOfWorkManager.Commit();
            return RedirectToAction("News");
        }

        #endregion

        #region Test

        public IActionResult Tests(int courseId)
        {
            var tests = _unitOfWorkManager.Repository<Test>().FindBy(t => t.CourseId == courseId).ToList();
            return View("Test/Tests", tests);
        }

        public IActionResult TestCreate()
        {
            return View("Test/TestCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TestCreate(Test test)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkManager.Repository<Test>().Add(test);
                _unitOfWorkManager.Commit();
                return RedirectToAction("Tests", new { courseId = test.CourseId });
            }
            return View("Test/TestCreate");
        }

        public IActionResult TestEdit(int id)
        {
            var test = _unitOfWorkManager.Repository<Test>().GetSingle(t => t.Id == id);
            return View("Test/TestEdit", test);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TestEdit(int id, Test test)
        {
            var testObj = _unitOfWorkManager.Repository<Test>().GetSingle(t => t.Id == id);
            testObj.Content = test.Content;
            testObj.TestType = test.TestType;
            _unitOfWorkManager.Repository<Test>().Update(testObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Tests", new { courseId = testObj.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TestDelete(int id)
        {
            var test = _unitOfWorkManager.Repository<Test>().GetSingle(t => t.Id == id);
            _unitOfWorkManager.Repository<Test>().Delete(test);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Tests", new { courseId = test.CourseId });
        }

        #endregion

        #region Answer

        public IActionResult Answers(int testId)
        {
            var answers = _unitOfWorkManager.Repository<Answer>().FindBy(t => t.TestId == testId).ToList();
            var testAnswer = _unitOfWorkManager.Repository<TestAnswer>().GetSingle(ta => ta.TestId == testId);
            ViewBag.CorrectAnswerId = testAnswer?.AnswerId ?? 0;
            return View("Answer/Answers", answers);
        }

        public IActionResult AnswerCreate()
        {
            return View("Answer/AnswerCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnswerCreate(Answer answer)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkManager.Repository<Answer>().Add(answer);
                _unitOfWorkManager.Commit();
                return RedirectToAction("Answers", new { testId = answer.TestId });
            }
            return View("Answer/AnswerCreate");
        }

        public IActionResult AnswerEdit(int id)
        {
            var answer = _unitOfWorkManager.Repository<Answer>().GetSingle(a => a.Id == id);
            return View("Answer/AnswerEdit", answer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnswerEdit(int id, Answer answer, string isCorrect)
        {
            var answerObj = _unitOfWorkManager.Repository<Answer>().GetSingle(a => a.Id == id);
            answerObj.Text = answer.Text;
            _unitOfWorkManager.Repository<Answer>().Update(answerObj);
            _unitOfWorkManager.Commit();

            if (isCorrect == "on")
            {
                var testAnswer = _unitOfWorkManager.Repository<TestAnswer>().GetSingle(ta => ta.TestId == answer.TestId);
                if (testAnswer == null)
                {
                    _unitOfWorkManager.Repository<TestAnswer>().Add(new TestAnswer
                    {
                        TestId = answerObj.TestId,
                        AnswerId = answerObj.Id
                    });
                }
                else
                {
                    testAnswer.AnswerId = answerObj.Id;
                    _unitOfWorkManager.Repository<TestAnswer>().Update(testAnswer);
                }
            }

            _unitOfWorkManager.Commit();
            return RedirectToAction("Answers", new { testId = answerObj.TestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnswerDelete(int id)
        {
            var answer = _unitOfWorkManager.Repository<Answer>().GetSingle(a => a.Id == id);
            _unitOfWorkManager.Repository<Answer>().Delete(answer);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Answers", new { testId = answer.TestId });
        }

        #endregion

        #region Data

        public IActionResult Datas()
        {
            var datas = _unitOfWorkManager.Repository<DataDictionary>().GetAll().ToList();
            return View("Data/Datas", datas);
        }

        public IActionResult DataCreate()
        {
            return View("Data/DataCreate");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DataCreate(DataDictionary data)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkManager.Repository<DataDictionary>().Add(data);
                _unitOfWorkManager.Commit();
                return RedirectToAction("Datas");
            }
            return View("Data/DataCreate");
        }

        public IActionResult DataEdit(int id)
        {
            var data = _unitOfWorkManager.Repository<DataDictionary>().GetSingle(n => n.Id == id);
            return View("Data/DataEdit", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DataEdit(int id, DataDictionary data)
        {
            var dataObj = _unitOfWorkManager.Repository<DataDictionary>().GetSingle(n => n.Id == id);
            dataObj.Key = data.Key;
            dataObj.Header = data.Header;
            dataObj.Value = data.Value;
            _unitOfWorkManager.Repository<DataDictionary>().Update(dataObj);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Datas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DataDelete(int id)
        {
            var data = _unitOfWorkManager.Repository<DataDictionary>().GetSingle(n => n.Id == id);
            _unitOfWorkManager.Repository<DataDictionary>().Delete(data);
            _unitOfWorkManager.Commit();
            return RedirectToAction("Datas");
        }

        #endregion

        public IActionResult ContactMessages()
        {
            var contactMessages = _unitOfWorkManager.Repository<ContactMessage>().GetAll().ToList();
            return View(contactMessages);
        }
    }
}