using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Infrastructutre.Security.Tokens.Access.Generator;

namespace ComonTestUtilities.Tokens;
public class JwtTokenGeneratorBuilder
{
    public static IAccessTokenGenerator Build() => new JwtTokenGenerator(expirationTimeMinutes: 5, signingKey: "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww");
}
