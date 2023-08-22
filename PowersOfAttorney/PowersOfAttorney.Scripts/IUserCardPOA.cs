using DocsVision.BackOffice.ObjectModel;
using DocsVision.BackOffice.ObjectModel.Services.Entities;
using System;

namespace PowersOfAttorney.Scripts
{
    internal interface IUserCardPOA
    {
        Guid Id { get; }

        Guid? PowerOfAttorneyCardId { get; set; }
        PowerOfAttorneyData PowerOfAttorneyData { get; }

        string PrincipalInn { get; }

        StaffEmployee Signer { get; }

        StaffEmployee Representative { get; }

        PowerOfAttorney ParentalPowerOfAttorney { get; }
    }
}
