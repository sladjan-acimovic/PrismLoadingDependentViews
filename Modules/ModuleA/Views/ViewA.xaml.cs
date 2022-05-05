using ModuleA.RibbonTabs;
using PrismDemo.Core;
using System.Windows.Controls;

namespace ModuleA.Views
{
    /// <summary>
    /// Interaction logic for ViewA
    /// </summary>
    /// 
    [RibbonTab(typeof(ViewATab))] //ViewA depends on RibbonTab ViewATab
    public partial class ViewA : UserControl, ISupportDataContext
    {
        public ViewA()
        {
            InitializeComponent();
        }
    }
}
