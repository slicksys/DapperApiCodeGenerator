using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace CodeGenerator
{
    public static class Helper
    {
        private const string CCI_REGKEY = @"SOFTWARE\CCI\AutoConfirm\Database";
        private static string _connString = ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;
        
        static Helper()
        {
            SetEnvironmentConnectionString();
            ConnectionString = _connString;
            //ConnectionString = ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;

            Modules = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Modules = (ConfigurationManager.GetSection("Modules") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());

            WordReplacements = new Dictionary<string, string>();
            WordReplacements = (ConfigurationManager.GetSection("WordReplacements") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());

            Methods = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Methods = (ConfigurationManager.GetSection("Methods") as System.Collections.Hashtable)
                 .Cast<System.Collections.DictionaryEntry>()
                 .ToDictionary(n => n.Key.ToString(), n => n.Value.ToString());

            BackEndColumns = new List<string>();
            BackEndColumns = ConfigurationManager.AppSettings["BackEndColumns"].Split('|').ToList();

            NonSerializedColumns = new List<string>();
            NonSerializedColumns = ConfigurationManager.AppSettings["NonSerializedColumns"].Split('|').ToList();
        }

        public static T CheckNull<T>(object obj)
        {
            return (obj == DBNull.Value ? default(T) : (T)obj);
        }
        public static string GetClrType(string sqltype)
        {
            switch (sqltype)
            {
                case "varchar":
                case "char":
                case "nvarchar":
                case "nchar":
                case "text":
                case "ntext":
                    return "string";

                case "int identity":
                case "int":
                case "bigint":
                case "money":
                case "bit":
                case "smallint":
                    return "int";

                case "datetime":
                case "date":
                case "smalldatetime":
                case "datetime2":
                    return "DateTime";

                case "time":
                    return "TimeSpan";
                default:
                    return null;

            }

        }
        public static SqlCredential Credential { get; private set; }

        public static string ConnectionString { get; private set; }

        public static Dictionary<string, string> Modules { get; private set; }

        public static Dictionary<string, string> WordReplacements { get; private set; }

        public static Dictionary<string, string> Methods { get; private set; }

        public static List<string> BackEndColumns { get; private set; }

        public static List<string> NonSerializedColumns { get; private set; }

        public static List<Environment> Environments { get; private set; }

        private static void SetEnvironmentConnectionString()
        {
            //var rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(CORP_REGKEY);
            //var csb = new SqlConnectionStringBuilder();
            //csb.DataSource = rk.GetValue("Server").ToString();
            //csb.InitialCatalog = rk.GetValue("Database").ToString();
            //Credential = new SqlCredential(rk.GetValue("UserName").ToString(), DecryptPassword(rk.GetValue("Password").ToString()));
            //_connString = csb.ConnectionString;
            //SetEnvironments();
        }

        private static SecureString DecryptPassword(string encryptedPassword)
        {
            if (!string.IsNullOrEmpty(encryptedPassword))
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedPassword);
                string key = "^@br549#!$";
                byte[] salt = new UTF8Encoding().GetBytes(key);

                using (var rfc = new Rfc2898DeriveBytes(key, salt))
                using (var aes = new AesManaged())
                {
                    aes.Key = rfc.GetBytes(16);

                    // Create a decrytor to perform the stream transform.
                    using (var ms = new MemoryStream(encryptedData))
                    {
                        // Set the IV based on the embedded value in the first 16 bytes
                        byte[] iv = new byte[16];
                        ms.Read(iv, 0, 16);
                        aes.IV = iv;

                        using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, iv))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                var password = new SecureString();

                                foreach (char c in sr.ReadToEnd().ToCharArray())
                                {
                                    password.AppendChar(c);
                                }

                                password.MakeReadOnly();

                                return password;
                            }
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public static void UpdateProperty(string propertyCode)
        {
            if (propertyCode != null)
            {
                using (var connection = new SqlConnection(_connString, Credential)) 
                using (SqlCommand command = new SqlCommand("spPropertySELECT", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Enabled", true);
                    command.Parameters.AddWithValue("@PropertyCode", propertyCode);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();

                            csb.DataSource = reader["DatabaseServer"].ToString();
                            csb.InitialCatalog = reader["DatabaseName"].ToString();

                            ConnectionString = csb.ConnectionString;
                        }
                    }
                }
            }
            else
            {
                ConnectionString = _connString;
            }
        }

        public static string ToParameterName(string name, string objectName = null)
        {
            string pascalCase = ReplaceWords(name);

            if (!string.IsNullOrEmpty(objectName))
            {
                string moduleName = ExtractModuleName(objectName);
                string entityname = GetEntityName(objectName);

                if (!string.IsNullOrEmpty(moduleName))
                {
                    pascalCase = pascalCase.Replace(moduleName, string.Empty);
                }

                if (!string.IsNullOrEmpty(entityname))
                {
                    pascalCase = pascalCase.Replace(entityname, string.Empty);
                }
            }

            if (pascalCase.StartsWith("@"))
            {
                pascalCase = pascalCase.Substring(1);
            }

            if (string.IsNullOrEmpty(pascalCase))
            {
                pascalCase = name;
            }

            pascalCase = pascalCase[0].ToString().ToLower() + pascalCase.Substring(1);

            return pascalCase;
        }

        public static string ToPropertyName(string name, string objectName = null)
        {
            string propertyName = ReplaceWords(name);

            if (!string.IsNullOrEmpty(objectName))
            {
                string moduleName = ExtractModuleName(objectName);
                string entityname = GetEntityName(objectName);

                if (!string.IsNullOrEmpty(moduleName))
                {
                    propertyName = propertyName.Replace(moduleName, string.Empty);
                }

                if (!string.IsNullOrEmpty(entityname))
                {
                    propertyName = propertyName.Replace(entityname, string.Empty);
                }
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = name;
            }

            propertyName = propertyName[0].ToString().ToUpper() + propertyName.Substring(1);

            return propertyName;
        }

        private static string ReplaceWords(string word)
        {
            foreach (string replacement in WordReplacements.Keys)
            {
                word = word.Replace(replacement, WordReplacements[replacement]);
            }

            return word;
        }

        private static bool IsUpper(char character)
        {
            return character.ToString() == character.ToString().ToUpper();
        }

        private static string ExtractModuleName(string name)
        {
            string moduleName = "UNKNOWN";

            string prefix = name.StartsWith("sp") ? "sp" : string.Empty;

            string pattern = "^" + prefix + "(" + string.Join("|", Modules.Keys.ToArray()) + ")";

            Match match = Regex.Match(name, pattern);

            if (match.Success)
            {
                moduleName = match.Captures[0].Value.Substring(prefix.Length);
            }

            return moduleName;
        }

        public static string GetModuleName(string name)
        {
            string moduleName = ExtractModuleName(name);

            if (Modules.ContainsKey(moduleName))
            {
                return Modules[moduleName];
            }
            else
            {
                return moduleName;
            }
        }

        private static string ExtractMethodName(string name)
        {
            string methodName = "UNKNOWN";

            string pattern = "(" + string.Join("|", Methods.Keys.ToArray()) + ")";

            Match match = Regex.Match(name, pattern.ToLower());

            if (match.Success)
            {
                methodName = match.Captures[0].Value;
            }

            return methodName;
        }

        public static string GetMethodName(string name)
        {
            string methodName = ExtractMethodName(name).FirstCharToUpper();

            if (Methods.ContainsKey(methodName))
            {
                return Methods[methodName];
            }
            else
            {
                return methodName;
            }
        }

        public static string GetEntityName(string name)
        {
            string entityName = "UNKNOWN";
            string moduleName = ExtractModuleName(name);
            string methodName = ExtractMethodName(name);

            if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(methodName))
            {
                entityName = name;

                if (entityName.StartsWith("sp"))
                {
                    entityName = entityName.Substring(2);
                }

                entityName = entityName.Replace(moduleName, string.Empty);
                entityName = entityName.Replace(methodName, string.Empty);
            }

            return entityName;
        }
    }
}