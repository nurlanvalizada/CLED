using System;
using System.Linq;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Core.Models.StaticFiles;
using CledAcademy.Repository.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CledAcademy.Web.Models
{
    public static class ControllerExtensions
    {
        public static Student GetStudent(this ControllerBase controller, UserManager<ApplicationUser> userManager,
            IRepository<Student> studentRepository)
        {
            var user = userManager.GetUserAsync(controller.User).Result;
            return studentRepository.GetSingle(s => s.PersonId == user.PersonId, s => s.Person);
        }

        public static ActionResult ViewImage(this ControllerBase controller, int studentId, IRepository<Student> studentRepository)
        {
            var student = studentRepository.GetSingle(studentId, s => s.Person);
            return student.Person.Image != null ? controller.File(student.Person.Image, student.Person.ImageContentType) : null;
        }

        public static NewsPageParameters GetNewsPageParams(this ControllerBase controller, int? pageNumber, int pageSize,
            int maxShowingPages, int totalNewsCount)
        {
            var pageCount = totalNewsCount / pageSize + (totalNewsCount % pageSize == 0 ? 0 : 1);
            pageNumber = pageNumber ?? 1;

            var pageStartIndex = pageNumber.Value - maxShowingPages / 2;
            var difference = 0;
            if (pageStartIndex < 1)
            {
                difference = 1 - pageStartIndex;
                pageStartIndex = 1;
            }

            var pageEndIndex = pageNumber.Value + maxShowingPages / 2 + difference;
            if (pageEndIndex > pageCount)
            {
                if (pageStartIndex - (pageEndIndex - pageCount) > 0) pageStartIndex = pageEndIndex - pageCount;
                pageEndIndex = pageCount;
            }

            return new NewsPageParameters
            {
                PageStartIndex = pageStartIndex,
                PageEndIndex = pageEndIndex,
                CurrentPageNumber = pageNumber.Value,
                TotalPageCount = pageCount
            };
        }

        public static SelectList GetDays(this ControllerBase controller)
        {
            return new SelectList(Enumerable.Range(1, 31).Select(r => new {Id = r, Name = r}), "Id", "Name");
        }

        public static SelectList GetMonths(this ControllerBase controller)
        {
            return new SelectList(StaticResources.Months.Select(m => new {Id = m.Key, Name = m.Value}), "Id", "Name");
        }

        public static SelectList GetYears(this ControllerBase controller)
        {
            return
                new SelectList(
                    Enumerable.Range(DateTime.Now.Year - 80, 74).OrderByDescending(r => r).Select(r => new {Id = r, Name = r}),
                    "Id", "Name");
        }

        public static Tuple<string, string> GetMessage(this ControllerBase controller, string buyResult)
        {
            if (StaticResources.CheckoutResults.ContainsKey(buyResult)) return StaticResources.CheckoutResults[buyResult];
            return null;
        }
    }
}