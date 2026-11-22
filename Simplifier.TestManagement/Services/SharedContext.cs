using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement.Services;

public class SharedContext : ISharedContext
{
    public IDictionary<Guid, TestCaseModel> OriginalTestCases { get; } = new Dictionary<Guid, TestCaseModel>();

    public void Clear()
    {
        OriginalTestCases.Clear();
    }
}
