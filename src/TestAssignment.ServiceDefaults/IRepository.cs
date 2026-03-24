using System;
using System.Collections.Generic;
using System.Text;

namespace TestAssignment.ServiceDefaults;

public interface IRepository<T> where T : class, IAggregateRoot
{
    public IUnitOfWork UnitOfWork { get; }
}
