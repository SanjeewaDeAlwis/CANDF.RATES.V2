using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using CANDF.RATES.WCF.SERVICE.LIBRARY;

namespace CANDF.RATES.DAL
{
    public class CachedDAO
    {
        private static int cacheTime = 180;

        /// <summary>
        /// Get web guest customer
        /// </summary>
        public static CustomerDB WebGuestCustomer
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["WebGuestCustomer"] == null)
                {
                    var context = new DBContext();
                    var data = context.CustomersDB.Where(c => c.Customer_Code == "4002").FirstOrDefault();
                    cache.Add("WebGuestCustomer", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["WebGuestCustomer"] as CustomerDB;
                }
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["WebGuestCustomer"] = value;
            //}
        }

        /// <summary>
        /// Get web guest customer
        /// </summary>
        public static System_Constaints GetGSTValue
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["GSTValue"] == null)
                {
                    var context = new DBContext();
                    var data = context.System_Constaints.Where(c => c.Constaint_Name == "GST").FirstOrDefault();
                    cache.Add("GSTValue", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["GSTValue"] as System_Constaints;
                }
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["WebGuestCustomer"] = value;
            //}
        }

        #region [Collections]

        public static List<Service> AddonServiceCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["AddonServiceCollection"] == null)
                {
                    var context = new DBContext();
                    var data = RateDAO.GetAvailableAddonServices(context).ToList();
                    cache.Add("AddonServiceCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["AddonServiceCollection"] as List<Service>;
                }
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["AddonServiceCollection"] = value;
            //}
        }

        public static List<CarrierRateComplex> CarrierComplexRateCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["BookingCarrierRateTableCollection"] == null)
                {
                    var context = new DBContext();
                    context.CarrierRateTableComplex.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.CarrierRateTableComplex.EnablePlanCaching = false;
                    var data = context.CarrierRateTableComplex
                        .Where(r => r.Effective_Date <= DateTime.Today.Date)
                        .OrderByDescending(o => o.Effective_Date).ThenBy(o=>o.To_Value).ToList();
                    cache.Add("BookingCarrierRateTableCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["BookingCarrierRateTableCollection"] as List<CarrierRateComplex>;
                }
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["BookingCarrierRateTableCollection"] = value;
            //}
        }

        /*
        public static List<CarrierRateComplex> CarrierRateComplexCollectionAnyCustomerOrderByToValue
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["CarrierRateTableCollectionAnyCustomerOrderByToValue"] == null)
                {
                    var context = new DBContext();
                    context.CarrierRateTableComplex.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.CarrierRateTableComplex.EnablePlanCaching = false;
                    var data = context.CarrierRateTableComplex
                        .Where(r => r.Effective_Date <= DateTime.Today.Date && r.CustomerID.Equals(Guid.Empty))
                        .OrderBy(o => o.To_Value).ToList();
                    cache.Add("CarrierRateTableCollectionAnyCustomerOrderByToValue", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["CarrierRateTableCollectionAnyCustomerOrderByToValue"] as List<CarrierRateComplex>;
                }
            }
            set
            {
                ObjectCache cache = MemoryCache.Default;
                cache["CarrierRateTableCollectionAnyCustomerOrderByToValue"] = value;
            }
        }
        */

        /*
        public static List<VW_Carrier_Rate_For_Booking> BookingRatesCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["BookingRatesCollection"] == null)
                {
                    var context = new DBContext();
                    context.VW_Carrier_Rate_For_Booking.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.VW_Carrier_Rate_For_Booking.EnablePlanCaching = false;
                    var data = context.VW_Carrier_Rate_For_Booking
                        .Where(r => r.Effective_From_Date.Date <= DateTime.Today.Date)
                        .OrderByDescending(o => o.Effective_From_Date).ToList();
                    cache.Add("BookingRatesCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["BookingRatesCollection"] as List<VW_Carrier_Rate_For_Booking>;
                }
            }
            set
            {
                ObjectCache cache = MemoryCache.Default;
                cache["BookingRatesCollection"] = value;
            }
        }
        */

        public static List<Carrier_Service> CarrierServiceCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["CarrierServiceCollection"] == null)
                {
                    var context = new DBContext();
                    context.Carrier_Service.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.Carrier_Service.EnablePlanCaching = false;
                    var data = context.Carrier_Service.ToList();
                    cache.Add("CarrierServiceCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }
                else
                {
                    return cache["CarrierServiceCollection"] as List<Carrier_Service>;
                }
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["CarrierServiceCollection"] = value;
            //}
        }

        public static Dictionary<Guid, CarrierDB> CarrierCollectionByID
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["CarrierCollectionByID"] == null)
                {
                    var context = new DBContext();
                    context.CarriersDB.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.CarriersDB.EnablePlanCaching = false;
                    var data = context.CarriersDB.ToDictionary(o => o.Carrier_Id, o => o);
                    cache.Add("CarrierCollectionByID", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }

                return cache["CarrierCollectionByID"] as Dictionary<Guid, CarrierDB>;
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["CarrierCollectionByID"] = value;
            //}
        }

        public static Dictionary<string, CarrierDB> CarrierCollectionByCode
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["CarrierCollectionByCode"] == null)
                {
                    var context = new DBContext();
                    context.CarriersDB.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.CarriersDB.EnablePlanCaching = false;
                    var data = context.CarriersDB.ToDictionary(o => o.Code, o => o);
                    cache.Add("CarrierCollectionByCode", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }

                return cache["CarrierCollectionByCode"] as Dictionary<string, CarrierDB>;
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["CarrierCollectionByCode"] = value;
            //}
        }

        public static List<ServiceDB> ServiceCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["ServiceCollection"] == null)
                {
                    var context = new DBContext();
                    context.ServicesDB.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.ServicesDB.EnablePlanCaching = false;
                    var data = context.ServicesDB.Where(s => s.Is_Deleted == false && s.Is_Active == true)
                        .ToList();
                    cache.Add("ServiceCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }

                return cache["ServiceCollection"] as List<ServiceDB>;
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["ServiceCollection"] = value;
            //}
        }

        public static List<Carrier_Shipment_Time> CarrierShipmentTimeCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["CarrierShipmentTimeCollection"] == null)
                {
                    var context = new DBContext();
                    context.Carrier_Shipment_Time.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.Carrier_Shipment_Time.EnablePlanCaching = false;
                    var data = context.Carrier_Shipment_Time.ToList();
                    cache.Add("CarrierShipmentTimeCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }

                return cache["CarrierShipmentTimeCollection"] as List<Carrier_Shipment_Time>;
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["CarrierShipmentTimeCollection"] = value;
            //}
        }

        public static List<Country> CountryCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["CountryCollection"] == null)
                {
                    var context = new DBContext();
                    context.Countries.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.Countries.EnablePlanCaching = false;
                    var data = context.Countries.ToList();
                    cache.Add("CountryCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }

                return cache["CountryCollection"] as List<Country>;
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["CountryCollection"] = value;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, int> AustraliaStateCFZoneCodeCollection
        {
            get
            {
                ObjectCache cache = MemoryCache.Default;
                if (cache["AustraliaStateCFZoneCodeCollection"] == null)
                {
                    var context = new DBContext();
                    context.CarriersDB.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    context.CarriersDB.EnablePlanCaching = false;
                    var data = context.CF_Zone.Where(o => o.Country_Id == 9 && o.CF_Zone_Code.Contains("ANY-AUS")).ToDictionary(o => o.CF_Zone_Code, o => o.CF_Zone_Id);
                    cache.Add("AustraliaStateCFZoneCodeCollection", data, DateTimeOffset.Now.AddMinutes(cacheTime));

                    context.Dispose();
                    return data;
                }

                return cache["AustraliaStateCFZoneCodeCollection"] as Dictionary<string, int>;
            }
            //set
            //{
            //    ObjectCache cache = MemoryCache.Default;
            //    cache["AustraliaStateCFZoneCodeCollection"] = value;
            //}
        }        

        //public static List<vwState> GetState(int CountryID, string suburb, string postcode)
        //{
        //    var context = new DBContext();
        //    var data = context.vwStates(a => a.);
        //}

        #endregion
    }
}
