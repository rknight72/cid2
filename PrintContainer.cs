using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CID2
{
    //Added a custom class for showing the shapes, this will be added to a stackpanel
    //from which you can drag these off of.
    public class PrintContainer : Button
    {
        public Grid theGrid { get; set; }
        public Shape theShape { get; set; }
        public TextBlock theBlock { get; set; }
        public string tbirdText { get; set; }
        public TableRowGroup printGroup { get; set; }

        public PrintContainer(Color color, string label, double height, double width, double font, TableRowGroup group, string tbirdtext)
        {
            Height = height;
            Width = width;

            theShape = new Rectangle { Fill = new SolidColorBrush(color) };
            theShape.Height = height;
            theShape.Width = width;
            theShape.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            theShape.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

            theBlock = new TextBlock();
            theBlock.Text = label;
            theBlock.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            theBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            theBlock.FontSize = font;
            theBlock.FontWeight = FontWeights.Bold;
            theBlock.Foreground = new SolidColorBrush(Colors.White);

            theGrid = new Grid();
            theGrid.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            theGrid.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);
            theGrid.Children.Add(theShape);
            theGrid.Children.Add(theBlock);
            Content = theGrid;

            printGroup = group;

            tbirdText = tbirdtext;
        }

        public PrintContainer GetShape()
        { return this; }

        //Overrides the OnMouseMove and checks if the left mousebutton is pressed, if so, it will set a new drop object.
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragDrop.DoDragDrop(this, new DataObject("ShapeContainer", this, true), DragDropEffects.Move);

            base.OnMouseMove(e);
        }
    }
}

