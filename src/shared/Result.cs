namespace SimpleMDB;

public class Result<T>
{
    public bool IsValid {get;}
    public T? Value {get;}
    public Exception? Error {get;}

    public Result(T value)
    {
        IsValid = true;
        Value = value;
    }

    public Result(Exception error)
    {
        IsValid = false;
        Error = error;
    }
}