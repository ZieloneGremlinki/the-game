using System;

namespace GreenGremlins.Dialoguer
{
    [Serializable]
    public class ExposedProperty
    {
        public static ExposedProperty CreateInstance() => new ExposedProperty();
        
        public string PropertyName = "New String";
        public string PropertyValue = "New Value";
    }
}