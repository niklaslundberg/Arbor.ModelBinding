using System.Collections.Generic;
using System.Threading.Tasks;
using Arbor.ModelBinding.SystemTextJson;
using FluentAssertions;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Arbor.ModelBinding.Tests.System.Text.Json
{
    public class TestComplexObject
    {
        [Fact]
        public void CreateComplexType()
        {
            object? parsed = new List<KeyValuePair<string, StringValues>>
            {
                new("SubTitle", "Titanic"),
                new("SubOtherProperty", "42"),
            }.ParseFromPairs(typeof(SubComplexType));

            parsed.Should().NotBeNull();

            if (parsed is SubComplexType sub)
            {
                sub.SubListItems.Should().NotBeNull();
                sub.SubListItems.Should().BeEmpty();
            }
            else
            {
                parsed.Should().BeOfType(typeof(SubComplexType));
            }
        }
    }
}