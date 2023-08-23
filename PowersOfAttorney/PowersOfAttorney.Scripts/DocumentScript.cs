using DevExpress.XtraBars;
using DocsVision.BackOffice.WinForms;

namespace PowersOfAttorney.Scripts
{
    [CardKindScriptClass]
    internal class DocumentScript : ScriptClassBase
    {
        protected POAScriptHelper scriptHelper;
        protected virtual POAScriptHelper POAScriptHelper
        {
            get
            {
                if (scriptHelper == null)
                {
                    scriptHelper = new POAScriptHelper(this.CardControl.ObjectContext, this.CardData.Id);
                }
                return scriptHelper;
            }
        }

        private void CreateEMCHD_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            CreateEMCHDPowerOfAttorney_ItemClick();
        }

        private void CreateEMCHDRetrust_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            CreateEMCHDRetrustPowerOfAttorney_ItemClick();
        }

        private void Sign_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            SignPowerOfAttorney_ItemClick();
        }

        private void ExportWithSignature_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            ExportWithSignaturePowerOfAttorney_ItemClick();
        }

        private void ExportWithoutSignature_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            ExportWithoutSignaturePowerOfAttorney_ItemClick();
        }

        private void MarkAsRevoked_ItemClick(System.Object sender, ItemClickEventArgs e)
        {
            MarkAsRevokedPowerOfAttorney_ItemClick();
        }

        /// <summary>
        /// Create
        /// </summary>
        public virtual void CreateEMCHDPowerOfAttorney_ItemClick()
        {
            POAScriptHelper.CreateEMCHDPowerOfAttorney();
        }

        /// <summary>
        /// Retrust
        /// </summary>
        public virtual void CreateEMCHDRetrustPowerOfAttorney_ItemClick()
        {
            POAScriptHelper.CreateEMCHDRetrustPowerOfAttorney();
        }

        /// <summary>
        /// Sign
        /// </summary>
        public virtual void SignPowerOfAttorney_ItemClick()
        {
            POAScriptHelper.SignPowerOfAttorney();
        }

        /// <summary>
        /// Export with signature
        /// </summary>
        public virtual void ExportWithSignaturePowerOfAttorney_ItemClick()
        {
            POAScriptHelper.Export(withSignature: true);
        }

        /// <summary>
        /// Export without signature
        /// </summary>
        public virtual void ExportWithoutSignaturePowerOfAttorney_ItemClick()
        {
            POAScriptHelper.Export(withSignature: false);
        } 
        
        /// <summary>
        /// Mark as revoked
        /// </summary>
        public virtual void MarkAsRevokedPowerOfAttorney_ItemClick()
        {
            POAScriptHelper.MarkAsRevokedPowerOfAttorney(withChildrenPowerOfAttorney: true);
        }
    }
}
