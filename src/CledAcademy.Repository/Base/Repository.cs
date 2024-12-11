using System;
using System.Linq;
using System.Linq.Expressions;
using CledAcademy.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CledAcademy.Repository.Base
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        public TEntity GetSingle(int id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.FirstOrDefault(e => e.Id == id);
        }

        public TEntity GetSingle(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.FirstOrDefault(predicate);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Any(predicate);
        }

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = Context.Set<TEntity>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.Where(predicate);
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Count(predicate);
        }

        public virtual void Add(TEntity entity)
        {
            EntityEntry dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Added;
        }

        public virtual void Update(TEntity entity)
        {
            EntityEntry dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            EntityEntry dbEntityEntry = Context.Entry(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public IQueryable<TEntity> DeleteWhere(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = Context.Set<TEntity>().Where(predicate);

            foreach (var entity in entities)
                Context.Entry(entity).State = EntityState.Deleted;

            return entities;
        }

        public virtual void Dispose()
        {
            if (Context == null) return;
            Context.Dispose();
            Context = null;
        }
    }
}