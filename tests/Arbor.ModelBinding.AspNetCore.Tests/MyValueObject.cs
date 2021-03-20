using System;
using Arbor.ModelBinding.Primitives;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public sealed class MyValueObject : ValueObjectBase<string>
    {
        public MyValueObject(string value): base(value, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}