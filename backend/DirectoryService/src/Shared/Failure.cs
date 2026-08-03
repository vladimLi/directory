using System.Collections;

namespace Shared;

public class Failure : IEnumerable<Error>
{
    private readonly IEnumerable<Error> _errors;

    public Failure(IEnumerable<Error> errors)
    {
        _errors = [..errors];
    }
    public IEnumerator<Error> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public static implicit operator Failure(Error[] errors)
        => new(errors);

    public static implicit operator Failure(Error error)
        => new([error]);
}