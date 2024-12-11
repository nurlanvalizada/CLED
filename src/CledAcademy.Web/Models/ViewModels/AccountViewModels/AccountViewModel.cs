using System.Collections.Generic;
using CledAcademy.Core.Domain;

namespace CledAcademy.Web.Models.ViewModels.AccountViewModels
{
    public class AccountViewModel
    {
        public IEnumerable<StudentOrderViewModel> StudentOrders { get; set; }
        public IEnumerable<AccountTopUp> AccountTopUps { get; set; }
    }
}