using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using System.Net;

namespace MyRecipeBook.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is MyRecipeBookException myRecipeBookException)
        {
            HandleProjectException(myRecipeBookException,context);
        }
        else
        {
            ThrowUnknownException(context);
        }
    }

    private static void HandleProjectException(MyRecipeBookException myRecipeBookException, ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)myRecipeBookException.GetStatusCode();
        context.Result = new ObjectResult(new ResponseErrorJson(myRecipeBookException.GetErrorMessages()));

    }


    private static void ThrowUnknownException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesExceptions.UNKNOWN_ERROR));
    }
}
