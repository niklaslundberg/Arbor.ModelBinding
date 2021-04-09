using System;

namespace Arbor.ModelBinding.Primitives
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LongValueTypeAttribute : Attribute
    {
        public LongValueTypeAttribute(long minValue = 1) => MinValue = minValue;

        public long MinValue { get; }
    }
}