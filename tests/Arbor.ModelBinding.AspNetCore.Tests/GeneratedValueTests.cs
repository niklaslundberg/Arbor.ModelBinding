using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbor.ModelBinding.AspNetCore.Tests.CodeGenParsers;
using FluentAssertions;
using Xunit;

namespace Arbor.ModelBinding.AspNetCore.Tests
{
    public class GeneratedValueTests
    {
        [Fact]
        public void NormalizedValueShouldBeLowerInvariantCaseForComparison()
        {
          var id =  new TestId("Abc");

          id.NormalizedValue.Should().Be("abc");
        }
    }
}
