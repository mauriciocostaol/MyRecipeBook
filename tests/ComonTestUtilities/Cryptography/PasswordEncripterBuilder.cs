using MyRecipeBook.Domain.Security.Cryptography;
using MyRecipeBook.Infrastructutre.Security.Cryptography;

namespace ComonTestUtilities.Cryptography;
public class PasswordEncripterBuilder
{
    public static IPasswordEncripter Build() => new SHA512Encripter("abc1234");
    

    
}
