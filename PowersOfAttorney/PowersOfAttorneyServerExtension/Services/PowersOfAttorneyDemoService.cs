using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.WebClient;

using PowersOfAttorney.UserCard.Common.Helpers;

using PowersOfAttorneyServerExtension.Models;

using System;

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

            var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
            var signerID = GetSigner(userCardPowerOfAttorney, formatId);

            var representative = context.GetObject<StaffEmployee>(representativeID);
            var signer = context.GetObject<StaffEmployee>(signerID);
            var format = context.GetObject<PowersPowerOfAttorneyFormat>(formatId);

            var powerOfAttorney = PowerOfAttorneyService.CreatePowerOfAttorney(powerOfAttorneyData,
                representative, signer, format, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);

            powerOfAttorney.MainInfo.UserCard = powerOfAttorneyUserCardId;
            if(string.IsNullOrEmpty(powerOfAttorney.MainInfo.PrincipalINN))
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

            var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
            var signerID = GetSigner(userCardPowerOfAttorney, formatId);
            var parentalPowerOfAttorney = GetParentalPowerOfAttorney(userCardPowerOfAttorney, formatId);

            var representative = context.GetObject<StaffEmployee>(representativeID);
            var signer = context.GetObject<StaffEmployee>(signerID);
            var parent = context.GetObject<PowerOfAttorney>(parentalPowerOfAttorney);


            var powerOfAttorney = PowerOfAttorneyService.RetrustPowerOfAttorney(powerOfAttorneyData, representative, signer, parent, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);

            powerOfAttorney.MainInfo.UserCard = powerOfAttorneyUserCardId;
            if(string.IsNullOrEmpty(powerOfAttorney.MainInfo.PrincipalINN))
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
                    var ceo = userCardPowerOfAttorney.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo);

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
                    revocationData.ApplicantInfo.Inn = cardWithPrincipalData.GenEntityPrincipal.HasValue ? cardWithPrincipalData.GenEntityPrincipal.Value.INN : cardWithPrincipalData.GenEntityPrinINN;
                    revocationData.ApplicantInfo.Ogrn = cardWithPrincipalData.GenEntityPrincipal.HasValue ? cardWithPrincipalData.GenEntityPrincipal.Value.OGRN : cardWithPrincipalData.GenEntPrinOGRN;
                    revocationData.ApplicantInfo.Name = cardWithPrincipalData.GenEntityPrincipal.HasValue ? cardWithPrincipalData.GenEntityPrincipal.Value.Name : cardWithPrincipalData.GenEntityPrinName;
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"Unsupported revocation type: {revocationType}");
            }

            var powerOfAttorney = context.GetObject<PowerOfAttorney>(userCardPowerOfAttorney.PowerOfAttorneyCardId.Value);

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

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
            {
                if (userCardPowerOfAttorney.GenParentalPowerOfAttorneyUserCard.HasValue)
                    return userCardPowerOfAttorney.GenParentalPowerOfAttorney.GetObjectId();

                return userCardPowerOfAttorney.GenOriginaPowerOfAttorney.GetObjectId();
            }

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