using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CANDF.RATES.DAL
{
    public class SuburbDAO
    {
        private static List<int> anyCountryZoneIDCollection;
        /// <summary>
        /// Any country zone id collection
        /// </summary>
        public static List<int> AnyCountryZoneIDCollection
        {
            get
            {
                if (anyCountryZoneIDCollection == null)
                {
                    anyCountryZoneIDCollection = GetZoneIDCollection("[ANY]");
                }

                return anyCountryZoneIDCollection;
            }
        }

        private static List<int> anyAustraliaZoneIDCollection;
        /// <summary>
        /// Any Australia zone id collection
        /// </summary>
        public static List<int> AnyAustraliaZoneIDCollection
        {
            get
            {
                if (anyAustraliaZoneIDCollection == null)
                {
                    anyAustraliaZoneIDCollection = GetZoneIDCollection("[ANY-AUS]");
                }

                return anyAustraliaZoneIDCollection;
            }
        }

        #region [COUNTRY RELATED METHODS]

        /// <summary>
        /// Get country by Country ID
        /// </summary>
        /// <param name="countryID"></param>
        /// <returns></returns>
        public static CANDF.RATES.WCF.SERVICE.LIBRARY.Country GetCountry(int countryID)
        {
            try
            {
                using (DBContext context = new DBContext())
                {
                    //context.Countries.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    //context.Countries.EnablePlanCaching = false;
                    //var country = context.Countries.Where(c => c.Country_Id == countryID).FirstOrDefault();
                    var country = CachedDAO.CountryCollection.Where(c => c.Country_Id == countryID).FirstOrDefault();
                    return new CANDF.RATES.WCF.SERVICE.LIBRARY.Country()
                    {
                        ID = country.Country_Id,
                        Code = country.ISO_3166_13_Letter_Code,
                        Name = country.Country_Name,
                        Description = country.Formal_Name
                    };
                }
            }
            catch //(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get country by Country name
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public static CANDF.RATES.WCF.SERVICE.LIBRARY.Country GetCountry(string countryName, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                //context.Countries.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                //context.Countries.EnablePlanCaching = false;
                //var country = context.Countries.Where(c => c.Country_Name == countryName).FirstOrDefault();
                var country = CachedDAO.CountryCollection.Where(c => c.Country_Name == countryName).FirstOrDefault();
                return new WCF.SERVICE.LIBRARY.Country()
                {
                    ID = country.Country_Id,
                    Code = country.ISO_3166_13_Letter_Code,
                    Name = country.Country_Name,
                    Description = country.Formal_Name
                };
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

        #endregion

        #region [STATE RELATED METHODS]

        /// <summary>
        /// Get state details by CountryID, Suburb Name and Post Code
        /// </summary>
        /// <param name="countryID"></param>
        /// <param name="suburbName"></param>
        /// <param name="postCode"></param>
        /// <returns></returns>
        public static CANDF.RATES.WCF.SERVICE.LIBRARY.State GetState(int countryID, string suburbName, string postCode, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                var result = context.GetStateByCountryIDSuburbNamePostCode(countryID, suburbName, postCode).FirstOrDefault();
                return new WCF.SERVICE.LIBRARY.State()
                {
                    ID = result.ID,
                    Code = result.Code,
                    Name = result.Name,
                    Description = result.Name
                };

                //context.vwSuburbs.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                //context.vwSuburbs.EnablePlanCaching = true;
                //if (postCode == "0000")
                //{
                //    return (from sb in context.vwSuburbs
                //            join st in context.StateforIntegrations on sb.State_Id equals st.State_Id
                //            where sb.Country_Id == countryID && sb.Suburb_Description == suburbName
                //            select new CANDF.RATES.WCF.SERVICE.LIBRARY.State() { ID = st.State_Id, Code = st.State_Code, Name = st.State_Name }
                //            ).FirstOrDefault();
                //}
                //else
                //{
                //    return (from sb in context.vwSuburbs
                //            join st in context.StateforIntegrations on sb.State_Id equals st.State_Id
                //            where sb.Country_Id == countryID && sb.Suburb_Description == suburbName && sb.Post_Code == postCode
                //            select new CANDF.RATES.WCF.SERVICE.LIBRARY.State() { ID = st.State_Id, Code = st.State_Code, Name = st.State_Name }
                //            ).FirstOrDefault();
                //}                
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
        /// Get state by State ID
        /// </summary>
        /// <param name="stateID"></param>
        /// <returns></returns>
        public static CANDF.RATES.WCF.SERVICE.LIBRARY.State GetState(int stateID)
        {
            try
            {
                using (DBContext context = new DBContext())
                {
                    //context.StateforIntegrations.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                    //context.StateforIntegrations.EnablePlanCaching = true;
                    //var state = context.StateforIntegrations.Where(s => s.State_Id == stateID).FirstOrDefault();
                    //return new WCF.SERVICE.LIBRARY.State()
                    //{
                    //    ID = state.State_Id,
                    //    Code = state.State_Code,
                    //    Name = state.State_Name,
                    //    Description = state.State_Name
                    //};

                    var state = context.GetStateByStateID(stateID).FirstOrDefault();
                    return new WCF.SERVICE.LIBRARY.State()
                    {
                        ID = state.ID,
                        Code = state.Code,
                        Name = state.Name,
                        Description = state.Name
                    };
                }
            }
            catch
            {

                throw;
            }
        }

        #endregion

        #region [SUBURB RELATED METHODS]

        /// <summary>
        /// Get Suburb by Country ID and Suburb Name
        /// </summary>
        /// <param name="countryID"></param>
        /// <param name="suburbName"></param>
        /// <returns></returns>
        public static CANDF.RATES.WCF.SERVICE.LIBRARY.Suburb GetSuburb(int countryID, string suburbName, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                //return context.vwSuburbs.Where(s => s.Country_Id == countryID && s.Suburb_Description == suburbName).Select(o =>
                //        new WCF.SERVICE.LIBRARY.Suburb()
                //        {
                //            ID = o.Suburb_ID,
                //            Code = o.Post_Code,
                //            Name = o.Suburb_Description,
                //            Description = String.Empty
                //        }
                //    ).FirstOrDefault();

                //context.vwSuburbs.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                //context.vwSuburbs.EnablePlanCaching = false;
                //var result = context.vwSuburbs.Where(s => s.Country_Id == countryID && s.Suburb_Description == suburbName).FirstOrDefault();
                //return new WCF.SERVICE.LIBRARY.Suburb()
                //{
                //    ID = result.Suburb_ID,
                //    Code = result.Post_Code,
                //    Name = result.Suburb_Description,
                //    Description = String.Empty
                //};

                var result = context.GetSuburbByCountryIDSuburbName(countryID, suburbName).FirstOrDefault();
                return new WCF.SERVICE.LIBRARY.Suburb() { 
                    ID = result.Suburb_ID,
                    Code = result.POst_Code,
                    Name = result.Suburb_Description,
                    Description = result.Suburb_Description
                };
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

        #endregion

        #region [ZONE RELATED METHODS]

        /// <summary>
        /// Get zone ID collection by CF Zone Code
        /// </summary>
        /// <param name="cfZoneCode"></param>
        /// <returns></returns>
        public static List<int> GetZoneIDCollection(string cfZoneCode, DBContext context = null)
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                //context.CF_Zone.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                //context.CF_Zone.EnablePlanCaching = false;
                if (!String.IsNullOrEmpty(cfZoneCode))
                {
                    //List<int> zoneIDs = (from p in context.CF_Zone
                    //                     where p.CF_Zone_Code == cfZoneCode
                    //                     select p.CF_Zone_Id).ToList<int>();
                    //return zoneIDs;
                    return context.GetZoneIDCollectionByCFZoneCode(cfZoneCode).Select(o=>o.Value).ToList<int>();
                }
                else
                    return null;
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
        /// Get zone ID collection
        /// </summary>
        /// <param name="countryID"></param>
        /// <param name="stateID"></param>
        /// <param name="postCode"></param>
        /// <param name="subrubDescription"></param>
        /// <returns></returns>
        public static List<int> GetZoneIDCollection(
            CANDF.RATES.WCF.SERVICE.LIBRARY.Country country,
            CANDF.RATES.WCF.SERVICE.LIBRARY.State state,
            CANDF.RATES.WCF.SERVICE.LIBRARY.Suburb suburb,
            string postCode, DBContext context
            )
        {
            var isNewContext = false;
            try
            {
                if (context == null)
                {
                    context = new DBContext();
                    isNewContext = true;
                }

                //context.vwSuburbs.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                //context.vwSuburbs.EnablePlanCaching = false;
                //return (from p in context.vwSuburbs
                //        where p.Country_Id == country.ID &&
                //            p.State_Id == state.ID &&
                //            p.Suburb_Description == suburb.Name &&
                //            p.Post_Code == postCode
                //        select p.Zone_Id).ToList();

                return context.GetZoneIDCollectionByContryIDStateIDSubNmPstCd(country.ID, state.ID, suburb.Name, postCode).Select(o => o.Value).ToList<int>();

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

        #endregion
    }
}
