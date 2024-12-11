using DocsVision.BackOffice.ObjectModel;
using DocsVision.Platform.ObjectModel;
using System;
using static DocsVision.BackOffice.ObjectModel.PowerOfAttorneyFNSDOVEL502RevocationData;
using static DocsVision.BackOffice.ObjectModel.Services.Entities.PowerOfAttorneyFNSData;
using static PowersOfAttorney.UserCard.Common.Helpers.UserCardPowerOfAttorney;

namespace PowersOfAttorney.UserCard.Common.Helpers
{
    internal class RevocationFNSDOVEL502Converter
    {
        private readonly UserCardPowerOfAttorney userCard;
        private readonly ObjectContext objectContext;
        private readonly string revocationReason;

        private RevocationFNSDOVEL502Converter(UserCardPowerOfAttorney userCard, ObjectContext objectContext, string revocationReason)
        {
            this.userCard = userCard ?? throw new ArgumentNullException(nameof(userCard));
            this.objectContext = objectContext;
            this.revocationReason = revocationReason;
        }

        internal static PowerOfAttorneyRevocationData Convert(UserCardPowerOfAttorney userCard, ObjectContext objectContext, string revocationReason)
        {
            return new RevocationFNSDOVEL502Converter(userCard, objectContext, revocationReason).Convert();
        }

        private PowerOfAttorneyRevocationData Convert()
        {
            var isRetrusted = userCard.IsRetrusted();

            PowerOfAttorneyFNSDOVEL502RevocationData revocationData = new PowerOfAttorneyFNSDOVEL502RevocationData
            {
                RecipientID = userCard.GenTaxAuthPOASubmit,
                FinalRecipientID = string.IsNullOrEmpty(userCard.GenFinalRecipientTaxID) ? userCard.GenTaxAuthPOASubmit : userCard.GenFinalRecipientTaxID,
                SenderID = GetSenderID(isRetrusted),
                Document = CreateRevocationDocument(revocationReason, isRetrusted)
            };

            return revocationData;
        }

        private string GetSenderID(bool isRetrusted)
        {
            //     Значение: для организаций – девятнадцатиразрядный код (ИНН и КПП организации)
            //     для физических лиц – двенадцатиразрядный код (ИНН физического лица, при наличии.
            //     При отсутствии ИНН – последовательность из двенадцати нулей)    
            //     НО!!! В примере разрешены только юр. лица
            if (isRetrusted)
            {
                var originalPOAUserCard = GetOriginalPowerOfAttorneyUserCard();
                return $"{originalPOAUserCard.GenEntityPrinINN}{originalPOAUserCard.GenEntityPrinKPP}";
            }
            else
            {
                return $"{userCard.GenEntityPrinINN}{userCard.GenEntityPrinKPP}";
            }
        }

        private UserCardPowerOfAttorney GetOriginalPowerOfAttorneyUserCard()
        {
            return GetUserCardPowerOfAttorney(objectContext, userCard.GenOriginalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_UnableToGetOriginalPowerOfAttorneyUserCard));
        }

        private UserCardPowerOfAttorney GetUserCardPowerOfAttorney(ObjectContext objectContext, Document document)
        {
            if (document is null)
                throw new ArgumentNullException(nameof(document));
            return new UserCardPowerOfAttorney(document, objectContext);
        }

        private RevocationDocument CreateRevocationDocument(string revocationReason, bool isRetrusted)
        {
            return new RevocationDocument()
            {
                TaxAuthorityCode = userCard.GenTaxAuthPOASubmit,
                Revocation = GetRevocationInfo(revocationReason),
                Principal = GetPrincipalInfo(isRetrusted),
                Signer = new SignerInfo
                {
                    Fio = GetFIO(userCard.Signer.GetValueOrThrow(Resources.Error_EmptySigner))
                }
            };

        }

        private RevocationInfo GetRevocationInfo(string revocationReason)
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

        private PrincipalInfo GetPrincipalInfo(bool isRetrust)
        {
            if (isRetrust)
            {
                var principalInfo = GetPrincipalInfo(GetOriginalPowerOfAttorneyUserCard());
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
            var executiveType = userCard.GenExecutiveBodyType;

            var principalWithoutInfo = new PrincipalWithoutPowerOfAttorneyInfo
            {
                Position = userCard.GenCeoPosition
            };

            if (executiveType.HasValue && executiveType == ExecutiveBodyType.entity)
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

    }
}
