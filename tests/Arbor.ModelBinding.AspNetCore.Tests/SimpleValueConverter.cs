using System;
using System.ComponentModel;
using System.Globalization;
using Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class SimpleValueConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);


        public override object? ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value switch
            {
                string stringValue => new SimpleValue(stringValue),
                _ => base.ConvertFrom(context, culture, value)
            };
    }
}