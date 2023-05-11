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
    public class ClinicServicesController : ControllerBase
    {
        private readonly IClinicServicesRepository _ClinicServicesRepository;
        public ClinicServicesController(IClinicServicesRepository clinicServicesRepository)
        {
            _ClinicServicesRepository = clinicServicesRepository;
        }

        #region ClinicServiceLink
        public async Task<IActionResult> GetClinicServiceLinkByBKey(int businessKey)
        {
            var ac = await _ClinicServicesRepository.GetClinicServiceLinkByBKey(businessKey);
            return Ok(ac);
        }
        public async Task<IActionResult> GetConsultationTypeByBKeyClinicType(int businessKey, int clinictype)
        {
            var ac = await _ClinicServicesRepository.GetConsultationTypeByBKeyClinicType(businessKey,clinictype);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServicesPerformedByDoctor()
        {
            var ac = await _ClinicServicesRepository.GetServicesPerformedByDoctor();
            return Ok(ac);
        }
        public async Task<IActionResult> AddClinicServiceLink(DO_ClinicServiceLink obj)
        {
            var msg = await _ClinicServicesRepository.AddClinicServiceLink(obj);
            return Ok(msg);
        }
        #endregion
        #region ClinicVisitRate
        public async Task<IActionResult> GetClinicVisitRateByBKeyClinicTypeCurrCodeRateType(int businessKey, int clinictype, string currencycode,int ratetype)
        {
            var ac = await _ClinicServicesRepository.GetClinicVisitRateByBKeyClinicTypeCurrCodeRateType(businessKey, clinictype, currencycode,ratetype);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateClinicVisitRate(List<DO_ClinicVisitRate> obj)
        {
            var msg = await _ClinicServicesRepository.AddOrUpdateClinicVisitRate(obj);
            return Ok(msg);
        }
        #endregion
        #region ClinicDoctorRate
        public async Task<IActionResult> GetClinicDoctorRateByBKeyDoctorIDCurrCode(int businessKey, int doctorid, string currencycode)
        {
            var ac = await _ClinicServicesRepository.GetClinicDoctorRateByBKeyDoctorIDCurrCode(businessKey, doctorid, currencycode);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateClinicDoctorRate(List<DO_ClinicVisitRate> obj)
        {
            var msg = await _ClinicServicesRepository.AddOrUpdateClinicDoctorRate(obj);
            return Ok(msg);
        }
        #endregion
    }
}