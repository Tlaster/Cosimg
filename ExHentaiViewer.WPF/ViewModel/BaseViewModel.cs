using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExHentaiLib.Common;
using TBase;

namespace ExHentaiViewer.WPF.ViewModel
{
    public class BaseViewModel : ProgressInfo
    {
        public string RequestUrl { get; set; }

    }
}
