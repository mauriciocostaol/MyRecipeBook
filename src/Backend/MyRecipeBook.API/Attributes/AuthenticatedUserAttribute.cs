﻿using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Filters;

namespace MyRecipeBook.API.Attributes;

public sealed class AuthenticatedUserAttribute : TypeFilterAttribute
{
    public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter))
    {
    }
}
