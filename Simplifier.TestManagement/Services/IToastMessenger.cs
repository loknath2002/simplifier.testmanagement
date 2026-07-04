namespace Simplifier.TestManagement.Services;

public interface IToastMessenger
{
    void Info(string theTitle, string theMessage);
    void Error(string theTitle, string theMessage);
    void Warning(string theTitle, string theMessage);
    void Success(string theTitle, string theMessage);
}
