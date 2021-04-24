using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CodeGenerator
{
    public class Table
    {
        public Table()
        {
        }

        public Table(string name)
        {
            Name = name;
            ModuleName = Helper.GetModuleName(name);

            EntityName = Helper.GetEntityName(name);

            StringBuilder sbEntityBase = new StringBuilder();
            StringBuilder sbEntityFrontEnd = new StringBuilder();
            StringBuilder sbEntityBackEnd = new StringBuilder();
            StringBuilder sbEntityBackEndConstructor = new StringBuilder();

            Columns = new List<Column>();

            using (var connection = new SqlConnection(Helper.ConnectionString, Helper.Credential))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "sp_columns";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@table_name", name);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string columnName = reader["COLUMN_NAME"].ToString();
                            string propertyName = Helper.ToPropertyName(columnName);

                            string dataType = reader["TYPE_NAME"].ToString();
                            bool isNullable = Convert.ToBoolean(reader["NULLABLE"]);
                            bool isIdentity = dataType == "int identity";
                            int length = Convert.ToInt32(reader["LENGTH"]);
                            string type = GetType(dataType, propertyName);

                            Columns.Add(new Column() { Name = columnName, IsNullable = isNullable, Type = type, IsIdentity = isIdentity, Length = length });
                        }
                    }
                }
            }
        }

        public string Name { get; set; }

        public List<Column> Columns { get; set; }

        public string MethodName { get; set; }

        public string ModuleName { get; set; }

        public string EntityName { get; set; }

        private string GetType(string dataType, string propertyName)
        {
            string type = dataType;

            switch (type)
            {
                case "varchar":
                case "char":
                case "nvarchar":
                case "nchar":
                case "text":
                case "ntext":
                    type = "string";
                    break;

                case "int identity":
                    type = "int";
                    break;

                case "datetime":
                case "date":
                case "smalldatetime":
                case "datetime2":
                    type = "DateTime";
                    break;

                case "time":
                    type = "TimeSpan";
                    break;

                case "datetimeoffset":
                    type = "DateTimeOffset";
                    break;

                case "bit":
                    type = "bool";
                    break;

                case "money":
                    type = "decimal";
                    break;

                case "tinyint":
                    if (propertyName == "Visibility")
                    {
                        type = "Visibility";
                    }
                    else
                    {
                        type = "byte";
                    }

                    break;

                case "smallint":
                    type = "short";
                    break;

                default:
                    break;
            }

            return type;
        }

        public static List<Table> GetList()
        {
            List<Table> tables = new List<Table>();

            using (var connection = new SqlConnection(Helper.ConnectionString, Helper.Credential))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_tables";
                    command.Parameters.AddWithValue("@table_owner", "dbo");
                    command.Parameters.AddWithValue("@table_type", "'TABLE'");

                    connection.Open();


                    tables.Add(new Table() { Name = "Select All" });
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(new Table() { Name = reader["TABLE_NAME"].ToString().Split(';')[0] });
                        }
                    }
                }
            }

            return tables;
        }
    }
}