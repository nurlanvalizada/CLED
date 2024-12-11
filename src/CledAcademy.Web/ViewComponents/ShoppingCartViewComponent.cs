using System.Linq;
using System.Threading.Tasks;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CledAcademy.Web.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartViewComponent(IUnitOfWorkManager unitOfWorkManager, UserManager<ApplicationUser> userManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var student = _unitOfWorkManager.Repository<Student>().GetSingle(s => s.PersonId == user.PersonId);

            var items = await _unitOfWorkManager.Repository<ShoppingCard>().FindBy(sc => sc.StudentId == student.Id)
                .Select(sc=>new ShoppingCardViewModel
            {
                Id = sc.Id,
                OrderId = sc.OrderId,
                StudentId = sc.StudentId,
                OrderType = sc.OrderType
            }).ToListAsync();
            foreach (var item in items)
            {
                switch (item.OrderType)
                {
                    case OrderType.Lesson:
                        var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(item.OrderId);
                        item.Name = lesson.Name;
                        item.Price = lesson.Price;
                        break;
                    case OrderType.Section:
                        var section = _unitOfWorkManager.Repository<Section>().GetSingle(item.OrderId);
                        item.Name = section.Name;
                        item.Price = _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == section.Id).Sum(l => l.Price);
                        break;
                    case OrderType.Course:
                        var course = _unitOfWorkManager.Repository<Course>().GetSingle(item.OrderId);
                        item.Name = course.Name;
                       item.Price = _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.Section.CourseId == course.Id).Sum(l => l.Price) * 0.9;
                        break;
                }
            }

            return View(items);
        }
    }
}