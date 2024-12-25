using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.BackOffice.WebClient.CardKind;
using DocsVision.BackOffice.WebClient.PowersOfAttorney;
using DocsVision.BackOffice.WebClient.State;
using DocsVision.Platform.WebClient;
using DocsVision.Platform.WebClient.Models;
using DocsVision.Platform.WebClient.Models.Generic;

using PowersOfAttorneyServerExtension.Models;
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
        private readonly IPowerOfAttorneyProxyService powerOfAttorneyProxyService;
        private readonly ICardKindService cardKindService;
        private readonly IStateService stateService;

        /// <summary>
        /// Создаёт новый экземпляр <see cref="PowersOfAttorneyDemoController"/>
        /// </summary>
        public PowersOfAttorneyDemoController(ICurrentObjectContextProvider currentObjectContextProvider, 
            IPowersOfAttorneyDemoService powersOfAttorneyDemoService,
            IPowerOfAttorneyProxyService powerOfAttorneyProxyService,
            ICardKindService cardKindService,
            IStateService stateService)
        {
            this.currentObjectContextProvider = currentObjectContextProvider;
            this.powersOfAttorneyDemoService = powersOfAttorneyDemoService;
            this.powerOfAttorneyProxyService = powerOfAttorneyProxyService;
            this.cardKindService = cardKindService;
            this.stateService = stateService;
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

            Guid powerOfAttorneyId = powersOfAttorneyDemoService.GetPowerOfAttorneyCardId(context, powerOfAttorneyUserCardId);
            

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
            RequestRevocationResponse result = powersOfAttorneyDemoService.RequestRevocationPowerOfAttorney(context, request.PowerOfAttorneyUserCardId, request.RevocationType, request.RevocationReason);
            return CommonResponse.CreateSuccess(result);
        }

        [HttpGet]
        public CommonResponse<PowerOfAttorneySignatureDataResponse> GetSignatureData(Guid powerOfAttorneyUserCardId)
        {
            var sessionContext = currentObjectContextProvider.GetOrCreateCurrentSessionContext();
            Guid powerOfAttorneyId = powersOfAttorneyDemoService.GetPowerOfAttorneyCardId(sessionContext.ObjectContext, powerOfAttorneyUserCardId);
            var machineReadablePowerOfAttorney = powerOfAttorneyProxyService.GetMachineReadablePowerOfAttorneyBase64Content(powerOfAttorneyId, out string fileName);
            var kindId = this.cardKindService.GetCardKindId(sessionContext, powerOfAttorneyUserCardId);
            var state = stateService.GetCardState(sessionContext, powerOfAttorneyUserCardId);
            var operations = sessionContext.AdvancedCardManager.GetOperations(powerOfAttorneyUserCardId);
            var timestamp = sessionContext.AdvancedCardManager.GetCardTimestamp(powerOfAttorneyUserCardId);

            var result = new PowerOfAttorneySignatureDataResponse()
            {
                CardId = powerOfAttorneyUserCardId,
                KindId = kindId,
                Operations = operations,
                PowerOfAttorneyContent = machineReadablePowerOfAttorney,
                PowerOfAttorneyFileName = fileName,
                PowerOfAttorneyId = powerOfAttorneyId,
                State = state,
                Timestamp = timestamp
            };
            return CommonResponse.CreateSuccess(result);
        }

        
        [HttpPost]
        public CommonResponse<Guid> CreateFNSDOVEL502PowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {           
            try
            {
                return CreatePowerOfAttorneyInternal(powerOfAttorneyUserCardId, PowerOfAttorneyFNSDOVEL502Data.FormatId);
            }
            catch (Exception ex)
            {
                var response = CommonResponse.CreateError<Guid>(ex.Message);
                return response;
            }
        }

        [HttpPost]
        public CommonResponse<Guid> CreateFNSDOVEL502RetrustPowerOfAttorney(Guid powerOfAttorneyUserCardId)
        {          
            try
            {
                return CreateRetrustPowerOfAttorneyInternal(powerOfAttorneyUserCardId, PowerOfAttorneyFNSDOVEL502Data.FormatId);
            }
            catch (Exception ex)
            {
                var response = CommonResponse.CreateError<Guid>(ex.Message);
                return response;
            }
        }

        private CommonResponse<Guid> CreatePowerOfAttorneyInternal(Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;
            Guid powerOfAttorneyId = powersOfAttorneyDemoService.CreatePowerOfAttorney(context, powerOfAttorneyUserCardId, formatId);

            return CommonResponse.CreateSuccess(powerOfAttorneyId);
        }

        private CommonResponse<Guid> CreateRetrustPowerOfAttorneyInternal(Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            var context = currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext;

            Guid powerOfAttorneyId = powersOfAttorneyDemoService.CreateRetrustPowerOfAttorney(context, powerOfAttorneyUserCardId, formatId);

            return CommonResponse.CreateSuccess(powerOfAttorneyId);
        }
    }
}