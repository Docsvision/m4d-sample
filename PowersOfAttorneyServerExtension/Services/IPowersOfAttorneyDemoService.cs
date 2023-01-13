using DocsVision.Platform.ObjectModel;

using System;

namespace PowersOfAttorneyServerExtension.Services
{
    /// <summary>
    /// Методы сервиса для работы с СКД
    /// </summary>
    public interface IPowersOfAttorneyDemoService
    {
        /// <summary>
        /// Создаёт СКД с МЧД формата ФНС
        /// </summary>
        Guid CreatePowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId);

        /// <summary>
        /// Создаёт СКД с МЧД формата ФНС в рамках передоверия
        /// </summary>
        Guid CreateRetrustPowerOfAttorney(ObjectContext context, Guid powerOfAttorneyUserCardId);

        /// <summary>
        /// Возвращает идентификатор СКД для переданной ПКД
        /// </summary>
        Guid GetPowerOfAttorneyCardId(ObjectContext context, Guid powerOfAttorneyUserCardId);
    }
}