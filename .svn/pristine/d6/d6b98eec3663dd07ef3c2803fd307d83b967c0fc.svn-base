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
    public class CommonMethodController : ControllerBase
    {
        private readonly ICommonMethodRepository _CommonMethodRepository;
        public CommonMethodController(ICommonMethodRepository commonmethodRepository)
        {
            _CommonMethodRepository = commonmethodRepository;
        }
        #region Common
        public async Task<IActionResult> GetBusinessKey()
        {
            var ac = await _CommonMethodRepository.GetBusinessKey();
            return Ok(ac);
        }
        public async Task<IActionResult> GetApplicationCodesByCodeType(int codetype)
        {
            var ac = await _CommonMethodRepository.GetApplicationCodesByCodeType(codetype);
            return Ok(ac);
        }
        public async Task<IActionResult> GetCurrencyCodes()
        {
            var ac = await _CommonMethodRepository.GetCurrencyCodes();
            return Ok(ac);
        }
        public async Task<IActionResult> GetDoctors()
        {
            var ac = await _CommonMethodRepository.GetDoctors();
            return Ok(ac);
        }
        public async Task<IActionResult> GetSpecialties()
        {
            var ac = await _CommonMethodRepository.GetSpecialties();
            return Ok(ac);
        }
        #endregion

    }
}