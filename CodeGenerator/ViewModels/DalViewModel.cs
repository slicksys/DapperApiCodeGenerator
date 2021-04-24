using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using Dapper;

namespace CodeGenerator.ViewModels
{
    public class EntityField
    {
        public EntityField()
        {
            Fields = new List<EntityField>();
        }
        public string PropertyText { get { return $"public {Type} {Name} {{get; set;}}"; } }
        public string Name { get; set; }
        public string Type { get; set; }
        public int FieldCount { get; set; }
        public virtual List<EntityField> Fields { get; set; }
    }
    public class DalViewModel : ViewModelBase
    {
        private string _code;
        private string _testcode;
        private string _controllercode;
        private string _repositorycode;
        private string _entitycode;
        private StoredProcedure _selectedStoredProcedure;
        private List<StoredProcedure> _storedProcedures;
        private Dto _shapedEntity;
        private List<Dto> _shapedEntities;

        public DalViewModel()
        {
            SearchFilterText = "SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE";
            GenerateCommand = new RelayCommand(param => GenerateAsync(), param => CanExecuteGenerateCommand());
            GenerateAllCommand = new RelayCommand(param => GenerateAllAsync(), param => CanExecuteGenerateCommand());
            GetProcedureList = new RelayCommand(param => GetProcs(), param => UseSearchFilter());
            SelectAllProcsCommand = new RelayCommand(param => SelectAll());
        }
        public string Code
        {
            get
            {
                return _code;
            }

            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TestCode
        {
            get
            {
                return _testcode;
            }

            set
            {
                if (_testcode != value)
                {
                    _testcode = value;
                    OnPropertyChanged();
                }
            }
        }
        public string ControllerCode
        {
            get
            {
                return _controllercode;
            }

            set
            {
                if (_controllercode != value)
                {
                    _controllercode = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RepositoryCode
        {
            get
            {
                return _repositorycode;
            }

            set
            {
                if (_repositorycode != value)
                {
                    _repositorycode = value;
                    OnPropertyChanged();
                }
            }
        }
        public string EntityCode
        {
            get
            {
                return _entitycode;
            }

            set
            {
                if (_entitycode != value)
                {
                    _entitycode = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SearchFilterText { get; set; }
        public string QueryText { get; set; }
        public List<StoredProcedure> StoredProcedures
        {
            get
            {
                return _storedProcedures;
            }

            set
            {
                if (_storedProcedures != value)
                {
                    _storedProcedures = value;
                    OnPropertyChanged();
                }
            }
        }
        public StoredProcedure SelectedStoredProcedure
        {
            get
            {
                return _selectedStoredProcedure;
            }

            set
            {
                if (_selectedStoredProcedure != value)
                {
                    _selectedStoredProcedure = value;
                    OnPropertyChanged();
                    Code = string.Empty;
                }
            }
        }
        public Dto CurrentDto
        {
            get
            {
                return _shapedEntity;
            }

            set
            {
                if (_shapedEntity != value)
                {
                    _shapedEntity = value;
                    OnPropertyChanged();
                    Code = string.Empty;
                }
            }
        }
        public RelayCommand GenerateCommand { get; private set; }
        public RelayCommand GenerateAllCommand { get; private set; }
        public RelayCommand GetProcedureList { get; set; }
        public RelayCommand SelectAllProcsCommand { get; set; }
        private void SelectAll()
        {
            if (StoredProcedures != null)
            {
                foreach (var p in StoredProcedures)
                {
                    p.IsChecked = p.IsChecked != true;
                }

            }
        }
        private void GetProcs()
        {
            StoredProcedures = StoredProcedure.GetList(!string.IsNullOrEmpty(SearchFilterText) ? SearchFilterText : null);
        }
        private void GenerateAsync()
        {
            StoredProcedures = _storedProcedures.Where(x=>x.Name == _selectedStoredProcedure.Name).ToList();
            GenerateAllAsync();
        }
        private void GenerateAllAsync()
        {
            var sb = new StringBuilder();
            var tsb = new StringBuilder();  // Interface Code
            var csb = new StringBuilder();  // Controller Code
            var rsb = new StringBuilder();  // Repository Code
            var esb = new StringBuilder();  // Entity Code

            foreach (var sp in StoredProcedures.Where(x=>x.IsChecked))
            {
                var storedProcdure = new StoredProcedure(sp.Name, true );
                //var storedProcdure = sp;
                var methodName = Helper.GetMethodName(storedProcdure.Name);
                var entityName = Helper.GetEntityName(storedProcdure.Name);
                var sbSignature = new StringBuilder();
                var ctrlReopsitorySignature = new StringBuilder();
                var sbParameters = new StringBuilder();

                foreach (Parameter parameter in storedProcdure.Parameters)
                {
                    if (sbSignature.Length > 0)
                    {
                        sbSignature.Append(", ");
                        ctrlReopsitorySignature.Append(", ");
                    }

                    var propertyName = Helper.ToParameterName(parameter.Name, storedProcdure.Name);

                    if (methodName != "Add" && methodName != "Update")
                    {
                        sbSignature.AppendFormat("{0} {1} {2}", parameter.Type, propertyName, parameter.IsNullable ? " = null" : string.Empty);
                        ctrlReopsitorySignature.Append("entity");
                        sbParameters.AppendLine($"\tsqlParams.Add(\"{parameter.Name}\", {propertyName});");
                    }
                    else
                    {
                       
                        sbParameters.AppendLine($"\tsqlParams.Add(\"{parameter.Name}\", {(propertyName != "userId" ? "entity." + Helper.ToPropertyName(propertyName) : propertyName)});");
                    }
                }
                ctrlReopsitorySignature.Append("entity");

                if (methodName == "Insert" || methodName == "Update" || methodName == "Add")
                {
                    sbSignature.AppendFormat($"{entityName} entity");
                }

                string returnType = null;
                string returnMethod = null;
                
                    switch (methodName)
                    {
                        case "Select":
                        case "GetActive":
                        case "GetAll":
                            returnType = $"IEnumerable<{entityName}>";
                            returnMethod = "QueryAsync";
                            break;

                        case "GetById":
                        case "Get":
                            returnType = $"{entityName}";
                            returnMethod = "QuerySingleOrDefaultAsync";
                            break;

                        case "Delete":
                        case "Update":
                            returnType = "int";
                            returnMethod = "ExecuteAsync";
                            break;

                        case "Insert":
                        case "Add":
                            returnType = "int";
                            returnMethod = "ExecuteScalarAsync";
                            break;

                        default:
                            returnType = "UNKNOWN";
                            break;
                    }

                #region Shaped Entities

                CurrentDto = new Dto(sp.Name);
                esb.Append(GenerateEntity());
                esb.AppendLine();


                #endregion
               

                #region Interface Signature
                      sb.AppendLine(string.IsNullOrEmpty(returnType)
                    ? $"Task {methodName}{entityName}Async ({sbSignature.ToString()})"
                    : $"Task<{returnType}> {methodName}{entityName}Async ({sbSignature.ToString()});");
                sb.AppendLine();

                #endregion


                #region Repository Method

                if (sp.ResultTableCount > 1)
                {
                    rsb.AppendLine(string.IsNullOrEmpty(returnType)
                        ? $"public async Task {methodName}{entityName}Async ({sbSignature.ToString()})"
                        : $"public async Task<{returnType}> {methodName}{entityName}Async ({sbSignature.ToString()})");
                    rsb.AppendLine("{");
                    rsb.AppendLine($"\tDefaultTypeMap.MatchNamesWithUnderscores = true;");
                    rsb.AppendLine($"\tvar sqlParams = new DynamicParameters();");
                    rsb.AppendLine(sbParameters.ToString());
                    rsb.AppendLine($"\tvar cmd = new CommandDefinition(\"{storedProcdure.Name}\", sqlParams, null, null, CommandType.StoredProcedure);");
                    rsb.AppendLine("\tusing (var multi = await _connectionFactory.GetConnection.QueryMultipleAsync(cmd))\n\t{\n");
                    rsb.AppendLine($"\t\tvar entityResult = multi.Read<{entityName}>().Single();");
                    for (int x = 0; x < sp.ResultTableCount-1 ; x++)
                    {
                        rsb.AppendLine($"\t\tentityResult.Result{x} = multi.Read<ResultType>().ToList();");
                    }
                    rsb.AppendLine($"\t\treturn entityResult;");
                    rsb.AppendLine("\t}\n");
                    rsb.AppendLine("}\n");

                }
                else
                {
                    rsb.AppendLine(string.IsNullOrEmpty(returnType)
                        ? $"public async Task {methodName}{entityName}Async ({sbSignature.ToString()})"
                        : $"public async Task<{returnType}> {methodName}{entityName}Async ({sbSignature.ToString()})");

                    //sb.AppendLine($"public async Task<{returnType}> {methodName}{entityName}Async({sbSignature.ToString()})");
                    rsb.AppendLine("{");
                    rsb.AppendLine($"\tvar sqlParams = new DynamicParameters();");
                    rsb.AppendLine(sbParameters.ToString());
                    rsb.AppendLine($"\tvar retval = await _connectionFactory");
                    rsb.AppendLine($"\t\t\t.GetConnection");

                    if (returnMethod == "ExecuteAsync")
                    {
                        rsb.AppendLine($"\t\t\t.{returnMethod}(\"{storedProcdure.Name}\", sqlParams, commandType: CommandType.StoredProcedure);");
                    }
                    else
                    {
                        rsb.AppendLine($"\t\t\t.{returnMethod}<{entityName}>(\"{storedProcdure.Name}\", sqlParams, commandType: CommandType.StoredProcedure);");
                    }


                    rsb.AppendLine($"\treturn retval;");
                    rsb.AppendLine("}\n\n");
                }

                #endregion

                #region TestCode

                //[TestMethod]
                //public void TestInventoryMaster()
                //{
                //     var result = new RestService(uri, uid, pwd)
                //     .UserRole.GetUserRole<UserRight>(userId: 88);
                //     Assert.IsTrue(true);
                //}

                tsb.AppendLine("[TestMethod]\n");
                tsb.AppendLine($"public void Test{entityName}()");
                tsb.AppendLine("{");
                tsb.AppendLine("\tvar result = new RestService(uri, uid, pwd)");
                tsb.AppendLine("\t.{methodName}{entityName}<entityName>(userId: 88);");
                tsb.AppendLine("}\n");

                #endregion


                #region Controller Code

                //[Route("division/{divId}/inventory/{sku}")]
                csb.AppendLine(@"[Route(""inventory/{divId}/inventory/{entity}"")]");
                csb.AppendLine(string.IsNullOrEmpty(returnType)
              ? $"public async Task {methodName}{entityName}Async ({sbSignature.ToString()})"
              : $"public async Task<{returnType}> {methodName}{entityName}Async ({sbSignature.ToString()})");

                //sb.AppendLine($"public async Task<{returnType}> {methodName}{entityName}Async({sbSignature.ToString()})");
                csb.AppendLine("{");
                csb.AppendLine($"\tvar retval = await _repository.{methodName}{entityName}Async({ctrlReopsitorySignature.ToString()});");
                csb.AppendLine($"\tif(retval== null)\n\t\tthrow new HttpResponseException(HttpStatusCode.NotFound);");
                csb.AppendLine($"\treturn retval;");
                csb.AppendLine("}\n\n");

                #endregion

        

               // _selectedStoredProcedure.Name = sp.Name;

               // sb.Append(Generate());
            }

            Code = sb.ToString();
            TestCode = tsb.ToString();
            RepositoryCode = rsb.ToString();
            ControllerCode = csb.ToString();
            EntityCode = esb.ToString();

            StoredProcedures.RemoveRange(1, StoredProcedures.Count-1);
        }
        private string GenerateEntity()
        {
            StringBuilder sbEntityBase = new StringBuilder();

            foreach (Column column in CurrentDto.Columns)
            {
                string propertyName = Helper.ToPropertyName(column.Name, CurrentDto.Name);

                // Entity Base
                if (sbEntityBase.Length > 0)
                {
                    sbEntityBase.AppendLine(System.Environment.NewLine);
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

                    if (!string.IsNullOrEmpty(CurrentDto.ModuleName))
                    {
                        propertyName = propertyName.Replace(CurrentDto.ModuleName, string.Empty);
                    }

                    if (!string.IsNullOrEmpty(CurrentDto.EntityName))
                    {
                        propertyName = propertyName.Replace(CurrentDto.EntityName, string.Empty);
                    }
                }
                else if (column.Type == "string")
                {
                    if (column.Name.EndsWith("Code"))
                    {
                        if (!string.IsNullOrEmpty(CurrentDto.ModuleName))
                        {
                            propertyName = propertyName.Replace(CurrentDto.ModuleName, string.Empty);
                        }

                        if (!string.IsNullOrEmpty(CurrentDto.EntityName))
                        {
                            propertyName = propertyName.Replace(CurrentDto.EntityName, string.Empty);
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

            // Base
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("\tpublic class {0} : DataEntity" + System.Environment.NewLine, CurrentDto.EntityName);
            sb.AppendLine("\t{");
            sb.AppendLine(sbEntityBase.ToString());
            sb.AppendLine("\t}");
            
            return sb.ToString();
        }
        private bool CanSelectAll()
        {
            return StoredProcedures != null;
        }
        public bool UseSearchFilter()
        {
            return SearchFilterText != null;
        }
        public bool CanExecuteGenerateCommand()
        {
            return _selectedStoredProcedure != null;
        }
    }
}

