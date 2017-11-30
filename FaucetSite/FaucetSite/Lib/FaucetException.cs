using System;

[Serializable]
public class FaucetException : Exception
{
    public FaucetException()
    { }

    public FaucetException(string message)
        : base(message)
    { }

    public FaucetException(string message, Exception innerException)
        : base(message, innerException)
    { }
}