namespace FileApp.Exceptions;

public class UnAuthorizedAccessException : Exception
{
    private readonly string _message = "Unauthorized access to the resource";
    public UnAuthorizedAccessException(string message) => _message = message;
    public override string Message => _message;
}