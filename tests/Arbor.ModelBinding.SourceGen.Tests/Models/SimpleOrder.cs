using Arbor.ModelBinding.Core;

namespace Arbor.ModelBinding.SourceGen.Tests;

[Bindable]
public class SimpleOrder
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }
}
