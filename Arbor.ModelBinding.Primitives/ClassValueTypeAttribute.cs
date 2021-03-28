using System;

namespace Arbor.ModelBinding.Primitives
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ClassValueTypeAttribute : Attribute
    {
        public ClassValueTypeAttribute(BackingValueType dataType) => DataType = dataType;

        public BackingValueType DataType { get; }
    }
}