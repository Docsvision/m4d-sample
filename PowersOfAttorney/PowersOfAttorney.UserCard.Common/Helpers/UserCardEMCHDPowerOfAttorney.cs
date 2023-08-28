using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;
using PowersOfAttorney.UserCard.Common;
using PowersOfAttorney.UserCard.Common.Helpers;
using System;

namespace PowersOfAttorney.Scripts
{
    public class UserCardEMCHDPowerOfAttorney
    {
        private readonly UserCardPowerOfAttorney userCardPowerOfAttorney;
        private readonly Document document;

        public UserCardEMCHDPowerOfAttorney(ObjectContext context, Document document)
        {
            this.Context = context;
            this.userCardPowerOfAttorney = new UserCardPowerOfAttorney(document, context);
            this.document = document;
        }

        private ObjectContext Context { get; }

        public Guid Id => document.GetObjectId();

        public Guid? PowerOfAttorneyCardId
        {
            get => userCardPowerOfAttorney.PowerOfAttorneyCardId;
            set => userCardPowerOfAttorney.PowerOfAttorneyCardId = value;
        }

        public UserCardPowerOfAttorney UserCardPowerOfAttorney => userCardPowerOfAttorney;

        public PowerOfAttorneyData PowerOfAttorneyData => userCardPowerOfAttorney.ConvertToPowerOfAttorneyEMCHDData(this.Context);

        public string PrincipalInn =>
            userCardPowerOfAttorney.GenEntityPrinINN ?? userCardPowerOfAttorney.GenEntityPrincipal.Value?.INN.AsNullable();

        public StaffEmployee Signer => userCardPowerOfAttorney.GenCeo.GetValueOrThrow(nameof(userCardPowerOfAttorney.GenCeo));

        public StaffEmployee Representative => userCardPowerOfAttorney.GenRepresentative.GetValueOrThrow(nameof(userCardPowerOfAttorney.GenRepresentative));

        public PowerOfAttorney ParentalPowerOfAttorney
        {
            get
            {
                if (userCardPowerOfAttorney.GenParentalPowerOfAttorneyUserCard.HasValue)
                    return userCardPowerOfAttorney.GenParentalPowerOfAttorney;

                return userCardPowerOfAttorney.GenOriginaPowerOfAttorney;
            }
        }

        public static UserCardEMCHDPowerOfAttorney GetUserCard(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            var document = context.GetObject<Document>(powerOfAttorneyUserCardId);

            if (document == null)
            {
                throw new Exception(string.Format(Resources.Error_UserCardNotFound, powerOfAttorneyUserCardId));
            }

            return new UserCardEMCHDPowerOfAttorney(context, document);
        }

        public static PowerOfAttorney GetPowerOfAttorneyCard(ObjectContext context, Guid powerOfAttorneyUserCardId)
        {
            var userCardPowerOfAttorney = GetUserCard(context, powerOfAttorneyUserCardId);

            if (userCardPowerOfAttorney.PowerOfAttorneyCardId.GetValueOrDefault() == Guid.Empty)
            {
                throw new Exception(Resources.Error_PoaIDNotFoundInUserCard);
            }

            var powerOfAttorneyId = userCardPowerOfAttorney.PowerOfAttorneyCardId.Value;
            return context.GetObject<PowerOfAttorney>(powerOfAttorneyId);
        }
    }
}
