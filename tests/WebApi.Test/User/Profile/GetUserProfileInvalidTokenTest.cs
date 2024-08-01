using ComonTestUtilities.Tokens;
using FluentAssertions;
using System.Net;

namespace WebApi.Test.User.Profile;
public class GetUserProfileInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string MTEHOD = "user";

    public GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {

    }

    [Fact]
    public async Task ErrorWhenTokenInvalid()
    {
        var response = await DoGet(MTEHOD, token: "tokenInvalid");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ErrorWhenWithoutToken()
    {
        var response = await DoGet(MTEHOD, token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ErrorWhenTokenWithUserNotFound()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());
        var response = await DoGet(MTEHOD, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
