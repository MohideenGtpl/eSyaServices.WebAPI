using HCP.Services.DL.Entities;
using HCP.Services.DO;
using HCP.Services.IF;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Services.DL.Repository
{
    public class CommonMethodRepository : ICommonMethodRepository
    {
        public async Task<List<DO_BusinessLocation>> GetBusinessKey()
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var bk = db.GtEcbsln
                        .Where(w => w.ActiveStatus)
                        .Select(r => new DO_BusinessLocation
                        {
                            BusinessKey = r.BusinessKey,
                            LocationDescription = r.BusinessName + " - " +r.LocationDescription
                        }).ToListAsync();

                    return await bk;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_ApplicationCode>> GetApplicationCodesByCodeType(int codetype)
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var bk = db.GtEcapcd
                        .Where(w => w.ActiveStatus && w.CodeType==codetype)
                        .Select(r => new DO_ApplicationCode
                        {
                            ApplicationCode = r.ApplicationCode,
                            CodeDesc = r.CodeDesc
                        }).ToListAsync();

                    return await bk;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_CurrencyCode>> GetCurrencyCodes()
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var cc = db.GtEccuco
                        .Where(w => w.ActiveStatus)
                        .Select(c => new DO_CurrencyCode
                        {
                            CurrencyCode = c.CurrencyCode,
                            CurrencyName = c.CurrencyName
                        }).ToListAsync();

                    return await cc;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_DoctorMaster>> GetDoctors()
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var doc = db.GtEsdocd
                        .Where(w => w.ActiveStatus)
                        .Select(d => new DO_DoctorMaster
                        {
                            DoctorId = d.DoctorId,
                            DoctorName = d.DoctorName 
                        }).OrderBy(o => o.DoctorName).ToListAsync();

                    return await doc;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<DO_Specialty>> GetSpecialties()
        {
            try
            {
                using (var db = new eSyaEnterprise())
                {
                    var sp = db.GtEsspcd
                        .Where(w => w.ActiveStatus)
                        .Select(s => new DO_Specialty
                        {
                            SpecialtyId = s.SpecialtyId,
                            SpecialtyDesc = s.SpecialtyDesc
                        }).OrderBy(o=> o.SpecialtyDesc).ToListAsync();

                    return await sp;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
