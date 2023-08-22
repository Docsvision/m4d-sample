using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.BackOffice.WebClient.PowersOfAttorney;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.WebClient;

using Microsoft.SqlServer.Server;

using PowersOfAttorney.UserCard.Common.Helpers;
using PowersOfAttorneyServerExtension.Models;

using System;

using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyData;

namespace PowersOfAttorneyServerExtension.Services
{
    internal class PowersOfAttorneyDemoService : IPowersOfAttorneyDemoService
    {
        private readonly IPowerOfAttorneyProxyService powerOfAttorneyProxyService;
        private readonly ICurrentObjectContextProvider currentObjectContextProvider;

        public PowersOfAttorneyDemoService(IPowerOfAttorneyProxyService powerOfAttorneyProxyService, ICurrentObjectContextProvider currentObjectContextProvider)
        {
            this.powerOfAttorneyProxyService = powerOfAttorneyProxyService;
            this.currentObjectContextProvider = currentObjectContextProvider;
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

        public RequestRevocationResponse RequestRevocationPowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId, PowerOfAttorneyRevocationType revocationType, string revocationReason)
        {
            var userCardPowerOfAttorney = GetUserCardPowerOfAttorney(context, powerOfAttorneyUserCardId);

            PowerOfAttorneyRevocationData revocationData = new PowerOfAttorneyRevocationData
            {
                RevocationReason = revocationReason,
                RevocationType = revocationType
            };

            switch (revocationType)
            {
                case PowerOfAttorneyRevocationType.Representative:
                    var representative = userCardPowerOfAttorney.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);
                    revocationData.ApplicantInfo = new PowerOfAttorneyRevocationApplicantInfo
                    {
                        ApplicantType = PowerOfAttorneyRevocationApplicantType.Individual,
                        FirstName = representative.FirstName,
                        LastName = representative.LastName,
                        MiddleName = representative.MiddleName,
                        Inn = userCardPowerOfAttorney.GenRepresentativeINN,
                        Snils = userCardPowerOfAttorney.GenRepresentativeSNILS,
                        Phone = userCardPowerOfAttorney.GenReprPhoneNum
                    };
                    break;
                case PowerOfAttorneyRevocationType.Principal:
                    var ceo = userCardPowerOfAttorney.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyCeo);

                    revocationData.ApplicantInfo = new PowerOfAttorneyRevocationApplicantInfo
                    {
                        ApplicantType = PowerOfAttorneyRevocationApplicantType.Organization,
                        FirstName = ceo.FirstName,
                        LastName = ceo.LastName,
                        MiddleName = ceo.MiddleName,
                        Inn = userCardPowerOfAttorney.GenCeoIIN,
                        Snils = userCardPowerOfAttorney.GenCeoSNILS,
                        Phone = userCardPowerOfAttorney.GenCeoPhoneNum,
                    };

                    // Для передоверия данные организации требуется брать из родительской доверености
                    var cardWithPrincipalData = userCardPowerOfAttorney.IsRetrusted() ? GetUserCardPowerOfAttorney(context, userCardPowerOfAttorney.ParentalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_ParentalCardNotFound).GetObjectId()) : userCardPowerOfAttorney;
      
                    revocationData.ApplicantInfo.Kpp = cardWithPrincipalData.GenEntityPrincipal.HasValue ? cardWithPrincipalData.GenEntityPrincipal.Value.KPP : cardWithPrincipalData.GenEntityPrinKPP;
                    revocationData.ApplicantInfo.Ogrn = cardWithPrincipalData.GenEntityPrincipal.HasValue ? cardWithPrincipalData.GenEntityPrincipal.Value.OGRN : cardWithPrincipalData.GenEntPrinOGRN;
                    revocationData.ApplicantInfo.Name = cardWithPrincipalData.GenEntityPrincipal.HasValue ? cardWithPrincipalData.GenEntityPrincipal.Value.Name : cardWithPrincipalData.GenEntityPrinName;
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unsupported revocation type: {revocationType}");
            }

            var data = powerOfAttorneyProxyService.RequestRevocationPowerOfAttorney(userCardPowerOfAttorney.PowerOfAttorneyCardId.Value, revocationData, out string fileName);
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

        private PowerOfAttorney GetPowerOfAttorneyCard(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            var powerOfAttorneyId = GetPowerOfAttorneyCardId(context, powerOfAttorneyUserCardId);
            return context.GetObject<PowerOfAttorney>(powerOfAttorneyId);
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
                return userCardPowerOfAttorney.RepresentativeIndividual.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual).GetObjectId();

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
                return userCardPowerOfAttorney.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual).GetObjectId();


            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private string GetPrincipalInn(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.PrincipalOrganization.Value?.INN;

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
                return userCardPowerOfAttorney.GenEntityPrinINN ?? userCardPowerOfAttorney.GenEntityPrincipal.Value?.INN.AsNullable();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private PowerOfAttorneyData GetPowerOfAttorneyData(UserCardPowerOfAttorney userCard, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCard.ConvertToPowerOfAttorneyFNSDOVBBData(PowerOfAttorneyService);

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
                return userCard.ConvertToPowerOfAttorneyEMCHDData(currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext);


            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private Guid GetSigner(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.Signer.GetValueOrThrow(Resources.Error_EmptySigner).GetObjectId();

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
                return userCardPowerOfAttorney.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo).GetObjectId();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private IPowerOfAttorneyService PowerOfAttorneyService => currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext.GetService<IPowerOfAttorneyService>();
    }
}