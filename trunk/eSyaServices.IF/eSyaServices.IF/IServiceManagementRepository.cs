using HCP.Services.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Services.IF
{
    public interface IServiceManagementRepository
    {
        #region ServiceTypes
        Task<List<DO_ServiceType>> GetServiceTypes();
        Task<DO_ServiceType> GetServiceTypeByID(int ServiceTypeID);
        Task<DO_ReturnParameter> AddOrUpdateServiceType(DO_ServiceType obj);
        Task<DO_ReturnParameter> UpdateServiceTypeIndex(int serviceTypeId, bool isMoveUp, bool isMoveDown);
        Task<DO_ReturnParameter> DeleteServiceType(int serviceTypeId);
        #endregion

        #region ServiceGroups
        Task<List<DO_ServiceGroup>> GetServiceGroups();
        Task<List<DO_ServiceGroup>> GetServiceGroupsByTypeID(int servicetype);
        Task<DO_ServiceGroup> GetServiceGroupByID(int ServiceGroupID);
        Task<DO_ReturnParameter> AddOrUpdateServiceGroup(DO_ServiceGroup obj);
        Task<DO_ReturnParameter> UpdateServiceGroupIndex(int serviceTypeId, int serviceGroupId, bool isMoveUp, bool isMoveDown);
        Task<DO_ReturnParameter> DeleteServiceGroup(int serviceGroupId);
        #endregion

        #region ServiceClass
        Task<List<DO_ServiceClass>> GetServiceClasses();
        Task<List<DO_ServiceClass>> GetServiceClassesByGroupID(int servicegroup);
        Task<DO_ServiceClass> GetServiceClassByID(int ServiceClassID);
        Task<DO_ReturnParameter> AddOrUpdateServiceClass(DO_ServiceClass obj);
        Task<DO_ReturnParameter> UpdateServiceClassIndex(int serviceGroupId, int serviceClassId, bool isMoveUp, bool isMoveDown);
        Task<DO_ReturnParameter> DeleteServiceClass(int serviceClassId);
        #endregion

        #region ServiceCode
        Task<List<DO_ServiceCode>> GetServiceCodes();
        Task<DO_ServiceCode> GetServiceCodeByID(int ServiceID);
        Task<DO_ReturnParameter> AddOrUpdateServiceCode(DO_ServiceCode obj);
        #endregion
       
        #region ServiceCodePattern
        Task<List<DO_ServiceCodePattern>> GetServiceCodePatterns();
        Task<DO_ReturnParameter> AddServiceCodePattern(DO_ServiceCodePattern obj);
        #endregion

        #region ServiceBusinessLink
        Task<List<DO_ServiceBusinessLink>> GetBusinessLocationServices(int businessKey);
        Task<DO_ReturnParameter> AddOrUpdateBusinessLocationServices(List<DO_ServiceBusinessLink> obj);
        Task<List<DO_ServiceBusinessLink>> GetServiceBusinessLocations(int ServiceId);
        Task<DO_ReturnParameter> UpdateServiceBusinessLocations(List<DO_ServiceBusinessLink> obj);
        Task<List<DO_ServiceCode>> GetServiceBusinessLink(int businessKey);
        #endregion

        #region ServiceMaster
        Task<List<DO_ServiceCode>> GetServiceMaster(int servicetype, int servicegroup, int serviceclass);
        #endregion


    }
}
