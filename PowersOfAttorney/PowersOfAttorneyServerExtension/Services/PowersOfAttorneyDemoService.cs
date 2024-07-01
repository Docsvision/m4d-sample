using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.WebClient;

using PowersOfAttorney.UserCard.Common.Helpers;

using PowersOfAttorneyServerExtension.Models;

using System;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using static DocsVision.BackOffice.CardLib.CardDefs.CardSignatureList;
using static DocsVision.BackOffice.ObjectModel.PowerOfAttorneyFNSDOVEL502RevocationData;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSData;
using static PowersOfAttorney.UserCard.Common.Helpers.UserCardPowerOfAttorney;

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
            var representativeType = userCardPowerOfAttorney.GenRepresentativeType;
            PowerOfAttorney powerOfAttorney = null;
            if (representativeType == null || representativeType.Value == UserCardPowerOfAttorney.GenRepresentativeTypes.individual)
            {
                var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
                var representative = context.GetObject<StaffEmployee>(representativeID);

                powerOfAttorney = PowerOfAttorneyService.CreatePowerOfAttorney(powerOfAttorneyData,
                    representative, signer, format, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);
            }
            else if (representativeType == UserCardPowerOfAttorney.GenRepresentativeTypes.entity)
            {
                var representative = userCardPowerOfAttorney.EntityRepresentative.Value;

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

            var representativeID = GetRepresentative(userCardPowerOfAttorney, formatId);
            var signerID = GetSigner(userCardPowerOfAttorney, formatId);
            var parentalPowerOfAttorney = GetParentalPowerOfAttorney(userCardPowerOfAttorney, formatId);

            var representative = context.GetObject<StaffEmployee>(representativeID);
            var signer = context.GetObject<StaffEmployee>(signerID);
            var parent = context.GetObject<PowerOfAttorney>(parentalPowerOfAttorney);

            var powerOfAttorney = PowerOfAttorneyService.RetrustPowerOfAttorney(powerOfAttorneyData, representative, signer, parent, PowerOfAttorneyHandlingFlags.SupportDistributedRegistryFederalTaxService);

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
            PowerOfAttorneyRevocationData revocationData = poaFormatId == PowerOfAttorneyFNSDOVEL502Data.FormatId
                ? RequestRevocationPowerOfAttorney502Internal(context, userCardPowerOfAttorney, revocationType, revocationReason)
                : RequestRevocationPowerOfAttorneyStdInternal(context, userCardPowerOfAttorney, revocationType, revocationReason);

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

        private PowerOfAttorneyRevocationData RequestRevocationPowerOfAttorneyStdInternal(ObjectContext context, UserCardPowerOfAttorney userCardPowerOfAttorney, PowerOfAttorneyRevocationType revocationType, string revocationReason)
        {
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

            return revocationData;
        }

        private PowerOfAttorneyRevocationData RequestRevocationPowerOfAttorney502Internal(ObjectContext context, UserCardPowerOfAttorney userCard, PowerOfAttorneyRevocationType revocationType, string revocationReason)
        {
            var isRetrusted = userCard.IsRetrusted();

            PowerOfAttorneyFNSDOVEL502RevocationData revocationData = new PowerOfAttorneyFNSDOVEL502RevocationData
            {
                RecipientID = userCard.GenTaxAuthPOASubmit,
                FinalRecipientID = userCard.GenFinalRecipientTaxID,
                SenderID = GetSenderID(context, userCard, isRetrusted),
                Document = CreateRevocationDocument(context, userCard, revocationReason, isRetrusted)
            };

            return revocationData;
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

            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
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

            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
                return userCardPowerOfAttorney.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual).GetObjectId();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private string GetPrincipalInn(UserCardPowerOfAttorney userCardPowerOfAttorney, Guid formatId)
        {
            if (formatId == PowerOfAttorneyFNSDOVBBData.FormatId)
                return userCardPowerOfAttorney.PrincipalOrganization.Value?.INN;

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
                return userCardPowerOfAttorney.GenEntityPrinINN ?? userCardPowerOfAttorney.GenEntityPrincipal.Value?.INN.AsNullable();

            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
                return userCardPowerOfAttorney.GenEntityPrinINN;

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

            if (formatId == PowerOfAttorneyEMCHDData.FormatId)
                return userCardPowerOfAttorney.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo).GetObjectId();

            if (formatId == PowerOfAttorneyFNSDOVEL502Data.FormatId)
                return userCardPowerOfAttorney.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo).GetObjectId();

            throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidPowerOfAttorneyFormat, formatId));
        }

        private string GetSenderID(ObjectContext context, UserCardPowerOfAttorney userCard, bool isRetrusted)
        {
            var principalType = userCard.GenPrincipalType;
            //     Значение: для организаций – девятнадцатиразрядный код (ИНН и КПП организации)
            //     для физических лиц – двенадцатиразрядный код (ИНН физического лица, при наличии.
            //     При отсутствии ИНН – последовательность из двенадцати нулей)    
            //     НО!!! В примере разрешены только юр. лица
            if (principalType.Value == GenPrincipalTypes.entity)
            {
                if (isRetrusted)
                {
                    var originalPOAUserCard = GetOriginalPowerOfAttorneyUserCard(context, userCard);
                    return $"{originalPOAUserCard.GenEntityPrinINN}{originalPOAUserCard.GenEntityPrinKPP}";
                }
                else
                {
                    return $"{userCard.GenEntityPrinINN}{userCard.GenEntityPrinKPP}";
                }
            }

            throw new ArgumentOutOfRangeException(nameof(principalType));
        }

        private UserCardPowerOfAttorney GetOriginalPowerOfAttorneyUserCard(ObjectContext context, UserCardPowerOfAttorney userCard)
        {
            return GetUserCardPowerOfAttorney(context, userCard.GenOriginalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetOriginalPowerOfAttorneyUserCard));
        }

        private UserCardPowerOfAttorney GetUserCardPowerOfAttorney(ObjectContext objectContext, Document document)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document));
            return new UserCardPowerOfAttorney(document, objectContext);
        }

        private RevocationDocument CreateRevocationDocument(ObjectContext context, UserCardPowerOfAttorney userCard, string revocationReason, bool isRetrusted)
        {
            return new RevocationDocument()
            {
                TaxAuthorityCode = userCard.GenTaxAuthPOASubmit,
                Revocation = GetRevocationInfo(context, userCard, revocationReason, isRetrusted),
                Principal = GetPrincipalInfo(context, isRetrusted, userCard),
                Signer = new SignerInfo
                {
                    Fio = GetFIO(userCard.Signer.GetValueOrThrow(Resources.Error_EmptySigner))
                }
            };

        }

        private RevocationInfo GetRevocationInfo(ObjectContext context, UserCardPowerOfAttorney userCard, string revocationReason, bool isRetrusted)
        {
            return new RevocationInfo
            {
                InternalPowerOfAttorneyNumber = userCard.GenInternalPOANumber,
                PowerOfAttorneyStartDate = userCard.GenPoaDateOfIssue ?? throw new ApplicationException(Resources.Error_PoaDateOfIssueIsEmpty),
                PowerOfAttorneyNumber = userCard.MainInfoRowId,
                RevocationDate = DateTime.UtcNow,
                RevocationReason = revocationReason
            };
        }

        private PrincipalInfo GetPrincipalInfo(ObjectContext context, bool isRetrust, UserCardPowerOfAttorney userCard)
        {            
            if (isRetrust)
            {
                var principalInfo = GetPrincipalInfo(GetOriginalPowerOfAttorneyUserCard(context, userCard));
                var documentKindCode = userCard.PrincipalWithoutPowerOfAttorneyIndividualDocumentKindCode;
                string authIssuer;
                if (documentKindCode == DocumentKindTypes.Passport)
                {
                    authIssuer = userCard?.GenAuthIssCEOIDDoc ?? throw new ArgumentException(Resources.Error_AuthIssIDDoc);
                }
                else
                {
                    authIssuer = userCard?.GenAuthIssCEOIDDoc;
                }                
                principalInfo.RetrustPrincipal = new RetrustPrincipalInfo
                {
                    Organization = new OrganizationInfo
                    {
                        Name = userCard.EntityWithoutPOA.GetValueOrThrow(Resources.Error_EmptyPrincipalWithoutPowerOfAttorneyIndividual).Name,
                        Inn = userCard.INNEntityWithoutPOA,
                        Kpp = userCard.KPPEntityWithoutPOA
                    },
                    Individual = new IndividualInfo1
                    {
                        Snils = userCard.GenCeoSNILS,
                        BirthDate = userCard.GenCeoDateOfBirth,
                        IdentityCard = new IdentityCardInfo
                        {
                            DocumentKindCode = ConvertDocumentKind(documentKindCode.Value),
                            DocumentSerialNumber = userCard.GenSerNumCEOIDDoc,
                            IssueDate = userCard.GenDateIssCEOIDDoc,
                            Issuer = authIssuer
                        }
                    }
                };
                return principalInfo;
            }

            return GetPrincipalInfo(userCard);
        }

        private PrincipalInfo GetPrincipalInfo(UserCardPowerOfAttorney userCard)
        {
            var executiveType = userCard.GenExecutiveBodyType.Value;

            var principalWithoutInfo = new PrincipalWithoutPowerOfAttorneyInfo
            {
                Position = userCard.GenCeoPosition
            };

            if (executiveType == ExecutiveBodyType.entity)
            {
                principalWithoutInfo.Organization = new OrganizationInfo
                {
                    Name = userCard.EntityWithoutPOA.GetValueOrThrow(Resources.Error_EmptyPrincipalWithoutPowerOfAttorneyIndividual).Name,
                    Inn = userCard.INNEntityWithoutPOA,
                    Kpp = userCard.KPPEntityWithoutPOA
                };
            }

            var principalInfo = new PrincipalInfo
            {
                RussianCompany = new RussianCompanyInfo
                {
                    Name = userCard.GenEntityPrincipal.GetValueOrThrow(Resources.Error_EmptyPrincipalOrganization).Name,
                    Inn = userCard.GenEntityPrinINN,
                    Kpp = userCard.GenEntityPrinKPP,
                    Ogrn = userCard.GenEntPrinOGRN,
                    PrincipalWithoutPowerOfAttorney = principalWithoutInfo
                }
            };

            return principalInfo;
        }

        private FIO GetFIO(StaffEmployee employee)
        {
            return new FIO
            {
                /* Фамилия (Обязательный) */
                LastName = employee.LastName,
                /* Имя (Обязательный) */
                FirstName = employee.FirstName,
                /* Отчество */
                MiddleName = employee?.MiddleName
            };
        }

        private DocumentKindCode ConvertDocumentKind(DocumentKindTypes documentKind)
        {
            switch (documentKind)
            {
                case DocumentKindTypes.ForeignPassport:
                    return DocumentKindCode.ForeignPassport;
                case DocumentKindTypes.Passport:
                    return DocumentKindCode.Passport;
            }

            throw new ArgumentOutOfRangeException(nameof(documentKind));
        }

        private IPowerOfAttorneyService PowerOfAttorneyService => currentObjectContextProvider.GetOrCreateCurrentSessionContext().ObjectContext.GetService<IPowerOfAttorneyService>();
    }
}