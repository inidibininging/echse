namespace Echse.Domain
{
    public class Variable
    {
        private const string UnknownScope = "???";
        private const string UnknownVariableName = UnknownScope;
        public string Scope { get; set; }
        public string Name { get; set; }
        public string Id
        {
            get  => $"{(string.IsNullOrWhiteSpace(Scope) ? UnknownScope : Scope)}_{(string.IsNullOrWhiteSpace(Name) ? UnknownVariableName : Name)}"; // TODO: get the scope
            set  => Name = value; // TODO: with scope
        }
    }
}