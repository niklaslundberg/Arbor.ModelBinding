using System;

namespace Arbor.ModelBinding.Primitives
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LongValueTypeAttribute : Attribute
    {
        public LongValueTypeAttribute(int minValue = 1) => MinValue = minValue;
        public int MinValue { get; }
    }
}