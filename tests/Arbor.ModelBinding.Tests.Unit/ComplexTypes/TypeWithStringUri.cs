using System;
using System.Text.Json.Serialization;

namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class TypeWithStringUri
    {
        public Uri Url { get; }
        public TypeWithStringUri(string url) : this(new Uri(url,UriKind.RelativeOrAbsolute))
        {
        }

        #if Newtonsoft
        [Newtonsoft.Json.JsonConstructor]
        #else

        [JsonConstructor]
        #endif
        public TypeWithStringUri(Uri url)
        {
            Url = url;
        }
    }
}