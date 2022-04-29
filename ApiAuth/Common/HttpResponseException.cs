namespace ApiAuth.Common;

public class HttpResponseException : Exception
{
    public HttpResponseException(int statusCode, object? value = null )
    {
        
    }
}