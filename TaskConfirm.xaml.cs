using System;
using System.Windows;

namespace CID2
{
    /// <summary>
    /// Interaction logic for TaskConfirm.xaml
    /// </summary>
    public partial class TaskConfirm : Window
    {
        public DateTime TaskDate { get; set; }
        public string TaskNotes { get; set; }
        private static DateTime minDate = new DateTime(2000, 1, 1);

        public TaskConfirm()
        {
            InitializeComponent();
            TaskDate = minDate;
            TaskNotes = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        { DateBox.Text = DateTime.Now.ToShortDateString(); }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (DateBox.SelectedDate > minDate)
            {
                TaskDate = (DateTime)DateBox.SelectedDate;
                TaskNotes = NotesBox.Text;
                Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            TaskDate = minDate;
            TaskNotes = "";
            Close();
        }
    }
}
