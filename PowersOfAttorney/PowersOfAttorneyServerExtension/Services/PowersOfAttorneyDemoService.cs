﻿using DocsVision.BackOffice.ObjectModel;
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

            var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
            var signerID = GetSigner(userCardPowerOfAttorney, formatId);
            var principalInn = GetPrincipalInn(userCardPowerOfAttorney, formatId);

            var powerOfAttorney = powerOfAttorneyProxyService.CreatePowerOfAttorney(powerOfAttorneyData,
                                                                                    representativeID,
                                                                                    signerID,
                                                                                    formatId,
                                                                                    powerOfAttorneyUserCardId,
                                                                                    principalInn);
            var powerOfAttorneyId = powerOfAttorney.GetObjectId();

            // Сохраним ИД созданной СКД в ПКД
            userCardPowerOfAttorney.PowerOfAttorneyCardId = powerOfAttorneyId;
            context.AcceptChanges();

            return powerOfAttorneyId;
        }

        public Guid CreateRetrustPowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            if (formatId == PowerOfAttorneyEMHCDData.FormatId)
            {
                // В данном примере передоверие Единого формата не реализовано
                throw new NotImplementedException(Resources.EmchdRetrustNotImplemented);
            }

            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);
            var powerOfAttorneyData = GetPowerOfAttorneyData(userCardPowerOfAttorney, formatId);

            var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
            var signerID = GetSigner(userCardPowerOfAttorney, formatId);
            var parentalPowerOfAttorney = GetParentalPowerOfAttorney(userCardPowerOfAttorney, formatId);
            var principalInn = GetPrincipalInn(userCardPowerOfAttorney, formatId);

            var powerOfAttorney = powerOfAttorneyProxyService.RetrustPowerOfAttorney(powerOfAttorneyData,
                                                                                    representativeID,
                                                                                    signerID,
                                                                                    parentalPowerOfAttorney,
                                                                                    powerOfAttorneyUserCardId,
                                                                                    principalInn);
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


        private UserCardPowerOfAttorney GetUserCardPowerOfAttorney(ObjectContext context, Guid documentId)
        {
            var card = context.GetObject<Document>(documentId);
            if (card == null)
            {
                throw new Exception(string.Format(Resources.Error_UserCardNotFound, documentId));
            }

            return new UserCardPowerOfAttorney(card, context);
        }


        private Guid GetParentalPowerOfAttorney(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.ParentalPowerOfAttorney.GetObjectId();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private Guid GetRepresentative(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.RepresentativeIndividual.GetObjectId();

            if (formatId == PowerOfAttorneyEMHCDData.FormatId)
                return userCardPowerOfAttorney.GenRepresentative.GetObjectId();


            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private string GetPrincipalInn(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.PrincipalOrganization?.INN;

            if (formatId == PowerOfAttorneyEMHCDData.FormatId)
                return userCardPowerOfAttorney.GenEntityPrinINN ?? userCardPowerOfAttorney.GenEntityPrincipal?.INN;

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private PowerOfAttorneyData GetPowerOfAttorneyData(UserCardPowerOfAttorney userCard, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCard.ConvertToPowerOfAttorneyFNSDOVBBData();

            if (formatId == PowerOfAttorneyEMHCDData.FormatId)
                return userCard.ConvertToPowerOfAttorneyEMHCDData();


            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private Guid GetSigner(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.Signer.GetObjectId();

            if (formatId == PowerOfAttorneyEMHCDData.FormatId)
                return userCardPowerOfAttorney.GenCeo.GetObjectId();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }
    }
}