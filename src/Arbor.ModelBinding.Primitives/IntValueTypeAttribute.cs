using System;

namespace Arbor.ModelBinding.Primitives
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IntValueTypeAttribute : Attribute
    {
        public IntValueTypeAttribute(int minValue = 1) => MinValue = minValue;

        public int MinValue { get; }
    }
}