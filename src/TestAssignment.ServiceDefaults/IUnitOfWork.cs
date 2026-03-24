using System;
using System.Collections.Generic;
using System.Text;

namespace TestAssignment.ServiceDefaults;

public interface IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}