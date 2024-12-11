using System;
using System.Collections.Generic;
using System.Linq;
using CledAcademy.Core;
using CledAcademy.Core.Domain;
using CledAcademy.Repository.Abstract;
using CledAcademy.Repository.Base;
using CledAcademy.Repository.Concret;
using Microsoft.EntityFrameworkCore;

namespace CledAcademy.Repository.UOW
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private ITestAnswerRepository _testAnswerRepository;

        private readonly Dictionary<Type, object> _repositories;
        private readonly DbContext _context;
        private bool _disposed;

        public UnitOfWorkManager(DbContext context)
        {
            _repositories = new Dictionary<Type, object>();
            _context = context;
        }

        public ITestAnswerRepository TestAnswerRepository
            =>
            _testAnswerRepository ?? (_testAnswerRepository = new TestAnswerRepository(_context, Repository<Answer>()));

        public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            if (_repositories.Keys.Contains(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;
            }
            IRepository<TEntity> repository = new Repository<TEntity>(_context);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}