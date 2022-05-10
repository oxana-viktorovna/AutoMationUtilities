namespace ADOCore.Models.VariableGroups
{
    public class VariableValue
    {
        public bool IsReadOnly { get; set; }
        public bool IsSecret { get; set; }
        public string Value { get; set; }

        public VariableValue(string value, bool isSecret = false, bool isReadOnly = false)
        {
            Value = value; ;
            IsSecret = isSecret;
            IsReadOnly = isReadOnly;
        }
    }
}
