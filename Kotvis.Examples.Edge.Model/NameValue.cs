using System;

namespace Kotvis.Examples.Edge.Model
{
    public class NameValue
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static NameValue Create(string name, string value)
        {
            return new NameValue()
            {
                Name = name,
                Value = value
            };
        }
    }
}
