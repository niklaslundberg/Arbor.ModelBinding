namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class SubTypeCtor
    {
        public SubTypeCtor(string s1, string s2)
        {
            S1 = s1;
            S2 = s2;
        }

        public string S1 { get; }

        public string S2 { get; }
    }
}
