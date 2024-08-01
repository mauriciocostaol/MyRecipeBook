using ComonTestUtilities.Cryptography;
using ComonTestUtilities.Mapper;
using ComonTestUtilities.Repositories;
using ComonTestUtilities.Requests;
using ComonTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Test.User.Register;
public class RegisterUserUseCaseTest
{
    [Fact]
    public async Task SuccessOnRegisterUserWhenRequesiIsValid()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Tokens.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Tokens.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ErrorOnRegisterWhenEmailAlreadyRegistered()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        
        var useCase = CreateUseCase(request.Email);

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
             .Where(error => error.ErrorMessages.Count == 1 && error.ErrorMessages.Contains(ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));
       
       

    }


    [Fact]
    public async Task ErrorOnRegisterWhenNameIsEmpty()
    {
        var request = RequestRegisterUserJsonBuilder.Build();
        request.Name = string.Empty;
        var useCase = CreateUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
             .Where(error => error.ErrorMessages.Count == 1 && error.ErrorMessages.Contains(ResourceMessagesExceptions.NAME_EMPTY));



    }


    private static RegisterUserUseCase CreateUseCase(string? email = null)
    {

        var mapper = MapperBuilder.Build();
        var passwordEncripter = PasswordEncripterBuilder.Build();
        var writeOnlyRepostory = UserWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var readOnlyRepostoryBuilder = new UserReadOnlyRepositoryBuilder();
        var accessTokenGenerator = JwtTokenGeneratorBuilder.Build();

        if (!string.IsNullOrEmpty(email))
            readOnlyRepostoryBuilder.ExistActiveUserWithEmail(email);


        return new RegisterUserUseCase(mapper, writeOnlyRepostory, readOnlyRepostoryBuilder.Build(), passwordEncripter, unitOfWork,accessTokenGenerator);
    }
}
