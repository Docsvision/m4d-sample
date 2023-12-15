using DocsVision.BackOffice.WebClient.State;
using DocsVision.Platform.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowersOfAttorneyServerExtension.Models
{
    public class PowerOfAttorneySignatureDataResponse
    {
        public Guid CardId { get; set; }

        /// <summary>
        /// Gets or sets card kind id
        /// </summary>
        public Guid? KindId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets state identifier
        /// </summary>
        public StateModel State
        {
            get;
            set;
        }

        /// <summary>
        /// Card edit operations
        /// </summary>
        public List<OperationModel> Operations { get; set; } = new List<OperationModel>();

        /// <summary>
        /// Power of attrorney id
        /// </summary>
        public Guid PowerOfAttorneyId
        {
            get;
            set;
        }

        /// <summary>
        /// Power of attrorney content in Base64 format
        /// </summary>
        public string PowerOfAttorneyContent { get; set; }

        /// <summary>
        /// Power of attrorney file name
        /// </summary>
        public string PowerOfAttorneyFileName { get; set; }
    }
}
