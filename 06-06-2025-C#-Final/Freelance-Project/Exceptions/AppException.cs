using System;

namespace Freelance_Project.Misc;

public class AppException : Exception
{
    public int StatusCode { get; }
    public Dictionary<string, string[]> Errors { get; }

    public AppException(string message, int statusCode = 400, Dictionary<string, string[]> errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}
