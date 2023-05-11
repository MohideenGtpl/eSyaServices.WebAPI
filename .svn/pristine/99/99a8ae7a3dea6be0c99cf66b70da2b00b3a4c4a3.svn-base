using HCP.Services.DO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Services.IF
{
    public interface ICommonMethodRepository
    {
        Task<List<DO_BusinessLocation>> GetBusinessKey();
        Task<List<DO_ApplicationCode>> GetApplicationCodesByCodeType(int codetype);
        Task<List<DO_CurrencyCode>> GetCurrencyCodes();
        Task<List<DO_DoctorMaster>> GetDoctors();
        Task<List<DO_Specialty>> GetSpecialties();

    }
}
