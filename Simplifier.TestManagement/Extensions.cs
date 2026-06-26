// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.IO;
namespace Simplifier.TestManagement;

public static class PathExtensions
{
    public static string CombineWithForwardSlash(params string[] paths)
    {
        return Path.Combine(paths).Replace('\\', '/');
    }
}