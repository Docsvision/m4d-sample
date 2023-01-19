using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.WebClient.PowersOfAttorney;
using DocsVision.Platform.ObjectModel;

using PowersOfAttorneyServerExtension.Helpers;

using System;

namespace PowersOfAttorneyServerExtension.Services
{
    internal class PowersOfAttorneyDemoService : IPowersOfAttorneyDemoService
    {
        private readonly IPowerOfAttorneyProxyService powerOfAttorneyProxyService;

        public PowersOfAttorneyDemoService(IPowerOfAttorneyProxyService powerOfAttorneyProxyService)
        {
            this.powerOfAttorneyProxyService = powerOfAttorneyProxyService;
        }

        public Guid CreatePowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);

            var powerOfAttorneyData = userCardPowerOfAttorney.ConvertToPowerOfAttorneyFNSDOVBBData();
            var representativeID = userCardPowerOfAttorney.RepresentativeIndividual.GetObjectId();
            var signerID = userCardPowerOfAttorney.Signer.GetObjectId();

            var powerOfAttorney = powerOfAttorneyProxyService.CreatePowerOfAttorney(powerOfAttorneyData,
                                                                                    representativeID,
                                                                                    signerID,
                                                                                    UserCardToPowerOfAttorneyDataConverter.PowerOfAttorneyTypeId);
            var powerOfAttorneyId = powerOfAttorney.GetObjectId();
            
            // Сохраним ИД созданной СКД в ПКД
            userCardPowerOfAttorney.PowerOfAttorneyCardId = powerOfAttorneyId;
            context.AcceptChanges();
            
            return powerOfAttorneyId;
        }

        public Guid GetPowerOfAttorneyCardId(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);

            if (userCardPowerOfAttorney.PowerOfAttorneyCardId == null)
            {
                throw new Exception("В ПКД отсутствует идентификатор СКД");
            }
            
            return userCardPowerOfAttorney.PowerOfAttorneyCardId.Value;
        }

        public Guid CreateRetrustPowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);

            var powerOfAttorneyData = userCardPowerOfAttorney.ConvertToPowerOfAttorneyFNSDOVBBData();
            var representativeID = userCardPowerOfAttorney.RepresentativeIndividual.GetObjectId();
            var signerID = userCardPowerOfAttorney.Signer.GetObjectId();
            
            var parentalPowerOfAttorney = userCardPowerOfAttorney.ParentalPowerOfAttorney.GetObjectId();

            var powerOfAttorney = powerOfAttorneyProxyService.RetrustPowerOfAttorney(powerOfAttorneyData,
                                                                                    representativeID,
                                                                                    signerID,
                                                                                    parentalPowerOfAttorney);
            var powerOfAttorneyId = powerOfAttorney.GetObjectId();
            
            // Сохраним ИД созданной СКД в ПКД
            userCardPowerOfAttorney.PowerOfAttorneyCardId = powerOfAttorneyId;
            context.AcceptChanges();
            
            return powerOfAttorneyId;
        }

        private UserCardPowerOfAttorney GetUserCardPowerOfAttorney(ObjectContext context, Guid documentId)
        {
            var card = context.GetObject<Document>(documentId);
            if (card == null)
            {
                throw new Exception($"ПКД {documentId} не найдена");
            }

            return new UserCardPowerOfAttorney(card, context);
        }
    }
}