using AutoMapper;
using MyRecipeBook.Application.Services.Cryptography;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;
using System.Reflection;

namespace MyRecipeBook.Application.UseCases.User.Register;
public class RegisterUserUseCase : IRegisterUseCase
{
    private readonly IMapper _mapper;
    private readonly IUserWriteOnlyRepository _writeOnlyRepository;
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly PasswordEncripter _passwordEncripter;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterUserUseCase(IMapper mapper, IUserWriteOnlyRepository writeOnlyRepository, IUserReadOnlyRepository readOnlyRepository, PasswordEncripter passwordEncripter, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _passwordEncripter = passwordEncripter;
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseRegisteredUserJson> Execute(RequestRegisterUserJson request)
    {

        
        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password =_passwordEncripter.Encrypt(request.Password);
        await Validate(request);

        await _writeOnlyRepository.Add(user);
        await _unitOfWork.Commit();
        return new ResponseRegisteredUserJson
        {
            Name = user.Name,
        };
    }

    private async Task Validate(RequestRegisterUserJson request)
    {
        var validator = new RegisterUserValidator();

        var result = validator.Validate(request);
       var emailExist = await _readOnlyRepository.ExistActiveUserWithEmail(request.Email);

        if (emailExist)
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty,ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));
        }
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }


}
