using System.Collections.Generic;

namespace Arbor.ModelBinding.Tests.System.Text.Json
{
    public class SubComplexType
    {
        public SubComplexType(string subTitle, int subOtherProperty)
        {
            SubTitle = subTitle;
            SubOtherProperty = subOtherProperty;
        }

        public string SubTitle { get; }

        public int SubOtherProperty { get; }

        public ICollection<SubListItem> SubListItems { get; set; }
    }
}
