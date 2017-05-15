﻿namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class SubListItem
    {
        public string Note { get; set; }

        public override string ToString()
        {
            return $"{nameof(Note)}: '{Note}'";
        }
    }
}
