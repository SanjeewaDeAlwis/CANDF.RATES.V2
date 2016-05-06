using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CANDF.RATES.WCF.SERVICE.LIBRARY;
using CANDF.RATES.DAL;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace CANDF.RATES.BLL
{    
    /// <summary>
    /// Business logics for rate
    /// </summary>
    public class Rates
    {
        /// <summary>
        /// Degree of parallelism
        /// </summary>
        public static int ParallelismDegree = 15;

        /// <summary>
        /// Get rate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static RateResponse GetRates(RateRequest request)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            /// Increase main current thread priority
            if (System.Threading.Thread.CurrentThread.Priority == System.Threading.ThreadPriority.Normal)
            {
                System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
            }

            object lockCheck = new object();
            List<Rate> rateCollection = null;
            List<Rate> rateCollectionFiltered = new List<Rate>();
            List<CarrierRate> serviceRateCollection = null;
            using (DBContext context = new DBContext())
            {
                try
                {
                    #region [REQUEST VALIDATION]
                    RateResponse validatedResponse = ValidateAndSetDefaultValues(request,context);
                    if (validatedResponse.Status == Status.FAIL)
                    {
                        return validatedResponse;
                    }
                    #endregion

                    /// Initialize properties
                    InitializeProperties(request, context);
                    /// Get available properties
                    rateCollection = RateDAO.GetAvailabeServices(request, context);
                    /// Get carrier rates for all available services at once
                    List<string> serviceIDCarrierIDCollection = new List<string>();
                    for (int i = 0; i < rateCollection.Count; i++)
                    {
                        var rate = rateCollection[i];
                        rate.AddonServiceCollection = new ServiceCollection();
                        /// Get add-on service collection
                        rate.AddonServiceCollection.AddRange(RateDAO.GetCarrierAddonServices(rate));
                        for (int j = 0; j < rate.AddonServiceCollection.Count; j++)
                        {
                            /// Add add-on service to carrier service ID collection
                            serviceIDCarrierIDCollection.Add(String.Format("{0}|{1}", rate.AddonServiceCollection[j].CFServiceID, rate.CarrierID));
                        }

                        /// Add main service to carrier service ID collection
                        serviceIDCarrierIDCollection.Add(String.Format("{0}|{1}", rateCollection[i].MainService.CFServiceID, rateCollection[i].CarrierID));

                        /// If promotion applied then set the markup percentage
                        if (request.IsPromotionApplied)
                        {
                            rate.DiscountPercentage = (100 - rate.MarkupPercentage);
                        }
                    }
                    /// Get all main services at once
                    serviceIDCarrierIDCollection = serviceIDCarrierIDCollection.Distinct().ToList();
                    serviceRateCollection = RateDAO.GetCarrierRateCollection(request, String.Join(",", serviceIDCarrierIDCollection), context);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    context.Dispose();
                }
            }

            Parallel.ForEach(rateCollection, new ParallelOptions { MaxDegreeOfParallelism = ParallelismDegree }, rate =>
            {
                /// DB context
                DBContext context = new DBContext();
                if (System.Threading.Thread.CurrentThread.Priority == System.Threading.ThreadPriority.Normal)
                {
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
                }

                try
                {
                    #region [MAIN SERVICES FILTERATION]
                    /// Filter dangerous goods required rates
                    if (request.IsDangerousGoodsDeliveryRequired && !rate.Carrier.IsAllowDGGoodsDelivery)
                    {
                        //rateCollection.Remove(rate);
                        return;
                    }

                    /// Filter PO Box delivery required rates
                    if (request.IsPOBoxDeliveryRequired && !rate.Carrier.IsAllowPOBoxDelivery)
                    {
                        //rateCollection.Remove(rate);
                        return; //continue;
                    }

                    /// Filter residential delivery required rates
                    if (request.IsResidentialDeliveryRequired && !rate.Carrier.IsAllowResidentialDelivery)
                    {
                        //rateCollection.Remove(rate);
                        return; //continue;
                    }

                    /// Filter residential pickup required rates
                    if (request.IsResidentialPickupRequired && !rate.Carrier.IsAllowResidentialPickup)
                    {
                        //rateCollection.Remove(rate);
                        return; //continue;
                    }
                    #endregion

                    #region [CALCULATE MAIN SERVICE CHARGE]
                    /// Get carrier rate
                    CarrierRate mainServiceRate = serviceRateCollection.Where(o => o.CF_service_ID == rate.MainService.CFServiceID && o.Carrier_Id == rate.CarrierID)
                        //.AsParallel()
                        //.WithDegreeOfParallelism(ParallelismDegree)
                        .FirstOrDefault();
                    /// Set main service details
                    rate.MainService.ComplexRateID = mainServiceRate.Carrier_Rate_Table_No;
                    rate.MainService.MeasuringQuantity = Convert.ToDecimal(CalculateMeasuringQuantity(request, mainServiceRate));
                    rate.MainService.Charge = CalculateRate(request, rate.MainService, mainServiceRate, context);
                    var shipmentTime = CachedDAO.CarrierShipmentTimeCollection.Where(o=>o.Carrier_Shipment_Time_Id == mainServiceRate.Carrier_Shipment_Time_Id && o.Carrier_Id == rate.CarrierID).FirstOrDefault();
                    if (shipmentTime != null)
                    {
                        rate.ETAText = shipmentTime.Carrier_Shipment_Time_Description;
                        if (shipmentTime.Unit == 1)
                        {
                            rate.ETAValue = DateTime.Now.AddHours(Convert.ToDouble(shipmentTime.Value));
                        }
                        else
                        {
                            rate.ETAValue = DateTime.Now.AddDays(Convert.ToDouble(shipmentTime.Value));
                        }
                    }
                    else
                    {
                        rate.ETAText = "Not Available";
                        rate.ETAValue = DateTime.Now;
                    }

                    rate.CutOffTime = (mainServiceRate.CutoffTime ?? TimeSpan.Zero).ToString();
                    #endregion

                    if (rate.MainService.Charge > 0)
                    {
                        CarrierRate addonServiceRate = null;
                        Service addonService = null;
                        List<Service> addonServiceCollection = null;
                        /// Loop add-on services to get charges
                        for (int i = 0; i < rate.AddonServiceCollection.Count; i++)
                        {
                            addonService = rate.AddonServiceCollection[i];

                            #region [ADDITIONAL SERVICES FILTERATION]

                            if (addonService.Code == "P-RP" && (request.SenderCountry.ID != 9 || !request.IsResidentialPickupRequired))
                            {
                                continue;
                            }
                            else if (addonService.Code == "D-RD" && (request.ReceiverCountry.ID != 9 || !request.IsResidentialDeliveryRequired))
                            {
                                continue;
                            }
                            else if ((addonService.Code == "DG-CM" || addonService.Code == "HPDGS") && !request.IsDangerousGoodsDeliveryRequired)
                            {
                                continue;
                            }

                            #endregion

                            #region [CALCULATE ADDITIONAL SERVICE CHARGE]
                            /// Get add-on carrier rate
                            addonServiceRate = serviceRateCollection.Where(o => o.CF_service_ID == addonService.CFServiceID && o.Carrier_Id == rate.CarrierID).FirstOrDefault();
                            if (addonServiceRate != null)
                            {
                                /// Set complex rate ID
                                addonService.ComplexRateID = addonServiceRate.Carrier_Rate_Table_No;
                                /*
                                if (addonServiceRate.Secondary_Rate_Type_Id == 8 && addonServiceRate.Primary_Rate_Type_Id == 10)
                                {
                                    addonService.MeasuringQuantity = Convert.ToDecimal(rate.MainService.Charge);
                                }
                                else
                                {
                                    addonService.MeasuringQuantity = Convert.ToDecimal(CalculateMeasuringQuantity(request, addonServiceRate, 0));
                                }
                                */
                                //if (addonService.ParentType == 1)
                                //{
                                //    addonService.MeasuringQuantity = Convert.ToDecimal(CalculateMeasuringQuantity(request, addonServiceRate, rate.MainService.Charge));
                                //}
                                //else
                                {
                                    //addonService.MeasuringQuantity = Convert.ToDecimal(CalculateMeasuringQuantity(request, addonServiceRate, rate.MainService.Charge + rate.AddonServiceCollection.Sum(o=>o.Charge)));
                                    addonService.MeasuringQuantity = Convert.ToDecimal(CalculateMeasuringQuantity(request, addonServiceRate, rate.MainService.Charge));
                                }

                                /// Set add-on service charge
                                addonService.Charge = CalculateRate(request, addonService, addonServiceRate, context);
                            }
                            else
                            {
                                addonService.Charge = 0;
                            }
                            #endregion
                        }

                        /// Calculate final charge
                        if (request.ServiceTypeID == (int)ServiceTypeID.International)
                        {
                            Rate.CalculateFinalCharge(rate, 0);
                        }
                        else
                        {
                            Rate.CalculateFinalCharge(rate, request.GSTPercentage);
                        }

                        #region [ADD CARRIER SERVICE MESSAGES]

                        rate.MessageCollection = new List<string>();

                        //if (rate.IsMinimumChargeApplicable == true)
                        //{
                        //    rate.MessageCollection.Add("This carrier charges minimum pickup fee for the booking. Please note your quote price has changed as applicable.");
                        //}

                        if (rate.Carrier.Code == CarrierCodes.AlliedExpress)
                        {
                            /// TODO: Needs to complete
                            if (mainServiceRate.Consigment_Minimum_Charge > 0 && request.Customer.Type == (int)CustomerTypes.Cash)
                            {
                                if (Convert.ToDouble(mainServiceRate.Consigment_Minimum_Charge) > rate.MainService.Charge)
                                {
                                    if (request.CustomerID != Guid.Empty && request.ShipmentDateTime != DateTime.MinValue)
                                    {
                                        if (Convert.ToDouble(mainServiceRate.Consigment_Minimum_Charge) > (rate.MainService.Charge + RateDAO.GetCustomerBookingTotalOfCurrentDay(request, rate, context)))
                                        {
                                            rate.MessageCollection.Add("This carrier charges minimum pickup fee for the booking. Please note your quote price has changed as applicable.");
                                            /// Set minimum charge as the main service charge
                                            rate.MainService.Charge = Convert.ToDouble(mainServiceRate.Consigment_Minimum_Charge);
                                            /// Re-calculate final charge
                                            Rate.CalculateFinalCharge(rate, request.GSTPercentage);
                                        }
                                    }
                                    else
                                    {
                                        rate.MessageCollection.Add("Please note, this carrier will charge minimum pickup fee. If consignment does not reach this, it will be charged the min fee as the booking charge.");
                                    }
                                }
                            }
                        }
                        else if (rate.Carrier.Code == CarrierCodes.DHT)
                        {
                            rate.MessageCollection.Add("This carrier supports business to business pickup/deliveries only. If you need residential pickup/delivery, please select another carrier service.");
                        }
                        else if (rate.Carrier.Code == CarrierCodes.VELLEX)
                        {
                            rate.MessageCollection.Add("This carrier supports business to business pickup/deliveries only. If you need residential pickup/delivery, please select another carrier service.");
                        }

                        /// Remove service message collection details if no messages exists
                        if (rate.MessageCollection.Count == 0)
                        {
                            rate.MessageCollection = null;
                        }
                        #endregion

                        /// Remove additional service details if not requested
                        if (!request.IsAddonServiceDetailsRequired)
                        {
                            rate.AddonServiceCollection = null;
                        }
                        else
                        {
                            /// Only add add-on services that has a charge
                            addonServiceCollection = rate.AddonServiceCollection.Where(o => o.Charge > 0).ToList();
                            rate.AddonServiceCollection.Clear();
                            rate.AddonServiceCollection.AddRange(addonServiceCollection);
                        }
                        /// Change main service ID as CF service ID after all the processing is complete
                        /// End users will see CF service ID as main service ID due to CF service is not visible from WCF service
                        /// CF service ID is a property with the [IgnoreDataMember] attribute
                        rate.MainService.ID = rate.MainService.CFServiceID;
                        /// Add current rate to filtered rate collection
                        /// Lock the filtered rate collection before inserting to make them thread safe
                        lock (lockCheck)
                        {
                            rateCollectionFiltered.Add(rate);
                        }
                    }
                }
                catch //(Exception)
                {
                    /// TODO: Need log the exception and continue
                    //throw;
                }
                finally
                {
                    if (context != null)
                    {
                        context.Dispose();
                    }
                }
            });

            if (rateCollectionFiltered != null)
            {
                /// Sort rate collection by service charge
                rateCollectionFiltered = rateCollectionFiltered.OrderBy(o => o.Charge).ThenBy(o=>o.ETAValue).ToList();
            }

            /// Stop time count
            stopWatch.Stop();

            return new RateResponse()
            {
                Message = String.Format("Successfully completed in {0}", stopWatch.Elapsed),
                RateCollection = rateCollectionFiltered,
                Status = Status.SUCCESS
            };
        }

        /// <summary>
        /// Validate rate request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static RateResponse ValidateAndSetDefaultValues(RateRequest request, DBContext context)
        {
            try
            {
                List<string> messageCollection = new List<string>();
                #region [VALIDATE SENDER DETAILS]
                if (String.IsNullOrEmpty(request.SenderCountryName))
                {
                    messageCollection.Add("Sender country name required");
                }
                else
                {
                    request.SenderCountry = SuburbDAO.GetCountry(request.SenderCountryName, context);
                    if (request.SenderCountry == null)
                    {
                        messageCollection.Add(String.Format("Sender country name \"{0}\" not found", request.SenderCountryName));
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(request.SenderSuburbName))
                        {
                            messageCollection.Add("Sender suburb name required");
                        }
                        else
                        {
                            request.SenderSuburb = SuburbDAO.GetSuburb(request.SenderCountry.ID, request.SenderSuburbName, context);
                            if (request.SenderSuburb == null)
                            {
                                messageCollection.Add(String.Format("Sender suburb name \"{0}\" not found", request.SenderSuburbName));
                            }
                        }

                        if (String.IsNullOrEmpty(request.SenderPostCode))
                        {
                            messageCollection.Add("Sender postcode required");
                        }
                    }
                }
                #endregion

                #region [VALIDATE RECEIVER DETAILS]
                if (String.IsNullOrEmpty(request.ReceiverCountryName))
                {
                    messageCollection.Add("Receiver country name required");
                }
                else
                {
                    request.ReceiverCountry = SuburbDAO.GetCountry(request.ReceiverCountryName, context);
                    if (request.ReceiverCountry == null)
                    {
                        messageCollection.Add(String.Format("Sender country name \"{0}\" not found", request.ReceiverCountryName));
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(request.ReceiverSuburbName))
                        {
                            messageCollection.Add("Receiver suburb name required");
                        }
                        else
                        {
                            request.ReceiverSuburb = SuburbDAO.GetSuburb(request.ReceiverCountry.ID, request.ReceiverSuburbName, context);
                            if (request.ReceiverSuburb == null)
                            {
                                messageCollection.Add(String.Format("Receiver suburb name \"{0}\" not found", request.ReceiverSuburbName));
                            }
                        }

                        if (String.IsNullOrEmpty(request.ReceiverPostCode))
                        {
                            messageCollection.Add("Receiver postcode required");
                        }
                    }
                }
                #endregion

                #region [VALIDATE PACKAGE DETAILS]
                if (request.PackageCollection == null || request.PackageCollection.Count == 0)
                {
                    messageCollection.Add("Package details required");
                }
                else
                {
                    Package package = null;
                    for (int i = 0; i < request.PackageCollection.Count; i++)
                    {
                        package = request.PackageCollection[i];
                        if (package.Length == 0)
                        {
                            messageCollection.Add(String.Format("Package {0} length required", i));
                        }

                        if (package.Width == 0)
                        {
                            messageCollection.Add(String.Format("Package {0} width required", i));
                        }

                        if (package.Height == 0)
                        {
                            messageCollection.Add(String.Format("Package {0} height required", i));
                        }

                        if (package.Weight == 0)
                        {
                            messageCollection.Add(String.Format("Package {0} weight required", i));
                        }
                    }
                }
                #endregion

                #region [SET DEFAULT VALUES]
                if (request.PackageType == 0)
                {
                    /// If not provided set default to "Any"
                    request.PackageType = 11;
                }

                if (request.ServiceTypeID == 0)
                {
                    /// If not provided set default to 1
                    request.ServiceTypeID = 1;
                }
                #endregion

                if (request.CustomerID == Guid.Empty)
                {
                    request.Customer = RateDAO.Map(CachedDAO.WebGuestCustomer);
                }
                else
                {
                    var customer = RateDAO.GetCustomer(request.CustomerID);
                    if (customer == null)
                    {
                        messageCollection.Add(String.Format("Customer ID \"{0}\" is invalid", request.CustomerID));
                    }
                    else
                    {
                        request.Customer = customer;
                    }
                }

                if (String.IsNullOrEmpty(request.PromoCode))
                {
                    request.CustomerScheduleIDActived = RateDAO.GetCustomerScheduleID(request.CustomerID, context);
                }
                else
                {
                    var customerSchedule = RateDAO.GetCustomerScheduleByPromoCode(request.PromoCode, context);
                    if (customerSchedule == null || customerSchedule.Customer_Schedule_Id == Guid.Empty)
                    {
                        //request.CustomerScheduleIDActived = RateDAO.GetCustomerScheduleID(request.CustomerID, context);
                        messageCollection.Add(String.Format("Promo Code \"{0}\" is invalid", request.PromoCode));
                    }
                    else
                    {
                        request.IsPromotionApplied = true;
                        request.CustomerScheduleIDActived = customerSchedule.Customer_Schedule_Id;
                    }
                }

                if (messageCollection.Count > 0)
                {
                    return new RateResponse()
                    {
                        Status = Status.FAIL,
                        Message = "Rate request validation fail for one or more properties. Please see detailed message collection",
                        DetailedMessageCollection = messageCollection
                    };
                }
                else
                {
                    return new RateResponse()
                    {
                        Status = Status.SUCCESS
                    };
                }
            }
            catch (Exception ex)
            {
                return new RateResponse()
                {
                    Status = Status.FAIL,
                    Message = "Error occurred in RateRequest validation",
                    DetailedMessageCollection = new List<string>()
                    {
                        ex.Message
                    }
                };
            }
        }

        /// <summary>
        /// Initialize rate request basic properties
        /// </summary>
        /// <returns></returns>
        private static void InitializeProperties(RateRequest request, DBContext context = null)
        {
            #region [INITIALIZE PROPERTIES]

            request.SenderState = SuburbDAO.GetState(request.SenderCountry.ID, request.SenderSuburb.Name, request.SenderPostCode, context);
            request.ReceiverState = SuburbDAO.GetState(request.ReceiverCountry.ID, request.ReceiverSuburb.Name, request.ReceiverPostCode, context);

            request.SenderZoneIDCollection = String.Join(",", RateDAO.MapZoneCollection(request.SenderCountry, request.SenderState, request.SenderSuburb, request.SenderPostCode, context));
            request.ReceiverZoneIDCollection = String.Join(",", RateDAO.MapZoneCollection(request.ReceiverCountry, request.ReceiverState, request.ReceiverSuburb, request.ReceiverPostCode, context));

            request.GSTPercentage = 10;
            request.PackageCollection.InitializeProperties();

            /// If with in Australia then set the service type ID to "Local"
            if (request.ReceiverCountryName.Equals("Australia", StringComparison.InvariantCultureIgnoreCase) &&
                request.SenderCountryName.Equals("Australia", StringComparison.InvariantCultureIgnoreCase))
            {
                request.ServiceTypeID = (int)ServiceTypeID.Local;
            }
            else
            {
                request.ServiceTypeID = (int)ServiceTypeID.International;
            }

            #endregion
        }

        /// <summary>
        /// Calculate basic rate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rate"></param>
        /// <param name="relatedCarrierRate"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static double CalculateRate(RateRequest request, Service service, CarrierRate relatedCarrierRate, DBContext context = null)
        {
            try
            {
                /*
                var relatedCarrierComplexRateCollection = RateDAO.GetRatesForComplexRateCollection(service);
                CarrierRateComplex relatedCarrierComplexRateFirstItemOrderByToValue = null;
                if (relatedCarrierComplexRateCollection.Count > 0)
                {
                    relatedCarrierComplexRateFirstItemOrderByToValue = relatedCarrierComplexRateCollection[0];
                }
                */
                CarrierRateComplex relatedCarrierComplexRateFirstItemOrderByToValue = RateDAO.GetComplexRate(service);
                switch (relatedCarrierRate.Primary_Rate_Type_Id)
                {
                    case (int)PrimaryRateTypes.ENTER:
                    case (int)PrimaryRateTypes.ZONE_TO_ZONE:
                    case (int)PrimaryRateTypes.TIME:
                    case (int)PrimaryRateTypes.DISTANCE:
                        #region [ZONE_TO_ZONE, DISTANCE, ENTER, TIME]

                        if (relatedCarrierRate.Secondary_Rate_Type_Id == (int)SecondaryRateTypes.FLAT)
                        {
                            /// If complex rate 
                            if (relatedCarrierRate.Carrier_Rate_Table_No != 0)
                            {
                                if (relatedCarrierComplexRateFirstItemOrderByToValue == null)
                                {
                                    return 0;
                                }

                                //return Convert.ToDouble((carrierComplexRate.CarrierBasicCharge > carrierComplexRate.CarrierMinimumCharge) ? carrierComplexRate.CarrierBasicCharge : carrierComplexRate.CarrierMinimumCharge);
                                return GetMaxValue(relatedCarrierComplexRateFirstItemOrderByToValue.CarrierBasicCharge, relatedCarrierComplexRateFirstItemOrderByToValue.CarrierMinimumCharge);
                            }
                            else
                            {
                                //return Convert.ToDouble((carrierRate.Carrier_Basic_Charge > carrierRate.Carrier_Minimum_Charge) ? carrierRate.Carrier_Basic_Charge : carrierRate.Carrier_Minimum_Charge);
                                return GetMaxValue(relatedCarrierRate.Carrier_Basic_Charge, relatedCarrierRate.Carrier_Minimum_Charge);
                            }
                        }
                        else
                        {
                            /// If complex rate 
                            if (relatedCarrierRate.Carrier_Rate_Table_No != 0)
                            {
                                if (relatedCarrierComplexRateFirstItemOrderByToValue == null)
                                {
                                    return 0;
                                }

                                var CarrierRateCalculationBasisId = relatedCarrierComplexRateFirstItemOrderByToValue.RateCalculationBasisId;
                                if (CarrierRateCalculationBasisId == (int)CalculationBasis.RANGE_MULTIPER)
                                {
                                    return CalculateRate_RangeMultiplier(request, service, relatedCarrierComplexRateFirstItemOrderByToValue);
                                }
                                else if (CarrierRateCalculationBasisId == (int)CalculationBasis.RANGE_ACCUMULATOR)
                                {
                                    return CalculateRate_RangeAccumulator(request, service, relatedCarrierComplexRateFirstItemOrderByToValue);
                                }
                                else if (CarrierRateCalculationBasisId == (int)CalculationBasis.RANGE_Total)
                                {
                                    return CalculateRate_RangeTotal(request, service, relatedCarrierComplexRateFirstItemOrderByToValue);
                                }
                                else
                                {
                                    return GetMaxValue(relatedCarrierComplexRateFirstItemOrderByToValue.CarrierBasicCharge, relatedCarrierComplexRateFirstItemOrderByToValue.CarrierMinimumCharge);
                                }
                            }
                            else
                            {
                                if (relatedCarrierRate.Base_Rate == 0)
                                {
                                    //return Convert.ToDouble((carrierRate.Carrier_Basic_Charge > carrierRate.Carrier_Minimum_Charge) ? carrierRate.Carrier_Basic_Charge : carrierRate.Carrier_Minimum_Charge);
                                    return GetMaxValue(relatedCarrierRate.Carrier_Basic_Charge, relatedCarrierRate.Carrier_Minimum_Charge);
                                }
                                else
                                {
                                    var tmpRate = (relatedCarrierRate.Base_Rate * service.MeasuringQuantity) + relatedCarrierRate.Carrier_Basic_Charge;
                                    //return Convert.ToDouble((tmpRate > carrierRate.Carrier_Minimum_Charge) ? tmpRate : carrierRate.Carrier_Minimum_Charge);
                                    return GetMaxValue(tmpRate, relatedCarrierRate.Carrier_Minimum_Charge);
                                }
                            }
                        }

                        #endregion
                    case (int)PrimaryRateTypes.PERCENTAGE:
                        #region [PERCENTAGE]

                        if (relatedCarrierRate.Carrier_Rate_Table_No != 0)
                        {
                            if (relatedCarrierComplexRateFirstItemOrderByToValue == null)
                            {
                                return 0;
                            }

                            var tmpRate = (service.MeasuringQuantity * (relatedCarrierComplexRateFirstItemOrderByToValue.Starting_Charge / 100)) + relatedCarrierRate.Carrier_Basic_Charge;
                            if (relatedCarrierRate.Carrier_Rate_Calculation_Basis_Id == (int)CalculationBasis.RANGE_MULTIPER)
                            {
                                //return Convert.ToDouble((carrierRate.Carrier_Minimum_Charge > tmpRate) ? carrierRate.Carrier_Minimum_Charge : tmpRate);
                                return GetMaxValue(relatedCarrierRate.Carrier_Minimum_Charge, tmpRate);
                            }
                            else
                            {
                                return Convert.ToDouble(tmpRate);
                            }
                        }
                        else
                        {
                            if (relatedCarrierRate.Base_Rate == 0)
                            {
                                //return Convert.ToDouble((carrierRate.Carrier_Basic_Charge > carrierRate.Carrier_Minimum_Charge) ? carrierRate.Carrier_Basic_Charge : carrierRate.Carrier_Minimum_Charge);
                                return GetMaxValue(relatedCarrierRate.Carrier_Basic_Charge, relatedCarrierRate.Carrier_Minimum_Charge);
                            }
                            else
                            {
                                var tmpRate = (service.MeasuringQuantity * (relatedCarrierRate.Base_Rate / 100)) + relatedCarrierRate.Carrier_Basic_Charge;
                                return GetMaxValue(tmpRate, relatedCarrierRate.Carrier_Minimum_Charge);
                            }
                        }

                        #endregion

                    default:
                        return 0;
                }
            }
            catch //(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Calculate range multiplier
        /// </summary>
        /// <returns></returns>
        private static double CalculateRate_RangeMultiplier(RateRequest request, Service service, CarrierRateComplex relatedCarrierComplexRate)
        {
            try
            {
                var minimumMeasuringQuantityAllowed = GetMinimumMeasuringQuantiryAllowed(service, relatedCarrierComplexRate);

                if (relatedCarrierComplexRate.Starting_Charge > 0)
                {
                    #region [COMMENTED OLD CODE LOGIC]
                    /*
                    if (FilteredRecordCollection.Count >= 1)
                    {
                        // split to second slab
                        //rates = (FilteredFirstRecord.Charge * (qty - rateTableDataByTableNo.Where(d => d.CustomerID.Equals(Guid.Empty) && d.ToValue < qty).OrderBy(o => o.ToValue).Last().ToValue)) + FilteredFirstRecord.StartingCharge + FilteredFirstRecord.CarrierBasicCharge;
                        rates = (FilteredFirstRecord.Charge * (qty - carrierRateTableFilteredByAnyCustomerAndCarrierRateTableNoOrderByToValue.Where(d => d.ToValue < qty).Last().ToValue)) + FilteredFirstRecord.StartingCharge + FilteredFirstRecord.CarrierBasicCharge;
                        if (rates < FilteredFirstRecord.CarrierMinimumCharge)
                        {
                            rates = FilteredFirstRecord.CarrierMinimumCharge;
                        }

                        return rates;
                    }
                    else
                    {
                        // only one record ..
                        rates = (FilteredFirstRecord.Charge * qty) + FilteredFirstRecord.StartingCharge + FilteredFirstRecord.CarrierBasicCharge;
                        if (rates < FilteredFirstRecord.CarrierMinimumCharge)
                        {
                            rates = FilteredFirstRecord.CarrierMinimumCharge;
                        }

                        return rates;
                    }
                    */
                    #endregion

                    return GetMaxValue(relatedCarrierComplexRate.CarrierMinimumCharge,
                        (relatedCarrierComplexRate.Charge * minimumMeasuringQuantityAllowed) + relatedCarrierComplexRate.Starting_Charge + relatedCarrierComplexRate.CarrierBasicCharge);
                }
                else
                {
                    if (relatedCarrierComplexRate.Sub_Sequent_Charge > 0 && request.PackageCollection.Count > 1)
                    {
                        var tmpRate = (relatedCarrierComplexRate.Charge * minimumMeasuringQuantityAllowed) + relatedCarrierComplexRate.CarrierBasicCharge +
                            (relatedCarrierComplexRate.Sub_Sequent_Charge * (request.PackageCollection.Count - 1));

                        return GetMaxValue(relatedCarrierComplexRate.CarrierMinimumCharge, tmpRate);
                    }
                    else
                    {
                        var tmpRate = (relatedCarrierComplexRate.Charge * minimumMeasuringQuantityAllowed) + relatedCarrierComplexRate.CarrierBasicCharge;

                        return GetMaxValue(relatedCarrierComplexRate.CarrierMinimumCharge, tmpRate);
                    }
                }
            }
            catch //(Exception)
            {   
                throw;
            }
        }

        /// <summary>
        /// Calculate range accumulator
        /// </summary>
        /// <returns></returns>
        private static double CalculateRate_RangeAccumulator(RateRequest request, Service service, CarrierRateComplex relatedCarrierComplexRate)
        {
            try
            {
                var relatedCarrierComplexRateCollection = RateDAO.GetComplexRateCollection(service).OrderBy(o => o.To_Value).ToList();
                var minimumMeasuringQuantityAllowed = GetMinimumMeasuringQuantiryAllowed(service, relatedCarrierComplexRate);
                var itemIndex = 0;
                decimal remainingQuantiry = minimumMeasuringQuantityAllowed;
                decimal allocatedQuantity = 0;
                decimal rateValue = 0;
                CarrierRateComplex carrierComplexRateItem = null;
                while (remainingQuantiry > 0)
                {
                    carrierComplexRateItem = relatedCarrierComplexRateCollection[itemIndex];
                    if (carrierComplexRateItem.To_Value < remainingQuantiry)
                    {
                        allocatedQuantity = relatedCarrierComplexRateCollection[itemIndex].To_Value;
                        remainingQuantiry -= allocatedQuantity;
                    }
                    else
                    {
                        allocatedQuantity = remainingQuantiry;
                        remainingQuantiry = 0;
                    }

                    rateValue += (allocatedQuantity * carrierComplexRateItem.Charge) + carrierComplexRateItem.Starting_Charge;
                    itemIndex++;
                }

                var maxCarrierComplexRate = relatedCarrierComplexRateCollection[itemIndex -1];
                rateValue += maxCarrierComplexRate.CarrierBasicCharge;

                return Convert.ToDouble(GetMaxValue(rateValue, maxCarrierComplexRate.CarrierMinimumCharge));
            }
            catch //(Exception)
            {   
                throw;
            }
        }

        /// <summary>
        /// Calculate range total
        /// </summary>
        /// <returns></returns>
        private static double CalculateRate_RangeTotal(RateRequest request, Service service, CarrierRateComplex relatedCarrierComplexRate)
        {
            try
            {
                return GetMaxValue(relatedCarrierComplexRate.CarrierMinimumCharge,
                    relatedCarrierComplexRate.Charge + relatedCarrierComplexRate.Starting_Charge + relatedCarrierComplexRate.CarrierBasicCharge
                    );
            }
            catch //(Exception)
            {   
                throw;
            }
        }

        /// <summary>
        /// Get minimum measuring quantity allowed
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="relatedCarrierComplexRate"></param>
        /// <returns></returns>
        private static decimal GetMinimumMeasuringQuantiryAllowed(Service service, CarrierRateComplex relatedCarrierComplexRate)
        {
            //BookingCarrierRateTable FilteredFirstRecord = FilteredRecordCollection.First();
            //decimal MinimumUnit = carrierComplexRate.MinimumUnit; //Change this to minimum unit
            var minimumMeasuringQuantityAllowed = service.MeasuringQuantity;
            if (!relatedCarrierComplexRate.Is_Math_Calculation)
            {
                if (relatedCarrierComplexRate.MinimumUnit != 0 && service.MeasuringQuantity % relatedCarrierComplexRate.MinimumUnit > 0)
                {
                    minimumMeasuringQuantityAllowed = (int)((Math.Floor(service.MeasuringQuantity / relatedCarrierComplexRate.MinimumUnit) * relatedCarrierComplexRate.MinimumUnit) + relatedCarrierComplexRate.MinimumUnit);
                }
            }
            //else
            //{
            //    minimumMeasuringQuantityAllowed = (int)(Math.Round(minimumMeasuringQuantityAllowed, MidpointRounding.AwayFromZero));
            //}
            return minimumMeasuringQuantityAllowed;
        }

        /// <summary>
        /// Calculate measuring quantity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="carrierRate"></param>
        /// <returns></returns>
        private static double CalculateMeasuringQuantity(RateRequest request, CarrierRate carrierRate, double overridenParentCharge = 0)
        {
            try
            {
                double measuringQuantity = 0;
                double cubicWeight = 0;
                double deadWeight = 0;

                cubicWeight = Convert.ToDouble(carrierRate.CubicConversionFactor) * request.PackageCollection.TotalWeight;
                deadWeight = request.PackageCollection.TotalVolume / Convert.ToDouble(carrierRate.CubicConversionFactor);

                switch (carrierRate.Secondary_Rate_Type_Id)
                {
                    case 4: /// Pieces
                        measuringQuantity = request.PackageCollection.Count;
                        break;
                    case 7: /// Weight
                        measuringQuantity = GetMaxValue(deadWeight, request.PackageCollection.TotalWeight);
                        break;
                    case 2: /// Cubic
                        measuringQuantity = GetMaxValue(cubicWeight, request.PackageCollection.TotalVolume);
                        break;
                    case 6: /// Volume
                        measuringQuantity = GetMaxValue(cubicWeight, request.PackageCollection.TotalVolume);
                        break;
                    case 12: /// Dimension (Converted to meters)
                        measuringQuantity = request.PackageCollection.MaxDimension / 100;
                        break;
                    case 13: /// Pallet
                        int palletID = RateDAO.GetCarrierService(carrierRate).Pallet_ID;
                        double palletMeasuringQty = 0;
                        if (palletID != 0)
                        {
                            List<Pallet> Pallet = RateDAO.GetPalletByPalletID(palletID);
                            double palletVolume = 0;
                            double palletCubic = 0;

                            if (Pallet != null && Pallet.Count > 0)
                            {
                                Package item = null;
                                for (int i = 0; i < request.PackageCollection.Count; i++)
                                {
                                    item = request.PackageCollection[i];
                                    /// palletVolume = length * width * height;
                                    palletVolume = Math.Ceiling((item.Length / (Convert.ToDouble(Pallet[0].Length) * 100))) *
                                        Math.Ceiling((item.Height / (Convert.ToDouble(Pallet[0].Height) * 100))) *
                                        Math.Ceiling((item.Height / (Convert.ToDouble(Pallet[0].Width) * 100)));
                                    palletCubic = item.Weight / Convert.ToDouble(Pallet[0].Weight);

                                    palletMeasuringQty += GetMaxValue(palletVolume, palletCubic);
                                }
                            }
                        }

                        measuringQuantity = palletMeasuringQty;
                        break;

                    case 14: ///Weight-Slap 
                        /// TODO: Needs to complete
                        /// 
                        int itemid = RateDAO.GetCarrierService(carrierRate).Item_ID;
                        if (itemid > 0)
                        {
                            decimal itemqty = 0;
                            decimal cubicweightqty = 0;
                            decimal pckgQty = 0;
                            decimal weightSlap = 0;
                                 

                            List<Item> item = RateDAO.GetItemByItemID(itemid);
                            decimal itemweight = 0;

                            if (item != null && item.Count > 0)
                            {
                                itemweight = Convert.ToDecimal(item[0].Weight);

                                if (request.PackageCollection.Count > 0)
                                {
                                    Package PckgItem = null;
                                    for (int i = 0; i < request.PackageCollection.Count; i++)
                                    {
                                        PckgItem = request.PackageCollection[i];
                                        itemqty = Math.Ceiling(Convert.ToInt32(PckgItem.Weight) / itemweight);
                                        cubicweightqty = Math.Ceiling((Convert.ToDecimal((PckgItem.Height * PckgItem.Length * PckgItem.Width)) / carrierRate.CubicConversionFactor) / itemweight);
                                        pckgQty += (cubicweightqty > itemqty) ? cubicweightqty : itemqty;
                                    }
                                }
                                weightSlap = pckgQty * itemweight;
                            }
                            measuringQuantity = Convert.ToDouble(weightSlap);
                        }
                        
                        break;
                    case 15://Weight-Item
                        /// TODO: Needs to complete
                        int itemId = RateDAO.GetCarrierService(carrierRate).Item_ID;
                        if (itemId > 0)
                        {
                            decimal cubicWeightQty = 0;
                            decimal perItemWeight = 0;
                            decimal packgQty = 0;

                            List<Item> Item = RateDAO.GetItemByItemID(itemId);
                            decimal carrierItemWeight = 0;

                            if (Item != null && Item.Count > 0)
                            {
                                carrierItemWeight = Convert.ToDecimal(Item[0].Weight);

                                if (request.PackageCollection.Count > 0)
                                {
                                    Package packageItem = null;
                                    for (int i = 0; i < request.PackageCollection.Count; i++)
                                    {
                                        packageItem = request.PackageCollection[i];
                                        cubicWeightQty = Math.Ceiling(Convert.ToDecimal((packageItem.Height * packageItem.Length * packageItem.Width)) / carrierRate.CubicConversionFactor);
                                        perItemWeight = Math.Ceiling(Convert.ToDecimal(packageItem.Weight));
                                        if (cubicWeightQty > carrierItemWeight)
                                        {
                                            packgQty++;
                                        }
                                        else if (perItemWeight > carrierItemWeight)
                                        {
                                            packgQty++;
                                        }
                                    }
                                }
                            }
                            measuringQuantity = (packgQty > 0) ? 1 : 0;
                        }
                        break;

                    default:
                        switch (carrierRate.Primary_Rate_Type_Id)
                        {
                            case 3: /// Distance
                                try
                                {
                                    measuringQuantity = CANDF.RATES.WEB.SERVICES.Services.GetDistance(request);
                                }
                                catch //(Exception)
                                {
                                    //measuringQuantity = 0;
                                    /// Throw the exception to avoid the rate being processing
                                    /// If 0 is passed, then the rate will be less than the actual rate
                                    /// Better avoid this rate when the distance service is down rather than providing less rates
                                    throw;
                                }

                                break;

                            case 10: /// Percentage
                                measuringQuantity = overridenParentCharge;
                                break;

                            default:
                                measuringQuantity = 1;
                                break;
                        }
                        break;
                }
                return measuringQuantity;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static double GetMaxValue(decimal value1, decimal value2)
        {
            return Convert.ToDouble((value1 > value2) ? value1 : value2);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static double GetMaxValue(double value1, double value2)
        {
            return ((value1 > value2) ? value1 : value2);
        }
    }
}

