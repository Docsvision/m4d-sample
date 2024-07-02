using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;
using System;
using System.Runtime.Remoting.Contexts;


namespace PowersOfAttorney.UserCard.Common.Helpers
{
    public static class UserCardPowerOfAttorneyExtensions
    {
        public static PowerOfAttorneyRevocationData ConvertToPowerOfAttorneyRevocationData(this UserCardPowerOfAttorney userCard, ObjectContext context, PowerOfAttorneyRevocationType revocationType, string revocationReason)
        {
            return RevocationConverter.Convert(userCard, context, revocationType, revocationReason);
        }

        class RevocationConverter
        {
            private readonly UserCardPowerOfAttorney userCard;
            private readonly ObjectContext objectContext;
            private readonly PowerOfAttorneyRevocationType revocationType;
            private readonly string revocationReason;

            private RevocationConverter(UserCardPowerOfAttorney userCard, ObjectContext objectContext, PowerOfAttorneyRevocationType revocationType, string revocationReason)
            {
                this.userCard = userCard ?? throw new ArgumentNullException(nameof(userCard));
                this.objectContext = objectContext;
                this.revocationReason = revocationReason;
                this.revocationType = revocationType;
            }

            public static PowerOfAttorneyRevocationData Convert(UserCardPowerOfAttorney userCard, ObjectContext objectContext, PowerOfAttorneyRevocationType revocationType, string revocationReason)
            {
                return new RevocationConverter(userCard, objectContext, revocationType, revocationReason).Convert();
            }

            private PowerOfAttorneyRevocationData Convert()
            {
                PowerOfAttorneyRevocationData revocationData = new PowerOfAttorneyRevocationData
                {
                    RevocationReason = revocationReason,
                    RevocationType = revocationType
                };

                switch (revocationType)
                {
                    case PowerOfAttorneyRevocationType.Representative:
                        var representative = userCard.GenRepresentative.GetValueOrThrow(Resources.Error_EmptyRepresentativeIndividual);
                        revocationData.ApplicantInfo = new PowerOfAttorneyRevocationApplicantInfo
                        {
                            ApplicantType = PowerOfAttorneyRevocationApplicantType.Individual,
                            FirstName = representative.FirstName,
                            LastName = representative.LastName,
                            MiddleName = representative.MiddleName,
                            Inn = userCard.GenRepresentativeINN,
                            Snils = userCard.GenRepresentativeSNILS,
                            Phone = userCard.GenReprPhoneNum
                        };
                        break;
                    case PowerOfAttorneyRevocationType.Principal:
                        var ceo = userCard.GenCeo.GetValueOrThrow(Resources.Error_EmptyCeo);

                        revocationData.ApplicantInfo = new PowerOfAttorneyRevocationApplicantInfo
                        {
                            ApplicantType = PowerOfAttorneyRevocationApplicantType.Organization,
                            FirstName = ceo.FirstName,
                            LastName = ceo.LastName,
                            MiddleName = ceo.MiddleName,
                            Inn = userCard.GenCeoIIN,
                            Snils = userCard.GenCeoSNILS,
                            Phone = userCard.GenCeoPhoneNum,
                        };

                        // Для передоверия данные организации требуется брать из родительской доверености
                        var cardWithPrincipalData = userCard.IsRetrusted() ? GetUserCardPowerOfAttorney(objectContext, userCard.ParentalPowerOfAttorneyUserCard.GetValueOrThrow(Resources.Error_ParentalCardNotFound).GetObjectId()) : userCard;

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

            private UserCardPowerOfAttorney GetUserCardPowerOfAttorney(ObjectContext context, Guid documentId)
            {
                var card = context.GetObject<Document>(documentId);
                if (card == null)
                {
                    throw new Exception(string.Format(Resources.Error_UserCardNotFound, documentId));
                }

                return new UserCardPowerOfAttorney(card, context);
            }
        }


    }
}
