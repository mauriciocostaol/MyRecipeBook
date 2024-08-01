using ComonTestUtilities.Requests;
using FluentAssertions;
using System.Net;

namespace WebApi.Test.User.Update;
public class UpdateUserInvalidTokenTest :MyRecipeBookClassFixture
{
    private const string METHOD = "user";

    public UpdateUserInvalidTokenTest(CustomWebApplicationFactory factory):base(factory)
    {
        
    }

    [Fact]
    public async Task ErrorWhenTokenIsInvalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        var response = await DoPut(METHOD, request,"tokenInvalid");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ErrorWhenWithoutToken()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        var response = await DoPut(METHOD, request, token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
