using System.Collections.Generic;
using System.Threading.Tasks;
using CledAcademy.Core.Domain;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CledAcademy.Web.ViewComponents
{
    public class SocialViewComponent : ViewComponent
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public SocialViewComponent(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var socialDictionary =
               await _unitOfWorkManager.Repository<DataDictionary>().FindBy(
                       dd => new List<string> { "FacebookPage", "TwitterPage", "InstagramPage", "LinkedinPage" }.Contains(dd.Key))
                   .ToDictionaryAsync(dd => dd.Header, dd => dd.Value);

            var socialViewModel = new SocialViewModel
            {
                FacebookPage =
                    socialDictionary.ContainsKey("FacebookPage") ? socialDictionary["FacebookPage"] : "#",
                TwitterPage =
                    socialDictionary.ContainsKey("TwitterPage") ? socialDictionary["TwitterPage"] : "#",
                InstagramPage = 
                    socialDictionary.ContainsKey("InstagramPage") ? socialDictionary["InstagramPage"] : "#",
                LinkedinPage =
                    socialDictionary.ContainsKey("LinkedinPage") ? socialDictionary["LinkedinPage"] : "#",
            };

            return View(socialViewModel);
        }
    }
}