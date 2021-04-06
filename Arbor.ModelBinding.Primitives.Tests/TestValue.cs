using System;

namespace Arbor.ModelBinding.Primitives.Tests
{
    public class TestValue : ValueObjectBase<string>
    {
        public TestValue(string value) : base(value, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}