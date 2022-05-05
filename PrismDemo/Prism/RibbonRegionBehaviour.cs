using Infragistics.Windows.Ribbon;
using Prism.Regions;
using PrismDemo.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismDemo.Prism
{
    public class RibbonRegionBehaviour: RegionBehavior
    {
        public const string BehavoiourKey = "RibbonRegionBehaviour";

        public const string RibbonTabRegionName = "RibbonTabRegion";


        protected override void OnAttach()
        {
            if (Region.Name == "ContentRegion")
            {
                Region.ActiveViews.CollectionChanged += Views_CollectionChanged;
            }
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var tabList = new List<RibbonTabItem>();

                foreach (var view in e.NewItems)
                {
                    foreach (var attribute in GetCustomAttributes<RibbonTabAttribute>(view.GetType()))
                    {
                        var tab = Activator.CreateInstance(attribute.Type) as RibbonTabItem;

                        if (tab is ISupportDataContext && view is ISupportDataContext)
                        {
                            ((ISupportDataContext)tab).DataContext = ((ISupportDataContext)view).DataContext;
                        }
                        tabList.Add(tab);
                    }

                    tabList.ForEach(x => Region.RegionManager.Regions[RibbonTabRegionName].Add(x));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var views = Region.RegionManager.Regions[RibbonTabRegionName].Views.ToList();
                views.ForEach(x => Region.RegionManager.Regions[RibbonTabRegionName].Remove(x));
            }
        }

        static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }
    }
}
