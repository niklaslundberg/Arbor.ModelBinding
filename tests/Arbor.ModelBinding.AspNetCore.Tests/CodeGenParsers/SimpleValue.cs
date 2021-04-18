using System.ComponentModel;

namespace Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers
{
    [TypeConverter(typeof(SimpleValueConverter))]
    public class SimpleValue
    {
        public string Value { get; }

        public SimpleValue(string value) => Value = value;
    }
}