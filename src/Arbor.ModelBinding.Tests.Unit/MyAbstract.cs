namespace Arbor.ModelBinding.Tests.Unit
{
    public abstract class MyAbstract
    {
        public string Value { get; }

        public MyAbstract(string value)
        {
            Value = value;
        }
    }
}