using System;

namespace Arbor.ModelBinding.Primitives
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class StringValueTypeAttribute : Attribute
    {
        public StringValueTypeAttribute(StringComparison stringComparison = StringComparison.Ordinal) =>
            StringComparison = stringComparison;

        public StringComparison StringComparison { get; }
    }
}