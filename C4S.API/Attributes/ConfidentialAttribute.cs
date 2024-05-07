namespace C4S.API.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ConfidentialAttribute : Attribute
    {
        public ConfidentialAttribute()
        {
        }
    }
}