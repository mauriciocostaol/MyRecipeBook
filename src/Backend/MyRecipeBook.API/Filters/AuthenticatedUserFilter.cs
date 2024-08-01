﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions.ExceptionsBase;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Domain.Extensions;

namespace MyRecipeBook.API.Filters;

public class AuthenticatedUserFilter: IAsyncAuthorizationFilter
{
    private readonly IAccessTokenValidator _accessTokenValidator;
    private readonly IUserReadOnlyRepository _repository;

    public AuthenticatedUserFilter(IAccessTokenValidator accessTokenValidator, IUserReadOnlyRepository repository)
    {
        _accessTokenValidator = accessTokenValidator;
        _repository = repository;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        try
        {
            var token = TokenOnRequest(context);

            var userIdentifier = _accessTokenValidator.ValidateAndGetUserIdentifier(token);

            var exist = await _repository.ExistActiveUserWithIdentifier(userIdentifier);

            if (exist.IsFalse())
            {
                throw new MyRecipeBookException(ResourceMessagesExceptions.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE);
            }

        }
        catch (MyRecipeBookException ex)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ex.Message));
        }
        catch (SecurityTokenException)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson("TokenIsExpired")
            {
                TokenIsExpired = true
            });
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesExceptions.USER_WITHOUT_PERMISSION_ACCESS_RESOURCE));
        }
    }

    private string TokenOnRequest(AuthorizationFilterContext context)
    {
        var authentication = context.HttpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authentication))
        {
            throw new MyRecipeBookException(ResourceMessagesExceptions.NO_TOKEN);
        }

        return authentication["Bearer".Length..].Trim();
    }
}