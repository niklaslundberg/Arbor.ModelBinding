namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public sealed class MyOrdinalValueObject : ValueObject<string>
    {
        public MyOrdinalValueObject(string value): base(value)
        {
        }
    }
}