using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CID2
{
    /// <summary>
    /// Interaction logic for EditTask.xaml
    /// </summary>
    public partial class EditTask : Window
    {
        public ComplaintTask Task { get; set; }
        public bool save { get; set; }

        public EditTask()
        {
            InitializeComponent();
            save = false;
        }

        public EditTask(ComplaintTask task)
        {
            InitializeComponent();
            Task = task;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ListCollectionView lst = MainWindow.AllTasks;
            int x = Task.Task.Type;
            if (x == 1 || x == 3) lst.Filter = (y) => ((TaskName)y).Type == 1 || ((TaskName)y).Type == 3;
            else if (x == 4) lst.Filter = (y) => ((TaskName)y).Type == 4;
            cboTasks.ItemsSource = lst;

            cboTasks.SelectedValue = Task.Task.ID;
            TaskDate.SelectedDate = Task.TaskDate;
            NotesBox.Text = Task.Details;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (Task.EnteredBy.ID != MainWindow.thisUser.ID)
            {
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("By completing this edit you will take ownership of this task. Continue?", "Attention", button);
                Task.EnteredBy = MainWindow.thisUser;
                save = (result == MessageBoxResult.Yes);
            }
            else save = true;

            Task.Task = (TaskName)cboTasks.SelectedItem;
            Task.TaskDate = (DateTime)TaskDate.SelectedDate;
            Task.Details = NotesBox.Text;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        { Close(); }
    }
}
