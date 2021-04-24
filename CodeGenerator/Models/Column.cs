namespace CodeGenerator
{
    public class Column
    {
        public string Name { get; set; }

        public bool IsNullable { get; set; }

        public string Type { get; set; }

        public int Length { get; set; }

        public bool IsIdentity { get; set; }
    }
}