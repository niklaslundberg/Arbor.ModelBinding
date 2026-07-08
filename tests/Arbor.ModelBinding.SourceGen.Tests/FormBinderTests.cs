using System.Collections.Generic;
using Arbor.ModelBinding.Core;
using FluentAssertions;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace Arbor.ModelBinding.SourceGen.Tests;

public class FormBinderTests
{
    [Fact]
    public void FlatPoco_Binds()
    {
        var pairs = new List<KeyValuePair<string, StringValues>>
        {
            new("Id", "42"),
            new("Name", "Arbor"),
            new("IsEnabled", "on"),
        };

        var binder = new Generated.FormBinder_SimpleOrder();
        var reader = new FormDataReader(pairs);

        FormBindResult<SimpleOrder> result = binder.Bind(reader);

        result.IsSuccess.Should().BeTrue();
        SimpleOrder order = result.Value;
        order.Id.Should().Be(42);
        order.Name.Should().Be("Arbor");
        order.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void MissingRequiredInt_ReturnsFailure()
    {
        var pairs = new List<KeyValuePair<string, StringValues>>
        {
            new("Name", "Arbor"),
            new("IsEnabled", "off"),
        };

        var binder = new Generated.FormBinder_SimpleOrder();
        var reader = new FormDataReader(pairs);

        FormBindResult<SimpleOrder> result = binder.Bind(reader);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Bool_FromOn_IsTrue()
    {
        var pairs = new List<KeyValuePair<string, StringValues>>
        {
            new("Id", "1"),
            new("Name", "X"),
            new("IsEnabled", "on"),
        };

        var binder = new Generated.FormBinder_SimpleOrder();
        var reader = new FormDataReader(pairs);

        var result = binder.Bind(reader);
        result.Value.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Bool_FromZero_IsFalse()
    {
        var pairs = new List<KeyValuePair<string, StringValues>>
        {
            new("Id", "1"),
            new("Name", "X"),
            new("IsEnabled", "0"),
        };

        var binder = new Generated.FormBinder_SimpleOrder();
        var reader = new FormDataReader(pairs);

        var result = binder.Bind(reader);
        result.Value.IsEnabled.Should().BeFalse();
    }
}
