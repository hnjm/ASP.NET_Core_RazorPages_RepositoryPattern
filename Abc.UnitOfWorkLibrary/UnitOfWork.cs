using Abc.AuthorLibrary;
using ABC.BooksLibrary;
using ABC.BusinessBase;
using ABC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc.UnitOfWorkLibrary
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly DbContext _context;
        private bool _disposed;
        
        public UnitOfWork(DbContext context)
        {
            this._context = context;          
        }

        public async Task<int> CommitAsync()
        {
           return await this._context.SaveChangesAsync();
        }

        public void Rollback()
        {
            foreach (var item in this._context.ChangeTracker.Entries())
            {
                switch (item.State)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        item.State = EntityState.Unchanged;
                        break;
                    case EntityState.Modified:
                        item.CurrentValues.SetValues(item.OriginalValues);
                        item.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        item.State = EntityState.Detached;
                        break;
                    default:
                        break;
                }
            }
        }

        public IReposistory<T> Repository<T>() where T: class, TEntity, new()
        {
            return new Repository<T>(this._context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._context.Dispose();
                }
            }

            this._disposed = true;
        }

    }
}
