using System;
using System.Collections.Generic;
using System.Text;

namespace TestAssignment.ServiceDefaults;

public static class SeedUsers
{
    public static readonly Guid TestUserId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid AdminUserId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");
}