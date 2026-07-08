namespace Arbor.ModelBinding.Core;

/// <summary>
/// Compile-time generated binder for a given type <typeparamref name="T" />.
/// </summary>
public interface IFormBinder<T>
    where T : notnull
{
    /// <summary>
    /// Binding result that carries either a valid value + diagnostic messages, or an error.
    /// </summary>
    FormBindResult<T> Bind(FormDataReader reader);
}
