namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class MainType
    {
        public string A { get; set; }

        public SubType S { get; set; }
    }

    public class SubType
    {
        public string S1 { get; set; }

        public string S2 { get; set; }
    }
}
