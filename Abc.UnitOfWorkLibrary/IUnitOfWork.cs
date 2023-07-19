using ABC.BusinessBase;
using ABC.Models;
using Microsoft.EntityFrameworkCore;

namespace Abc.UnitOfWorkLibrary
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
        
        void Rollback();

        IReposistory<T> Repository<T>() where T : class, TEntity, new();

    }
}