namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public sealed    class MyIntValueObject : ValueObject<int>
    {
        public MyIntValueObject(int value): base(value)
        {
        }

        public MyIntValueObject(string value) : this(int.Parse(value))
        {

        }
    }
}