using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using DocsVision.Platform.ObjectModel;
using PowersOfAttorneyServerExtension.Helpers;
using System;

namespace PowersOfAttorney.Scripts
{
    internal class UserCardEMCHDPOA : IUserCardPOA
    {
        private readonly UserCardPowerOfAttorney userCardPowerOfAttorney;
        private readonly Document document;

        public UserCardEMCHDPOA(ObjectContext context, Document document)
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

        public PowerOfAttorneyData PowerOfAttorneyData => userCardPowerOfAttorney.ConvertToPowerOfAttorneyEMCHDData(this.Context);

        public string PrincipalInn => 
            userCardPowerOfAttorney.GenEntityPrinINN ?? userCardPowerOfAttorney.GenEntityPrincipal.Value?.INN.AsNullable();

        public StaffEmployee Signer => userCardPowerOfAttorney.GenCeo.GetValueOrThrow(nameof(userCardPowerOfAttorney.GenCeo));

        public StaffEmployee Representative => userCardPowerOfAttorney.GenRepresentative.GetValueOrThrow(nameof(userCardPowerOfAttorney.GenRepresentative));

        public PowerOfAttorney ParentalPowerOfAttorney => userCardPowerOfAttorney.ParentalPowerOfAttorney;
    }
}
