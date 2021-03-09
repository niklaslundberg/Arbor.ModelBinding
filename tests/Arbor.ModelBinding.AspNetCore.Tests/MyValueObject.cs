using System;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public sealed class MyValueObject : ValueObject<string>
    {
        public MyValueObject(string value): base(value, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}