using C1.Win.C1Ribbon;
using System;

namespace PolicyClient
{
    public partial class frm_PolicyParameter : C1RibbonForm
    {
        Object o;
        public frm_PolicyParameter(Object ObjParameter)
        {
            InitializeComponent();
            this.o = ObjParameter;

        }

        private void frm_PolicyParameter_Load(object sender, EventArgs e)
        {
            this.propertyGrid1.SelectedObject = o;
        }
    }
}
