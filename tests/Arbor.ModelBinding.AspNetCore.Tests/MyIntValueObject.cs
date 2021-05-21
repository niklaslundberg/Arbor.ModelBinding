using Arbor.ModelBinding.Primitives;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public sealed class MyIntValueObject : ValueObjectBase<int>
    {
        public MyIntValueObject(int value): base(value)
        {
        }

        public MyIntValueObject(string value) : this(int.Parse(value))
        {

        }
    }
}