using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Simplifier.TestManagement.Models;

namespace Simplifier.TestManagement.Services;

public interface ISharedContext
{
    IDictionary<Guid, TestCaseModel> OriginalTestCases { get; }
}
