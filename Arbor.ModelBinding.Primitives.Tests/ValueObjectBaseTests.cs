using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Arbor.ModelBinding.Primitives.Tests
{
    public class ValueObjectBaseTests
    {
        [Fact]
        public void CompareToEqual()
        {
            var a = new TestValue("abc");
            var b = new TestValue("abc");

            a.CompareTo(b).Should().Be(0);
        }

        [Fact]
        public void EqualsTrue()
        {
            var a = new TestValue("abc");
            var b = new TestValue("abc");

            a.Equals(b).Should().BeTrue();
        }

        [Fact]
        public void EqualsTrueDifferentCasing()
        {
            var a = new TestValue("abc");
            var b = new TestValue("Abc");

            a.Equals(b).Should().BeTrue();
        }
    }
}
