namespace Utils.Core.Attributes
{
    public class ConditionalReadOnlyAttribute : ConditionalBaseAttribute
    {
        public ConditionalReadOnlyAttribute(string conditionalSourceField) : base(conditionalSourceField)
        {

        }
    }
}