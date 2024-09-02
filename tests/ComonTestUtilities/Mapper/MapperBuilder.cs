using AutoMapper;
using ComonTestUtilities.IdEncprytion;
using MyRecipeBook.Application.Services.AutoMapper;

namespace ComonTestUtilities.Mapper;
public class MapperBuilder
{
    public static IMapper Build()
    {
        var idEncripter = IdEncripterBuilder.Build();
       return new MapperConfiguration(options =>
        {
            options.AddProfile(new AutoMapping(idEncripter));
        }).CreateMapper();
    }
}
