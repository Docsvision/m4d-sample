using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Models;
using DocsVision.Platform.WebClient.Models.Generic;

using Microsoft.SqlServer.Server;

using PowersOfAttorneyServerExtension.Models;
using PowersOfAttorneyServerExtension.Services;

using System;
using System.Web.Http;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyEMCHDData;

namespace PowersOfAttorneyServerExtension.Controllers
{
    /// <summary>
    /// Представляет собой контроллер для проверки лицензии
    /// </summary>
    public class PowersOfAttorneyDemoController : ApiController
    {
        private readonly ICurrentObjectContextProvider currentObjectContextProvider;
        private readonly IPowersOfAttorneyDemoService powersOfAttorneyDemoService;

        /// <summary>
        /// Создаёт новый экземпляр <see cref="PowersOfAttorneyDemoController"/>
        /// </summary>
        public PowersOfAttorneyDemoController(ICurrentObjectContextProvider currentObjectContextProvider, IPowersOfAttorneyDemoService powersOfAttorneyDemoService)
        {
            this.currentObjectContextProvider = currentObjectContextProvider;
            this.powersOfAttorneyDemoService = powersOfAttorneyDemoService;
        }

        /// <summary>
        /// Creates new power of attorney with DovBb format
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>ID of created power of attorney</returns>
        [HttpPost]
        public CommonResponse<Guid> CreatePowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            return CreatePowerOfAttorneyInternal(powerOfAttorneyUserCardId, PowerOfAttorneyFNSDOVBBData.FormatId);
        }

        /// <summary>
        /// Creates new power of attorney with general format
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>ID of created power of attorney</returns>
        [HttpPost]
        public CommonResponse<Guid> CreateEMCHDPowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            return CreatePowerOfAttorneyInternal(powerOfAttorneyUserCardId, PowerOfAttorneyEMCHDData.FormatId);
        }

        /// <summary>
        /// Creates retrusted power of attorney with DovBb format
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>ID of created power of attorney</returns>
        [HttpPost]
        public CommonResponse<Guid> CreateRetrustPowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            return CreateRetrustPowerOfAttorneyInternal(powerOfAttorneyUserCardId, PowerOfAttorneyFNSDOVBBData.FormatId);
        }

        /// <summary>
        /// Creates retrusted power of attorney with general format
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>ID of created power of attorney</returns>
        [HttpPost]
        public CommonResponse<Guid> CreateEMCHDRetrustPowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            return CreateRetrustPowerOfAttorneyInternal(powerOfAttorneyUserCardId, PowerOfAttorneyEMCHDData.FormatId);
        }

        /// <summary>
        /// Return System power of attorney card ID by User power of attorney card ID
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>System power of attorney card ID</returns>
        [HttpGet]
        public CommonResponse<Guid> GetPowerOfAttorneyCardId(Guid powerOfAttorneyUserCardId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;

            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.GetPowerOfAttorneyCardId(context, powerOfAttorneyUserCardId);
            }
            catch (Exception ex)
            {
                return CommonResponse.CreateError<Guid>(ex.ToString());
            }

            return CommonResponse.CreateSuccess(powerOfAttorneyId);
        }

        /// <summary>
        /// Return power Of attorney number by User power of attorney card ID
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>Power Of attorney number</returns>
        [HttpGet]
        public CommonResponse<string> GetPowerOfAttorneyNumber(Guid powerOfAttorneyUserCardId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;

            var powerOfAttorneyId = powersOfAttorneyDemoService.GetPowerOfAttorneyCardId(context, powerOfAttorneyUserCardId);
            if(powerOfAttorneyId == Guid.Empty)
                return CommonResponse.CreateError<string>(Resources.Error_PowerOfAttorneyNumberNotFound);

            var powerOfAttorney = context.GetObject<PowerOfAttorney>(powerOfAttorneyId);
            if (powerOfAttorney ==null || powerOfAttorney.MainInfo.PowerOfAttorneyNumber == Guid.Empty)
                return CommonResponse.CreateError<string>(Resources.Error_PowerOfAttorneyNumberNotFound);

            return CommonResponse.CreateSuccess(powerOfAttorney.MainInfo.PowerOfAttorneyNumber.ToString());
        }

        [HttpPost]
        public CommonResponse<RequestRevocationResponse> RequestRevocationPowerOfAttorney([FromBody] RequestRevocationRequest request)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;
            RequestRevocationResponse result;

            try
            {
                result = powersOfAttorneyDemoService.RequestRevocationPowerOfAttorney(context, request.PowerOfAttorneyUserCardId, request.RevocationType, request.RevocationReason);
            }
            catch (Exception ex)
            {
                return CommonResponse.CreateError<RequestRevocationResponse>(ex.ToString());
            }

            return CommonResponse.CreateSuccess(result);
        }

        private CommonResponse<Guid> CreatePowerOfAttorneyInternal(Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;
            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.CreatePowerOfAttorney(context, powerOfAttorneyUserCardId, formatId);
            }
            catch (Exception ex)
            {
                return CommonResponse.CreateError<Guid>(ex.ToString());
            }

            return CommonResponse.CreateSuccess(powerOfAttorneyId);
        }

        private CommonResponse<Guid> CreateRetrustPowerOfAttorneyInternal(Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;

            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.CreateRetrustPowerOfAttorney(context, powerOfAttorneyUserCardId, formatId);
            }
            catch (Exception ex)
            {
                return CommonResponse.CreateError<Guid>(ex.ToString());
            }

            return CommonResponse.CreateSuccess(powerOfAttorneyId);
        }
    }
}