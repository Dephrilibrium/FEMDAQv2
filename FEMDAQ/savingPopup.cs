using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FEMDAQ
{
    public partial class SavingPopup : Form
    {
        public SavingPopup(int barMax)
        {
            InitializeComponent();
            pbSaveStatus.Minimum = 0;
            pbSaveStatus.Value = 0;
            SetBarMax(barMax);
        }

        public void CenterToOwner()
        {
            base.CenterToParent();
        }

        public void HideSaving()
        {
            Hide();
        }


        private void SetBarValue(int value)
        {
            if (value >= 0 && value <= pbSaveStatus.Maximum)
                pbSaveStatus.Value = value;
        }


        public void ResetBar()
        {
            SetBarValue(0);
        }


        public void SetBarMax(int barMax)
        {
            if (barMax > 0)
            {
                pbSaveStatus.Style = ProgressBarStyle.Continuous;
                pbSaveStatus.Maximum = barMax;
            }
            else
            {
                pbSaveStatus.Maximum = 0;
                pbSaveStatus.Style = ProgressBarStyle.Marquee;
            }
        }


        public void IncrementBarValueBy(int incrementValue)
        {
            if (pbSaveStatus.Maximum == 0)
                return;

            pbSaveStatus.Value += incrementValue;
            UpdateLabelText(pbSaveStatus.Value, pbSaveStatus.Maximum);
        }


        private void UpdateLabelText(int val, int ofAmount)
        {
            var saveAttachment = (val == 0 ?
                                    "(?/?):" :
                                    string.Format("({0}/{1})", val, ofAmount));
            lSavingFiles.Text = "Saving files " + saveAttachment;
        }
    }
}
