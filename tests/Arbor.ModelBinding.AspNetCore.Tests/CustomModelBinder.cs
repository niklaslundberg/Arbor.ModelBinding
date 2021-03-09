using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class CustomModelBinder : IModelBinder
    {
        private readonly int _name;

        public CustomModelBinder(int name)
        {
            _name = name;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Console.WriteLine("Running bind " + _name);
            if (bindingContext.ModelType == typeof(MyValueObject) && bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue is { } value)
            {
                bindingContext.Result = ModelBindingResult.Success(new MyValueObject(value));
            }

            return Task.CompletedTask;
        }
    }
}