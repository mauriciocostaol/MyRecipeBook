using System.Net;

namespace MyRecipeBook.Exceptions.ExceptionsBase;

public class UnathorizedException : MyRecipeBookException
{
    public UnathorizedException(string message):base(message)
    {
        
    }
    public override IList<string> GetErrorMessages() =>[Message];
    

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
    
    
}
