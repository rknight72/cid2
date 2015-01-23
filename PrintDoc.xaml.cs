using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
    /// Interaction logic for PrintDoc.xaml
    /// </summary>
    public partial class PrintDoc : Window
    {
        public Grid grid { get; set; }
        public Table printTab { get; set; }
        public PrintDialog printDialog { get; set; }
        public FlowDocument printDoc { get; set; }
        public List<TableRowGroup> tabList { get; set; }
        public List<PrintTableStruct> PrintList;
        public UIElement[] A { get; set; }
        public UIElement[] B { get; set; }

        public PrintDoc()
        { InitializeComponent(); }

        public PrintDoc(List<TableRowGroup> tablist, List<PrintTableStruct> printList, UIElement[] a, UIElement[] b)
        {
            InitializeComponent();

            PrintList = printList;
            tabList = tablist;
            A = a;
            B = b;

            printDoc = new FlowDocument();
            printTab = new Table();
            printTab.Margin = new Thickness(25, 0, 0, 0);
            printDialog = new PrintDialog();
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            printDialog.SelectedPagesEnabled = true;
            PrintCapabilities prnt = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);

            for (int i = 0; i < 6; i++)
            { printTab.Columns.Add(new TableColumn()); }
            printDoc.ColumnWidth = printDoc.PageWidth = printDialog.PrintableAreaWidth;
            printDoc.PageHeight = printDialog.PrintableAreaHeight;
            printTab.SetValue(WidthProperty, printDoc.ColumnWidth);
            printTab.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            printDoc.Blocks.Add(printTab);
            viewer.MinZoom = 10;
            viewer.MaxZoom = 200;
            viewer.Zoom = 50;
            viewer.Document = printDoc;

            foreach(TableRowGroup x in tabList)
            {
                TableRowGroup group = new TableRowGroup();
                TableRow row = new TableRow();
                Paragraph y = new Paragraph(new Run(""));
                TableCell cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(new TableCell(y));
                row.FontSize = 12;
                group.Rows.Add(row);
                printTab.RowGroups.Add(group);

                x.SetValue(WidthProperty, printDoc.ColumnWidth - 10);
                x.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                x.SetValue(MarginProperty, new Thickness(5, 0, 5, 0));
                printTab.RowGroups.Add(x);

                group = new TableRowGroup();
                row = new TableRow();
                y = new Paragraph(new Run(""));
                cell = new TableCell(y);
                cell.ColumnSpan = 6;
                row.Cells.Add(new TableCell(y));
                row.FontSize = 1;
                row.Background = Brushes.WhiteSmoke;
                group.Rows.Add(row);
                printTab.RowGroups.Add(group);
            }
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if (printDialog.ShowDialog() == true)
            {
                PrintCapabilities prnt = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                printDoc.PageWidth = printDoc.ColumnWidth = printDialog.PrintableAreaWidth;
                printDoc.PageHeight = printDialog.PrintableAreaHeight;
                printDoc.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                IDocumentPaginatorSource source = printDoc;
                source.DocumentPaginator.ComputePageCount();
                printDialog.PrintDocument(source.DocumentPaginator, "Print Record");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            printTab.RowGroups.Clear();
            PrintShapes shape = new PrintShapes(A, B);
            try { shape.Show(); }
            catch (Exception Exception) { System.Windows.MessageBox.Show(Exception.Message); shape.Close(); return; }
        }

        private void btnPaper_Click(object sender, RoutedEventArgs e)
        {
            if (printDialog.ShowDialog() == true)
            {
                PrintCapabilities prnt = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
                printDoc.ColumnWidth = printDoc.PageWidth = printDialog.PrintableAreaWidth;
                printDoc.PageHeight = printDialog.PrintableAreaHeight;
            }
        }
    }
}
