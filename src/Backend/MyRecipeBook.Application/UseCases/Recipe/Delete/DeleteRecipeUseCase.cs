using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Delete;

public class DeleteRecipeUseCase : IDeleteRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeWriteOnlyRepository _writeOnlyRepository;
    private readonly IRecipeReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRecipeUseCase(ILoggedUser loggedUser, IRecipeWriteOnlyRepository writeOnlyRepository, IRecipeReadOnlyRepository readOnlyRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(long recipeId)
    {
        var loggedUser = await _loggedUser.User();

        var recipe = await _readOnlyRepository.GetById(loggedUser, recipeId);

        if (recipe is null)
            throw new NotFoundException(ResourceMessagesExceptions.RECIPE_NOT_FOUND);

        await _writeOnlyRepository.Delete(recipeId);
       
        await _unitOfWork.Commit();
    }
}
