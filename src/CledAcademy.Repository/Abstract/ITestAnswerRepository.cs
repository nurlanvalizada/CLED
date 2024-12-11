using System.Collections.Generic;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Repository.Base;

namespace CledAcademy.Repository.Abstract
{
    public interface ITestAnswerRepository : IRepository<TestAnswer>
    {
        IEnumerable<TestCorrectness> CheckTestAnswers(IEnumerable<ClosedTestAnswer> closedTestAnswers, IEnumerable<OpenTestAnswer> openTestAnswers);
    }
}