using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Models;
using DocsVision.Platform.WebClient.Models.Generic;

using PowersOfAttorneyServerExtension.Services;

using System;
using System.Web.Http;

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
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;
            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.CreatePowerOfAttorney(context, powerOfAttorneyUserCardId);
            }
            catch (Exception ex)
            {
                return CommonResponse.CreateError< Guid>(ex.ToString());
            }

            return CommonResponse.CreateSuccess(powerOfAttorneyId);
        }

        /// <summary>
        /// Creates retrusted power of attorney with DovBb format
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>ID of created power of attorney</returns>
        [HttpPost]
        public CommonResponse<Guid> CreateRetrustPowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;

            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.CreateRetrustPowerOfAttorney(context, powerOfAttorneyUserCardId);
            }
            catch (Exception ex)
            {
                return CommonResponse.CreateError<Guid>(ex.ToString());
            }

            return CommonResponse.CreateSuccess(powerOfAttorneyId);
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
    }
}