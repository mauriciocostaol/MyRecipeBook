using ComonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Infrastructutre.Security.Tokens.Access.Generator;
using System.Net;

namespace WebApi.Test.User.ChangePassword;
public class ChangePasswordInvalidTokenTest: MyRecipeBookClassFixture
{
    private const string METHOD = "user/change-password";

    public ChangePasswordInvalidTokenTest(CustomWebApplicationFactory webApplication):base(webApplication)
    {
        
    }

    [Fact]
    public async Task ErrorWhenTokenIsInvalid()
    {
        var request = new RequestChangePasswordJson();

        var response = await DoPut(METHOD, request, token: "tokenInvalid");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ErrorWhenWithoutToken()
    {
        var request = new RequestChangePasswordJson();

        var response = await DoPut(METHOD, request, token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task ErrorWhenTokenWithUserNotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());  
        var request = new RequestChangePasswordJson();

        var response = await DoPut(METHOD, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
