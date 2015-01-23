using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Controls;

namespace CID2
{
    public class FileBrowserControl
    {
        public Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation.ExplorerBrowser Control { get; set; }
        
        public FileBrowserControl(Microsoft.WindowsAPICodePack.Controls.WindowsPresentationFoundation.ExplorerBrowser control)
        { Control = control; }
    }
}
