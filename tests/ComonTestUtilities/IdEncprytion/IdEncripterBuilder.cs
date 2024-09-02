using Sqids;

namespace ComonTestUtilities.IdEncprytion;

public class IdEncripterBuilder
{
    public static SqidsEncoder<long> Build()
    {
        return new SqidsEncoder<long>(new()
        {
            MinLength = 3,
            Alphabet = "KbLfwg3Ivl6oMCV1uUiJ9PBj4k0a2hcen8pOzZAyqdNQ7m5WFxYGDXEH"
        });
    }
}
