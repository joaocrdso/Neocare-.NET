namespace Neocare.Domain.Exceptions;

public class DomainException : Exception
{
    public int StatusCode { get; }

    public DomainException(string message, int statusCode = 400)
        : base(message) => StatusCode = statusCode;
}

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object id)
        : base($"{entity} com id '{id}' não encontrado.", 404) { }
}

public class ValidationException : DomainException
{
    public ValidationException(string message)
        : base(message, 422) { }
}
