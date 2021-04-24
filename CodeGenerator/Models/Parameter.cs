using System;

namespace CodeGenerator
{
    public class Parameter : IComparable<Parameter>
    {
        public string Name { get; set; }

        public bool IsNullable { get; set; }

        public string Type { get; set; }

        public int CompareTo(Parameter other)
        {
            return this.IsNullable.CompareTo(other.IsNullable);
        }
    }
}