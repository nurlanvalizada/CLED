using System.Collections.Generic;
using System.Linq;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Repository.Abstract;
using CledAcademy.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace CledAcademy.Repository.Concret
{
    public class TestAnswerRepository : Repository<TestAnswer>, ITestAnswerRepository
    {
        private readonly IRepository<Answer> _answerRepository;

        public TestAnswerRepository(DbContext context, IRepository<Answer> answerRepository) : base(context)
        {
            _answerRepository = answerRepository;
        }

        public override void Dispose()
        {
            base.Dispose();
            _answerRepository.Dispose();
        }

        public IEnumerable<TestCorrectness> CheckTestAnswers(IEnumerable<ClosedTestAnswer> closedTestAnswers, IEnumerable<OpenTestAnswer> openTestAnswers)
        {
            var results = (from closedTestAnswer in closedTestAnswers
                           let testAnswer =
                           Context.Set<TestAnswer>().FirstOrDefault(ta => ta.TestId == closedTestAnswer.TestId)
                           select
                           new TestCorrectness
                           {
                               TestId = testAnswer.TestId,
                               AnswerId = testAnswer.AnswerId,
                               TestType = TestType.Closed,
                               IsCorrect = closedTestAnswer.AnswerId == testAnswer.AnswerId
                           }
                    ).ToList();

            foreach (var openTestAnswer in openTestAnswers)
            {
                var testAnswer = Context.Set<TestAnswer>().FirstOrDefault(ta => ta.TestId == openTestAnswer.TestId);
                var findAnswer = _answerRepository.GetSingle(a => a.TestId == openTestAnswer.TestId && a.Text == openTestAnswer.AnswerText);
                if (findAnswer != null)
                {
                    results.Add(new TestCorrectness
                        {
                            TestId = testAnswer.TestId,
                            AnswerId = 0,
                            AnswerText =  _answerRepository.GetSingle(a=>a.TestId == testAnswer.TestId && a.Id == testAnswer.AnswerId).Text,
                            TestType = TestType.OpenValue,
                            IsCorrect = findAnswer.Id == testAnswer.AnswerId
                        });
                }
                else
                {
                    results.Add(new TestCorrectness
                    {
                        TestId = openTestAnswer.TestId,
                        AnswerId = 0,
                        AnswerText = _answerRepository.GetSingle(a => a.TestId == testAnswer.TestId && a.Id == testAnswer.AnswerId).Text,
                        TestType = TestType.OpenValue,
                        IsCorrect = false
                    });
                }
            }

            return results;
        }
    }
}