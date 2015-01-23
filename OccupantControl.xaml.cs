using System;

namespace CID2
{
    /// <summary>
    /// Interaction logic for OccupantControl.xaml
    /// </summary>
    public partial class OccupantControl : ContactControlBase
    {
        public OccupantControl()
        {
            InitializeComponent();
        }

        public OccupantControl(Contact controlowner)
        {
            InitializeComponent();
            ControlOwner = controlowner;
            FillCombo(cboPropConState, MainWindow.States);
        }
    }
}
