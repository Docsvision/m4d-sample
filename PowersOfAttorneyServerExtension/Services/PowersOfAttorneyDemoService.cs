using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.BackOffice.WebClient.PowersOfAttorney;
using DocsVision.Platform.ObjectModel;

using Microsoft.SqlServer.Server;

using PowersOfAttorneyServerExtension.Helpers;

using System;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyData;

namespace PowersOfAttorneyServerExtension.Services
{
    internal class PowersOfAttorneyDemoService : IPowersOfAttorneyDemoService
    {
        private readonly IPowerOfAttorneyProxyService powerOfAttorneyProxyService;

        public PowersOfAttorneyDemoService(IPowerOfAttorneyProxyService powerOfAttorneyProxyService)
        {
            this.powerOfAttorneyProxyService = powerOfAttorneyProxyService;
        }

        public Guid CreatePowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);
            var powerOfAttorneyData = GetPowerOfAttorneyData(userCardPowerOfAttorney, formatId);

            var representativeID = userCardPowerOfAttorney.RepresentativeIndividual.GetObjectId();
            var signerID = userCardPowerOfAttorney.Signer.GetObjectId();
            var powerOfAttorney = powerOfAttorneyProxyService.CreatePowerOfAttorney(powerOfAttorneyData,
                                                                                    representativeID,
                                                                                    signerID,
                                                                                    formatId);
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
                throw new Exception(Resources.Error_PoaIDNotFoundInUserCard);
            }

            return userCardPowerOfAttorney.PowerOfAttorneyCardId.Value;
        }

        public Guid CreateRetrustPowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);
            var parentPoaFormat = userCardPowerOfAttorney.ParentalPowerOfAttorney.MainInfo.PowerOfAttorneyFormat.GetObjectId();
            var powerOfAttorneyData = GetPowerOfAttorneyData(userCardPowerOfAttorney, parentPoaFormat);
            
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
                throw new Exception(string.Format(Resources.Error_UserCardNotFound, documentId));
            }

            return new UserCardPowerOfAttorney(card, context);
        }

        private PowerOfAttorneyData GetPowerOfAttorneyData(UserCardPowerOfAttorney userCard, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCard.ConvertToPowerOfAttorneyFNSDOVBBData();

            if (formatId == PowerOfAttorneyEMHCDData.FormatId)
                return userCard.ConvertToPowerOfAttorneyEMHCDData();
           
                
            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }
    }
}