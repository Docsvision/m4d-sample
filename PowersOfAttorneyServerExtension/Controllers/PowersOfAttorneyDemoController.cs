using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Helpers;
using DocsVision.Platform.WebClient.Models;

using PowersOfAttorneyServerExtension.Services;

using System;
using System.Web.Mvc;

namespace PowersOfAttorneyServerExtension.Controllers
{
    /// <summary>
    /// Представляет собой контроллер для проверки лицензии
    /// </summary>
    public class PowersOfAttorneyDemoController : Controller
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
        public ActionResult CreatePowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;
            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.CreatePowerOfAttorney(context, powerOfAttorneyUserCardId);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ex.ToString());
            }

            return CreateSuccessResponse(powerOfAttorneyId);
        }

        /// <summary>
        /// Creates retrusted power of attorney with DovBb format
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>ID of created power of attorney</returns>
        [HttpPost]
        public ActionResult RetrustPowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;

            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.RetrustPowerOfAttorney(context, powerOfAttorneyUserCardId);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ex.ToString());
            }

            return CreateSuccessResponse(powerOfAttorneyId);
        }

        /// <summary>
        /// Return System power of attorney card ID by User power of attorney card ID
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>System power of attorney card ID</returns>
        [HttpGet]
        public ActionResult GetPowerOfAttorneyCardId(Guid powerOfAttorneyUserCardId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;

            Guid powerOfAttorneyId;
            try
            {
                powerOfAttorneyId = powersOfAttorneyDemoService.GetPowerOfAttorneyCardId(context, powerOfAttorneyUserCardId);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ex.ToString());
            }

            return CreateSuccessResponse(powerOfAttorneyId);
        }

        private static ActionResult CreateErrorResponse(string message)
        {
            return new ContentResult
            {
                Content = JsonHelper.SerializeToJson(CommonResponse.CreateError(message)),
                ContentType = "application/json"
            };
        }

        private static ActionResult CreateSuccessResponse<TData>(TData data)
        {
            return new ContentResult
            {
                Content = JsonHelper.SerializeToJson(CommonResponse.CreateSuccess(data)),
                ContentType = "application/json"
            };
        }
    }
}