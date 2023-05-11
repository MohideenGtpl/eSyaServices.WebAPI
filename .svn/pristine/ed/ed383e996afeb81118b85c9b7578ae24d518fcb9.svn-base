using HCP.Services.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Services.IF
{
    public interface IServiceRatesRepository
    {
              
        #region ServiceBaseRate
        Task<List<DO_ServiceBaseRate>> GetServiceBaseRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode);
        Task<DO_ReturnParameter> AddOrUpdateServiceBaseRate(List<DO_ServiceBaseRate> obj);
        Task<DO_ReturnParameter> AddServiceBaseRate(List<DO_ServiceBaseRate> obj);
        #endregion

        #region ServiceRatePlan
        Task<List<DO_ServiceRatePlan>> GetServiceRatePlansByBKey(int businessKey);
        Task<DO_ServiceRatePlan> GetServiceRatePlanByBKeyRType(int businessKey, int ratetype);
        Task<DO_ReturnParameter> AddOrUpdateServiceRatePlan(DO_ServiceRatePlan obj);
        #endregion

        #region SpcialtyRate
        Task<List<DO_ServiceSpecialtyRate>> GetServiceSpecialtyRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode,int specialtId);
        Task<DO_ReturnParameter> AddOrUpdateServiceSpecialtyRate(List<DO_ServiceSpecialtyRate> obj);
        Task<DO_ReturnParameter> AddServiceSpecialtyRate(List<DO_ServiceSpecialtyRate> obj);
        #endregion

        #region DoctorRate
        Task<List<DO_ServiceDoctorRate>> GetServiceDoctorRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode, int doctorId);
        Task<DO_ReturnParameter> AddOrUpdateServiceDoctorRate(List<DO_ServiceDoctorRate> obj);
        Task<DO_ReturnParameter> AddServiceDoctorRate(List<DO_ServiceDoctorRate> obj);
        #endregion

    }
}
