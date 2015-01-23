using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Input;
using Microsoft.Win32;

namespace CID2
{
    /// <summary>
    /// Interaction logic for PrintShapes.xaml
    /// </summary>
    public partial class PrintShapes : Window
    {
        public Dictionary<int, PrintContainer> printDict = new Dictionary<int, PrintContainer>();
        public List<PrintTableStruct> PrintList;
        public UIElement[] A { get; set; }
        public UIElement[] B { get; set; }

        public PrintShapes(List<PrintTableStruct> printList)
        {
            InitializeComponent();
            PrintList = printList;

            for (int i = 0; i < printList.Count; i++)
            {
                int colorcount = (i > 9) ? i % 10 : i;
                PrintContainer container = new PrintContainer(MainWindow.colors[colorcount], (string)printList[i].printgroup.GetValue(TitleProperty),
                    40, 80, 9, printList[i].printgroup, printList[i].tbirdtext);
                printDict.Add(printDict.Count, container);
                shapes.Children.Add(container);
            }
        }

        public PrintShapes(UIElement[] a, UIElement[]b)
        {
            InitializeComponent();

            A = a;
            B = b;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (A != null && A.Length > 0)
            {
                PrintGrid.RowDefinitions.Clear();
                PrintGrid.Children.Clear();

                for (int i = 0; i < A.Length; i++)
                {
                    btnAddRow_Click(null, null);
                    if (A[i] != null) ((Canvas)PrintGrid.Children[i]).Children.Add(A[i]);
                }
            }

            if (B != null && B.Length > 0)
            {
                shapes.Children.Clear();
                foreach (UIElement y in B)
                { if (y != null) shapes.Children.Add(y); }
            }
        }

        private void canvas_Drop(object sender, DragEventArgs e)
        {
            Canvas can = sender as Canvas;

            double h, w;
            h = can.ActualHeight;
            w = can.ActualWidth;

            if (e.Data.GetData("ShapeContainer") != null)
            {
                PrintContainer x = (PrintContainer)e.Data.GetData("ShapeContainer");

                if (x.Parent != null)
                {
                    if (x.Parent is System.Windows.Controls.StackPanel)
                    {
                        shapes.Children.Remove(x);
                        
                        if (can.Children.Count > 0)
                        {
                            PrintContainer n = (PrintContainer)can.Children[0];
                            PrintContainer o = new PrintContainer(((SolidColorBrush)n.theShape.Fill).Color, n.theBlock.Text, 40, 80, 9, n.printGroup, n.tbirdText);
                            can.Children.Clear();
                            shapes.Children.Add(o);
                        }
                    }
                    else if (x.Parent is System.Windows.Controls.Canvas)
                    {
                        if (can.Children.Count > 0)
                        {
                            Canvas m = x.Parent as Canvas;
                            PrintContainer n = (PrintContainer)can.Children[0];
                            can.Children.Clear();
                            m.Children.Clear();
                            m.Children.Add(n);
                        }
                        else
                        {
                            Canvas q = x.Parent as Canvas;
                            q.Children.Clear();
                        }
                    }
                    PrintContainer y = new PrintContainer(((SolidColorBrush)x.theShape.Fill).Color, x.theBlock.Text, h, w, ((h / 10) * 2.5), x.printGroup, x.tbirdText);
                    can.Children.Add(y);
                }
            }
        }

        private void shapes_Drop(object sender, DragEventArgs e)
        {
            StackPanel stack = sender as StackPanel;
            if (e.Data.GetData("ShapeContainer") != null)
            {
                PrintContainer x = (PrintContainer)e.Data.GetData("ShapeContainer");

                if (x.Parent != null && x.Parent is System.Windows.Controls.Canvas)
                {
                    Canvas can = x.Parent as Canvas;
                    can.Children.Clear();
                    PrintContainer y = new PrintContainer(((SolidColorBrush)x.theShape.Fill).Color, x.theBlock.Text, 40, 80, 9, x.printGroup, x.tbirdText);
                    stack.Children.Add(y);
                }
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas can = sender as Canvas;

            if (can.Children.Count > 0)
            {
                for(int i = 0; i < can.Children.Count; i++)
                {
                    double h = can.ActualHeight;
                    double w = can.ActualWidth;

                    PrintContainer x = (PrintContainer)can.Children[i];
                    x.Height = x.theGrid.Height = x.theShape.Height = h;
                    x.Width = x.theGrid.Width = x.theShape.Width = w;
                    x.theBlock.FontSize = (h / 10) * 2.5;
                }
            }
        }

        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            int index = PrintGrid.RowDefinitions.Count;

            if (index < 12)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                PrintGrid.RowDefinitions.Add(row);

                double count = PrintGrid.Children.Count;

                Canvas can = new Canvas();
                if (((count / 2) * 10) - (((int)(count / 2)) * 10) > 0) can.Background = new SolidColorBrush(Colors.White);
                else can.Background = new SolidColorBrush(Colors.WhiteSmoke);
                can.AllowDrop = true;
                can.Name = "row" + ((int)count).ToString();
                can.SetValue(Grid.RowProperty, (index));
                can.SetValue(Grid.ColumnProperty, 0);
                can.Drop += canvas_Drop;
                can.SizeChanged += canvas_SizeChanged;

                PrintGrid.Children.Add(can);
            }
        }

        private void btnRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            int index = PrintGrid.Children.Count - 1;
            int rowindex = PrintGrid.RowDefinitions.Count - 1;


            if (index > 2 && (((Canvas)PrintGrid.Children[index]).Children.Count == 0))
            {
                PrintGrid.Children.RemoveAt(index);
                PrintGrid.RowDefinitions.RemoveAt(rowindex);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (PrintGrid.Children.Count > 0)
            {
                UIElement[] a = new UIElement[PrintGrid.Children.Count];
                List<TableRowGroup> tabList = new List<TableRowGroup>();
                for(int i = 0; i < PrintGrid.Children.Count; i++)
                {
                    if (PrintGrid.Children[i] is Canvas)
                    {
                        if (((Canvas)PrintGrid.Children[i]).Children.Count > 0)
                        {
                            ((Canvas)PrintGrid.Children[i]).Children.CopyTo(a, i);
                            for (int x = 0; x < ((Canvas)PrintGrid.Children[i]).Children.Count; x++)
                            {
                                if (((Canvas)PrintGrid.Children[i]).Children[x] is PrintContainer)
                                {
                                    PrintContainer n = (PrintContainer)((Canvas)PrintGrid.Children[i]).Children[x];
                                    PrintContainer o = new PrintContainer(((SolidColorBrush)n.theShape.Fill).Color, n.theBlock.Text, 40, 80, 9, n.printGroup, n.tbirdText);
                                    tabList.Add(o.printGroup);
                                }
                            }
                            ((Canvas)PrintGrid.Children[i]).Children.Clear();
                        }
                    }
                }
                PrintGrid.Children.Clear();
                UIElement[] b = new UIElement[shapes.Children.Count];
                shapes.Children.CopyTo(b, 0);
                shapes.Children.Clear();
                PrintDoc doc = new PrintDoc(tabList, PrintList, a, b);
                Close();
                try { doc.ShowDialog(); }
                catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); doc.Close(); return; }
            }
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Clients\Mail\");
            string val = (string)key.GetValue("");

            switch (val)
            {
                case "Microsoft Outlook":
                    //todo
                    break;
                case "Mozilla Thunderbird":
                    GenerateThunderbirdEmail();
                    break;
                default:
                    MessageBox.Show("Only Microsoft Outlook and Mozilla Thunderbird are supported.");
                    break;
            }
        }

        public void GenerateThunderbirdEmail()
        {
            DateTime min = new DateTime(2001, 1, 1);
            string strSubject, strBody, strAttachment, strCommand, strPath;
            strSubject = "test test test";
            bool bExists = false;
            strPath = Environment.GetEnvironmentVariable("PROGRAMFILES");
            strPath += "\\Mozilla Thunderbird\\thunderbird.exe";
            bExists = File.Exists(strPath);

            strBody = "<table border=\"1\" cellpadding=\"10\" style=\"width:816px;\">";

            if (bExists)
            {
                if (PrintGrid.Children.Count > 0)
                {
                    List<TableRowGroup> tabList = new List<TableRowGroup>();
                    for (int i = 0; i < PrintGrid.Children.Count; i++)
                    {
                        if (PrintGrid.Children[i] is Canvas)
                            if (((Canvas)PrintGrid.Children[i]).Children.Count > 0)
                                for (int x = 0; x < ((Canvas)PrintGrid.Children[i]).Children.Count; x++)
                                    if (((Canvas)PrintGrid.Children[i]).Children[x] is PrintContainer)
                                        strBody += ((PrintContainer)((Canvas)PrintGrid.Children[i]).Children[x]).tbirdText;
                    }

                    strBody += "</table>";
                    strBody = strBody.Replace("-", ":");
                    strCommand = " -compose subject='" + strSubject + "',body='" + strBody + "'";

                    Process proc = new Process();
                    proc.StartInfo.FileName = strPath;
                    proc.StartInfo.Arguments = strCommand;
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                }
            }
            else MessageBox.Show("I can't find your email client");
        }
    }
}
