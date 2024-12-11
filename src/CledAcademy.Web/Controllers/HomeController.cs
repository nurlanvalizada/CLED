using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CledAcademy.Core.Domain;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Models;
using CledAcademy.Web.Models.ViewModels;
using CledAcademy.Web.Services.Abstract;

namespace CledAcademy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataDictionaryService _dataDictionaryService;

        public HomeController(IUnitOfWorkManager unitOfWorkManager, UserManager<ApplicationUser> userManager,
                              IDataDictionaryService dataDictionaryService)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
            _dataDictionaryService = dataDictionaryService;
        }

        public IActionResult Index()
        {
            var courses = _unitOfWorkManager.Repository<Course>().GetAll().ToList();
            // var teachers = _unitOfWorkManager.Repository<Teacher>().GetAll().ToList();
            var indexViewModel = new IndexViewModel
            {
                Courses = courses,
                // Teachers = teachers
            };
            return View(indexViewModel);
        }

        [Authorize]
        public IActionResult Test()
        {
            //var tests = _unitOfWorkManager.Repository<Test>().GetAll(t => t.Answers).ToList();
            return View(new List<Test>());
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            var contactMessageViewModel = new ContactMessageViewModel();
            if(User.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserAsync(User).Result;
                var student = _unitOfWorkManager.Repository<Student>().GetSingle(s => s.PersonId == user.PersonId, s => s.Person);
                contactMessageViewModel.FullName = student.Person.FirstName + " " + student.Person.LastName;
                contactMessageViewModel.Email = user.Email;
                contactMessageViewModel.Phone = student.Phone;
            }

            return View(contactMessageViewModel);
        }

        [HttpPost]
        public object Contact(ContactMessageViewModel contactMessageViewModel)
        {
            if(ModelState.IsValid)
            {
                var contactMessage = new ContactMessage
                {
                    FullName = contactMessageViewModel.FullName,
                    Email = contactMessageViewModel.Email,
                    Phone = contactMessageViewModel.Phone,
                    Subject = contactMessageViewModel.Subject,
                    Message = contactMessageViewModel.Message,
                    SendDate = DateTime.Now
                };
                _unitOfWorkManager.Repository<ContactMessage>().Add(contactMessage);
                _unitOfWorkManager.Commit();
                return new { Message = "Sizin mesajınız qəbul edildi. Təşəkkür edirik.", Result = true };
            }

            return false;
        }

        public IActionResult Error(int id)
        {
            return id == 404 ? View("404") : View();
        }

        public IActionResult Teachers()
        {
            var teachers = _unitOfWorkManager.Repository<Teacher>().GetAll().ToList();
            return View(teachers);
        }

        public IActionResult TeacherDetails(int? id)
        {
            if(id == null) return NotFound();
            var teacher = _unitOfWorkManager.Repository<Teacher>().GetSingle(t => t.Id == id);
            if(teacher == null) return NotFound();
            return View(teacher);
        }

        public IActionResult News(int? pageNumber, string searchPattern)
        {
            ViewBag.searchPattern = searchPattern;

            var newsConfigurations = _dataDictionaryService.Get(new List<string> { "NewsPageSize", "NewsMaxShowingPages" });

            var pageSize = int.Parse(newsConfigurations.Single(nc => nc.Key == "NewsPageSize").Value);
            var maxShowingPages = int.Parse(newsConfigurations.Single(nc => nc.Key == "NewsMaxShowingPages").Value);
            var totalNewsCount = string.IsNullOrEmpty(searchPattern)
                ? _unitOfWorkManager.Repository<News>().Count(n => true)
                : _unitOfWorkManager.Repository<News>().Count(n => n.Title.Contains(searchPattern));

            ViewBag.NewsPageParams = this.GetNewsPageParams(pageNumber, pageSize, maxShowingPages, totalNewsCount);

            IEnumerable<News> news = string.IsNullOrEmpty(searchPattern)
                ? _unitOfWorkManager.Repository<News>().FindBy(n => true)
                : _unitOfWorkManager.Repository<News>().FindBy(n => n.Title.Contains(searchPattern));

            news = news.Skip((pageNumber - 1) * pageSize ?? 0).Take(pageSize).OrderByDescending(n => n.PublishDate).ToList();

            return View(news);
        }

        public IActionResult NewsDetails(int? id)
        {
            if(id == null) return NotFound();
            var news = _unitOfWorkManager.Repository<News>().GetSingle(id.Value, n => n.Admin.Person);
            if(news == null) return NotFound();
            return View(news);
        }

        public async Task<IActionResult> Faq()
        {
            var faqs = await _unitOfWorkManager.Repository<Faq>().GetAll().ToListAsync();
            return View(faqs);
        }

        public IActionResult TermsAndConditions()
        {
            var termsAndConditions = _dataDictionaryService.GetSingle("TermsAndConditions");
            return View(termsAndConditions);
        }
    }
}