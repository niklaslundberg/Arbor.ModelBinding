using System;
using System.ComponentModel;
using System.Globalization;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class ParserDemoConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);


        public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value switch
            {
                string stringValue => new Test2Value(stringValue),
                _ => base.ConvertFrom(context, culture, value)
            };
    }
}