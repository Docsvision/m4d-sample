﻿using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.WebClient;
using Microsoft.SqlServer.Server;
using PowersOfAttorney.UserCard.Common.Helpers;
using PowersOfAttorneyServerExtension.Models;
using System;
using System.Net.Security;
using System.Runtime.Remoting.Contexts;
using static DocsVision.BackOffice.CardLib.CardDefs.CardDocument;
using static DocsVision.BackOffice.CardLib.CardDefs.CardSignatureList;


namespace PowersOfAttorneyServerExtension.Services
{
    internal class PowersOfAttorneyDemoService : IPowersOfAttorneyDemoService
    {
        private readonly ICurrentObjectContextProvider currentObjectContextProvider;

        public PowersOfAttorneyDemoService(ICurrentObjectContextProvider currentObjectContextProvider)
        {
            this.currentObjectContextProvider = currentObjectContextProvider;
        }

        public Guid CreatePowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);
            var powerOfAttorneyData = GetPowerOfAttorneyData(userCardPowerOfAttorney, formatId);
            var signerID = GetSigner(userCardPowerOfAttorney, formatId);
            var signer = context.GetObject<StaffEmployee>(signerID);
            var format = context.GetObject<PowersPowerOfAttorneyFormat>(formatId);
            
            PowerOfAttorney powerOfAttorney;
            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
            {
                powerOfAttorney = CreateFNSDOVEL502502PowerOfAttorney(context, powerOfAttorneyData, userCardPowerOfAttorney, signer, format);
            } else
            {
                var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
                var representative = context.GetObject<StaffEmployee>(representativeID);

                powerOfAttorney = PowerOfAttorneyService.CreatePowerOfAttorney(powerOfAttorneyData,
                    representative, signer, format, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);
            }                       

            powerOfAttorney.MainInfo.UserCard = powerOfAttorneyUserCardId;
            if (string.IsNullOrEmpty(powerOfAttorney.MainInfo.PrincipalINN))
                powerOfAttorney.MainInfo.PrincipalINN = GetPrincipalInn(userCardPowerOfAttorney, formatId);

            context.SaveObject(powerOfAttorney);

            var powerOfAttorneyId = powerOfAttorney.GetObjectId();

            // Сохраним ИД созданной СКД в ПКД
            userCardPowerOfAttorney.PowerOfAttorneyCardId = powerOfAttorneyId;
            context.AcceptChanges();

            return powerOfAttorneyId;
        }

        public Guid CreateRetrustPowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId, Guid formatId)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);
            var powerOfAttorneyData = GetPowerOfAttorneyData(userCardPowerOfAttorney, formatId);
            
            var signerID = GetSigner(userCardPowerOfAttorney, formatId);
            var parentalPowerOfAttorney = GetParentalPowerOfAttorney(userCardPowerOfAttorney, formatId);            
            var signer = context.GetObject<StaffEmployee>(signerID);
            var parent = context.GetObject<PowerOfAttorney>(parentalPowerOfAttorney);

            PowerOfAttorney powerOfAttorney;
            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
            {
                powerOfAttorney = CreateFNSDOVEL502RetrustPowerOfAttorney(context, powerOfAttorneyData, userCardPowerOfAttorney, parent, signer);
            }
            else
            {
                var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
                var representative = context.GetObject<StaffEmployee>(representativeID);
                powerOfAttorney = PowerOfAttorneyService.RetrustPowerOfAttorney(powerOfAttorneyData, representative, signer, parent, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);
            }

            powerOfAttorney.MainInfo.UserCard = powerOfAttorneyUserCardId;
            if (string.IsNullOrEmpty(powerOfAttorney.MainInfo.PrincipalINN))
                powerOfAttorney.MainInfo.PrincipalINN = GetPrincipalInn(userCardPowerOfAttorney, formatId);

            context.SaveObject(powerOfAttorney);

            var powerOfAttorneyId = powerOfAttorney.GetObjectId();

            // Сохраним ИД созданной СКД в ПКД
            userCardPowerOfAttorney.PowerOfAttorneyCardId = powerOfAttorneyId;
            context.AcceptChanges();

            return powerOfAttorneyId;
        }

        public RequestRevocationResponse RequestRevocationPowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId, PowerOfAttorneyRevocationType revocationType, string revocationReason)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);
            var powerOfAttorney = context.GetObject<PowerOfAttorney>(userCardPowerOfAttorney.PowerOfAttorneyCardId.Value);
            var poaFormatId = powerOfAttorney.MainInfo.PowerOfAttorneyFormat.GetObjectId();

            PowerOfAttorneyRevocationData revocationData = GetPowerOfAttorneyRevocationData(context, userCardPowerOfAttorney, poaFormatId, revocationType, revocationReason);

            PowerOfAttorneyService.RequestRevocation(powerOfAttorney, revocationData);
            context.SaveObject(powerOfAttorney);

            var data = PowerOfAttorneyService.GetRevocationPowerOfAttorneyFileData(powerOfAttorney, out string fileName);

            return new RequestRevocationResponse
            {
                Content = Convert.ToBase64String(data),
                FileName = fileName
            };
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

            if (formatId == PowerOfAttorneyEMCHDData.FormatId || formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
            {
                if (userCardPowerOfAttorney.GenParentalPowerOfAttorneyUserCard.HasValue)
                    return userCardPowerOfAttorney.GenParentalPowerOfAttorney.GetObjectId();

                return userCardPowerOfAttorney.GenOriginaPowerOfAttorney.GetObjectId();
            }

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private PowerOfAttorneyRevocationData GetPowerOfAttorneyRevocationData(ObjectContext context, UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId, PowerOfAttorneyRevocationType revocationType, string revocationReason)
        {
            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
            {
                return userCardPowerOfAttorney.ConvertToPowerOfAttorneyFNSDOVEL502RevocationData(context, revocationReason);
            }

            return userCardPowerOfAttorney.ConvertToPowerOfAttorneyRevocationData(context, revocationType, revocationReason);
        }

        private Guid GetRepresentative(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.RepresentativeIndividual.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual).GetObjectId();

            if (formatId == PowerOfAttorneyEMCHDData.FormatId || formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
                return userCardPowerOfAttorney.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual).GetObjectId();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private PowerOfAttorney CreateFNSDOVEL502502PowerOfAttorney(ObjectContext context, PowerOfAttorneyData powerOfAttorneyData, UserCardPowerOfAttorney userCardPowerOfAttorney, StaffEmployee signer, PowersPowerOfAttorneyFormat format)
        {
            var representativeType = userCardPowerOfAttorney.GenRepresentativeType502 ?? throw new ApplicationException(Resources.Error_GenRepresentativeIsEmpty);
            if (representativeType == UserCardPowerOfAttorney.RepresentativeType.individual)
            {
                var representativeID = GetRepresentative(userCardPowerOfAttorney, PowerOfAttorneyFNSDOVEL502Data.FormatId);
                var representative = context.GetObject<StaffEmployee>(representativeID);

                return PowerOfAttorneyService.CreatePowerOfAttorney(powerOfAttorneyData,
                    representative, signer, format, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);
            }
            else if (representativeType == UserCardPowerOfAttorney.RepresentativeType.entity)
            {
                var representative = userCardPowerOfAttorney.EntityRepresentative.Value;
                return PowerOfAttorneyService.CreatePowerOfAttorney(powerOfAttorneyData,
                    representative, signer, format, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);
            }

            throw new ArgumentOutOfRangeException(nameof(representativeType));
        }

        private PowerOfAttorney CreateFNSDOVEL502RetrustPowerOfAttorney(ObjectContext context, PowerOfAttorneyData powerOfAttorneyData,
             UserCardPowerOfAttorney userCardPowerOfAttorney, PowerOfAttorney parent, StaffEmployee signer)
        {
            var representativeType = userCardPowerOfAttorney.GenRepresentativeType502 ?? throw new ApplicationException(Resources.Error_GenRepresentativeIsEmpty);
            if (representativeType == UserCardPowerOfAttorney.RepresentativeType.individual)
            {
                var representativeID = GetRepresentative(userCardPowerOfAttorney, PowerOfAttorneyFNSDOVEL502Data.FormatId);
                var representative = context.GetObject<StaffEmployee>(representativeID);

                return PowerOfAttorneyService.RetrustPowerOfAttorney(powerOfAttorneyData, representative, signer,
                    parent, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);
            }
            else if (representativeType == UserCardPowerOfAttorney.RepresentativeType.entity)
            {
                var representative = userCardPowerOfAttorney.EntityRepresentative.Value;
                return PowerOfAttorneyService.RetrustPowerOfAttorney(powerOfAttorneyData, representative, signer,
                    parent, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);
            }

            throw new ArgumentOutOfRangeException(nameof(representativeType));
        }

        private string GetPrincipalInn(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.PrincipalOrganization.Value?.INN;

            if (formatId == PowerOfAttorneyEMCHDData.FormatId || formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
                return userCardPowerOfAttorney.GenEntityPrinINN ?? userCardPowerOfAttorney.GenEntityPrincipal.Value?.INN.AsNullable();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private PowerOfAttorneyData GetPowerOfAttorneyData(UserCardPowerOfAttorney userCard, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCard.ConvertToPowerOfAttorneyFNSDOVBBData(PowerOfAttorneyService);

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
                return userCard.ConvertToPowerOfAttorneyEMCHDData(currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext);

            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
                return userCard.ConvertToPowerOfAttorneyFNSDOVEL502Data(currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext);

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private Guid GetSigner(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.Signer.GetValueOrThrow(Resources.Error_EmptySigner).GetObjectId();

            if (formatId == PowerOfAttorneyEMCHDData.FormatId || formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
                return userCardPowerOfAttorney.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo).GetObjectId();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private IPowerOfAttorneyService PowerOfAttorneyService => currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext.GetService<IPowerOfAttorneyService>();
    }
}