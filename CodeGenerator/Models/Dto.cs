using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CodeGenerator
{
    public class Dto
    {
        public Dto()
        {
        }

        public Dto(string procname)
        {
            Name = procname;
            ModuleName = Helper.GetModuleName(procname);
            EntityName = Helper.GetEntityName(procname);
            Columns = new List<Column>();

            using (var connection = new SqlConnection(Helper.ConnectionString, Helper.Credential))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $@"SELECT Name as FIELDNAME,
                                                system_type_name as TYPENAME,
                                                max_length as FIELDLEN, 
                                                is_nullable as NULLABLE
                                             FROM sys.dm_exec_describe_first_result_set(N'EXEC dbo.{procname}', NULL, 0)";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!string.IsNullOrEmpty(reader["FIELDNAME"].ToString()))
                            {

                                var colnameparts = reader["FIELDNAME"].ToString().Split('_');
                                var columnName = colnameparts.Take(colnameparts.Length - 0).Aggregate((f, s) => char.ToUpper(f[0]) + f.Substring(1) + char.ToUpper(s[0]) + s.Substring(1));
                                string propertyName = Helper.ToPropertyName(columnName);
                                string dataType = reader["TYPENAME"].ToString().Split('(')[0];
                                bool isNullable = Convert.ToBoolean(reader["NULLABLE"]);
                                bool isIdentity = dataType == "int identity";
                                int length = Convert.ToInt32(reader["FIELDLEN"]);
                                string type = GetType(dataType, propertyName);
#if DEBUG
                                Debug.WriteLine($"FieldName: {reader["FIELDNAME"]}  PropName: {propertyName}   Type: {dataType}  Nullable: {isNullable}  ");
#endif
                                Columns.Add(new Column() { Name = columnName ,IsNullable = isNullable ,Type = type ,IsIdentity = isIdentity ,Length = length });
                            }

                        }
                    }
                }
            }
        }

        public string Name { get; set; }

        public bool IsSelected { get; set; }
        public List<Column> Columns { get; set; }

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

    }
}