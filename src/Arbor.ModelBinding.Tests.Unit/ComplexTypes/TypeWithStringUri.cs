using System;

namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class TypeWithStringUri
    {
        public TypeWithStringUri(string url)
        {
            _ = Uri.TryCreate(url, UriKind.Absolute, out var uri);

            Url = uri;
        }

        public Uri Url { get; }
    }
}