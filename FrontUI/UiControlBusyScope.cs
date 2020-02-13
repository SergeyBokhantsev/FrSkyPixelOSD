using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrontUI
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "<Pending>")]
    public class UiControlBusyScope : IDisposable
    {
        private readonly object sender;

        public UiControlBusyScope(object sender)
        {
            this.sender = sender;

            switch (sender)
            {
                case Control ctrl:
                    ctrl.Enabled = false;
                    break;
            }
        }

        public void Dispose()
        {
            switch (sender)
            {
                case Control ctrl:
                    ctrl.Enabled = true;
                    break;
            }
        }
    }
}
