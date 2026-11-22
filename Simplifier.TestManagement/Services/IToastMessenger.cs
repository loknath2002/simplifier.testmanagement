using System;

namespace Simplifier.TestManagement.Services;

public interface IToastMessenger
{
    void Info(string theTitle, string theMessage, TimeSpan? theTimeout = null);
    void Error(string theTitle, string theMessage, TimeSpan? theTimeout = null);
    void Warning(string theTitle, string theMessage, TimeSpan? theTimeout = null);
    void Success(string theTitle, string theMessage, TimeSpan? theTimeout = null);
}
