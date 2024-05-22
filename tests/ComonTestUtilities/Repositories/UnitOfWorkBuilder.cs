using Moq;
using MyRecipeBook.Domain.Repositories;

namespace ComonTestUtilities.Repositories;
public class UnitOfWorkBuilder
{
    public static IUnitOfWork Build()
    {
        var mock = new Mock<IUnitOfWork>();
        
        return mock.Object;
    }
}
