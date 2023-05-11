﻿using HCP.Services.DL.Entities;
using HCP.Services.DO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Services.IF;

namespace HCP.Services.DL.Repository
{
    public class ServiceManagementRepository : IServiceManagementRepository
    {
        #region ServiceTypes
        public async Task<List<DO_ServiceType>> GetServiceTypes()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrty
                                 .Select(x => new DO_ServiceType
                                 {
                                     ServiceTypeId = x.ServiceTypeId,
                                     ServiceTypeDesc = x.ServiceTypeDesc,
                                     PrintSequence = x.PrintSequence,
                                     ActiveStatus = x.ActiveStatus
                                 }
                        ).OrderBy(o => o.PrintSequence).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ServiceType> GetServiceTypeByID(int ServiceTypeID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrty
                        .Where(i => i.ServiceTypeId == ServiceTypeID)
                                 .Select(x => new DO_ServiceType
                                 {
                                     ServiceTypeId = x.ServiceTypeId,
                                     ServiceTypeDesc = x.ServiceTypeDesc,
                                     ActiveStatus = x.ActiveStatus
                                 }
                        ).FirstOrDefaultAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceType(DO_ServiceType obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (obj.ServiceTypeId == 0)
                        {
                            var RecordExist = db.GtEssrty.Where(w => w.ServiceTypeDesc == obj.ServiceTypeDesc).Count();
                            if (RecordExist > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, Message = "Service Type Already Exists" };
                            }
                            else
                            {


                                var newServiceTypeId = db.GtEssrty.Select(a => (int)a.ServiceTypeId).DefaultIfEmpty(0).Max() + 1;

                                var servicetype = new GtEssrty
                                {
                                    ServiceTypeId = newServiceTypeId,
                                    ServiceTypeDesc = obj.ServiceTypeDesc,
                                    PrintSequence = newServiceTypeId,
                                    ActiveStatus = obj.ActiveStatus,
                                    FormId = obj.FormID,
                                    CreatedBy = obj.UserID,
                                    CreatedOn = obj.CreatedOn,
                                    CreatedTerminal = obj.TerminalID
                                };
                                db.GtEssrty.Add(servicetype);

                            }
                        }
                        else
                        {
                            if (!obj.ActiveStatus)
                            {
                                var LinkExist = db.GtEssrgr.Where(w => w.ServiceTypeId == obj.ServiceTypeId && w.ActiveStatus).Count();
                                if (LinkExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "This Service Type is linked to a Service Group(s). Please unlink it before deactivating" };
                                }
                            }
                            var updatedServiceType = db.GtEssrty.Where(w => w.ServiceTypeId == obj.ServiceTypeId).FirstOrDefault();
                            if (updatedServiceType.ServiceTypeDesc != obj.ServiceTypeDesc)
                            {
                                var RecordExist = db.GtEssrty.Where(w => w.ServiceTypeDesc == obj.ServiceTypeDesc).Count();
                                if (RecordExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "Service Type Description Already Exists" };
                                }

                            }

                            updatedServiceType.ServiceTypeDesc = obj.ServiceTypeDesc;
                            updatedServiceType.ActiveStatus = obj.ActiveStatus;
                            updatedServiceType.ModifiedBy = obj.UserID;
                            updatedServiceType.ModifiedOn = obj.CreatedOn;
                            updatedServiceType.ModifiedTerminal = obj.TerminalID;



                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<DO_ReturnParameter> UpdateServiceTypeIndex(int serviceTypeId, bool isMoveUp, bool isMoveDown)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var stCurrent = db.GtEssrty.Where(w => w.ServiceTypeId == serviceTypeId).FirstOrDefault();
                        int switchIndex = 0;

                        if (isMoveUp)
                        {
                            var isTop = db.GtEssrty.Where(w => w.PrintSequence < stCurrent.PrintSequence).Count();
                            if (isTop > 0)
                            {
                                var stTarget = db.GtEssrty.Where(w => w.PrintSequence < stCurrent.PrintSequence).OrderByDescending(o => o.PrintSequence).FirstOrDefault();
                                switchIndex = stCurrent.PrintSequence;
                                stCurrent.PrintSequence = stTarget.PrintSequence;
                                stTarget.PrintSequence = switchIndex;
                            }
                            else
                            {
                                return new DO_ReturnParameter() { Status = false, Message = stCurrent.ServiceTypeDesc + " is already on top" };
                            }
                        }
                        else if (isMoveDown)
                        {
                            var isBottom = db.GtEssrty.Where(w => w.PrintSequence > stCurrent.PrintSequence).Count();
                            if (isBottom > 0)
                            {
                                var stTarget = db.GtEssrty.Where(w => w.PrintSequence > stCurrent.PrintSequence).OrderBy(o => o.PrintSequence).FirstOrDefault();
                                switchIndex = stCurrent.PrintSequence;
                                stCurrent.PrintSequence = stTarget.PrintSequence;
                                stTarget.PrintSequence = switchIndex;
                            }
                            else
                            {
                                return new DO_ReturnParameter() { Status = false, Message = stCurrent.ServiceTypeDesc + " is already on bottom" };
                            }
                        }




                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter() { Status = true };
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<DO_ReturnParameter> DeleteServiceType(int serviceTypeId)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var LinkExist = db.GtEssrgr.Where(w => w.ServiceTypeId == serviceTypeId).Count();
                        if (LinkExist > 0)
                        {
                            return new DO_ReturnParameter() { Status = false, Message = "This Service Type is linked to a Service Group(s). Please unlink it before deletion" };
                        }

                        var ServiceType = db.GtEssrty.Where(w => w.ServiceTypeId == serviceTypeId).FirstOrDefault();
                        if (ServiceType != null)
                        {
                            db.GtEssrty.Remove(ServiceType);
                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region ServiceGroups
        public async Task<List<DO_ServiceGroup>> GetServiceGroups()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrgr
                                 .Select(x => new DO_ServiceGroup
                                 {
                                     ServiceTypeId = x.ServiceTypeId,
                                     ServiceGroupId = x.ServiceGroupId,
                                     ServiceGroupDesc = x.ServiceGroupDesc,
                                     ServiceCriteria=x.ServiceCriteria,
                                     PrintSequence = x.PrintSequence,
                                     ActiveStatus = x.ActiveStatus
                                 }
                        ).OrderBy(g => g.PrintSequence).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_ServiceGroup>> GetServiceGroupsByTypeID(int servicetype)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrgr
                        .Where(w => w.ServiceTypeId == servicetype && w.ActiveStatus)
                                 .Select(x => new DO_ServiceGroup
                                 {
                                     ServiceGroupId = x.ServiceGroupId,
                                     ServiceGroupDesc = x.ServiceGroupDesc,
                                     ServiceCriteria=x.ServiceCriteria,
                                     PrintSequence = x.PrintSequence,
                                 }
                        ).OrderBy(g => g.PrintSequence).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ServiceGroup> GetServiceGroupByID(int ServiceGroupID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrgr
                        .Where(i => i.ServiceGroupId == ServiceGroupID)
                                 .Select(x => new DO_ServiceGroup
                                 {
                                     ServiceGroupId = x.ServiceGroupId,
                                     ServiceGroupDesc = x.ServiceGroupDesc,
                                     ServiceCriteria=x.ServiceCriteria,
                                     ActiveStatus = x.ActiveStatus
                                 }
                        ).FirstOrDefaultAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceGroup(DO_ServiceGroup obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (obj.ServiceGroupId == 0)
                        {
                            var RecordExist = db.GtEssrgr.Where(w => w.ServiceGroupDesc == obj.ServiceGroupDesc).Count();
                            if (RecordExist > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, Message = "Service Group Already Exists" };
                            }
                            else
                            {


                                var newServiceGroupId = db.GtEssrgr.Select(a => (int)a.ServiceGroupId).DefaultIfEmpty(0).Max() + 1;
                                var newPrintSequence = db.GtEssrgr.Where(w => w.ServiceTypeId == obj.ServiceTypeId).Select(a => (int)a.PrintSequence).DefaultIfEmpty(0).Max() + 1;

                                var servicegroup = new GtEssrgr
                                {
                                    ServiceTypeId = obj.ServiceTypeId,
                                    ServiceGroupId = newServiceGroupId,
                                    ServiceGroupDesc = obj.ServiceGroupDesc,
                                    ServiceCriteria=obj.ServiceCriteria,
                                    PrintSequence = newPrintSequence,
                                    ActiveStatus = obj.ActiveStatus,
                                    FormId = obj.FormId,
                                    CreatedBy = obj.UserID,
                                    CreatedOn = obj.CreatedOn,
                                    CreatedTerminal = obj.TerminalID
                                };
                                db.GtEssrgr.Add(servicegroup);

                            }
                        }
                        else
                        {
                            if (!obj.ActiveStatus)
                            {
                                var LinkExist = db.GtEssrcl.Where(w => w.ServiceGroupId == obj.ServiceGroupId && w.ActiveStatus).Count();
                                if (LinkExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "This Service Group is linked to a Service Class(es). Please unlink it before deactivating" };
                                }
                            }
                            var updatedServiceGroup = db.GtEssrgr.Where(w => w.ServiceGroupId == obj.ServiceGroupId).FirstOrDefault();
                            if (updatedServiceGroup.ServiceGroupDesc != obj.ServiceGroupDesc)
                            {
                                var RecordExist = db.GtEssrgr.Where(w => w.ServiceGroupDesc == obj.ServiceGroupDesc).Count();
                                if (RecordExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "Service Group Description Already Exists" };
                                }
                            }
                            updatedServiceGroup.ServiceGroupDesc = obj.ServiceGroupDesc;
                            updatedServiceGroup.ServiceCriteria = obj.ServiceCriteria;
                            updatedServiceGroup.ActiveStatus = obj.ActiveStatus;
                            updatedServiceGroup.ModifiedBy = obj.UserID;
                            updatedServiceGroup.ModifiedOn = obj.CreatedOn;
                            updatedServiceGroup.ModifiedTerminal = obj.TerminalID;

                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<DO_ReturnParameter> UpdateServiceGroupIndex(int serviceTypeId, int serviceGroupId, bool isMoveUp, bool isMoveDown)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var sgCurrent = db.GtEssrgr.Where(w => w.ServiceTypeId == serviceTypeId && w.ServiceGroupId == serviceGroupId).FirstOrDefault();
                        int switchIndex = 0;
                        if (isMoveUp)
                        {
                            var isTop = db.GtEssrgr.Where(w => w.PrintSequence < sgCurrent.PrintSequence && w.ServiceTypeId == serviceTypeId).Count();
                            if (isTop > 0)
                            {
                                var sgTarget = db.GtEssrgr.Where(w => w.PrintSequence < sgCurrent.PrintSequence && w.ServiceTypeId == serviceTypeId).OrderByDescending(o => o.PrintSequence).FirstOrDefault();
                                switchIndex = sgCurrent.PrintSequence;
                                sgCurrent.PrintSequence = sgTarget.PrintSequence;
                                sgTarget.PrintSequence = switchIndex;
                            }
                            else
                            {
                                return new DO_ReturnParameter() { Status = false, Message = sgCurrent.ServiceGroupDesc + " is already on top" };
                            }
                        }
                        else if (isMoveDown)
                        {
                            var isBottom = db.GtEssrgr.Where(w => w.PrintSequence > sgCurrent.PrintSequence && w.ServiceTypeId == serviceTypeId).Count();
                            if (isBottom > 0)
                            {
                                var sgTarget = db.GtEssrgr.Where(w => w.PrintSequence > sgCurrent.PrintSequence && w.ServiceTypeId == serviceTypeId).OrderBy(o => o.PrintSequence).FirstOrDefault();
                                switchIndex = sgCurrent.PrintSequence;
                                sgCurrent.PrintSequence = sgTarget.PrintSequence;
                                sgTarget.PrintSequence = switchIndex;
                            }
                            else
                            {
                                return new DO_ReturnParameter() { Status = false, Message = sgCurrent.ServiceGroupDesc + " is already on bottom" };
                            }
                        }


                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter() { Status = true };
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<DO_ReturnParameter> DeleteServiceGroup(int serviceGroupId)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {

                        var LinkExist = db.GtEssrcl.Where(w => w.ServiceGroupId == serviceGroupId).Count();
                        if (LinkExist > 0)
                        {
                            return new DO_ReturnParameter() { Status = false, Message = "This Service Group is linked to a Service Class(es). Please unlink it before deactivating" };
                        }

                        var ServiceGroup = db.GtEssrgr.Where(w => w.ServiceGroupId == serviceGroupId).FirstOrDefault();
                        if (ServiceGroup != null)
                        {
                            db.GtEssrgr.Remove(ServiceGroup);
                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region ServiceClass
        public async Task<List<DO_ServiceClass>> GetServiceClasses()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrcl
                                 .Select(x => new DO_ServiceClass
                                 {
                                     ServiceGroupId = x.ServiceGroupId,
                                     ServiceClassId = x.ServiceClassId,
                                     ServiceClassDesc = x.ServiceClassDesc,
                                     IsBaseRateApplicable = x.IsBaseRateApplicable,
                                     ParentId = x.ParentId,
                                     PrintSequence = x.PrintSequence,
                                     ActiveStatus = x.ActiveStatus
                                 }
                        ).OrderBy(g => g.PrintSequence).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_ServiceClass>> GetServiceClassesByGroupID(int servicegroup)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrcl
                        .Where(w => w.ServiceGroupId == servicegroup && w.ActiveStatus)
                                 .Select(x => new DO_ServiceClass
                                 {
                                     ServiceClassId = x.ServiceClassId,
                                     ServiceClassDesc = x.ServiceClassDesc,
                                     PrintSequence = x.PrintSequence,
                                 }
                        ).OrderBy(g => g.PrintSequence).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ServiceClass> GetServiceClassByID(int ServiceClassID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrcl
                        .Where(i => i.ServiceClassId == ServiceClassID)
                                 .Select(x => new DO_ServiceClass
                                 {
                                     ServiceClassId = x.ServiceClassId,
                                     ServiceClassDesc = x.ServiceClassDesc,
                                     IsBaseRateApplicable = x.IsBaseRateApplicable,
                                     ActiveStatus = x.ActiveStatus,
                                     l_ClassParameter = x.GtEspasc.Select(p => new DO_eSyaParameter
                                     {
                                         ParameterID = p.ParameterId,
                                         ParmAction = p.ParmAction,
                                         ParmPerc = p.ParmPerc,
                                         ParmDesc = p.ParmDesc,
                                         ParmValue = p.ParmValue,
                                     }).ToList()
                                 }
                        ).FirstOrDefaultAsync();

                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceClass(DO_ServiceClass obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (obj.ServiceClassId == 0)
                        {
                            var RecordExist = db.GtEssrcl.Where(w => w.ServiceClassDesc == obj.ServiceClassDesc).Count();
                            if (RecordExist > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, Message = "Service Class Already Exists" };
                            }
                            else
                            {


                                var newServiceClassId = db.GtEssrcl.Select(a => (int)a.ServiceClassId).DefaultIfEmpty(0).Max() + 1;
                                var parentId = obj.ParentId;
                                if (parentId == 0)
                                {
                                    parentId = newServiceClassId;
                                }
                                var newPrintSequence = db.GtEssrcl.Where(w => w.ServiceGroupId == obj.ServiceGroupId).Select(a => (int)a.PrintSequence).DefaultIfEmpty(0).Max() + 1;

                                var serviceclass = new GtEssrcl
                                {
                                    ServiceGroupId = obj.ServiceGroupId,
                                    ServiceClassId = newServiceClassId,
                                    ServiceClassDesc = obj.ServiceClassDesc,
                                    IsBaseRateApplicable = obj.IsBaseRateApplicable,
                                    ParentId = parentId,
                                    PrintSequence = newPrintSequence,
                                    ActiveStatus = obj.ActiveStatus,
                                    FormId = obj.FormId,
                                    CreatedBy = obj.UserID,
                                    CreatedOn = obj.CreatedOn,
                                    CreatedTerminal = obj.TerminalID
                                };
                                db.GtEssrcl.Add(serviceclass);
                                foreach (DO_eSyaParameter cp in obj.l_ClassParameter)
                                {
                                    var cParameter = new GtEspasc
                                    {
                                        ServiceClassId = newServiceClassId,
                                        ParameterId = cp.ParameterID,
                                        ParmPerc = cp.ParmPerc,
                                        ParmAction = cp.ParmAction,
                                        ParmDesc = cp.ParmDesc,
                                        ParmValue = cp.ParmValue,
                                        ActiveStatus = cp.ActiveStatus,
                                        FormId = obj.FormId,
                                        CreatedBy = obj.UserID,
                                        CreatedOn = System.DateTime.Now,
                                        CreatedTerminal = obj.TerminalID,
                                    };
                                    db.GtEspasc.Add(cParameter);

                                }
                            }
                        }
                        else
                        {
                            if (!obj.ActiveStatus)
                            {
                                var cLinkExist = db.GtEssrcl.Where(w => w.ParentId == obj.ServiceClassId && w.ParentId != w.ServiceClassId && w.ActiveStatus).Count();
                                if (cLinkExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "This Service Class is linked to another Service Class(es). Please unlink it before deactivating" };
                                }
                                var sLinkExist = db.GtEssrms.Where(w => w.ServiceClassId == obj.ServiceClassId && w.ActiveStatus).Count();
                                if (sLinkExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "This Service Class is linked to a Service(s). Please unlink it before deactivating" };
                                }
                            }
                            var updatedServiceClass = db.GtEssrcl.Where(w => w.ServiceClassId == obj.ServiceClassId).FirstOrDefault();
                            if (updatedServiceClass.ServiceClassDesc != obj.ServiceClassDesc)
                            {
                                var RecordExist = db.GtEssrcl.Where(w => w.ServiceClassDesc == obj.ServiceClassDesc).Count();
                                if (RecordExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "Service Class Description Already Exists" };
                                }
                            }
                            updatedServiceClass.ServiceClassDesc = obj.ServiceClassDesc;
                            updatedServiceClass.IsBaseRateApplicable = obj.IsBaseRateApplicable;
                            updatedServiceClass.ActiveStatus = obj.ActiveStatus;
                            updatedServiceClass.ModifiedBy = obj.UserID;
                            updatedServiceClass.ModifiedOn = obj.CreatedOn;
                            updatedServiceClass.ModifiedTerminal = obj.TerminalID;

                            foreach (DO_eSyaParameter cp in obj.l_ClassParameter)
                            {
                                var cPar = db.GtEspasc.Where(x => x.ServiceClassId == obj.ServiceClassId && x.ParameterId == cp.ParameterID).FirstOrDefault();
                                if (cPar != null)
                                {
                                    cPar.ParmAction = cp.ParmAction;
                                    cPar.ParmDesc = cp.ParmDesc;
                                    cPar.ParmPerc = cp.ParmPerc;
                                    cPar.ParmValue = cp.ParmValue;
                                    cPar.ActiveStatus = obj.ActiveStatus;
                                    cPar.ModifiedBy = obj.UserID;
                                    cPar.ModifiedOn = System.DateTime.Now;
                                    cPar.ModifiedTerminal = obj.TerminalID;
                                }
                                else
                                {
                                    var cParameter = new GtEspasc
                                    {
                                        ServiceClassId = obj.ServiceClassId,
                                        ParameterId = cp.ParameterID,
                                        ParmPerc = cp.ParmPerc,
                                        ParmAction = cp.ParmAction,
                                        ParmDesc = cp.ParmDesc,
                                        ParmValue = cp.ParmValue,
                                        ActiveStatus = cp.ActiveStatus,
                                        FormId = obj.FormId,
                                        CreatedBy = obj.UserID,
                                        CreatedOn = System.DateTime.Now,
                                        CreatedTerminal = obj.TerminalID,

                                    };
                                    db.GtEspasc.Add(cParameter);
                                }

                            }
                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<DO_ReturnParameter> UpdateServiceClassIndex(int serviceGroupId, int serviceClassId, bool isMoveUp, bool isMoveDown)
        {
            using (var db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var scCurrent = db.GtEssrcl.Where(w => w.ServiceGroupId == serviceGroupId && w.ServiceClassId == serviceClassId).FirstOrDefault();
                        int switchIndex = 0;
                        if (scCurrent.ParentId == scCurrent.ServiceClassId)
                        {
                            if (isMoveUp)
                            {
                                var isTop = db.GtEssrcl.Where(w => w.PrintSequence < scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId == w.ParentId).Count();
                                if (isTop > 0)
                                {
                                    var scTarget = db.GtEssrcl.Where(w => w.PrintSequence < scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId == w.ParentId).OrderByDescending(o => o.PrintSequence).FirstOrDefault();
                                    switchIndex = scCurrent.PrintSequence;
                                    scCurrent.PrintSequence = scTarget.PrintSequence;
                                    scTarget.PrintSequence = switchIndex;
                                }
                                else
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = scCurrent.ServiceClassDesc + " is already on top" };
                                }
                            }
                            else if (isMoveDown)
                            {
                                var isBottom = db.GtEssrcl.Where(w => w.PrintSequence > scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId == w.ParentId).Count();
                                if (isBottom > 0)
                                {
                                    var scTarget = db.GtEssrcl.Where(w => w.PrintSequence > scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId == w.ParentId).OrderBy(o => o.PrintSequence).FirstOrDefault();
                                    switchIndex = scCurrent.PrintSequence;
                                    scCurrent.PrintSequence = scTarget.PrintSequence;
                                    scTarget.PrintSequence = switchIndex;
                                }
                                else
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = scCurrent.ServiceClassDesc + " is already on bottom" };
                                }
                            }
                        }
                        else
                        {
                            if (isMoveUp)
                            {
                                var isTop = db.GtEssrcl.Where(w => w.PrintSequence < scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId != w.ParentId && w.ParentId == scCurrent.ParentId).Count();
                                if (isTop > 0)
                                {
                                    var scTarget = db.GtEssrcl.Where(w => w.PrintSequence < scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId != w.ParentId && w.ParentId == scCurrent.ParentId).OrderByDescending(o => o.PrintSequence).FirstOrDefault();
                                    switchIndex = scCurrent.PrintSequence;
                                    scCurrent.PrintSequence = scTarget.PrintSequence;
                                    scTarget.PrintSequence = switchIndex;
                                }
                                else
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = scCurrent.ServiceClassDesc + " is already on top" };
                                }
                            }
                            else if (isMoveDown)
                            {
                                var isBottom = db.GtEssrcl.Where(w => w.PrintSequence > scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId != w.ParentId && w.ParentId == scCurrent.ParentId).Count();
                                if (isBottom > 0)
                                {
                                    var scTarget = db.GtEssrcl.Where(w => w.PrintSequence > scCurrent.PrintSequence && w.ServiceGroupId == serviceGroupId && w.ServiceClassId != w.ParentId && w.ParentId == scCurrent.ParentId).OrderBy(o => o.PrintSequence).FirstOrDefault();
                                    switchIndex = scCurrent.PrintSequence;
                                    scCurrent.PrintSequence = scTarget.PrintSequence;
                                    scTarget.PrintSequence = switchIndex;
                                }
                                else
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = scCurrent.ServiceClassDesc + " is already on bottom" };
                                }
                            }
                        }



                        await db.SaveChangesAsync();
                        dbContext.Commit();

                        return new DO_ReturnParameter() { Status = true };
                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        public async Task<DO_ReturnParameter> DeleteServiceClass(int serviceClassId)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {

                                var cLinkExist = db.GtEssrcl.Where(w => w.ParentId == serviceClassId && w.ParentId != w.ServiceClassId ).Count();
                                if (cLinkExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "This Service Class is linked to another Service Class(es). Please unlink it before deactivating" };
                                }
                                var sLinkExist = db.GtEssrms.Where(w => w.ServiceClassId == serviceClassId ).Count();
                                if (sLinkExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "This Service Class is linked to a Service(s). Please unlink it before deactivating" };
                                }

                        var ServiceClass = db.GtEssrcl.Where(w => w.ServiceClassId == serviceClassId).FirstOrDefault();
                        if (ServiceClass != null)
                        {
                            var classParam = db.GtEspasc.Where(w => w.ServiceClassId == serviceClassId).ToList();
                            foreach (GtEspasc p in classParam)
                            {
                                db.GtEspasc.Remove(p);
                            } 
                            db.GtEssrcl.Remove(ServiceClass);
                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region ServiceCode
        public async Task<List<DO_ServiceCode>> GetServiceCodes()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrms
                                 .Select(x => new DO_ServiceCode
                                 {
                                     ServiceId = x.ServiceId,
                                     // ServiceTypeId=x.ServiceTypeId,
                                     // ServiceGroupId = x.ServiceGroupId,
                                     ServiceClassId = x.ServiceClassId,
                                     ServiceDesc = x.ServiceDesc,
                                     ServiceShortDesc = x.ServiceShortDesc,
                                     ActiveStatus = x.ActiveStatus
                                 }
                        ).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ServiceCode> GetServiceCodeByID(int ServiceID)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrms
                        .Where(i => i.ServiceId == ServiceID)
                                 .Select(x => new DO_ServiceCode
                                 {
                                     ServiceId = x.ServiceId,
                                     ServiceDesc = x.ServiceDesc,
                                     ServiceShortDesc = x.ServiceShortDesc,
                                     InternalServiceCode = x.InternalServiceCode,
                                     Gender = x.Gender,
                                     IsServiceBillable = x.IsServiceBillable,
                                     ActiveStatus = x.ActiveStatus,
                                     l_ServiceParameter = x.GtEspasm.Select(p => new DO_eSyaParameter
                                     {
                                         ParameterID = p.ParameterId,
                                         ParmAction = p.ParmAction,
                                         ParmPerc = p.ParmPerc,
                                         ParmValue = p.ParmValue,
                                     }).ToList()
                                 }
                        ).FirstOrDefaultAsync();

                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateServiceCode(DO_ServiceCode obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (obj.ServiceId == 0)
                        {
                            var RecordExist = db.GtEssrms.Where(w => w.ServiceDesc == obj.ServiceDesc || (w.ServiceShortDesc == obj.ServiceShortDesc && w.ServiceShortDesc != null && w.ServiceShortDesc != "")).Count();
                            if (RecordExist > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, Message = "Service Already Exists" };
                            }
                            else
                            {
                                var internalcode = obj.InternalServiceCode;
                                var newServiceId = db.GtEssrms.Select(a => (int)a.ServiceId).DefaultIfEmpty(0).Max() + 1;
                                // If internal service code pattern is defined
                                var pattern = db.GtEssrcg.Where(w => w.ServiceClassId == obj.ServiceClassId && w.ActiveStatus).FirstOrDefault();
                                if (pattern != null)
                                {
                                    var internalserId = db.GtEssrms.Where(w => w.ServiceClassId == obj.ServiceClassId).Count() + 1;
                                    string digits = (Math.Pow(10, pattern.IntSccode)).ToString();
                                    digits = digits + internalserId.ToString();
                                    digits = digits.Substring(digits.Length - pattern.IntSccode, pattern.IntSccode);
                                    internalcode = pattern.IntScpattern + digits;
                                }


                                var servicecode = new GtEssrms
                                {
                                    ServiceId = newServiceId,
                                    // ServiceTypeId = obj.ServiceTypeId,
                                    // ServiceGroupId = obj.ServiceGroupId,
                                    ServiceClassId = obj.ServiceClassId,
                                    ServiceDesc = obj.ServiceDesc,
                                    ServiceShortDesc = obj.ServiceShortDesc,
                                    Gender = obj.Gender,
                                    InternalServiceCode = internalcode,
                                    IsServiceBillable = obj.IsServiceBillable,
                                    ActiveStatus = obj.ActiveStatus,
                                    FormId = obj.FormId,
                                    CreatedBy = obj.UserID,
                                    CreatedOn = obj.CreatedOn,
                                    CreatedTerminal = obj.TerminalID
                                };
                                db.GtEssrms.Add(servicecode);
                                foreach (DO_eSyaParameter sp in obj.l_ServiceParameter)
                                {
                                    var sParameter = new GtEspasm
                                    {
                                        ServiceId = newServiceId,
                                        ParameterId = sp.ParameterID,
                                        ParmPerc = sp.ParmPerc,
                                        ParmAction = sp.ParmAction,
                                        ParmValue = sp.ParmValue,
                                        ActiveStatus = sp.ActiveStatus,
                                        FormId = obj.FormId,
                                        CreatedBy = obj.UserID,
                                        CreatedOn = System.DateTime.Now,
                                        CreatedTerminal = obj.TerminalID,
                                    };
                                    db.GtEspasm.Add(sParameter);

                                }
                            }
                        }
                        else
                        {
                            var updatedServiceCode = db.GtEssrms.Where(w => w.ServiceId == obj.ServiceId).FirstOrDefault();
                            if (updatedServiceCode.ServiceDesc != obj.ServiceDesc)
                            {
                                var RecordExist = db.GtEssrms.Where(w => w.ServiceDesc == obj.ServiceDesc).Count();
                                if (RecordExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "Service Description Already Exists" };
                                }
                            }
                            if (updatedServiceCode.ServiceShortDesc != obj.ServiceShortDesc)
                            {
                                var RecordExist = db.GtEssrms.Where(w => w.ServiceShortDesc == obj.ServiceShortDesc).Count();
                                if (RecordExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "Service Short Description Already Exists" };
                                }
                            }
                            if (updatedServiceCode.InternalServiceCode != obj.InternalServiceCode)
                            {
                                var RecordExist = db.GtEssrms.Where(w => w.InternalServiceCode == obj.InternalServiceCode).Count();
                                if (RecordExist > 0)
                                {
                                    return new DO_ReturnParameter() { Status = false, Message = "Internal Service Code Already Exists" };
                                }
                            }
                            updatedServiceCode.ServiceDesc = obj.ServiceDesc;
                            updatedServiceCode.ServiceShortDesc = obj.ServiceShortDesc;
                            updatedServiceCode.Gender = obj.Gender;
                            updatedServiceCode.IsServiceBillable = obj.IsServiceBillable;
                            updatedServiceCode.InternalServiceCode = obj.InternalServiceCode;
                            updatedServiceCode.ActiveStatus = obj.ActiveStatus;
                            updatedServiceCode.ModifiedBy = obj.UserID;
                            updatedServiceCode.ModifiedOn = obj.CreatedOn;
                            updatedServiceCode.ModifiedTerminal = obj.TerminalID;

                            foreach (DO_eSyaParameter sp in obj.l_ServiceParameter)
                            {
                                var sPar = db.GtEspasm.Where(x => x.ServiceId == obj.ServiceId && x.ParameterId == sp.ParameterID).FirstOrDefault();
                                if (sPar != null)
                                {
                                    sPar.ParmAction = sp.ParmAction;
                                    sPar.ParmPerc = sp.ParmPerc;
                                    sPar.ParmValue = sp.ParmValue;
                                    sPar.ActiveStatus = obj.ActiveStatus;
                                    sPar.ModifiedBy = obj.UserID;
                                    sPar.ModifiedOn = System.DateTime.Now;
                                    sPar.ModifiedTerminal = obj.TerminalID;
                                }
                                else
                                {
                                    var sParameter = new GtEspasm
                                    {
                                        ServiceId = obj.ServiceId,
                                        ParameterId = sp.ParameterID,
                                        ParmPerc = sp.ParmPerc,
                                        ParmAction = sp.ParmAction,
                                        ParmValue = sp.ParmValue,
                                        ActiveStatus = sp.ActiveStatus,
                                        FormId = obj.FormId,
                                        CreatedBy = obj.UserID,
                                        CreatedOn = System.DateTime.Now,
                                        CreatedTerminal = obj.TerminalID,

                                    };
                                    db.GtEspasm.Add(sParameter);
                                }

                            }
                        }

                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region ServiceCodePattern
        public async Task<List<DO_ServiceCodePattern>> GetServiceCodePatterns()
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrcg
                        .Join(db.GtEssrcl,
                            a => a.ServiceClassId,
                            b => b.ServiceClassId,
                            (a, b) => new { a, b })
                                 .Select(x => new DO_ServiceCodePattern
                                 {
                                     ServiceClassId = x.a.ServiceClassId,
                                     ServiceClassDesc = x.b.ServiceClassDesc,
                                     IntScpattern = x.a.IntScpattern,
                                     IntSccode = x.a.IntSccode,
                                     ActiveStatus = x.a.ActiveStatus
                                 }
                        ).OrderBy(o => o.ServiceClassDesc).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddServiceCodePattern(DO_ServiceCodePattern obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        var RecordExist = db.GtEssrcg.Where(w => w.ServiceClassId == obj.ServiceClassId).Count();
                        if (RecordExist > 0)
                        {
                            return new DO_ReturnParameter() { Status = false, Message = "Pattern already exists for this class" };
                        }
                        else
                        {
                            var servicecodepattern = new GtEssrcg
                            {
                                ServiceClassId = obj.ServiceClassId,
                                IntScpattern = obj.IntScpattern,
                                IntSccode = obj.IntSccode,
                                ActiveStatus = obj.ActiveStatus,
                                FormId = obj.FormId,
                                CreatedBy = obj.UserID,
                                CreatedOn = obj.CreatedOn,
                                CreatedTerminal = obj.TerminalID
                            };
                            db.GtEssrcg.Add(servicecodepattern);

                        }



                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region ServiceBusinessLink
        public async Task<List<DO_ServiceBusinessLink>> GetBusinessLocationServices(int businessKey)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrbl.Where(w => w.BusinessKey == businessKey)
                        .Join(db.GtEssrms,
                        b => b.ServiceId,
                        s => s.ServiceId,
                        (b, s) => new { b, s }
                        )
                        .Join(db.GtEssrcl,
                        bs => bs.s.ServiceClassId,
                        c => c.ServiceClassId,
                        (bs, c) => new { bs, c })
                                 .Select(x => new DO_ServiceBusinessLink
                                 {
                                     ServiceId = x.bs.b.ServiceId,
                                     ServiceDesc = x.bs.s.ServiceDesc,
                                     ServiceClassDesc = x.c.ServiceClassDesc,
                                     InternalServiceCode = x.bs.b.InternalServiceCode,
                                     ServiceCost = x.bs.b.ServiceCost,
                                     NightLinePercentage = x.bs.b.NightLinePercentage,
                                     HolidayPercentage = x.bs.b.HolidayPercentage,
                                     ActiveStatus = x.bs.b.ActiveStatus
                                 }
                        ).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> AddOrUpdateBusinessLocationServices(List<DO_ServiceBusinessLink> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_bl in obj)
                        {
                            var ServiceExist = db.GtEssrbl.Where(w => w.ServiceId == ser_bl.ServiceId && w.BusinessKey == ser_bl.BusinessKey).FirstOrDefault();
                            if (ServiceExist != null)
                            {
                                ServiceExist.InternalServiceCode = ser_bl.InternalServiceCode;
                                ServiceExist.ServiceCost = ser_bl.ServiceCost;
                                ServiceExist.NightLinePercentage = ser_bl.NightLinePercentage;
                                ServiceExist.HolidayPercentage = ser_bl.HolidayPercentage;
                                ServiceExist.ActiveStatus = ser_bl.ActiveStatus;
                                ServiceExist.ModifiedBy = ser_bl.UserID;
                                ServiceExist.ModifiedOn = ser_bl.CreatedOn;
                                ServiceExist.ModifiedTerminal = ser_bl.TerminalID;
                            }
                            else
                            {
                                var businesslocationservice = new GtEssrbl
                                {
                                    BusinessKey = ser_bl.BusinessKey,
                                    ServiceId = ser_bl.ServiceId,
                                    InternalServiceCode = ser_bl.InternalServiceCode,
                                    ServiceCost = ser_bl.ServiceCost,
                                    NightLinePercentage = ser_bl.NightLinePercentage,
                                    HolidayPercentage = ser_bl.HolidayPercentage,
                                    ActiveStatus = ser_bl.ActiveStatus,
                                    FormId = ser_bl.FormId,
                                    CreatedBy = ser_bl.UserID,
                                    CreatedOn = ser_bl.CreatedOn,
                                    CreatedTerminal = ser_bl.TerminalID
                                };
                                db.GtEssrbl.Add(businesslocationservice);
                            }
                        }
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }
            }
        }
        public async Task<List<DO_ServiceBusinessLink>> GetServiceBusinessLocations(int ServiceId)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var bk = db.GtEcbsln.Where(w => w.ActiveStatus)
                        .GroupJoin(db.GtEssrbl.Where(w => w.ServiceId == ServiceId),
                        b => b.BusinessKey,
                        l => l.BusinessKey,
                        (b, l) => new { b, l = l.FirstOrDefault() })
                        .Select(r => new DO_ServiceBusinessLink
                        {
                            ServiceId = ServiceId,
                            BusinessKey = r.b.BusinessKey,
                            LocationDescription = r.b.LocationDescription,
                            ActiveStatus = r.l != null ? r.l.ActiveStatus : false
                        }).ToListAsync();

                    return await bk;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DO_ReturnParameter> UpdateServiceBusinessLocations(List<DO_ServiceBusinessLink> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_bl in obj)
                        {
                            var ServiceExist = db.GtEssrbl.Where(w => w.ServiceId == ser_bl.ServiceId && w.BusinessKey == ser_bl.BusinessKey).FirstOrDefault();
                            if (ServiceExist != null)
                            {
                                if (ServiceExist.ActiveStatus != ser_bl.ActiveStatus)
                                {
                                    ServiceExist.ActiveStatus = ser_bl.ActiveStatus;
                                    ServiceExist.ModifiedBy = ser_bl.UserID;
                                    ServiceExist.ModifiedOn = ser_bl.CreatedOn;
                                    ServiceExist.ModifiedTerminal = ser_bl.TerminalID;
                                }

                            }
                            else
                            {
                                if (ser_bl.ActiveStatus)
                                {
                                    var businesslocationservice = new GtEssrbl
                                    {
                                        BusinessKey = ser_bl.BusinessKey,
                                        ServiceId = ser_bl.ServiceId,
                                        ActiveStatus = ser_bl.ActiveStatus,
                                        FormId = ser_bl.FormId,
                                        CreatedBy = ser_bl.UserID,
                                        CreatedOn = ser_bl.CreatedOn,
                                        CreatedTerminal = ser_bl.TerminalID
                                    };
                                    db.GtEssrbl.Add(businesslocationservice);
                                }

                            }
                        }
                        await db.SaveChangesAsync();
                        dbContext.Commit();
                        return new DO_ReturnParameter { Status = true };


                    }
                    catch (Exception ex)
                    {
                        dbContext.Rollback();
                        return new DO_ReturnParameter() { Status = false, Message = ex.Message };
                    }
                }
            }
        }
        public async Task<List<DO_ServiceCode>> GetServiceBusinessLink(int businessKey)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrms
                        .GroupJoin(db.GtEssrbl.Where(w => w.BusinessKey == businessKey),
                        s => s.ServiceId,
                        b => b.ServiceId,
                        (s, b) => new { s, b = b.FirstOrDefault() })
                                 .Select(x => new DO_ServiceCode
                                 {
                                     ServiceId = x.s.ServiceId,
                                     ServiceClassId = x.s.ServiceClassId,
                                     ServiceDesc = x.s.ServiceDesc,
                                     ActiveStatus = x.s.ActiveStatus,
                                     BusinessLinkStatus = x.b != null ? x.b.ActiveStatus : false
                                 }
                        ).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ServiceMaster
        public async Task<List<DO_ServiceCode>> GetServiceMaster(int servicetype, int servicegroup, int serviceclass)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrms
                        .Join(db.GtEssrcl,
                        s => s.ServiceClassId,
                        c => c.ServiceClassId,
                        (s, c) => new { s, c })
                        .Join(db.GtEssrgr,
                        sc => sc.c.ServiceGroupId,
                        g => g.ServiceGroupId,
                        (sc, g) => new { sc, g })
                        .Join(db.GtEssrty,
                        scg => scg.g.ServiceTypeId,
                        t => t.ServiceTypeId,
                        (scg, t) => new { scg, t })
                        .Where(w => (servicetype == 0 ? true : w.t.ServiceTypeId == servicetype) && (servicegroup == 0 ? true : w.scg.g.ServiceGroupId == servicegroup) && (serviceclass == 0 ? true : w.scg.sc.c.ServiceClassId == serviceclass))
                                 .Select(x => new DO_ServiceCode
                                 {
                                     ServiceId = x.scg.sc.s.ServiceId,
                                     ServiceTypeId = x.t.ServiceTypeId,
                                     ServiceGroupId = x.scg.g.ServiceGroupId,
                                     ServiceClassId = x.scg.sc.c.ServiceClassId,
                                     ServiceDesc = x.scg.sc.s.ServiceDesc,
                                     ServiceTypeDesc = x.t.ServiceTypeDesc,
                                     ServiceGroupDesc = x.scg.g.ServiceGroupDesc,
                                     ServiceClassDesc = x.scg.sc.c.ServiceClassDesc,
                                     ActiveStatus = x.scg.sc.s.ActiveStatus

                                 }
                        ).OrderBy(o => o.ServiceTypeDesc).ThenBy(o => o.ServiceGroupDesc).ThenBy(o => o.ServiceClassDesc).ThenBy(o => o.ServiceDesc).ToListAsync();
                    return await result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
