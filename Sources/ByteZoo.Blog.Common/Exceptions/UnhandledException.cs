namespace ByteZoo.Blog.Common.Exceptions;

/// <summary>
/// Unhandled exception
/// </summary>
/// <param name="message"></param>
public class UnhandledException(string message) : Exception(message)
{
}