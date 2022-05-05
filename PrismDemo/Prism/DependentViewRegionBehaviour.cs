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
    public class DependentViewRegionBehaviour : RegionBehavior
    {
        public const string BehavoiourKey = "DependentViewRegionBehaviour";

      

        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                var viewList = new List<DependentViewInfo>();

                foreach (var view in e.NewItems)
                {
                    foreach (var attribute in GetCustomAttributes<DependentViewAttribute>(view.GetType()))
                    {
                        var info = CreateDependentView(attribute);

                        if (info.View is ISupportDataContext && view is ISupportDataContext)
                        {
                            ((ISupportDataContext)info.View).DataContext = ((ISupportDataContext)view).DataContext;
                        }
                        viewList.Add(info);
                    }

                    viewList.ForEach(x => Region.RegionManager.Regions[x.TargetRegionName].Add(x.View));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                //To do after caching
            }
        }

        private DependentViewInfo CreateDependentView(DependentViewAttribute attribute)
        {
            var info = new DependentViewInfo();

            info.TargetRegionName = attribute.TargetRegionName;

            if (attribute.Type!=null)
            {
                info.View = Activator.CreateInstance(attribute.Type);
            }


            return info;
        }

        static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }
    }


    internal class DependentViewInfo
    {
        public object View { get; set; }

        public string TargetRegionName { get; set; }
    }
}
