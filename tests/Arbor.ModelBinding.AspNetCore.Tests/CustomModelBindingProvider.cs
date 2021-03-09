using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class CustomModelBindingProvider : IModelBinderProvider
    {

        private static int counter = 1;
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(MyValueObject))
            {
                var name = counter++;
                return new CustomModelBinder(name);
                Console.WriteLine("Creating new model binder " + name);
            }

            return default;
        }
    }
}