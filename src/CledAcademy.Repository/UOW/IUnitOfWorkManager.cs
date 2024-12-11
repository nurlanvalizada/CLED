using System;
using CledAcademy.Core;
using CledAcademy.Repository.Abstract;
using CledAcademy.Repository.Base;

namespace CledAcademy.Repository.UOW
{
    public interface IUnitOfWorkManager : IDisposable
    {
        ITestAnswerRepository TestAnswerRepository { get; }

        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;

        void Commit();
    }
}
