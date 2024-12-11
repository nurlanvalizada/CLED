using System.Collections.Generic;
using CledAcademy.Core.Domain;

namespace CledAcademy.Web.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Teacher> Teachers { get; set; }
    }
}