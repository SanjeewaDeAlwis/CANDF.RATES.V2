using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CANDF.RATES.WCF.SERVICE.LIBRARY;

namespace CANDF.RATES.DAL
{
    public class RateDAO
    {
        /// <summary>
        /// Get available services
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Rate> GetAvailabeServices(RateRequest request, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                #region [COMMENTED CODE]
                /*
                var resultCollection = context.GetAvailableServiceCollection(
                    request.SenderZoneIDCollection,
                    request.ReceiverZoneIDCollection,
                    request.PackageType,
                    request.DeliveryMethodID,
                    request.ServiceTypeID,
                    1,   /// TODO: This value is hard coded. So remove from the SP.
                    request.SenderSuburbName,
                    request.ReceiverSuburbName,
                    request.CarrierID,
                    Convert.ToDecimal(request.PackageCollection.MaxWeight),
                    Convert.ToDecimal(request.PackageCollection.MaxVolumn),
                    Convert.ToDecimal(request.PackageCollection.TotalWeight),
                    Convert.ToDecimal(request.PackageCollection.TotalVolumn),
                    Convert.ToDecimal(request.PackageCollection.MaxDimention / 100) /// Get maximum dimension converted to meters
                    ).ToList();
                */
                #endregion

                var resultCollection = context.GetAvailableServicesByCustomerSchedule(
                    request.CustomerScheduleIDActived,
                    request.SenderZoneIDCollection,
                    request.ReceiverZoneIDCollection,
                    request.PackageType,
                    request.DeliveryMethodID,
                    request.ServiceTypeID,
                    //1,   /// TODO: This value is hard coded. So remove from the SP.
                    request.SenderCountry.ID,
                    request.SenderState.ID,
                    request.SenderSuburbName,
                    request.SenderPostCode,
                    request.ReceiverCountry.ID,
                    request.ReceiverState.ID,
                    request.ReceiverSuburbName,
                    request.ReceiverPostCode,
                    request.CarrierID,
                    Convert.ToDecimal(request.PackageCollection.MaxWeight),
                    Convert.ToDecimal(request.PackageCollection.MaxVolumn),
                    Convert.ToDecimal(request.PackageCollection.TotalWeight),
                    Convert.ToDecimal(request.PackageCollection.TotalVolume),
                    Convert.ToDecimal(request.PackageCollection.MaxDimension / 100) /// Get maximum dimension converted to meters
                    ).ToList();

                var rateCollection = new List<Rate>();
                Rate rate = null;
                CarrierDB carrierDB = null;
                AvailableService item = null;
                for (int i = 0; i < resultCollection.Count; i++)
                {
                    item = resultCollection[i];

                    #region [MAP RATE]

                    rate = new Rate()
                    {
                        MainService = new Service()
                        {
                            ID = item.Carrier_Service_ID ?? 0,
                            Code = item.Service_Code,
                            Name = item.Service_Description,
                            CFServiceID = item.CF_service_ID ?? 0,
                            //Description = String.Empty
                        },
                        MarkupPercentage = Convert.ToDouble(item.Markup ?? 0)
                    };

                    carrierDB = CachedDAO.CarrierCollectionByID[item.Carrier_Id.Value];
                    rate.Carrier = new Carrier()
                    {
                        ID = carrierDB.Carrier_Id,
                        Code = carrierDB.Code,
                        Name = carrierDB.Carrier_Name,
                        IsActive = carrierDB.Is_Active_Carrier,
                        IsAllowDGGoodsDelivery = carrierDB.IsAllowDGGoodsDelivery ?? true,
                        IsAllowPOBoxDelivery = carrierDB.IsAllowPOBoxDelivery ?? true,
                        IsAllowResidentialDelivery = carrierDB.IsAllowResidentialDelivery ?? true,
                        IsAllowResidentialPickup = carrierDB.IsAllowResidentialPickup ?? true,
                    };

                    #endregion
                    rateCollection.Add(rate);
                }

                return rateCollection;
                //throw new NotImplementedException();
            }
            catch //(Exception)
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        /// <summary>
        /// Get available add-on services
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Service> GetCarrierAddonServices(Rate rate/*, DBContext context = null*/)
        {
            //var isNewContext = false;
            try
            {
                //if (context == null)
                //{
                //    context = new DBContext();
                //    isNewContext = true;
                //}

                //var vv= context.GetCarrierAddonService(rate.CarrierID, rate.Service.CFServiceID)
                //    .Select(o => new Service()
                //    {
                //        CFServiceID = o.Service_ID,
                //        Code = o.Service_Code,
                //        Name = o.Service_Description,
                //        Description = o.Service_Description
                //    }).ToList();

                //var item2 = CachedDAO.AddonServiceCollection.Where(o => o.CarrierID == rate.CarrierID ).ToList();
                //var item3 = CachedDAO.AddonServiceCollection.Where(o => o.ID == rate.MainService.CFServiceID).ToList();
                return CachedDAO.AddonServiceCollection.Where(o => o.CarrierID == rate.CarrierID && o.ID == rate.MainService.CFServiceID)
                    //.AsParallel()
                    .ToList();
            }
            catch ///(Exception)
            {
                throw;
            }
            finally
            {
                //if (isNewContext)
                //{
                //    context.Dispose();
                //}
            }
        }

        /// Get carrier rate collection
        /// </summary>
        /// <param name="request"></param>
        /// <param name="serviceIDCarrierIDCollectionText"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<CarrierRate> GetCarrierRateCollection(RateRequest request, string serviceIDCarrierIDCollectionText, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                return context.GetCarrierRateCollection(
                    serviceIDCarrierIDCollectionText,
                    request.SenderZoneIDCollection,
                    request.ReceiverZoneIDCollection,
                    DateTime.Now).ToList();
            }
            catch //(Exception)
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        /// <summary>
        /// Map zone collection
        /// </summary>
        /// <param name="country"></param>
        /// <param name="state"></param>
        /// <param name="suburb"></param>
        /// <param name="postCode"></param>
        /// <returns></returns>
        public static List<int> MapZoneCollection(
            CANDF.RATES.WCF.SERVICE.LIBRARY.Country country,
            CANDF.RATES.WCF.SERVICE.LIBRARY.State state,
            CANDF.RATES.WCF.SERVICE.LIBRARY.Suburb suburb,
            string postCode,
            DBContext context = null)
        {
            try
            {
                var zoneIDCollection = SuburbDAO.GetZoneIDCollection(country, state, suburb, postCode, context); 
                /// Get any country CF zone id collection
                zoneIDCollection.AddRange(SuburbDAO.AnyCountryZoneIDCollection);
                if (country.ID == 9) /// If Australia
                {
                    ///zoneIDCollection.AddRange(SuburbDAO.AnyAustraliaZoneIDCollection);
                    ///zoneIDCollection.AddRange(SuburbDAO.GetZoneIDCollection(String.Format("[ANY-AUS-{0}]", state.Code), context));
                    zoneIDCollection.Add(CachedDAO.AustraliaStateCFZoneCodeCollection["[ANY-AUS]"]);
                    zoneIDCollection.Add(CachedDAO.AustraliaStateCFZoneCodeCollection[String.Format("[ANY-AUS-{0}]", state.Code)]);
                }
                else
                {
                    zoneIDCollection.AddRange(SuburbDAO.GetZoneIDCollection(String.Format("[ANY-{0}]", country.Code), context));
                    zoneIDCollection.AddRange(SuburbDAO.GetZoneIDCollection(String.Format("[ANY-{0}-{1}]", country.Code, state.Code), context));
                }

                return zoneIDCollection;
            }
            catch //(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Customer Schedule ID by Customer ID
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public static Guid GetCustomerScheduleID(Guid customerID, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                context.CustomersDB.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                context.CustomersDB.EnablePlanCaching = false;
                if (customerID != Guid.Empty)
                {
                    //return context.CustomersDB.Where(c => c.Customer_Id == customerID).FirstOrDefault().Customer_Schedule_Id;
                    var customerScheduleID = context.GetCustomerScheduleIDByCustomerID(customerID).FirstOrDefault();
                    if (customerScheduleID == null)
                    {
                        /// Get web guest customer
                        return CachedDAO.WebGuestCustomer.Customer_Schedule_Id;
                    }
                    else
                    {
                        return customerScheduleID.Value;
                    }
                }
                else
                {
                    /// Get web guest customer
                    return CachedDAO.WebGuestCustomer.Customer_Schedule_Id;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        /// <summary>
        /// Get complex rate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rate"></param>
        public static CarrierRateComplex GetComplexRate(Service service/*, DBContext context = null*/)
        {
            //var isNewContext = false;
            try
            {
                //if (context == null)
                //{
                //    context = new DBContext();
                //    isNewContext = true;
                //}

                //return context.GetRatesForComplexRates(
                //    rate.ComplexRateID,
                //    rate.MeasuringQuantity
                //    ).FirstOrDefault();

                //return context.CarrierRateTableComplex.Where(o =>
                //    o.Carrier_Rate_Table_No == service.ComplexRateID && 
                //    o.To_Value >= tmpMeasuringQuantity
                //    ).OrderBy(o => o.To_Value).ToList();

                //return CachedDAO.CarrierComplexRateCollection.Where(o =>
                //    o.Carrier_Rate_Table_No == service.ComplexRateID &&
                //    o.To_Value >= tmpMeasuringQuantity)
                //    //.OrderBy(o => o.To_Value) /// No need to order by ToValue cause the collection is already ordered by this column
                //    //.AsParallel()
                //    //.WithDegreeOfParallelism(6)
                //    .FirstOrDefault();

                if (service.ComplexRateID == 0)
                {
                    return null;
                }
                else
                {
                    var tmpMeasuringQuantity = Convert.ToDecimal(service.MeasuringQuantity);
                    return CachedDAO.CarrierComplexRateCollection.Find(o =>
                        o.Carrier_Rate_Table_No == service.ComplexRateID &&
                        o.To_Value >= tmpMeasuringQuantity);
                }
                //return context.GetZoneIDCollectionByContryIDStateIDSubNmPstCd(country.ID, state.ID, suburb.Name, postCode).Select(o => o.Value).ToList<int>();
            }
            catch
            {
                throw;
            }
            finally
            {
                //if (isNewContext)
                //{
                //    context.Dispose();
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rate"></param>
        public static List<CarrierRateComplex> GetComplexRateCollection(Service service/*, DBContext context = null*/)
        {
            //var isNewContext = false;
            try
            {
                //if (context == null)
                //{
                //    context = new DBContext();
                //    isNewContext = true;
                //}

                //return context.GetRatesForComplexRates(
                //    rate.ComplexRateID,
                //    rate.MeasuringQuantity
                //    ).FirstOrDefault();
                                
                //return context.CarrierRateTableComplex.Where(o =>
                //    o.Carrier_Rate_Table_No == service.ComplexRateID && 
                //    o.To_Value >= tmpMeasuringQuantity
                //    ).OrderBy(o => o.To_Value).ToList();

                //return CachedDAO.CarrierComplexRateCollection.Where(o =>
                //    o.Carrier_Rate_Table_No == service.ComplexRateID && 
                //    o.To_Value >= tmpMeasuringQuantity)
                //    //.OrderBy(o => o.To_Value) /// No need to order by ToValue cause the collection is already ordered by this column
                //    .AsParallel()
                //    .WithDegreeOfParallelism(6)
                //    .ToList();

                if (service.ComplexRateID == 0)
                {
                    return null;
                }
                else
                {
                    var tmpMeasuringQuantity = Convert.ToDecimal(service.MeasuringQuantity);
                    return CachedDAO.CarrierComplexRateCollection.FindAll(o =>
                        o.Carrier_Rate_Table_No == service.ComplexRateID &&
                        o.To_Value >= tmpMeasuringQuantity);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                //if (isNewContext)
                //{
                //    context.Dispose();
                //}
            }
        }

        public static List<Service> GetAvailableAddonServices(DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }
                var result = context.GetAddonServicesAvailable();
                return result.Select(o => new Service()
                {
                    ID = o.Service_ID,
                    CFServiceID = o.CFService_ID,
                    Code = o.Service_Code,
                    Name = o.Service_Description,
                    ParentTypeID = o.Carrier_Service_Parent_Type,
                    CarrierID = o.Carrier_Id
                }).ToList();

                
                /*
                context.Carrier_Addon_Service.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                context.Carrier_Addon_Service.EnablePlanCaching = false;
                context.Carrier_Service.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                context.Carrier_Service.EnablePlanCaching = false;
                context.Carrier_Service_Parent_Type.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                context.Carrier_Service_Parent_Type.EnablePlanCaching = false;

                return (from CarrierAddOnServices in context.Carrier_Addon_Service
                        join MainService in context.Carrier_Service on CarrierAddOnServices.Carrier_Service_Id equals MainService.Carrier_Service_ID
                        join AddonService in context.Carrier_Service on CarrierAddOnServices.Carrier_Addon_Service_Id equals AddonService.Carrier_Service_ID
                        join Svc in context.ServicesDB on AddonService.Mapping_CF_Service_Id equals Svc.Service_ID
                        join CarrierSvcPrtType1 in context.Carrier_Service_Parent_Type on AddonService.Carrier_Service_ID equals CarrierSvcPrtType1.Carrier_Service_ID

                        into t
                        from rt in t.DefaultIfEmpty()
                        orderby AddonService.Carrier_Service_ID
                        //where CarrierService1.Carrier_Id equals @carrierID && CarrierService1.Mapping_CF_Service_Id equals @carrierServiceId
                        select
                            new Service
                            {
                                ID = MainService.Mapping_CF_Service_Id,
                                CFServiceID = AddonService.Mapping_CF_Service_Id,
                                Code = Svc.Service_Code,
                                Name = Svc.Service_Description,
                                CarrierID = AddonService.Carrier_Id
                            })
                            .Distinct()
                            .ToList();
                 */
            }
            catch //(Exception)
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        public static List<Pallet> GetPalletByPalletID(int palletID, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                context.Pallets.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                context.Pallets.EnablePlanCaching = false;
                return context.Pallets.Where(o => o.PalletID == palletID)
                    //.AsParallel()
                    .ToList();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        public static List<Item> GetItemByItemID(int itemID, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                context.Items.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                context.Items.EnablePlanCaching = false;
                return context.Items.Where(o => o.ItemID == itemID)
                    //.AsParallel()
                    .ToList();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        public static Carrier_Service GetCarrierService(CarrierRate carrierRate)
        {
            return CachedDAO.CarrierServiceCollection.Where(o => o.Carrier_Id == carrierRate.Carrier_Id && o.Mapping_CF_Service_Id == carrierRate.CF_service_ID)
                //.AsParallel()
                .FirstOrDefault();
        }

        public static Customer_Schedule GetCustomerScheduleByPromoCode(string promoCode, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }
                return context.GetScheduleByPromoCode(promoCode).FirstOrDefault();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {   
                    context.Dispose();
                }
            }
        }

        public static double GetCustomerBookingTotalOfCurrentDay(RateRequest request, Rate rate, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                return Convert.ToDouble(context.GetCustomerBookingTotalOfCurrentDay(request.CustomerID, rate.CarrierID, request.SenderSuburbName, request.SenderPostCode).FirstOrDefault() ?? 0);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        public static Customer GetCustomer(Guid customerID, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                var customerDB = context.GetCustomerByCustomerID(customerID).FirstOrDefault();
                return RateDAO.Map(customerDB);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }
        }

        public static Customer Map(CustomerDB customerDB)
        {
            return new Customer()
            {
                Code = Convert.ToInt32(customerDB.Customer_Code),
                ID = customerDB.Customer_Id,
                ScheduleID = customerDB.Customer_Schedule_Id,
                Type = Convert.ToInt32(customerDB.Customer_Type)
            };
        }

        #region [CURRENTLY NOT USED METHOD]
        /*
        /// <summary>
        /// Filter rates by customer schedule
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rateCollection"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Rate> FilterRatesByCustomerSchedule(RateRequest request, List<Rate> rateCollection, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                StringBuilder serviceIDCarrierIDCollection = new StringBuilder();
                foreach (var rate in rateCollection)
                {
                    serviceIDCarrierIDCollection.AppendFormat("{0}|{1},", rate.MainService.CFServiceID, rate.CarrierID);
                }

                var result = context.GetServicesFilterByCustomerScheduler(
                        request.CustomerScheduleID,
                        request.SenderCountry.ID,
                        request.SenderState.ID,
                        request.SenderSuburb.Name,
                        request.SenderPostCode,
                        request.ReceiverCountry.ID,
                        request.ReceiverState.ID,
                        request.ReceiverSuburb.Name,
                        request.ReceiverPostCode,
                        serviceIDCarrierIDCollection.ToString().Trim(',')
                        ).ToList();

                rateCollection = (from rate in rateCollection
                                  join rateFilterd in result on
                                    new { ServiceID = rate.MainService.CFServiceID, CarrierID = rate.CarrierID } equals
                                    new { ServiceID = rateFilterd.ServiceID.Value, CarrierID = rateFilterd.CarrierID.Value }
                                  select rate).ToList();

                return rateCollection;
            }
            catch //(Exception)
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }

            //throw new NotImplementedException();
        }
        */

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerScheduleID"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<CustomerScheduleDetail> GetCustomerSchedule(Guid customerScheduleID, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                context.Customer_Schedule_Detail.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                context.Customer_Schedule_Detail.EnablePlanCaching = false;
                return context.Customer_Schedule_Detail.Where(a => a.Customer_Schedule_Id == customerScheduleID)
                    //.AsParallel()
                    .ToList();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (isNewContext)
                {
                    context.Dispose();
                }
            }

        }
        */
        #endregion
    }
}
