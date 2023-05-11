using HCP.Services.DL.Entities;
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
    public class ServiceRatesRepository : IServiceRatesRepository
    {
       
        #region ServiceBaseRate
        public async Task<List<DO_ServiceBaseRate>> GetServiceBaseRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var defaultDate = DateTime.Now.Date;
                    var result = db.GtEssrms
                        .Join(db.GtEssrcl,
                        s=> s.ServiceClassId,
                        c=> c.ServiceClassId,
                        (s,c) => new {s,c})
                        .Join(db.GtEssrgr,
                        sc => sc.c.ServiceGroupId,
                        g => g.ServiceGroupId,
                        (sc, g) => new { sc, g })
                        .Join(db.GtEssrty,
                        scg => scg.g.ServiceTypeId,
                        t => t.ServiceTypeId,
                        (scg, t) => new { scg, t })
                        .GroupJoin(db.GtEssrbr.Where(w => w.BusinessKey == businessKey && w.RateType == ratetype && w.CurrencyCode==currencycode).OrderByDescending(o=> o.ActiveStatus),
                        scgt => scgt.scg.sc.s.ServiceId,
                        r => r.ServiceId,
                        (scgt, r) => new { scgt, r = r.FirstOrDefault() })
                                 .Select(x => new DO_ServiceBaseRate
                                 {
                                     ServiceId = x.scgt.scg.sc.s.ServiceId,
                                     ServiceDesc = x.scgt.scg.sc.s.ServiceDesc,
                                     ServiceTypeDesc = x.scgt.t.ServiceTypeDesc,
                                     ServiceGroupDesc = x.scgt.scg.g.ServiceGroupDesc,
                                     ServiceClassDesc =x.scgt.scg.sc.c.ServiceClassDesc,                                    
                                     EffectiveDate = x.r != null ? x.r.EffectiveDate : defaultDate,
                                     ServiceRule = x.r != null ? x.r.ServiceRule : "F",
                                     OpbaseRate = x.r != null ? x.r.OpbaseRate : 0,
                                     IpbaseRate = x.r != null ? x.r.IpbaseRate : 0,
                                     IsIprateWardwise = x.r != null ? x.r.IsIprateWardwise : true,
                                     ActiveStatus = x.r != null ? x.r.ActiveStatus : true,
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
        public async Task<DO_ReturnParameter> AddOrUpdateServiceBaseRate(List<DO_ServiceBaseRate> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_br in obj)
                        {
                            var ServiceExist = db.GtEssrbr.Where(w => w.ServiceId == ser_br.ServiceId && w.BusinessKey == ser_br.BusinessKey && w.RateType == ser_br.RateType && w.CurrencyCode==ser_br.CurrencyCode && w.EffectiveTill==null).FirstOrDefault();
                            if (ServiceExist != null)
                            {
                                if (ser_br.EffectiveDate != ServiceExist.EffectiveDate)
                                {
                                    if (ser_br.EffectiveDate < ServiceExist.EffectiveDate)
                                    {
                                        return new DO_ReturnParameter() { Status = false, Message = "New effective date can't be less than the current effective date" };
                                    }
                                    ServiceExist.EffectiveTill = ser_br.EffectiveDate.AddDays(-1);
                                    ServiceExist.ModifiedBy = ser_br.UserID;
                                    ServiceExist.ModifiedOn = ser_br.CreatedOn;
                                    ServiceExist.ModifiedTerminal = ser_br.TerminalID;
                                    ServiceExist.ActiveStatus = false;

                                    var servicebaserate = new GtEssrbr
                                    {
                                        BusinessKey = ser_br.BusinessKey,
                                        ServiceId = ser_br.ServiceId,
                                        RateType = ser_br.RateType,
                                        CurrencyCode = ser_br.CurrencyCode,
                                        EffectiveDate = ser_br.EffectiveDate,

                                        ServiceRule = ser_br.ServiceRule,                                       
                                        OpbaseRate = ser_br.OpbaseRate,
                                        IpbaseRate = ser_br.IpbaseRate,
                                        IsIprateWardwise=ser_br.IsIprateWardwise,

                                        ActiveStatus = ser_br.ActiveStatus,
                                        FormId = ser_br.FormId,
                                        CreatedBy = ser_br.UserID,
                                        CreatedOn = ser_br.CreatedOn,
                                        CreatedTerminal = ser_br.TerminalID
                                    };
                                    db.GtEssrbr.Add(servicebaserate);


                                }
                                else
                                {
                                    ServiceExist.ServiceRule = ser_br.ServiceRule;
                                    ServiceExist.OpbaseRate = ser_br.OpbaseRate;
                                    ServiceExist.IpbaseRate = ser_br.IpbaseRate;
                                    ServiceExist.IsIprateWardwise = ser_br.IsIprateWardwise;
                                    ServiceExist.ActiveStatus = ser_br.ActiveStatus;

                                    ServiceExist.ModifiedBy = ser_br.UserID;
                                    ServiceExist.ModifiedOn = ser_br.CreatedOn;
                                    ServiceExist.ModifiedTerminal = ser_br.TerminalID;
                                }

                            }
                            else
                            {
                                if (ser_br.OpbaseRate != 0 || ser_br.IpbaseRate != 0)
                                {
                                    var servicebaserate = new GtEssrbr
                                    {
                                        BusinessKey = ser_br.BusinessKey,
                                        ServiceId = ser_br.ServiceId,
                                        RateType = ser_br.RateType,
                                        CurrencyCode = ser_br.CurrencyCode,
                                        EffectiveDate = ser_br.EffectiveDate,

                                        ServiceRule = ser_br.ServiceRule,
                                        OpbaseRate = ser_br.OpbaseRate,
                                        IpbaseRate = ser_br.IpbaseRate,
                                        IsIprateWardwise = ser_br.IsIprateWardwise,

                                        ActiveStatus = ser_br.ActiveStatus,
                                        FormId = ser_br.FormId,
                                        CreatedBy = ser_br.UserID,
                                        CreatedOn = ser_br.CreatedOn,
                                        CreatedTerminal = ser_br.TerminalID
                                    };
                                    db.GtEssrbr.Add(servicebaserate);
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
        public async Task<DO_ReturnParameter> AddServiceBaseRate(List<DO_ServiceBaseRate> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_br in obj)
                        {
                                if (ser_br.OpbaseRate != 0 || ser_br.IpbaseRate != 0)
                                {
                                var ServiceExist = db.GtEssrbr.Where(w => w.ServiceId == ser_br.ServiceId && w.BusinessKey == ser_br.BusinessKey && w.RateType == ser_br.RateType && w.CurrencyCode == ser_br.CurrencyCode).FirstOrDefault();
                                if (ServiceExist != null)
                                {
                                    var ser = db.GtEssrms.Where(w => w.ServiceId == ser_br.ServiceId).Select(r => r.ServiceDesc).ToArray();
                                    return new DO_ReturnParameter() { Status = false, Message = "Rate Already Exist for the service '" + ser[0].ToString() + "'" };
                                }
                                else
                                {
                                    var servicebaserate = new GtEssrbr
                                    {
                                        BusinessKey = ser_br.BusinessKey,
                                        ServiceId = ser_br.ServiceId,
                                        RateType = ser_br.RateType,
                                        CurrencyCode = ser_br.CurrencyCode,
                                        EffectiveDate = ser_br.EffectiveDate,

                                        ServiceRule = ser_br.ServiceRule,
                                        OpbaseRate = ser_br.OpbaseRate,
                                        IpbaseRate = ser_br.IpbaseRate,
                                        IsIprateWardwise = ser_br.IsIprateWardwise,

                                        ActiveStatus = ser_br.ActiveStatus,
                                        FormId = ser_br.FormId,
                                        CreatedBy = ser_br.UserID,
                                        CreatedOn = ser_br.CreatedOn,
                                        CreatedTerminal = ser_br.TerminalID
                                    };
                                    db.GtEssrbr.Add(servicebaserate);
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
        #endregion

        #region ServiceRatePlan
        public async Task<List<DO_ServiceRatePlan>> GetServiceRatePlansByBKey(int businessKey)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrrp
                        .Where(w=> w.BusinessKey==businessKey)
                                 .Select(x => new DO_ServiceRatePlan
                                 {
                                     RateType=x.RateType,
                                     BaseRateType=x.BaseRateType,
                                     RateVariationBy=x.RateVariationBy,
                                     RateVariationAmount=x.RateVariationAmount,
                                     RateVariationPercentage=x.RateVariationPercentage,
                                     RoundOffBy=x.RoundOffBy,
                                     RateVariationFormula=x.RateVariationFormula,
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
        public async Task<DO_ServiceRatePlan> GetServiceRatePlanByBKeyRType(int businessKey,int ratetype)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var result = db.GtEssrrp
                        .Where(w => w.BusinessKey == businessKey && w.RateType==ratetype)
                                 .Select(x => new DO_ServiceRatePlan
                                 {
                                     RateType = x.RateType,
                                     BaseRateType = x.BaseRateType,
                                     RateVariationBy = x.RateVariationBy,
                                     RateVariationAmount = x.RateVariationAmount,
                                     RateVariationPercentage = x.RateVariationPercentage,
                                     RoundOffBy = x.RoundOffBy,
                                     RateVariationFormula = x.RateVariationFormula,
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
        public async Task<DO_ReturnParameter> AddOrUpdateServiceRatePlan(DO_ServiceRatePlan obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (obj.AddFlag)
                        {
                            var RecordExist = db.GtEssrrp.Where(w => w.BusinessKey == obj.BusinessKey && w.RateType == obj.RateType).Count();
                            if (RecordExist > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, Message = "Service Rate Plan Already Exists" };
                            }
                            var RecordNotValid = db.GtEssrrp.Where(w => w.BusinessKey == obj.BusinessKey && w.RateType == obj.BaseRateType).Count();
                            if (RecordNotValid > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, Message = "This entry is not allowed" };
                            }
                            else
                            {

                                var servicerateplan = new GtEssrrp
                                {
                                    BusinessKey = obj.BusinessKey ,
                                    RateType = obj.RateType,
                                    BaseRateType=obj.BaseRateType,
                                    RateVariationBy=obj.RateVariationBy,
                                    RateVariationAmount=obj.RateVariationAmount,
                                    RateVariationPercentage=obj.RateVariationPercentage,
                                    RoundOffBy=obj.RoundOffBy,
                                    RateVariationFormula=obj.RateVariationFormula,
                                    ActiveStatus = obj.ActiveStatus,
                                   // FormId = obj.FormID,
                                    CreatedBy = obj.UserID,
                                    CreatedOn = obj.CreatedOn,
                                    CreatedTerminal = obj.TerminalID
                                };
                                db.GtEssrrp.Add(servicerateplan);

                            }
                        }
                        else
                        {
                            var RecordNotValid = db.GtEssrrp.Where(w => w.BusinessKey == obj.BusinessKey && w.RateType == obj.BaseRateType).Count();
                            if (RecordNotValid > 0)
                            {
                                return new DO_ReturnParameter() { Status = false, Message = "This entry is not allowed" };
                            }
                            var updatedServiceRatePlan = db.GtEssrrp.Where(w => w.BusinessKey == obj.BusinessKey && w.RateType==obj.RateType).FirstOrDefault();
                            updatedServiceRatePlan.BaseRateType = obj.BaseRateType;
                            updatedServiceRatePlan.RateVariationBy = obj.RateVariationBy;
                            updatedServiceRatePlan.RateVariationAmount = obj.RateVariationAmount;
                            updatedServiceRatePlan.RateVariationPercentage = obj.RateVariationPercentage;
                            updatedServiceRatePlan.RoundOffBy = obj.RoundOffBy;
                            updatedServiceRatePlan.RateVariationFormula = obj.RateVariationFormula;
                            updatedServiceRatePlan.ActiveStatus = obj.ActiveStatus;
                            updatedServiceRatePlan.ModifiedBy = obj.UserID;
                            updatedServiceRatePlan.ModifiedOn = obj.CreatedOn;
                            updatedServiceRatePlan.ModifiedTerminal = obj.TerminalID;



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

        #region SpecialtyRate
        public async Task<List<DO_ServiceSpecialtyRate>> GetServiceSpecialtyRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode,int specialtyId)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var defaultDate = DateTime.Now.Date;
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
                        .GroupJoin(db.GtEssrsp.Where(w => w.BusinessKey == businessKey && w.RateType == ratetype && w.CurrencyCode == currencycode && w.SpecialtyId==specialtyId).OrderByDescending(o => o.ActiveStatus),
                        scgt => scgt.scg.sc.s.ServiceId,
                        r => r.ServiceId,
                        (scgt, r) => new { scgt, r = r.FirstOrDefault() })
                                 .Select(x => new DO_ServiceSpecialtyRate
                                 {
                                     SpecialtyId=specialtyId,
                                     ServiceId = x.scgt.scg.sc.s.ServiceId,
                                     ServiceDesc = x.scgt.scg.sc.s.ServiceDesc,
                                     ServiceTypeDesc = x.scgt.t.ServiceTypeDesc,
                                     ServiceGroupDesc = x.scgt.scg.g.ServiceGroupDesc,
                                     ServiceClassDesc = x.scgt.scg.sc.c.ServiceClassDesc,
                                     EffectiveDate = x.r != null ? x.r.EffectiveDate : defaultDate,
                                     ServiceRule = x.r != null ? x.r.ServiceRule : "F",
                                     ServiceRate = x.r != null ? x.r.ServiceRate : 0,
                                     ActiveStatus = x.r != null ? x.r.ActiveStatus : true,
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
        public async Task<DO_ReturnParameter> AddOrUpdateServiceSpecialtyRate(List<DO_ServiceSpecialtyRate> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_spr in obj)
                        {
                            var ServiceExist = db.GtEssrsp.Where(w => w.ServiceId == ser_spr.ServiceId && w.BusinessKey == ser_spr.BusinessKey && w.RateType == ser_spr.RateType && w.CurrencyCode == ser_spr.CurrencyCode && w.SpecialtyId==ser_spr.SpecialtyId && w.EffectiveTill == null).FirstOrDefault();
                            if (ServiceExist != null)
                            {
                                if (ser_spr.EffectiveDate != ServiceExist.EffectiveDate)
                                {
                                    if (ser_spr.EffectiveDate < ServiceExist.EffectiveDate)
                                    {
                                        return new DO_ReturnParameter() { Status = false, Message = "New effective date can't be less than the current effective date" };
                                    }
                                    ServiceExist.EffectiveTill = ser_spr.EffectiveDate.AddDays(-1);
                                    ServiceExist.ModifiedBy = ser_spr.UserID;
                                    ServiceExist.ModifiedOn = ser_spr.CreatedOn;
                                    ServiceExist.ModifiedTerminal = ser_spr.TerminalID;
                                    ServiceExist.ActiveStatus = false;

                                    var servicesprate = new GtEssrsp
                                    {
                                        BusinessKey = ser_spr.BusinessKey,
                                        ServiceId = ser_spr.ServiceId,
                                        SpecialtyId=ser_spr.SpecialtyId,
                                        RateType = ser_spr.RateType,
                                        CurrencyCode = ser_spr.CurrencyCode,
                                        EffectiveDate = ser_spr.EffectiveDate,

                                        ServiceRule = ser_spr.ServiceRule,
                                        ServiceRate = ser_spr.ServiceRate,

                                        ActiveStatus = ser_spr.ActiveStatus,
                                        FormId = ser_spr.FormId,
                                        CreatedBy = ser_spr.UserID,
                                        CreatedOn = ser_spr.CreatedOn,
                                        CreatedTerminal = ser_spr.TerminalID
                                    };
                                    db.GtEssrsp.Add(servicesprate);


                                }
                                else
                                {
                                    ServiceExist.ServiceRule = ser_spr.ServiceRule;
                                    ServiceExist.ServiceRate = ser_spr.ServiceRate;
                                    ServiceExist.ActiveStatus = ser_spr.ActiveStatus;

                                    ServiceExist.ModifiedBy = ser_spr.UserID;
                                    ServiceExist.ModifiedOn = ser_spr.CreatedOn;
                                    ServiceExist.ModifiedTerminal = ser_spr.TerminalID;
                                }

                            }
                            else
                            {
                                if (ser_spr.ServiceRate != 0 )
                                {
                                    var servicesprate = new GtEssrsp
                                    {
                                        BusinessKey = ser_spr.BusinessKey,
                                        ServiceId = ser_spr.ServiceId,
                                        SpecialtyId=ser_spr.SpecialtyId,
                                        RateType = ser_spr.RateType,
                                        CurrencyCode = ser_spr.CurrencyCode,
                                        EffectiveDate = ser_spr.EffectiveDate,

                                        ServiceRule = ser_spr.ServiceRule,
                                        ServiceRate = ser_spr.ServiceRate,

                                        ActiveStatus = ser_spr.ActiveStatus,
                                        FormId = ser_spr.FormId,
                                        CreatedBy = ser_spr.UserID,
                                        CreatedOn = ser_spr.CreatedOn,
                                        CreatedTerminal = ser_spr.TerminalID
                                    };
                                    db.GtEssrsp.Add(servicesprate);
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
        public async Task<DO_ReturnParameter> AddServiceSpecialtyRate(List<DO_ServiceSpecialtyRate> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_spr in obj)
                        {
                            if (ser_spr.ServiceRate != 0 )
                            {
                                var ServiceExist = db.GtEssrsp.Where(w => w.ServiceId == ser_spr.ServiceId && w.BusinessKey == ser_spr.BusinessKey && w.RateType == ser_spr.RateType && w.CurrencyCode == ser_spr.CurrencyCode && w.SpecialtyId==ser_spr.SpecialtyId).FirstOrDefault();
                                if (ServiceExist != null)
                                {
                                    var ser = db.GtEssrms.Where(w => w.ServiceId == ser_spr.ServiceId).Select(r => r.ServiceDesc).ToArray();
                                    return new DO_ReturnParameter() { Status = false, Message = "Rate Already Exist for the service '" + ser[0].ToString() + "'" };
                                }
                                else
                                {
                                    var servicesprate = new GtEssrsp
                                    {
                                        BusinessKey = ser_spr.BusinessKey,
                                        ServiceId = ser_spr.ServiceId,
                                        SpecialtyId=ser_spr.SpecialtyId,
                                        RateType = ser_spr.RateType,
                                        CurrencyCode = ser_spr.CurrencyCode,
                                        EffectiveDate = ser_spr.EffectiveDate,

                                        ServiceRule = ser_spr.ServiceRule,
                                        ServiceRate = ser_spr.ServiceRate,

                                        ActiveStatus = ser_spr.ActiveStatus,
                                        FormId = ser_spr.FormId,
                                        CreatedBy = ser_spr.UserID,
                                        CreatedOn = ser_spr.CreatedOn,
                                        CreatedTerminal = ser_spr.TerminalID
                                    };
                                    db.GtEssrsp.Add(servicesprate);
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
        #endregion

        #region DoctorRate
        public async Task<List<DO_ServiceDoctorRate>> GetServiceDoctorRateByBKeyRateTypeCurrCode(int businessKey, int ratetype, string currencycode, int doctorId)
        {
            try
            {
                using (eSyaEnterprise db = new eSyaEnterprise())
                {
                    var defaultDate = DateTime.Now.Date;
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
                        .GroupJoin(db.GtEssrdr.Where(w => w.BusinessKey == businessKey && w.RateType == ratetype && w.CurrencyCode == currencycode && w.DoctorId == doctorId).OrderByDescending(o => o.ActiveStatus),
                        scgt => scgt.scg.sc.s.ServiceId,
                        r => r.ServiceId,
                        (scgt, r) => new { scgt, r = r.FirstOrDefault() })
                                 .Select(x => new DO_ServiceDoctorRate
                                 {
                                     DoctorId=doctorId,
                                     ServiceId = x.scgt.scg.sc.s.ServiceId,
                                     ServiceDesc = x.scgt.scg.sc.s.ServiceDesc,
                                     ServiceTypeDesc = x.scgt.t.ServiceTypeDesc,
                                     ServiceGroupDesc = x.scgt.scg.g.ServiceGroupDesc,
                                     ServiceClassDesc = x.scgt.scg.sc.c.ServiceClassDesc,
                                     EffectiveDate = x.r != null ? x.r.EffectiveDate : defaultDate,
                                     ServiceRule = x.r != null ? x.r.ServiceRule : "F",
                                     ServiceRate = x.r != null ? x.r.ServiceRate : 0,
                                     ActiveStatus = x.r != null ? x.r.ActiveStatus : true,
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
        public async Task<DO_ReturnParameter> AddOrUpdateServiceDoctorRate(List<DO_ServiceDoctorRate> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_dr in obj)
                        {
                            var ServiceExist = db.GtEssrdr.Where(w => w.ServiceId == ser_dr.ServiceId && w.BusinessKey == ser_dr.BusinessKey && w.RateType == ser_dr.RateType && w.CurrencyCode == ser_dr.CurrencyCode && w.DoctorId == ser_dr.DoctorId && w.EffectiveTill == null).FirstOrDefault();
                            if (ServiceExist != null)
                            {
                                if (ser_dr.EffectiveDate != ServiceExist.EffectiveDate)
                                {
                                    if (ser_dr.EffectiveDate < ServiceExist.EffectiveDate)
                                    {
                                        return new DO_ReturnParameter() { Status = false, Message = "New effective date can't be less than the current effective date" };
                                    }
                                    ServiceExist.EffectiveTill = ser_dr.EffectiveDate.AddDays(-1);
                                    ServiceExist.ModifiedBy = ser_dr.UserID;
                                    ServiceExist.ModifiedOn = ser_dr.CreatedOn;
                                    ServiceExist.ModifiedTerminal = ser_dr.TerminalID;
                                    ServiceExist.ActiveStatus = false;

                                    var servicedrrate = new GtEssrdr
                                    {
                                        BusinessKey = ser_dr.BusinessKey,
                                        ServiceId = ser_dr.ServiceId,
                                        DoctorId = ser_dr.DoctorId,
                                        RateType = ser_dr.RateType,
                                        CurrencyCode = ser_dr.CurrencyCode,
                                        EffectiveDate = ser_dr.EffectiveDate,

                                        ServiceRule = ser_dr.ServiceRule,
                                        ServiceRate = ser_dr.ServiceRate,

                                        ActiveStatus = ser_dr.ActiveStatus,
                                        FormId = ser_dr.FormId,
                                        CreatedBy = ser_dr.UserID,
                                        CreatedOn = ser_dr.CreatedOn,
                                        CreatedTerminal = ser_dr.TerminalID
                                    };
                                    db.GtEssrdr.Add(servicedrrate);


                                }
                                else
                                {
                                    ServiceExist.ServiceRule = ser_dr.ServiceRule;
                                    ServiceExist.ServiceRate = ser_dr.ServiceRate;
                                    ServiceExist.ActiveStatus = ser_dr.ActiveStatus;

                                    ServiceExist.ModifiedBy = ser_dr.UserID;
                                    ServiceExist.ModifiedOn = ser_dr.CreatedOn;
                                    ServiceExist.ModifiedTerminal = ser_dr.TerminalID;
                                }

                            }
                            else
                            {
                                if (ser_dr.ServiceRate != 0)
                                {
                                    var servicedrrate = new GtEssrdr
                                    {
                                        BusinessKey = ser_dr.BusinessKey,
                                        ServiceId = ser_dr.ServiceId,
                                        DoctorId = ser_dr.DoctorId,
                                        RateType = ser_dr.RateType,
                                        CurrencyCode = ser_dr.CurrencyCode,
                                        EffectiveDate = ser_dr.EffectiveDate,

                                        ServiceRule = ser_dr.ServiceRule,
                                        ServiceRate = ser_dr.ServiceRate,

                                        ActiveStatus = ser_dr.ActiveStatus,
                                        FormId = ser_dr.FormId,
                                        CreatedBy = ser_dr.UserID,
                                        CreatedOn = ser_dr.CreatedOn,
                                        CreatedTerminal = ser_dr.TerminalID
                                    };
                                    db.GtEssrdr.Add(servicedrrate);
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
        public async Task<DO_ReturnParameter> AddServiceDoctorRate(List<DO_ServiceDoctorRate> obj)
        {
            using (eSyaEnterprise db = new eSyaEnterprise())
            {
                using (var dbContext = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var ser_dr in obj)
                        {
                            if (ser_dr.ServiceRate != 0)
                            {
                                var ServiceExist = db.GtEssrdr.Where(w => w.ServiceId == ser_dr.ServiceId && w.BusinessKey == ser_dr.BusinessKey && w.RateType == ser_dr.RateType && w.CurrencyCode == ser_dr.CurrencyCode && w.DoctorId == ser_dr.DoctorId).FirstOrDefault();
                                if (ServiceExist != null)
                                {
                                    var ser = db.GtEssrms.Where(w => w.ServiceId == ser_dr.ServiceId).Select(r => r.ServiceDesc).ToArray();
                                    return new DO_ReturnParameter() { Status = false, Message = "Rate Already Exist for the service '" + ser[0].ToString() + "'" };
                                }
                                else
                                {
                                    var servicedrrate = new GtEssrdr
                                    {
                                        BusinessKey = ser_dr.BusinessKey,
                                        ServiceId = ser_dr.ServiceId,
                                        DoctorId = ser_dr.DoctorId,
                                        RateType = ser_dr.RateType,
                                        CurrencyCode = ser_dr.CurrencyCode,
                                        EffectiveDate = ser_dr.EffectiveDate,

                                        ServiceRule = ser_dr.ServiceRule,
                                        ServiceRate = ser_dr.ServiceRate,

                                        ActiveStatus = ser_dr.ActiveStatus,
                                        FormId = ser_dr.FormId,
                                        CreatedBy = ser_dr.UserID,
                                        CreatedOn = ser_dr.CreatedOn,
                                        CreatedTerminal = ser_dr.TerminalID
                                    };
                                    db.GtEssrdr.Add(servicedrrate);
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
        #endregion

    }
}
