using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace CodeGenerator.ViewModels
{
    public class DtoViewModel : ViewModelBase
    {
        private string _baseCode;
        private Table _selectedTable;
        private List<Table> _tables;
        private List<StoredProcedure> _procs;

        public DtoViewModel()
        {
            GenerateCommand = new RelayCommand(param => Generate(), param => CanExecuteGenerateCommand());
        }
        public string BaseCode
        {
            get
            {
                return _baseCode;
            }

            set
            {
                if (_baseCode != value)
                {
                    _baseCode = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<StoredProcedure> StoredProcedures
        {
            get
            {
                return _procs;
            }

            set
            {
                if (_procs != value)
                {
                    _procs = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Table> Tables
        {
            get
            {
                return _tables;
            }

            set
            {
                if (_tables != value)
                {
                    _tables = value;
                    OnPropertyChanged();
                }
            }
        }
        public Table SelectedTable
        {
            get
            {
                return _selectedTable;
            }

            set
            {
                if (_selectedTable != value)
                {
                    _selectedTable = value;
                    OnPropertyChanged();
                    BaseCode = string.Empty;
                }
            }
        }
        public RelayCommand GenerateCommand { get; private set; }
        private void Generate()
        {
            var procName = "";
            var query = $"SELECT * FROM sys.dm_exec_describe_first_result_set(N'EXEC dbo.{procName}', NULL, 0)"; 
            Table table = new Table(_selectedTable.Name);

            StringBuilder sbEntityBase = new StringBuilder();
            StringBuilder sbEntityFrontEnd = new StringBuilder();
            StringBuilder sbEntityBackEnd = new StringBuilder();
            StringBuilder sbEntityBackEndConstructor = new StringBuilder();

            foreach (Column column in table.Columns)
            {
                string propertyName = Helper.ToPropertyName(column.Name, table.Name);

                // Entity Base
                if (column.Name.EndsWith("StringID"))
                {
                    propertyName = propertyName.Substring(0, propertyName.Length - 8);

                    if (sbEntityFrontEnd.Length > 0)
                    {
                        sbEntityFrontEnd.AppendLine(System.Environment.NewLine);
                    }

                    sbEntityFrontEnd.AppendLine("\t\t[DataMember]");
                    sbEntityFrontEnd.AppendFormat("\t\tpublic string {0} {{ get; set; }}", propertyName);

                    if (sbEntityBackEnd.Length > 0)
                    {
                        sbEntityBackEnd.AppendLine(System.Environment.NewLine);
                    }

                    sbEntityBackEndConstructor.AppendLine(string.Format("\t\t\t{0} = new StringLanguage();", propertyName));

                    if (!column.IsNullable)
                    {
                        sbEntityBackEnd.AppendLine("\t\t[Required]");
                    }

                    sbEntityBackEnd.AppendLine("\t\t[StringLanguageValidator]");
                    sbEntityBackEnd.AppendFormat("\t\tpublic StringLanguage {0} {{ get; set; }}", propertyName);
                }
                else if (Helper.BackEndColumns.Contains(propertyName))
                {
                    if (sbEntityBackEnd.Length > 0)
                    {
                        sbEntityBackEnd.AppendLine(System.Environment.NewLine);
                    }

                    if (column.Type == "string" && column.Length <= 8000)
                    {
                        sbEntityBackEnd.AppendFormat("\t\t[MaxLength({0})]" + System.Environment.NewLine, column.Length);
                    }

                    if (propertyName.EndsWith("ID"))
                    {
                        propertyName = propertyName.Substring(0, propertyName.Length - 1) + "d";
                    }

                    if (propertyName.ToUpper() != column.Name.ToUpper())
                    {
                        sbEntityBase.AppendFormat("\t\t[Column(\"{0}\")]" + System.Environment.NewLine, column.Name);
                    }

                    sbEntityBackEnd.AppendFormat("\t\tpublic {0}{1} {2} {{ get; set; }}", column.Type, column.IsNullable && column.Type != "string" ? "?" : string.Empty, propertyName);
                }
                else
                {
                    if (sbEntityBase.Length > 0)
                    {
                        sbEntityBase.AppendLine(System.Environment.NewLine);
                    }

                    if (!Helper.NonSerializedColumns.Contains(propertyName))
                    {
                        sbEntityBase.AppendLine("\t\t[DataMember]");
                    }

                    if (!column.IsNullable)
                    {
                        sbEntityBase.AppendLine("\t\t[Required]");
                    }

                    if (column.Name.EndsWith("ID"))
                    {
                        if (column.IsIdentity)
                        {
                            propertyName = "Id";
                        }

                        if (!string.IsNullOrEmpty(table.ModuleName))
                        {
                            propertyName = propertyName.Replace(table.ModuleName, string.Empty);
                        }

                        if (!string.IsNullOrEmpty(table.EntityName))
                        {
                            propertyName = propertyName.Replace(table.EntityName, string.Empty);
                        }
                    }
                    else if (column.Type == "string")
                    {
                        if (column.Name.EndsWith("Code"))
                        {
                            if (!string.IsNullOrEmpty(table.ModuleName))
                            {
                                propertyName = propertyName.Replace(table.ModuleName, string.Empty);
                            }

                            if (!string.IsNullOrEmpty(table.EntityName))
                            {
                                propertyName = propertyName.Replace(table.EntityName, string.Empty);
                            }
                        }

                        if (column.Length <= 8000)
                        {
                            sbEntityBase.AppendFormat("\t\t[MaxLength({0})]" + System.Environment.NewLine, column.Length);
                        }
                    }

                    if (propertyName.EndsWith("ID"))
                    {
                        propertyName = propertyName.Substring(0, propertyName.Length - 1) + "d";
                    }

                    if (propertyName.ToUpper() != column.Name.ToUpper())
                    {
                        sbEntityBase.AppendFormat("\t\t[Column(\"{0}\")]" + System.Environment.NewLine, column.Name);
                    }

                    sbEntityBase.AppendFormat("\t\tpublic {0}{1} {2} {{ get; set; }}", column.Type, column.IsNullable && column.Type != "string" ? "?" : string.Empty, propertyName);
                }
            }

            // Base
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("namespace CCI.AutoConfirm{0}" + System.Environment.NewLine, !string.IsNullOrEmpty(table.ModuleName) ? "." + table.ModuleName : string.Empty);
            sb.AppendLine("{");
            sb.AppendLine("\tusing System;");
            sb.AppendLine("\tusing System.ComponentModel.DataAnnotations;");
            sb.AppendLine("\tusing System.Runtime.Serialization;" + System.Environment.NewLine);
            sb.AppendFormat("\tpublic abstract class {0}Entity<TEntity> : EntityBase<TEntity>" + System.Environment.NewLine, table.EntityName);
            sb.AppendLine("\t{");
            sb.AppendLine(sbEntityBase.ToString());
            sb.AppendLine("\t}");
            sb.Append("}");

            BaseCode = sb.ToString();
        }
        public bool CanExecuteGenerateCommand()
        {
            return _selectedTable != null;
        }
        public static List<StoredProcedure> GetList(string searchTerm)
        {
            var storedProcedures = new List<StoredProcedure>();

            using (var connection = new SqlConnection(Helper.ConnectionString, Helper.Credential))
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_stored_procedures";
                    command.Parameters.AddWithValue("@sp_owner", "dbo");
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        command.Parameters.AddWithValue("@sp_name", $"%{searchTerm}%");
                        command.Parameters.AddWithValue("@fUsePattern", true);
                    }
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader["PROCEDURE_NAME"].ToString().Split(';')[0];
                            storedProcedures.Add(new StoredProcedure() { Name = reader["PROCEDURE_NAME"].ToString().Split(';')[0] });
                        }
                    }
                }
            }

            return storedProcedures;
        }
    }
}