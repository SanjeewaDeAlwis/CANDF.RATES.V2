using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace CANDF.RATES.DAL
{
    public class CarrierSvc
    {
        public int CarrierServiceId { get; set; }
        public string CarrierId { get; set; }
        public string CarrierServiceCode { get; set; }
        public string CarrierServiceDescription { get; set; }
        public int PrimaryRateTypeId { get; set; }
        public int SecondaryRateTypeId { get; set; }
        public decimal CubicConversionFactor { get; set; }
        public decimal RoundFeeTo { get; set; }
        public decimal DriverPercentage { get; set; }
        public int ServicePriority { get; set; }
        public bool PayDriver { get; set; }
        public bool DoubleReturn { get; set; }
        public string ServiceArea { get; set; }
        public int MinitesToPickup { get; set; }
        public int MinutesPerKm { get; set; }
        public int MinutesToAllocate { get; set; }
        public string CutoffTime { get; set; }
        public decimal Markup_Percentage { get; set; }
        public decimal MarkupValue { get; set; }
        public int MappingCFServiceId { get; set; }
        public string RowVersion { get; set; }
        public int ComplexTableId { get; set; }
        public bool ExclusiveHire { get; set; }
        public decimal FromWeight { get; set; }
        public decimal ToWeight { get; set; }
        public decimal FromVolume { get; set; }
        public decimal ToVolume { get; set; }
        public decimal Quality { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdated_On { get; set; }
        public bool GstInclude { get; set; }
        public bool IsConnoteApplicable { get; set; }
        public decimal MinimumChargeUnit { get; set; }
        public bool IsAdditionalCharge { get; set; }
        public bool IsPrepaidService { get; set; }
        public decimal FromDistance { get; set; }
        public decimal ToDistance { get; set; }
        public string CarrierOriginal_Service_Code { get; set; }
        public int ServiceDirection { get; set; }
        public bool AvailableOnline { get; set; }
        public bool GSTNotApplicable { get; set; }
        public decimal PalletFromWeight { get; set; }
        public decimal PalletToWeight { get; set; }
        public decimal MaximumLength { get; set; }
        public decimal MaximumWeight { get; set; }
        public decimal MaximumHeight { get; set; }
        public bool IsNotAvalSameMetro { get; set; }
        public string DisplayComment { get; set; }
        public int PalletID { get; set; }
        public bool IsAdditionalService { get; set; }
        public bool IsServiceLevelCubic { get; set; }
        public decimal ConsigmentMinimumCharge { get; set; }
        public decimal MinimumLength { get; set; }
        public decimal MinimumWidth { get; set; }
        public decimal MinimumHeight { get; set; }
        public int ItemID { get; set; }
        public string CarrierOriginalServiceId { get; set; }
        public string CarrierOriginalProductCode { get; set; }
        public decimal ItemMaximumLength { get; set; }
        public decimal ItemMaximumWidth { get; set; }
        public decimal ItemMaximumHeight { get; set; }
        public decimal ItemMaximumWeight { get; set; }
        public decimal ItemMaximumVolume { get; set; }
        public bool IsStepCalculation { get; set; }
        public bool IsDiscountApply { get; set; }
        //public decimal Discount { get; set; }


        //public List<VW_Carrier_Rate_For_Booking> GetCarrierCharges(Guid carrierId, int serviceId)
        //{
        //    CF_LIVE_DB context = new CF_LIVE_DB();
        //    var carrierCharges = context.GetCarrierCharges(carrierId, serviceId);
        //    carrierCharges.FirstOrDefault();
        //    return carrierCharges;
            //CarrierSvc carrierCharges = new CarrierSvc();
            //try
            //{
            //    SqlConnection con = OpenConnection();
            //    SqlCommand cmd = new SqlCommand("GetCarrierCharges", con);
            //    cmd.Parameters.Add(new SqlParameter("@carrierID", carrierId));
            //    cmd.Parameters.Add(new SqlParameter("@carrierServiceId", serviceId));
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    SqlDataReader dr = cmd.ExecuteReader();

            //    while (dr.Read())
            //    {
                    
            //        carrierCharges.CarrierServiceId = Convert.ToInt32(dr["Carrier_Service_ID"]);
            //        carrierCharges.CarrierId = Convert.ToString(dr["Carrier_Id"]);
            //        carrierCharges.CarrierServiceCode = Convert.ToString(dr["Carrier_Service_Code"]);
            //        carrierCharges.CarrierServiceDescription = Convert.ToString("Carrier_Service_Description");
            //        carrierCharges.PrimaryRateTypeId = Convert.ToInt32(dr["Primary_Rate_Type_Id"]);
            //        carrierCharges.CubicConversionFactor = Convert.ToDecimal(dr["Cubic_Conversion_Factor"]);
            //        carrierCharges.RoundFeeTo = Convert.ToDecimal(dr["Round_Fee_To"]);
            //        carrierCharges.DriverPercentage = Convert.ToDecimal(dr["Driver_Percentage"]);
            //        carrierCharges.ServicePriority = Convert.ToInt32(dr["Service_Priority"]);
            //        carrierCharges.PayDriver = Convert.ToBoolean(dr["Pay_Driver"]);
            //        carrierCharges.DoubleReturn = Convert.ToBoolean(dr["Double_Return"]);
            //        carrierCharges.ServiceArea = Convert.ToString(dr["Service_Area"]);
            //        carrierCharges.MinitesToPickup = Convert.ToInt32(dr["Minites_To_Pickup"]);
            //        carrierCharges.MinutesPerKm = Convert.ToInt32(dr["Minutes_Per_Km"]);
            //        carrierCharges.MinutesToAllocate = Convert.ToInt32(dr["Minutes_To_Allocate"]);
            //        carrierCharges.CutoffTime = Convert.ToString(dr["Cutoff_Time"]);
            //        carrierCharges.Markup_Percentage = Convert.ToDecimal(dr["Markup_Percentage"]);
            //        carrierCharges.MarkupValue = Convert.ToDecimal(dr["Markup_Value"]); 
            //        carrierCharges.MappingCFServiceId = Convert.ToInt32(dr["Mapping_CF_Service_Id"]); 
            //        carrierCharges.RowVersion = Convert.ToString(dr["Row_Version"]);
            //        carrierCharges.ComplexTableId = Convert.ToInt32(dr["Complex_Table_Id"]); 
            //        carrierCharges.ExclusiveHire = Convert.ToBoolean(dr["Exclusive_Hire"]);
            //        carrierCharges.FromWeight = Convert.ToDecimal(dr["From_Weight"]); 
            //        carrierCharges.ToWeight = Convert.ToDecimal(dr["To_weight"]);
            //        carrierCharges.FromVolume =  Convert.ToDecimal(dr["From_Volume"]);
            //        carrierCharges.ToVolume =  Convert.ToDecimal(dr["To_Volume"]);
            //        carrierCharges.Quality =  Convert.ToDecimal(dr["Quality"]);
            //        carrierCharges.CreatedBy =  Convert.ToInt32(dr["Created_By"]);
            //        carrierCharges.CreatedOn =  Convert.ToDateTime(dr["Created_On"]);
            //        carrierCharges.LastUpdatedBy =  Convert.ToInt32(dr["Last_Updated_By"]);
            //        carrierCharges.LastUpdated_On =  Convert.ToDateTime(dr["Last_Updated_On"]);
            //        carrierCharges.GstInclude =  Convert.ToBoolean(dr["Gst_Include"]);
            //        carrierCharges.IsConnoteApplicable =  Convert.ToBoolean(dr["Is_Connote_Applicable"]);
            //        carrierCharges.MinimumChargeUnit =  Convert.ToDecimal(dr["MinimumChargeUnit"]);
            //        carrierCharges.IsAdditionalCharge =  Convert.ToBoolean(dr["Is_Additional_Charge"]);
            //        carrierCharges.IsPrepaidService =  Convert.ToBoolean(dr["Is_Prepaid_Service"]);
            //        carrierCharges.FromDistance =  Convert.ToDecimal(dr["From_Distance"]);
            //        carrierCharges.ToDistance =  Convert.ToDecimal(dr["To_Distance"]);
            //        carrierCharges.CarrierOriginal_Service_Code =  Convert.ToString(dr["Carrier_Original_Service_Code"]);
            //        carrierCharges.ServiceDirection =  Convert.ToInt32(dr["Service_Direction"]);
            //        carrierCharges.AvailableOnline =  Convert.ToBoolean(dr["Available_online"]);
            //        carrierCharges.GSTNotApplicable =  Convert.ToBoolean(dr["GST_Not_Applicable"]);
            //        carrierCharges.PalletFromWeight =  Convert.ToDecimal(dr["Pallet_From_Weight"]);
            //        carrierCharges.PalletToWeight =  Convert.ToDecimal(dr["Pallet_To_Weight"]);
            //        carrierCharges.MaximumLength =  Convert.ToDecimal(dr["Maximum_Length"]);
            //        carrierCharges.MaximumWeight =  Convert.ToDecimal(dr["Maximum_Weight"]);
            //        carrierCharges.MaximumHeight =  Convert.ToDecimal(dr["Maximum_Height"]);
            //        carrierCharges.IsNotAvalSameMetro =  Convert.ToBoolean(dr["Is_NotAval_SameMetro"]);
            //        carrierCharges.DisplayComment =  Convert.ToString(dr["Display_Comment"]);
            //        carrierCharges.PalletID =  Convert.ToInt32(dr["Pallet_ID"]);
            //        carrierCharges.IsAdditionalService =  Convert.ToBoolean(dr["Is_Additional_Service"]);
            //        carrierCharges.IsServiceLevelCubic =  Convert.ToBoolean(dr["Is_ServiceLevel_Cubic"]);
            //        carrierCharges.ConsigmentMinimumCharge =  Convert.ToDecimal(dr["Consigment_Minimum_Charge"]);
            //        carrierCharges.MinimumLength =  Convert.ToDecimal(dr["Minimum_Length"]);
            //        carrierCharges.MinimumWidth =  Convert.ToDecimal(dr["Minimum_Width"]);
            //        carrierCharges.MinimumHeight =  Convert.ToDecimal(dr["Minimum_Height"]);
            //        carrierCharges.ItemID =  Convert.ToInt32(dr["Item_ID"]);
            //        carrierCharges.CarrierOriginalServiceId =  Convert.ToString(dr["Carrier_Original_Service_Id"]);
            //        carrierCharges.CarrierOriginalProductCode =  Convert.ToString(dr["Carrier_Original_Product_Code"]);
            //        carrierCharges.ItemMaximumLength =  Convert.ToDecimal(dr["Item_Maximum_Length"]);
            //        carrierCharges.ItemMaximumWidth =  Convert.ToDecimal(dr["Item_Maximum_Width"]);
            //        carrierCharges.ItemMaximumHeight =  Convert.ToDecimal(dr["Item_Maximum_Height"]);
            //        carrierCharges.ItemMaximumWeight =  Convert.ToDecimal(dr["Item_Maximum_Weight"]);
            //        carrierCharges.ItemMaximumVolume =  Convert.ToDecimal(dr["Item_Maximum_Volume"]);
            //        carrierCharges.IsStepCalculation =  Convert.ToBoolean(dr["Is_Step_Calculation"]);
            //        carrierCharges.IsDiscountApply =  Convert.ToBoolean(dr["Is_Discount_Apply"]);
            //        carrierCharges.Discount = Convert.ToDecimal(dr["Discount"]);
            //    }
            //    return carrierCharges;

            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
            //finally
            //{
            //    CloseConnection();
            //}
        //}


        public static List<Carrier_Service> CarrierServiceCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["CarrierServiceCollection"] == null)
                {
                    var context = new DBContext();
                    var data = context.Carrier_Service.ToList();
                    cache.Add("CarrierServiceCollection", data, DateTimeOffset.Now.AddMinutes(60));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["CarrierServiceCollection"] as List<Carrier_Service>;
                }
            }
            set
            {
                ObjectCache cache = MemoryCache.Default;
                cache["CarrierServiceCollection"] = value;
            }
        }
    }
}
