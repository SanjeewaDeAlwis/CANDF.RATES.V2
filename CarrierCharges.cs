using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CANDF.RATES.DAL
{
    public class CarrierCharges
    {
        public int CarrierServiceId { get; set; }
        public string CarrierId { get; set; }
        public string CarrierServiceCode { get; set; }
        public string CarrierServiceDescription { get; set; }
        public int PrimaryRateTypeId { get; set; }
        public int SecondaryRateTypeId { get; set; }
        public decimal CubicConversionFactor{ get; set; }
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
        public decimal Discount { get; set; }

    }
}
