using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    public static class Extensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("Didn't Work Dude!");
            return input.ToLower().First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
/*
 
   ('getPoChangeHist','getPoLineDetail','getPoLineDivDetail','getPoMaster','getPoNote','getPoOrderMaster','getPoPrintHist','getPoPrintItem','getPoReceiptDetail','getPoReview','getPosRegister','getPoVerifyDetail','getPurchReportDetail','getPurchReportMaster')
 
   ('addExtPoLineDetail','addPoLineDetail','deleteExtPoLineDetail','deletePoLineDetail','getExtPoLineDetail','getPoLineDetail','getVendorPoLineDetail','updateExtPoLineDetail','updatePoLineDetail','addPoLineDivDetail','deletePoLineDivDetail','getPoLineDivDetail','updatePoLineDivDetail','addExtPoMaster','addPoMaster','deleteExtPoMaster','deletePoMaster','getExtPoMaster','getPoMaster','lockPoMaster','updateExtPoMaster','updatePoMaster','addPoNote','deletePoNote','selectPoNotesDynamic','updatePoNote','getPoOrderMaster','addPoPrintHist','deletePoPrintHist','getPoPrintHist','getPoPrintHistLastStatus','updatePoPrintHist','addPoReceiptDetail','deletePoReceiptDetail','getPoReceiptDetail','updatePoReceiptDetail','getPoReview','validatePosRegister','deletePosRegister','addPosRegister','updatePosRegister','getPosRegister','addPoVerifyDetail','deletePoVerifyDetail','getPoVerifyDetail','updatePoVerifyDetail','addPurchReportDetail','deletePurchReportDetail','getPurchReportDetail','selectPurchReportDetailsAll','updatePurchReportDetail','addPurchReportMaster','deletePurchReportMaster','getPurchReportMaster','selectPurchReportMastersAll','updatePurchReportMaster')

    // Shipmemts
   ('addFreightTable','updateFreightTable','addFreightDetail','deleteFreightTable','deleteFreightDetail','getFreightTable','getFreightDetail','updateFreightDetail','addFreightDetail','deleteFreightDetail','getFreightDetail','updateFreightDetail','updatePromoShippingCharge','getPromoShippingCharge','getPromoShipping','addPromoShippingCharge','deletePromoShippingCharge','updatePromoShippingPgc','getPromoShippingPgc','deletePromoShippingPgc','addPromoShippingPgc','addPromoShipping','updatePromoShipping','deletePromoShipping','selectPromoShippingsDynamic','getPromoShippingMethod','updatePromoShippingMethod','addPromoShippingMethod','deletePromoShippingMethod','selectPromoShippingsAll','addPromoShippingCharge','deletePromoShippingCharge','getPromoShippingCharge','updatePromoShippingCharge','addPromoShippingMethod','deletePromoShippingMethod','getPromoShippingMethod','updatePromoShippingMethod','addPromoShippingPgc','deletePromoShippingPgc','getPromoShippingPgc','updatePromoShippingPgc','addInventoryRestrictedShipping','addUpdateRestrictedShipping','deleteInventoryRestrictedShipping','deleteRestrictedShipping','getInventoryRestrictedShipping','getInventoryRestrictedShippingDestinations','getRestrictedShipping','getRestrictedShippingCodeAll','updateInventoryRestrictedShipping','addCpsShipBoxImport','updateCpsShipBoxImport','getCpsShipBoxImport','getCPSShipBoxImportList','deleteCpsShipBoxImport','updateShipBoxImport','getShipBoxImport','getShipBoxImportsList','addShipBoxImport','deleteShipBoxImport','addDivisionShipCarrier','addShipCarrier','deleteDivisionShipCarrier','deleteShipCarrier','getDivisionShipCarrier','getShipCarrier','updateDivisionShipCarrier','updateShipCarrier','addShipConfirmTran','deleteShipConfirmTran','getShipConfirmTran','getShipConfirmTranByOrder','updateShipConfirmTran','getShipmentBox','deleteShipmentBoxDetail','addShipmentBoxDetail','getShipmentBoxDetail','updateShipmentBoxDetail','usp_caGetShipmentClasses','deleteShipmentBox','updateShipmentBox','addShipmentBox','addShipmentBox','addShipmentBoxDetail','deleteShipmentBox','deleteShipmentBoxDetail','getShipmentBox','getShipmentBoxDetail','updateShipmentBox','updateShipmentBoxDetail','addShipmentBoxDetail','deleteShipmentBoxDetail','getShipmentBoxDetail','updateShipmentBoxDetail','addDivisionShipMethod','addDivisionShipMethodToHold','addShipMethod','addShipMethodMapping','addSwitchShipMethod','addVendorShipMethods','deleteDivisionShipMethod','deleteDivisionShipMethodToHold','deleteShipMethod','deleteshipMethodMapping','deleteSwitchShipMethod','deleteVendorShipMethods','getDivisionShipMethod','getDivisionShipMethodsToHold','getShipMethod','getShipMethodByAcctSysId','getShipMethodMapping','getShipMethodMappingList','getSwitchShipMethod','getThirdPartyShipMethodIds','getVendorShipMethods','selectShipMethodsDynamic','selectSwitchShipMethodsDynamic','selectVendorShipMethodsDynamic','updateDivisionShipMethod','updateDivisionShipMethodToHold','updateShipMethod','updateSwitchShipMethod','updateVendorShipMethods','addShipMethodMapping','deleteshipMethodMapping','getShipMethodMapping','getShipMethodMappingList','updateShiptoDetailPacking','assignShiptoDetailBins','addShiptoDetail','updateShiptoDetail','getShiptoDetail','deleteShiptoDetail','updateShiptoDetailPacking','addShiptoMaster','deleteShiptoMaster','getShipToMaster','getShiptoMasterPacking','updateShiptoMaster','updateShiptoMasterPacking','UpdateShipToMasterPrintFlag','usp_getShipToMaster','getShiptoMasterPacking','updateShiptoMasterPacking','addShiptoPrintHist','addShiptoPrintHistCalcPrintNum','deleteShiptoPrintHist','getShiptoPrintHist','updateShiptoPrintHist','getShiptoSerialNumList','addShipZone','calcShipZone','deleteShipZone','getShipZone','selectShipZonesAll','updateShipZone','addSwitchShipMethod','deleteSwitchShipMethod','getSwitchShipMethod','selectSwitchShipMethodsDynamic','updateSwitchShipMethod','addThirdPartyShippingMethod','deleteThirdPartyShippingMethod','getThirdPartyShippingMethod','selectThirdPartyShippingMethodsAll','updateThirdPartyShippingMethod','addTransitTime','deleteTransitTime','getTransitTime','selectTransitTimesAll','updateTransitTime')


addFreightTable
updateFreightTable
addFreightDetail
deleteFreightTable
deleteFreightDetail
getFreightTable
getFreightDetail
updateFreightDetail
addFreightDetail
deleteFreightDetail
getFreightDetail
updateFreightDetail
updatePromoShippingCharge
getPromoShippingCharge
getPromoShipping
addPromoShippingCharge
deletePromoShippingCharge
updatePromoShippingPgc
getPromoShippingPgc
deletePromoShippingPgc
addPromoShippingPgc
addPromoShipping
updatePromoShipping
deletePromoShipping
selectPromoShippingsDynamic
getPromoShippingMethod
updatePromoShippingMethod
addPromoShippingMethod
deletePromoShippingMethod
selectPromoShippingsAll
addPromoShippingCharge
deletePromoShippingCharge
getPromoShippingCharge
updatePromoShippingCharge
addPromoShippingMethod
deletePromoShippingMethod
getPromoShippingMethod
updatePromoShippingMethod
addPromoShippingPgc
deletePromoShippingPgc
getPromoShippingPgc
updatePromoShippingPgc
addInventoryRestrictedShipping
addUpdateRestrictedShipping
deleteInventoryRestrictedShipping
deleteRestrictedShipping
getInventoryRestrictedShipping
getInventoryRestrictedShippingDestinations
getRestrictedShipping
getRestrictedShippingCodeAll
updateInventoryRestrictedShipping
addCpsShipBoxImport
updateCpsShipBoxImport
getCpsShipBoxImport
getCPSShipBoxImportList
deleteCpsShipBoxImport
updateShipBoxImport
getShipBoxImport
getShipBoxImportsList
addShipBoxImport
deleteShipBoxImport
addDivisionShipCarrier
addShipCarrier
deleteDivisionShipCarrier
deleteShipCarrier
getDivisionShipCarrier
getShipCarrier
updateDivisionShipCarrier
updateShipCarrier
addShipConfirmTran
deleteShipConfirmTran
getShipConfirmTran
getShipConfirmTranByOrder
updateShipConfirmTran
getShipmentBox
deleteShipmentBoxDetail
addShipmentBoxDetail
getShipmentBoxDetail
updateShipmentBoxDetail
usp_caGetShipmentClasses
deleteShipmentBox
updateShipmentBox
addShipmentBox
addShipmentBox
addShipmentBoxDetail
deleteShipmentBox
deleteShipmentBoxDetail
getShipmentBox
getShipmentBoxDetail
updateShipmentBox
updateShipmentBoxDetail
addShipmentBoxDetail
deleteShipmentBoxDetail
getShipmentBoxDetail
updateShipmentBoxDetail
addDivisionShipMethod
addDivisionShipMethodToHold
addShipMethod
addShipMethodMapping
addSwitchShipMethod
addVendorShipMethods
deleteDivisionShipMethod
deleteDivisionShipMethodToHold
deleteShipMethod
deleteshipMethodMapping
deleteSwitchShipMethod
deleteVendorShipMethods
getDivisionShipMethod
getDivisionShipMethodsToHold
getShipMethod
getShipMethodByAcctSysId
getShipMethodMapping
getShipMethodMappingList
getSwitchShipMethod
getThirdPartyShipMethodIds
getVendorShipMethods
selectShipMethodsDynamic
selectSwitchShipMethodsDynamic
selectVendorShipMethodsDynamic
updateDivisionShipMethod
updateDivisionShipMethodToHold
updateShipMethod
updateSwitchShipMethod
updateVendorShipMethods
addShipMethodMapping
deleteshipMethodMapping
getShipMethodMapping
getShipMethodMappingList
updateShiptoDetailPacking
assignShiptoDetailBins
addShiptoDetail
updateShiptoDetail
getShiptoDetail
deleteShiptoDetail
updateShiptoDetailPacking
addShiptoMaster
deleteShiptoMaster
getShipToMaster
getShiptoMasterPacking
updateShiptoMaster
updateShiptoMasterPacking
UpdateShipToMasterPrintFlag
usp_getShipToMaster
getShiptoMasterPacking
updateShiptoMasterPacking
addShiptoPrintHist
addShiptoPrintHistCalcPrintNum
deleteShiptoPrintHist
getShiptoPrintHist
updateShiptoPrintHist
getShiptoSerialNumList
addShipZone
calcShipZone
deleteShipZone
getShipZone
selectShipZonesAll
updateShipZone
addSwitchShipMethod
deleteSwitchShipMethod
getSwitchShipMethod
selectSwitchShipMethodsDynamic
updateSwitchShipMethod
addThirdPartyShippingMethod
deleteThirdPartyShippingMethod
getThirdPartyShippingMethod
selectThirdPartyShippingMethodsAll
updateThirdPartyShippingMethod
addTransitTime
deleteTransitTime
getTransitTime
selectTransitTimesAll
updateTransitTime
     
     SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE
    
SELECT * INTO #import
FROM ( SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportBinLocation%' UNION ALL
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportFieldTranslation%' UNION ALL
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportMap%' UNION ALL
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportMapField%' UNION ALL
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportMapRule%' UNION ALL
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderAdjustment%' UNION ALL
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderDetail%' UNION ALL
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderHistory%' UNION ALL       
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderMap%' UNION ALL            
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderMaster%' UNION ALL              
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportPaymentMaster%' UNION ALL          
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportPoHistory%' UNION ALL                 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportPoMap%') t;

SELECT * FROM #import


SELECT * INTO #import

       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportBinLocation%' 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportFieldTranslation%' 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportMap%' 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportMapField%' 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportMapRule%' 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderAdjustment%' 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderDetail%' 
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderHistory%'        
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderMap%'             
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportOrderMaster%'               
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportPaymentMaster%'           
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportPoHistory%'                  
       SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME LIKE '%ImportPoMap%') t;

SELECT * FROM #import

    //ImportBinLocation
//ImportFieldTranslation
//ImportMap
//ImportMapField
//ImportMapRule
//ImportOrderAdjustment
//ImportOrderDetail
//ImportOrderHistory
//ImportOrderMap
//ImportOrderMaster
//ImportPaymentMaster
//ImportPoHistory
//ImportPoMap





Freight
FreightDetail
FreightTermList
PromoShipping
PromoShippingCharge
PromoShippingMethod
PromoShippingPgc
RestrictedShipping
ShipBoxImport
ShipCarrier
ShipConfirmTran
Shipment
ShipmentBox
ShipmentBoxDetail
ShipMethod
ShipMethodMapping
ShippingBox
ShiptecBox
ShiptecBoxDetail
ShipToDataExtractList
ShiptoDetail
ShiptoDetailPacking
ShiptoMaster
ShiptoMasterPacking
ShiptoPrintHist
ShiptoSerialNumList
ShipZone
SwitchShipMethod
ThirdPartyShippingMethod
TransitTime





  
  
   */
