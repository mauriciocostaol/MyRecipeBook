using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Filters;

namespace MyRecipeBook.API.Attributes;

[AttributeUsage(AttributeTargets.All)]
public sealed class AuthenticatedUserAttribute : TypeFilterAttribute
{
    public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter))
    {
    }
}
