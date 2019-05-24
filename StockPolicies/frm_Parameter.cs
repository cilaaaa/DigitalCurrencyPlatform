using System.Windows.Forms;

namespace StockPolicies
{
    public partial class frm_Parameter : Form
    {
        public frm_Parameter()
        {
            InitializeComponent();
        }

        internal void setParameter(PolicyParameter policyParameter, StockData.SecurityInfo securityInfo, string policyname)
        {
            this.propertyGrid1.SelectedObject = policyParameter;
            this.Text = string.Format("{0}-{1}-{2}", securityInfo.Code, securityInfo.Name, policyname);
        }
    }
}
