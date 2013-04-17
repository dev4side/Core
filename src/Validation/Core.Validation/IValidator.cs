using System;

namespace Core.Validation
{
    public interface IValidator<T> 
    {
        ValidationResult Validate(T obj);
    }
}