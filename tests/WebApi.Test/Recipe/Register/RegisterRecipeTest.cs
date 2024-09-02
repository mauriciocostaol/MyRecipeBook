using System.Globalization;
using System.Net;
using System.Text.Json;
using ComonTestUtilities.Requests;
using ComonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Exceptions;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Register;

public class RegisterRecipeTest : MyRecipeBookClassFixture
{
    private const string METHOD = "recipe";
    private readonly Guid _userIdentifier;
    public RegisterRecipeTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.GetUserIdentifier();
    }

    [Fact]
    public async Task SuccessOnRequestToRegisterRecipe()
    {
        var request = RequestRecipeJsonBuilder.Build();
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(method:METHOD,request:request,token:token);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
        responseData.RootElement.GetProperty("id").GetString().Should().NotBeNullOrWhiteSpace();
    }


    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task ErrrorOnRequestWhenTitleIsEmpty(string culture)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

         var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPost(method:METHOD,request: request,token:token,culture:culture);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);


        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesExceptions.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(c => c.GetString()!.Equals(expectedMessage));

    }
}
