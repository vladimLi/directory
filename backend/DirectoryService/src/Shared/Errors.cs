using System.Collections;

namespace Shared;

public class Errors : IEnumerable<Error>
{
    private readonly IEnumerable<Error> _errors;

    public Errors(IEnumerable<Error> errors)
    {
        _errors = [..errors];
    }
    public IEnumerator<Error> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public static implicit operator Errors(Error[] errors)
        => new(errors);

    public static implicit operator Errors(Error error)
        => new([error]);
}