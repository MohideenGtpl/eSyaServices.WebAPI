﻿using System;
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
    public class ServiceManagementController : ControllerBase
    {
        private readonly IServiceManagementRepository _ServiceManagementRepository;
        public ServiceManagementController(IServiceManagementRepository serviceManagementRepository)
        {
            _ServiceManagementRepository = serviceManagementRepository;
        }
        
        #region ServiceType
        public async Task<IActionResult> GetServiceTypes()
        {
            var ac = await _ServiceManagementRepository.GetServiceTypes();
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceTypeByID(int ServiceTypeID)
        {
            var ac = await _ServiceManagementRepository.GetServiceTypeByID(ServiceTypeID);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceType(DO_ServiceType obj)
        {
            var msg = await _ServiceManagementRepository.AddOrUpdateServiceType(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> UpdateServiceTypeIndex(int serviceTypeId, bool isMoveUp, bool isMoveDown)
        {
            var msg = await _ServiceManagementRepository.UpdateServiceTypeIndex(serviceTypeId, isMoveUp, isMoveDown);
            return Ok(msg);
        }
        public async Task<IActionResult> DeleteServiceType(int serviceTypeId)
        {
            var msg = await _ServiceManagementRepository.DeleteServiceType(serviceTypeId);
            return Ok(msg);
        }
        #endregion
        #region ServiceGroup
        public async Task<IActionResult> GetServiceGroups()
        {
            var ac = await _ServiceManagementRepository.GetServiceGroups();
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceGroupsByTypeID(int servicetype)
        {
            var ac = await _ServiceManagementRepository.GetServiceGroupsByTypeID(servicetype);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceGroupByID(int ServiceGroupID)
        {
            var ac = await _ServiceManagementRepository.GetServiceGroupByID(ServiceGroupID);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceGroup(DO_ServiceGroup obj)
        {
            var msg = await _ServiceManagementRepository.AddOrUpdateServiceGroup(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> UpdateServiceGroupIndex(int serviceTypeId, int serviceGroupId, bool isMoveUp, bool isMoveDown)
        {
            var msg = await _ServiceManagementRepository.UpdateServiceGroupIndex(serviceTypeId,serviceGroupId,isMoveUp,isMoveDown);
            return Ok(msg);
        }
        public async Task<IActionResult> DeleteServiceGroup(int serviceGroupId)
        {
            var msg = await _ServiceManagementRepository.DeleteServiceGroup(serviceGroupId);
            return Ok(msg);
        }
        #endregion
        #region ServiceClass
        public async Task<IActionResult> GetServiceClasses()
        {
            var ac = await _ServiceManagementRepository.GetServiceClasses();
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceClassesByGroupID(int servicegroup)
        {
            var ac = await _ServiceManagementRepository.GetServiceClassesByGroupID(servicegroup);
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceClassByID(int ServiceClassID)
        {
            var ac = await _ServiceManagementRepository.GetServiceClassByID(ServiceClassID);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceClass(DO_ServiceClass obj)
        {
            var msg = await _ServiceManagementRepository.AddOrUpdateServiceClass(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> UpdateServiceClassIndex(int serviceGroupId, int serviceClassId, bool isMoveUp, bool isMoveDown)
        {
            var msg = await _ServiceManagementRepository.UpdateServiceClassIndex(serviceGroupId, serviceClassId, isMoveUp, isMoveDown);
            return Ok(msg);
        }
        public async Task<IActionResult> DeleteServiceClass(int serviceClassId)
        {
            var msg = await _ServiceManagementRepository.DeleteServiceClass(serviceClassId);
            return Ok(msg);
        }
        #endregion
        #region ServiceCode
        public async Task<IActionResult> GetServiceCodes()
        {
            var ac = await _ServiceManagementRepository.GetServiceCodes();
            return Ok(ac);
        }
        public async Task<IActionResult> GetServiceCodeByID(int ServiceID)
        {
            var ac = await _ServiceManagementRepository.GetServiceCodeByID(ServiceID);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateServiceCode(DO_ServiceCode obj)
        {
            var msg = await _ServiceManagementRepository.AddOrUpdateServiceCode(obj);
            return Ok(msg);
        }
        #endregion
        #region ServiceCodePattern
        public async Task<IActionResult> GetServiceCodePatterns()
        {
            var ac = await _ServiceManagementRepository.GetServiceCodePatterns();
            return Ok(ac);
        }
        public async Task<IActionResult> AddServiceCodePattern(DO_ServiceCodePattern obj)
        {
            var msg = await _ServiceManagementRepository.AddServiceCodePattern(obj);
            return Ok(msg);
        }
        #endregion
        #region ServiceBusinessLink
        
        public async Task<IActionResult> GetBusinessLocationServices(int businessKey)
        {
            var ac = await _ServiceManagementRepository.GetBusinessLocationServices(businessKey);
            return Ok(ac);
        }
        public async Task<IActionResult> AddOrUpdateBusinessLocationServices(List<DO_ServiceBusinessLink> obj)
        {
            var msg = await _ServiceManagementRepository.AddOrUpdateBusinessLocationServices(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> GetServiceBusinessLocations(int ServiceId)
        {
            var ac = await _ServiceManagementRepository.GetServiceBusinessLocations(ServiceId);
            return Ok(ac);
        }
        public async Task<IActionResult> UpdateServiceBusinessLocations(List<DO_ServiceBusinessLink> obj)
        {
            var msg = await _ServiceManagementRepository.UpdateServiceBusinessLocations(obj);
            return Ok(msg);
        }
        public async Task<IActionResult> GetServiceBusinessLink(int businessKey)
        {
            var ac = await _ServiceManagementRepository.GetServiceBusinessLink(businessKey);
            return Ok(ac);
        }
        #endregion
        #region ServiceMaster
        public async Task<IActionResult> GetServiceMaster(int servicetype, int servicegroup, int serviceclass)
        {
            var ac = await _ServiceManagementRepository.GetServiceMaster(servicetype, servicegroup, serviceclass);
            return Ok(ac);
        }

        #endregion

    }
}