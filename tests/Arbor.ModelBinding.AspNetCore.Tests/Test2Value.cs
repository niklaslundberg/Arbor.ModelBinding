using System.ComponentModel;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    [TypeConverter(typeof(ParserDemoConverter))]
    public class Test2Value
    {
        public string Value { get; }

        public Test2Value(string value)
        {
            Value = value;
        }
    }
}