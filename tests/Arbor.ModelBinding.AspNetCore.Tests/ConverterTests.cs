using Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers;
using FluentAssertions;
using Xunit;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class CodGenParserTests
    {
        [Fact]
        public void Do()
        {
            Partial1Parser.Consume();
        }
    }

    public class ConverterTests
    {

        [Fact]
        public void ValueObjectIgnoreCaseCompare()
        {
            var myValueObject = new MyValueObject("Abc");
            var valueObject = new MyValueObject("abc");
            myValueObject.Equals(valueObject).Should().BeTrue();
            myValueObject.GetHashCode().Should().Be(valueObject.GetHashCode());
        }

        [Fact]
        public void ValueObjectCompareSameIntValue()
        {
            var myValueObject = new MyIntValueObject(1);
            var valueObject = new MyIntValueObject(1);
            myValueObject.Equals(valueObject).Should().BeTrue();
            myValueObject.GetHashCode().Should().Be(valueObject.GetHashCode());
        }

        [Fact]
        public void ValueObjectCompareStringWithInt()
        {
            var myValueObject = new MyIntValueObject("1");
            var valueObject = new MyIntValueObject(1);
            myValueObject.Equals(valueObject).Should().BeTrue();
            myValueObject.GetHashCode().Should().Be(valueObject.GetHashCode());
        }

        [Fact]
        public void ValueObjectOridnalCompareCasing()
        {
            var myValueObject = new MyOrdinalValueObject("Abc");
            var valueObject = new MyOrdinalValueObject("abc");
            myValueObject.Equals(valueObject).Should().BeFalse();
            myValueObject.GetHashCode().Should().NotBe(valueObject.GetHashCode());
        }
    }
}