using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.Interfaces
{
    public interface IEntityObservable<TEntity>
    {
        void RegisterObserver(IEntityObserver observer);
        void NotifyObservers(TEntity oldEntity, TEntity newEntity, EntityState state);
    }
}
