using System;

namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class TypeWithUri
    {
        public Uri Url { get; }

        public TypeWithUri(Uri url)
        {
            Url = url;
        }
    }
}