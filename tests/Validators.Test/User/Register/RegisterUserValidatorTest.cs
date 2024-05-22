using ComonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;

namespace Validators.Test.User.Register;

public class RegisterUserValidatorTest
{
    [Fact]
    public void SuccessOnValidationToRegisterUser()
    {
        var validator = new RegisterUserValidator();

        var request = RequestRegisterUserJsonBuilder.Build();
        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void ErrorOnValidationToRegisterUserWhenTheNameIsEmpty()
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Name = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesExceptions.NAME_EMPTY));
    }
    [Fact]
    public void ErrorOnValidationToRegisterUserWhenEmailIsEmpty()
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Email = string.Empty;
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_EMPTY));
    }
    [Fact]
    public void ErrorOnValidationToRegisterUserWhenEmailIsInvalid()
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build();

        request.Email = "email.com";
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_INVALID));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void ErrorOnValidationToRegisterUserWhenPasswordIsInvalid(int passwordLength)
    {
        var validator = new RegisterUserValidator();
        var request = RequestRegisterUserJsonBuilder.Build(passwordLength);

        
        var result = validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle().And.Contain(error => error.ErrorMessage.Equals(ResourceMessagesExceptions.PASSWORD_INVALID));
    }
}
