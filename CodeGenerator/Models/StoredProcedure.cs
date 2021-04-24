using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Dapper;

namespace CodeGenerator
{
    public class StoredProcedure: INotifyPropertyChanged
    {
        private string _text;
        private bool blnIsChecked;

        #region Ctor
        public StoredProcedure()
        {
        }
        public StoredProcedure(string name, bool isSelected)
        {
            Name = name;
            IsSelected = isSelected;
            MethodName = Helper.GetMethodName(name);
            string moduleCode = Helper.GetModuleName(name);
            string moduleName = Helper.GetModuleName(name);

            string entityName = name;
            if (!string.IsNullOrEmpty(moduleName))
            {
                entityName = entityName.Replace(moduleName, string.Empty);
            }

            entityName = entityName.Replace(MethodName, string.Empty).Substring(2);

            using (var connection = new SqlConnection(Helper.ConnectionString, Helper.Credential))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText = string.Format("SELECT text FROM syscomments WHERE id=OBJECT_ID('{0}')", name);

                    _text = command.ExecuteScalar().ToString();

                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = name;

                    SqlCommandBuilder.DeriveParameters(command);

                    Parameters = new List<Parameter>();

                    foreach (SqlParameter parameter in command.Parameters)
                    {
                        if (parameter.ParameterName != "@RETURN_VALUE" && parameter.ParameterName != "@Debug")
                        {
                            string parameterName = parameter.ParameterName;
                            bool isNullable = IsNullable(parameter);
                            bool isNullableType = IsNullableType(parameter);
                            string dataType = MapSqlDbType(parameter);
                            if (isNullable && dataType != "string")
                            {
                                dataType += "?";
                            }

                            Parameters.Add(new Parameter()
                            {
                                Name = parameterName,
                                IsNullable = isNullable,
                                Type = dataType
                            });
                        }
                    }

                    Parameters.Sort();
                }
            }
        }
        #endregion

        public bool IsChecked
        {
            get { return blnIsChecked; }
            set
            {
                blnIsChecked = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public int ResultTableCount { get; set; }
        public List<Parameter> Parameters { get; set; }
        public string MethodName { get; set; }
        public string ModuleName { get; set; }
        public string EntityName { get; set; }
        private static string MapSqlDbType(SqlParameter parameter)
        {
            string dataType;

            switch (parameter.SqlDbType)
            {
                case SqlDbType.Bit:
                    dataType = "bool";
                    break;

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                    dataType = "string";
                    break;

                case SqlDbType.Int:
                case SqlDbType.BigInt:
                    dataType = "int?";
                    break;

                case SqlDbType.TinyInt:
                    dataType = parameter.ParameterName == "@Visibility" ? "Visibility" : "byte";
                    break;

                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                    dataType = "DateTime";
                    break;

                case SqlDbType.Time:
                    dataType = "TimeSpan";
                    break;

                case SqlDbType.DateTimeOffset:
                    dataType = "DateTimeOffset";
                    break;

                case SqlDbType.Money:
                    dataType = "decimal";
                    break;

                default:
                    dataType = "?";
                    break;
            }

            return dataType;
        }
        public bool IsNullable(SqlParameter parameter)
        {
            return Regex.IsMatch(_text, parameter.ParameterName + @".*=\s*NULL[,)]", RegexOptions.IgnoreCase);
        }
        private bool IsNullableType(SqlParameter parameter)
        {
            bool isNullableType = false;

            switch (parameter.SqlDbType)
            {
                case SqlDbType.Int:
                case SqlDbType.Bit:
                case SqlDbType.TinyInt:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                    isNullableType = IsNullable(parameter);
                    break;

                default:
                    break;
            }

            return isNullableType;
        }
        public static List<StoredProcedure> GetListWOTableCount(string searchTerm)
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
                            storedProcedures.Add(new StoredProcedure()
                            {
                                Name = reader["PROCEDURE_NAME"].ToString().Split(';')[0]
                                ,
                                IsSelected = true
                            });
                        }
                    }
                }
            }

            return storedProcedures;
        }
        public static List<StoredProcedure> GetList(string searchTerm)
        {
            var storedProcedures = new List<StoredProcedure>();

            using (var connection = new SqlConnection(Helper.ConnectionString, Helper.Credential))
            {
                using (var command = connection.CreateCommand())
                {
                    var query = $@"SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES ";
                    // if (!string.IsNullOrEmpty(searchTerm)) query += $"WHERE ROUTINE_NAME LIKE '{searchTerm}'";

                    // Bins
                    //query += @" where routine_name in ('deleteBinLocation','getBinLocation','validateBinLocation','selectBinLocationsDynamic','updateBinLocation','selectBinLocationsAll','addBinLocation','getBinLocations',
                    //                                   'getBinLocationItems','addBinReplenDetail','updateBinReplenDetail','deleteBinReplenDetail','getBinReplenDetail','getBinReplenGenItem','getBinReplenInvMaster','addBinReplenMaster',
                    //                                   'deleteBinReplenMaster','getBinReplenMaster','updateBinReplenMaster','addBinReplenMove','updateBinReplenMove','deleteBinReplenMove','getBinReplenMove','addBinReplenNote',
                    //                                   'deleteBinReplenNote','getBinReplenNote','updateBinReplenNote','addBinReplenPrint','deleteBinReplenPrint','getBinReplenPrint','updateBinReplenPrint','addBinType','deleteBinType',
                    //                                   'getBinType','getBinTypesAll','updateBinType','addBkrNotifTran','deleteBkrNotifTran','getBkrNotifTran','getBkrNotifTransByOrder','updateBkrNotifTran')";

                    query +=
                        @" where routine_name in  ('addInventoryPersonalizationTemplate','addPersonalizationTemplate','addPersonalizationTemplateProperty','deletePersonalizationTemplate','deletePersonalizationTemplateProperty','getPersonalizationTemplate','getPersonalizationTemplateDetails','selectPersonalizationTemplatesAll','updatePersonalizationTemplate','updatePersonalizationTemplateProperty')";

                    
                                               /* ('addFreightTable','updateFreightTable','addFreightDetail','deleteFreightTable','deleteFreightDetail','getFreightTable','getFreightDetail','updateFreightDetail',
                                                       'addFreightDetail','deleteFreightDetail','getFreightDetail','updateFreightDetail','updatePromoShippingCharge','getPromoShippingCharge','getPromoShipping',
                                                       'addPromoShippingCharge','deletePromoShippingCharge','updatePromoShippingPgc','getPromoShippingPgc','deletePromoShippingPgc','addPromoShippingPgc','addPromoShipping',
                                                       'updatePromoShipping','deletePromoShipping','selectPromoShippingsDynamic','getPromoShippingMethod','updatePromoShippingMethod','addPromoShippingMethod','deletePromoShippingMethod',
                                                       'selectPromoShippingsAll','addPromoShippingCharge','deletePromoShippingCharge','getPromoShippingCharge','updatePromoShippingCharge','addPromoShippingMethod',
                                                       'deletePromoShippingMethod','getPromoShippingMethod','updatePromoShippingMethod','addPromoShippingPgc','deletePromoShippingPgc','getPromoShippingPgc','updatePromoShippingPgc',
                                                       'addInventoryRestrictedShipping','addUpdateRestrictedShipping','deleteInventoryRestrictedShipping','deleteRestrictedShipping','getInventoryRestrictedShipping',
                                                       'getInventoryRestrictedShippingDestinations','getRestrictedShipping','getRestrictedShippingCodeAll','updateInventoryRestrictedShipping','addCpsShipBoxImport',
                                                       'updateCpsShipBoxImport','getCpsShipBoxImport','getCPSShipBoxImportList','deleteCpsShipBoxImport','updateShipBoxImport','getShipBoxImport','getShipBoxImportsList',
                                                       'addShipBoxImport','deleteShipBoxImport','addDivisionShipCarrier','addShipCarrier','deleteDivisionShipCarrier','deleteShipCarrier','getDivisionShipCarrier',
                                                       'getShipCarrier','updateDivisionShipCarrier','updateShipCarrier','addShipConfirmTran','deleteShipConfirmTran','getShipConfirmTran','getShipConfirmTranByOrder',
                                                       'updateShipConfirmTran','getShipmentBox','deleteShipmentBoxDetail','addShipmentBoxDetail','getShipmentBoxDetail','updateShipmentBoxDetail','usp_caGetShipmentClasses',
                                                       'deleteShipmentBox','updateShipmentBox','addShipmentBox','addShipmentBox','addShipmentBoxDetail','deleteShipmentBox','deleteShipmentBoxDetail','getShipmentBox',
                                                       'getShipmentBoxDetail','updateShipmentBox','updateShipmentBoxDetail','addShipmentBoxDetail','deleteShipmentBoxDetail','getShipmentBoxDetail','updateShipmentBoxDetail',
                                                       'addDivisionShipMethod','addDivisionShipMethodToHold','addShipMethod','addShipMethodMapping','addSwitchShipMethod','addVendorShipMethods','deleteDivisionShipMethod',
                                                       'deleteDivisionShipMethodToHold','deleteShipMethod','deleteshipMethodMapping','deleteSwitchShipMethod','deleteVendorShipMethods','getDivisionShipMethod',
                                                       'getDivisionShipMethodsToHold','getShipMethod','getShipMethodByAcctSysId','getShipMethodMapping','getShipMethodMappingList','getSwitchShipMethod','getThirdPartyShipMethodIds',
                                                       'getVendorShipMethods','selectShipMethodsDynamic','selectSwitchShipMethodsDynamic','selectVendorShipMethodsDynamic','updateDivisionShipMethod','updateDivisionShipMethodToHold',
                                                       'updateShipMethod','updateSwitchShipMethod','updateVendorShipMethods','addShipMethodMapping','deleteshipMethodMapping','getShipMethodMapping','getShipMethodMappingList',
                                                       'updateShiptoDetailPacking','assignShiptoDetailBins','addShiptoDetail','updateShiptoDetail','getShiptoDetail','deleteShiptoDetail','updateShiptoDetailPacking','addShiptoMaster',
                                                       'deleteShiptoMaster','getShipToMaster','getShiptoMasterPacking','updateShiptoMaster','updateShiptoMasterPacking','UpdateShipToMasterPrintFlag','usp_getShipToMaster',
                                                       'getShiptoMasterPacking','updateShiptoMasterPacking','addShiptoPrintHist','addShiptoPrintHistCalcPrintNum','deleteShiptoPrintHist','getShiptoPrintHist','updateShiptoPrintHist',
                                                       'getShiptoSerialNumList','addShipZone','calcShipZone','deleteShipZone','getShipZone','selectShipZonesAll','updateShipZone','addSwitchShipMethod','deleteSwitchShipMethod',
                                                       'getSwitchShipMethod','selectSwitchShipMethodsDynamic','updateSwitchShipMethod','addThirdPartyShippingMethod','deleteThirdPartyShippingMethod','getThirdPartyShippingMethod',
                                                       'selectThirdPartyShippingMethodsAll','updateThirdPartyShippingMethod','addTransitTime','deleteTransitTime','getTransitTime','selectTransitTimesAll','updateTransitTime')";
                    */

                    // Division
                    // query += @" where routine_name in ('getDivision','getDivisionalBinQty','getDivisionCustomerStatusMethodToHold','getDivisionLock','getDivisionPaymeth','getDivisionShipCarrier','getDivisionShipMethod','getDivisionShipMethodToHold','getDivisionUserDef','getDivisionWarehouse','getDonotMailDataExtractList','getExceptionLog','getExtPoLineDetail','getExtPoMaster','getFinderFile')";

                    // Inventory
                    //query += @" where routine_name in ('getAssetType','getBaseItem','getBaseOption','getBaseOptionGroup','getInvAdjustReason','getInvAttributeImport','getInvBin','getInvBinQty','getInvCountLock','getInvCountMaster','getInvCountryListMatrixWrapper','getInvCountSheet','getInvCountSheetDetail','getInvCrossRef','getInvDataOptionsImport','getInventoryAsset','getInventoryAttribute','getInventoryAttributeDef','getInventoryAttributeOption','getInventoryBox','getInventoryNote','getInventoryPersonalizationTemplate','getInventoryPriority','getInventoryRestrictedDestinationList','getInventoryRestrictedShippingCode','getInventoryReturnProduct','getInventoryTransaction','getInventoryVendorRC','getInvExtPoDetail','getInvImportQty','getInvImportRelatedItem','getInvItem','getInvListER','getInvMaster','getInvPoDetail','getInvPrice','getInvPriceGroupListMatrixWrapper','getInvPriceImport','getInvQty','getInvShipSurcharge','getInvTransBinDetail','getInvTransCostDetail','getInvTransfer','getInvTransferDetail','getInvTransferMove','getInvTransferNote','getInvTransferPrint','getInvTransferReceipt','getInvUmMultiplier','getInvUserDef','getInvVendor','getInvVendorImport','getKit','getKitComponent','getKitOption','getKitOptionDetail','getKitPrice','getKitRule','getKitRuleDetail','getKitStaticImport','getMonthlyInvHist','getPriceGroup','getPriceList','getPriceListPromo','getPriceListPromoTypeList','getPricePromoList','getPrintBatch','getPrintBatchDetail','getProductClass','getProductClassAttribute','getProductClassList','getProductGroup','getProductGroupGL','getProductInfoDataExtractList','getProductSku','getRelatedItem','getRelationType','getSerialTemplate','getSkuHistory','getSkuHistoryMonth','getSkuHistoryPoDetail','getSkuKitList','getSourceCode','getSourceCodeDataExtractList','getSourceCurve','getSourceCurveTemplate','getSourceCurveTemplateDetail','getTaxCode','getTaxRate','getUnitOfMeasure','getVendor','getVendorContact','getVendorNote','getVendorPo','getVendorTypeList','getWarehouse','getWarehouseAisle','getWarehouseAisleImport','getWeeklyInvHist')";
                    //query += @" where routine_name in ('deleteAssetType','deleteBaseItem','deleteBaseOption','deleteBaseOptionGroup','deleteInvAdjustReason','deleteInvAttributeImport','deleteInvBin','deleteInvBinQty','deleteInvCountLock','deleteInvCountMaster','deleteInvCountryListMatrixWrapper','deleteInvCountSheet','deleteInvCountSheetDetail','deleteInvCrossRef','deleteInvDataOptionsImport','deleteInventoryAsset','deleteInventoryAttribute','deleteInventoryAttributeDef','deleteInventoryAttributeOption','deleteInventoryBox','deleteInventoryNote','deleteInventoryPersonalizationTemplate','deleteInventoryPriority','deleteInventoryRestrictedDestinationList','deleteInventoryRestrictedShippingCode','deleteInventoryReturnProduct','deleteInventoryTransaction','deleteInventoryVendorRC','deleteInvExtPoDetail','deleteInvImportQty','deleteInvImportRelatedItem','deleteInvItem','deleteInvListER','deleteInvMaster','deleteInvPoDetail','deleteInvPrice','deleteInvPriceGroupListMatrixWrapper','deleteInvPriceImport','deleteInvQty','deleteInvShipSurcharge','deleteInvTransBinDetail','deleteInvTransCostDetail','deleteInvTransfer','deleteInvTransferDetail','deleteInvTransferMove','deleteInvTransferNote','deleteInvTransferPrint','deleteInvTransferReceipt','deleteInvUmMultiplier','deleteInvUserDef','deleteInvVendor','deleteInvVendorImport','deleteKit','deleteKitComponent','deleteKitOption','deleteKitOptionDetail','deleteKitPrice','deleteKitRule','deleteKitRuleDetail','deleteKitStaticImport','deleteMonthlyInvHist','deletePriceGroup','deletePriceList','deletePriceListPromo','deletePriceListPromoTypeList','deletePricePromoList','deletePrintBatch','deletePrintBatchDetail','deleteProductClass','deleteProductClassAttribute','deleteProductClassList','deleteProductGroup','deleteProductGroupGL','deleteProductInfoDataExtractList','deleteProductSku','deleteRelatedItem','deleteRelationType','deleteSerialTemplate','deleteSkuHistory','deleteSkuHistoryMonth','deleteSkuHistoryPoDetail','deleteSkuKitList','deleteSourceCode','deleteSourceCodeDataExtractList','deleteSourceCurve','deleteSourceCurveTemplate','deleteSourceCurveTemplateDetail','deleteTaxCode','deleteTaxRate','deleteUnitOfMeasure','deleteVendor','deleteVendorContact','deleteVendorNote','deleteVendorPo','deleteVendorTypeList','deleteWarehouse','deleteWarehouseAisle','deleteWarehouseAisleImport','deleteWeeklyInvHist')";
                    //query += @" where routine_name in ('getAssetType','getBaseItem','getBaseOption','getBaseOptionGroup','getInvAdjustReason','getInvAttributeImport','getInvBin','getInvBinQty','getInvCountLock','getInvCountMaster','getInvCountryListMatrixWrapper','getInvCountSheet','getInvCountSheetDetail','getInvCrossRef','getInvDataOptionsImport','getInventoryAsset','getInventoryAttribute','getInventoryAttributeDef','getInventoryAttributeOption','getInventoryBox','getInventoryNote','getInventoryPersonalizationTemplate','getInventoryPriority','getInventoryRestrictedDestinationList','getInventoryRestrictedShippingCode','getInventoryReturnProduct','getInventoryTransaction','getInventoryVendorRC','getInvExtPoDetail','getInvImportQty','getInvImportRelatedItem','getInvItem','getInvListER','getInvMaster','getInvPoDetail','getInvPrice','getInvPriceGroupListMatrixWrapper','getInvPriceImport','getInvQty','getInvShipSurcharge','getInvTransBinDetail','getInvTransCostDetail','getInvTransfer','getInvTransferDetail','getInvTransferMove','getInvTransferNote','getInvTransferPrint','getInvTransferReceipt','getInvUmMultiplier','getInvUserDef','getInvVendor','getInvVendorImport','getKit','getKitComponent','getKitOption','getKitOptionDetail','getKitPrice','getKitRule','getKitRuleDetail','getKitStaticImport','getMonthlyInvHist','getPriceGroup','getPriceList','getPriceListPromo','getPriceListPromoTypeList','getPricePromoList','getPrintBatch','getPrintBatchDetail','getProductClass','getProductClassAttribute','getProductClassList','getProductGroup','getProductGroupGL','getProductInfoDataExtractList','getProductSku','getRelatedItem','getRelationType','getSerialTemplate','getSkuHistory','getSkuHistoryMonth','getSkuHistoryPoDetail','getSkuKitList','getSourceCode','getSourceCodeDataExtractList','getSourceCurve','getSourceCurveTemplate','getSourceCurveTemplateDetail','getTaxCode','getTaxRate','getUnitOfMeasure','getVendor','getVendorContact','getVendorNote','getVendorPo','getVendorTypeList','getWarehouse','getWarehouseAisle','getWarehouseAisleImport','getWeeklyInvHist')";
                    /*
                   
('addInventoryPersonalizationTemplate
','addPersonalizationTemplate  
','addPersonalizationTemplateProperty  
','deletePersonalizationTemplate   
','deletePersonalizationTemplateProperty   
','getPersonalizationTemplate  
','getPersonalizationTemplateDetails   
','selectPersonalizationTemplatesAll   
','updatePersonalizationTemplate   
','updatePersonalizationTemplateProperty')

    */

                    command.CommandType = CommandType.Text;
                    command.CommandText = searchTerm;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                      while (reader.Read())
                      {
                        string name = reader["ROUTINE_NAME"].ToString().Split(';')[0];
                        storedProcedures.Add(new StoredProcedure()
                        {
                            Name = reader["ROUTINE_NAME"].ToString().Split(';')[0]
                        });
                      }
                    }
                }
               
                foreach (var sp in storedProcedures)
                {
                    DefaultTypeMap.MatchNamesWithUnderscores = true;
                    var param = new DynamicParameters();
                    var spstr = new StringBuilder();
                    try
                    {
                      using (var cmd = connection.CreateCommand())
                      {
                        cmd.CommandText = $@"SELECT  SPECIFIC_NAME, PARAMETER_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
                                               FROM  INFORMATION_SCHEMA.PARAMETERS 
                                              WHERE  SPECIFIC_NAME = '{sp.Name}' 
                                              ORDER  BY ORDINAL_POSITION";
                        var pmr = cmd.ExecuteReader();
                        while (pmr.Read())
                        {
                          int? plen;
                          string pname = pmr["PARAMETER_NAME"].ToString();
                          if (!string.IsNullOrEmpty(pname))
                          {
                            plen = Helper.CheckNull<int?>(pmr["CHARACTER_MAXIMUM_LENGTH"]);
                            var t = Helper.GetClrType(pmr["DATA_TYPE"].ToString());
                            switch (t)
                            {
                              case "string":
                                  spstr.Append($"null, ");
                                  break;
                              case "int":
                                  param.Add(pname, 0);
                                  spstr.Append($"0, ");
                                  break;
                              case "DateTime":
                                  param.Add(pname, DateTime.Now.ToString());
                                  spstr.Append($"'2016-12-29', ");
                                  break;
                              case "TimeSpan":
                                  param.Add(pname, new TimeSpan(1, 1, 1));
                                  spstr.Append($"'01:01:01', ");
                                  break;
                              case "null":
                                  param.Add(pname, null);
                                  spstr.Append($"null, ");
                                  break;
                            }

                          }
                        }
                        pmr.Close();
                        var procText =$@"SET FMTONLY ON; exec {sp.Name} {spstr.ToString().Trim().TrimEnd(',')}; SET FMTONLY OFF; ";
                        DataSet ds = new DataSet();
                        var sqlCmd = new SqlCommand(procText, new SqlConnection(connection.ConnectionString));
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlCmd))
                        {
                            da.Fill(ds);
                        }
                        sp.ResultTableCount = ds.Tables.Count;
                      }
                    }
                    catch (Exception ex)
                    {
                        // add later
                    }
                }
            }
            
            return storedProcedures;
        }
        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}