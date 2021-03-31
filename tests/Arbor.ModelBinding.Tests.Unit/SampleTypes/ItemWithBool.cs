namespace Arbor.ModelBinding.Tests.Unit.SampleTypes
{
    public class ItemWithBool
    {
        public bool Enabled { get; }

        public ItemWithBool(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
