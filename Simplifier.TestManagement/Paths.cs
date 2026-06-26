// /*
// * Copyright (c) 2026 Loknath, Quantum Technology
// * All rights reserved.
// */

using System;
using System.IO;
namespace Simplifier.TestManagement
{
    public static class Paths
    {
        private const string AppName = "QuantumControlCenter";

        static Paths()
        {
            EnsureDirectoriesExist();
        }

        public static string AppExecutableDirectory { get; } = AppContext.BaseDirectory;

        public static string AppDataRoot { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);

        public static string LogDirectory { get; } = Path.Combine(Paths.AppDataRoot, "Logs");

        public static string LogFilePath { get; } = Path.Combine(Paths.LogDirectory, "STestMgmt.log");
        
        public static string TempPath { get; } = Path.Combine(Paths.AppDataRoot, "Temp");

        private static void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(Paths.AppDataRoot);
            Directory.CreateDirectory(Paths.LogDirectory);
            Directory.CreateDirectory(Paths.TempPath);
        }
    }
}