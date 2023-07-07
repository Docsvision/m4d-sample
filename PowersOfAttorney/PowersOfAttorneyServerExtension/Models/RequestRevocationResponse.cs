namespace PowersOfAttorneyServerExtension.Models
{
    /// <summary>
    /// Ответ запроса на отзыв доверенности
    /// </summary>
    public class RequestRevocationResponse
    {
        /// <summary>
        /// Содержимое файла отзыва доверенности
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Название файла отзыва доверенности
        /// </summary>
        public string FileName {get;set;}
    }
}