using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.WebClient.PowersOfAttorney;
using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Models;
using DocsVision.Platform.WebClient.Models.Generic;

using PowersOfAttorneyServerExtension.Helpers;

using System;
using System.Web.Mvc;

namespace LicenseCheckServerExtension.Controllers
{
    /// <summary>
    /// Представляет собой контроллер для проверки лицензии
    /// </summary>
    public class PowersOfAttorneyDemoController : Controller
    {
        private readonly ICurrentObjectContextProvider currentObjectContextProvider;
        private readonly IPowerOfAttorneyProxyService powerOfAttorneyProxyService;

        /// <summary>
        /// Создаёт новый экземпляр <see cref="PowersOfAttorneyDemoController"/>
        /// </summary>
        public PowersOfAttorneyDemoController(ICurrentObjectContextProvider currentObjectContextProvider, IPowerOfAttorneyProxyService powerOfAttorneyProxyService)
        {
            this.currentObjectContextProvider = currentObjectContextProvider;
            this.powerOfAttorneyProxyService = powerOfAttorneyProxyService;
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
            var card = context.GetObject<Document>(powerOfAttorneyUserCardId);
            var userCardPowerOfAttorney = new UserCardPowerOfAttorney(card, context);
            
            var powerOfAttorneyData = userCardPowerOfAttorney.ConvertToPowerOfAttorneyFNSDOVBBData();
            var representativeID = userCardPowerOfAttorney.RepresentativeIndividual.GetObjectId();
            var signerID = userCardPowerOfAttorney.Signer.GetObjectId();
            
            var powerOfAttorney = powerOfAttorneyProxyService.CreatePowerOfAttorney(powerOfAttorneyData,
                                                                                    representativeID,
                                                                                    signerID,
                                                                                    PowerOfAttorneyFNSDOVBBDataProxy.PowerOfAttorneyTypeId);

            // Сохраним ИД созданной СКД в ПКД
            userCardPowerOfAttorney.PowerOfAttorneyCardId = powerOfAttorney.GetObjectId();
            context.AcceptChanges();

            return CommonResponse.CreateSuccess(powerOfAttorney.GetObjectId());
        }

        /// <summary>
        /// Creates retrusted power of attorney with DovBb format
        /// </summary>
        /// <param name="powerOfAttorneyUserCardId">User card of Power of attorney</param>
        /// <returns>ID of created power of attorney</returns>
        [HttpPost]
        public CommonResponse<Guid> RetrustPowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;
            var card = context.GetObject<Document>(powerOfAttorneyUserCardId);
            var userCardPowerOfAttorney = new UserCardPowerOfAttorney(card, context);

            var powerOfAttorneyData = userCardPowerOfAttorney.ConvertToPowerOfAttorneyFNSDOVBBData();
            var representativeID = userCardPowerOfAttorney.RepresentativeIndividual.GetObjectId();
            var signerID = userCardPowerOfAttorney.Signer.GetObjectId();
            var parentalPowerOfAttorney = userCardPowerOfAttorney.ParentalPowerOfAttorney.GetObjectId();


            var powerOfAttorney = powerOfAttorneyProxyService.RetrustPowerOfAttorney(powerOfAttorneyData,
                                                                                    representativeID,
                                                                                    signerID,
                                                                                    parentalPowerOfAttorney);
            // Сохраним ИД созданной СКД в ПКД
            userCardPowerOfAttorney.PowerOfAttorneyCardId = powerOfAttorney.GetObjectId();
            context.AcceptChanges();
            
            return CommonResponse.CreateSuccess(powerOfAttorney.GetObjectId());
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
            var card = context.GetObject<Document>(powerOfAttorneyUserCardId);
            var userCardPowerOfAttorney = new UserCardPowerOfAttorney(card, context);

            if (userCardPowerOfAttorney.PowerOfAttorneyCardId == null)
            {
                return CommonResponse.CreateError<Guid>("В ПКД отсутствует идентификатор СКД");
            }
            
            return CommonResponse.CreateSuccess(userCardPowerOfAttorney.PowerOfAttorneyCardId.Value);
        }

    }
}