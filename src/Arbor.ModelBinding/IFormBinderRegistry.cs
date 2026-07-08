namespace Arbor.ModelBinding.Core;

public interface IFormBinderRegistry
{
    bool TryGet<T>(out IFormBinder<T> binder) where T : notnull;
}
