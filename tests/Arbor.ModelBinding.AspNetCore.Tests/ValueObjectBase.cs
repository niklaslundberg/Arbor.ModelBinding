using System;
using System.Collections.Generic;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public abstract class ValueObjectBase<T> : IEquatable<ValueObjectBase<T>>
    {
        private readonly IComparer<T>? _comparer;

        protected ValueObjectBase(T value, IComparer<T>? comparer = null)
        {
            Value = value;
            _comparer = comparer;
        }

        public bool Equals(ValueObjectBase<T>? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (_comparer is { })
            {
                return _comparer.Compare(Value, other.Value) == 0;
            }

            return EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object? obj) => Equals(obj as ValueObjectBase<T>);


        public override int GetHashCode()
        {
            if (_comparer is StringComparer comparer && comparer == StringComparer.OrdinalIgnoreCase && Value is string stringValue)
            {
                return stringValue.ToLowerInvariant().GetHashCode();
            }

            return EqualityComparer<T>.Default.GetHashCode(Value!);
        }

        public static bool operator ==(ValueObjectBase<T>? left, ValueObjectBase<T>? right) => Equals(left, right);

        public static bool operator !=(ValueObjectBase<T>? left, ValueObjectBase<T>? right) => !Equals(left, right);

        public T Value { get; }
    }
}