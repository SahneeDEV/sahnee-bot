namespace SahneeBotController;

/// <summary>
/// A success data type.
/// </summary>
public interface ISuccess
{
    /// <summary>
    /// Was the operation a success?
    /// </summary>
    public bool IsSuccess { get; }
    /// <summary>
    /// The success value.
    /// </summary>
    public object? Value { get; }
    /// <summary>
    /// The actual error.
    /// </summary>
    public string Message { get; }
}

/// <summary>
/// A success data type.
/// </summary>
/// <typeparam name="T">The data type.</typeparam>
public interface ISuccess<out T>: ISuccess
{
    /// <summary>
    /// The success value.
    /// </summary>
    public new T Value { get; }

    object? ISuccess.Value => Value;
}

/// <summary>
/// A success data type.
/// </summary>
public class Success<T> : ISuccess<T>
{
    public Success(T value)
    {
        Value = value;
        IsSuccess = true;
        Message = string.Empty;
    }

    public bool IsSuccess { get; }
    public T Value { get; }
    public string Message { get; }
}

/// <summary>
/// An error data type.
/// </summary>
public class Error<T> : ISuccess<T>
{
    public Error(string error)
    {
        IsSuccess = false;
        Message = error;
    }

    public bool IsSuccess { get; }
    public T Value => throw new InvalidOperationException("Cannot get the success value of an error.");
    public string Message { get; }
}
