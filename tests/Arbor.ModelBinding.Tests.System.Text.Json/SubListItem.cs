namespace Arbor.ModelBinding.Tests.System.Text.Json
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
