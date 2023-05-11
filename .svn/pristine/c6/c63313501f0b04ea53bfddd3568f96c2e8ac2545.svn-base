using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HCP.Services.DL.Repository;
using HCP.Services.DO;
using HCP.Services.IF;
using Microsoft.AspNetCore.Mvc;

namespace HCP.Services.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceRatesController : ControllerBase
    {
        private readonly IServiceRatesRepository _ServiceRatesRepository;
        public ServiceRatesController(IServiceRatesRepository serviceRatesRepository)
        {
            _ServiceRatesRepository = serviceRatesRepository;
        }   
       
        
        #region ServiceBaseRate
        public async Task<IActionResult> GetServiceBaseRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode)
        {
            var ac = await _ServiceRatesRepository.GetServiceBaseRateByBKeyRateTypeCurrCode(businessKey,ratetype,currencycode);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceBaseRateByBKeyRateTypeCurrCodeForAdd(int businessKey, int ratetype, string currencycode)
        {
            var defaultDate = DateTime.Now.Date;
            var ac = await _ServiceRatesRepository.GetServiceBaseRateByBKeyRateTypeCurrCode(businessKey, ratetype, currencycode);
            ac = ac.FindAll(w => w.OpbaseRate == 0 && w.IpbaseRate == 0 && w.EffectiveDate == defaultDate);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceBaseRate(List<DO_ServiceBaseRate> obj)
        {
            var msg = await _ServiceRatesRepository.AddOrUpdateServiceBaseRate(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> AddServiceBaseRate(List<DO_ServiceBaseRate> obj)
        {
            var msg = await _ServiceRatesRepository.AddServiceBaseRate(obj);
            return Ok(msg);
        }
        #endregion
        #region ServiceRatePlan
        public async Task<IActionResult> GetServiceRatePlansByBKey(int businessKey)
        {
            var ac = await _ServiceRatesRepository.GetServiceRatePlansByBKey(businessKey);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceRatePlanByBKeyRType(int businessKey,int ratetype)
        {
            var ac = await _ServiceRatesRepository.GetServiceRatePlanByBKeyRType(businessKey,ratetype);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceRatePlan(DO_ServiceRatePlan obj)
        {
            var msg = await _ServiceRatesRepository.AddOrUpdateServiceRatePlan(obj);
            return Ok(msg);
        }
        #endregion
        #region SpecialtyRate
        public async Task<IActionResult> GetServiceSpecialtyRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode,int specialtyId)
        {
            var ac = await _ServiceRatesRepository.GetServiceSpecialtyRateByBKeyRateTypeCurrCode(businessKey, ratetype, currencycode,specialtyId);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceSpecialtyRateByBKeyRateTypeCurrCodeForAdd(int businessKey, int ratetype, string currencycode, int specialtyId)
        {
            var defaultDate = DateTime.Now.Date;
            var ac = await _ServiceRatesRepository.GetServiceSpecialtyRateByBKeyRateTypeCurrCode(businessKey, ratetype, currencycode, specialtyId);
            ac = ac.FindAll(w => w.SpecialtyId==specialtyId && w.ServiceRate == 0  && w.EffectiveDate == defaultDate);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceSpecialtyRate(List<DO_ServiceSpecialtyRate> obj)
        {
            var msg = await _ServiceRatesRepository.AddOrUpdateServiceSpecialtyRate(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> AddServiceSpecialtyRate(List<DO_ServiceSpecialtyRate> obj)
        {
            var msg = await _ServiceRatesRepository.AddServiceSpecialtyRate(obj);
            return Ok(msg);
        }
        #endregion
        #region DoctorRate
        public async Task<IActionResult> GetServiceDoctorRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode, int doctorId)
        {
            var ac = await _ServiceRatesRepository.GetServiceDoctorRateByBKeyRateTypeCurrCode(businessKey, ratetype, currencycode, doctorId);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceDoctorRateByBKeyRateTypeCurrCodeForAdd(int businessKey, int ratetype, string currencycode, int doctorId)
        {
            var defaultDate = DateTime.Now.Date;
            var ac = await _ServiceRatesRepository.GetServiceDoctorRateByBKeyRateTypeCurrCode(businessKey, ratetype, currencycode, doctorId);
            ac = ac.FindAll(w => w.DoctorId==doctorId && w.ServiceRate == 0 && w.EffectiveDate == defaultDate);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceDoctorRate(List<DO_ServiceDoctorRate> obj)
        {
            var msg = await _ServiceRatesRepository.AddOrUpdateServiceDoctorRate(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> AddServiceDoctorRate(List<DO_ServiceDoctorRate> obj)
        {
            var msg = await _ServiceRatesRepository.AddServiceDoctorRate(obj);
            return Ok(msg);
        }
        #endregion

    }
}