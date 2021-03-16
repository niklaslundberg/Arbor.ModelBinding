namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public sealed class MyOrdinalValueObject : ValueObjectBase<string>
    {
        public MyOrdinalValueObject(string value): base(value)
        {
        }
    }
}