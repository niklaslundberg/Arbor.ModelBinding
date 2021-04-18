using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class CustomModelBindingProvider : IModelBinderProvider
    {
        private static int _counter = 1;

        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(MyValueObject))
            {
                int name = _counter++;
                Console.WriteLine("Creating new model binder " + name);
                return new CustomModelBinder(name);
            }

            return default;
        }
    }
}