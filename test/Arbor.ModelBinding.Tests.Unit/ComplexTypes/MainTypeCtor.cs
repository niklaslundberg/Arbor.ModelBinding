namespace Arbor.ModelBinding.Tests.Unit.ComplexTypes
{
    public class MainTypeCtor
    {
        public MainTypeCtor(string a, SubTypeCtor s)
        {
            A = a;
            S = s;
        }

        public string A { get; }

        public SubTypeCtor S { get; }
    }
}
