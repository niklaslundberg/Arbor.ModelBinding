using System;
using System.ComponentModel.DataAnnotations;

namespace Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ClassValueTypeAttribute : Attribute
    {
        public DataType DataType { get; }

        public ClassValueTypeAttribute(DataType dataType)
        {
            DataType = dataType;
        }
    }
}