using DocsVision.BackOffice.ObjectModel;

using System;

namespace PowersOfAttorneyServerExtension.Models
{
    public class RequestRevocationRequest
    {
        /// <summary>
        /// Идентификатор ПКД
        /// </summary>
        public Guid PowerOfAttorneyUserCardId { get; set; }

        /// <summary>
        /// Тип заявления на отзыв
        /// </summary>
        public PowerOfAttorneyRevocationType RevocationType { get; set; }

        /// <summary>
        /// Причина отзыва
        /// </summary>
        public string RevocationReason { get; set; }
    }
}