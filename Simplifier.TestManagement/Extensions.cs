// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */
using System.IO;
namespace Simplifier.TestManagement;

public static class NewLineExtensions
{
    public static string EOL = "\n";
    public static char EOLC = '\n';
    
    public static string ToUnixEol(this string theLine)
    {
        return theLine.Replace("\r\n", EOL).Replace('\r', EOLC);
    }
}

public static class PathExtensions
{
    public static string CombineWithForwardSlash(params string[] paths)
    {
        return Path.Combine(paths).Replace('\\', '/');
    }
}