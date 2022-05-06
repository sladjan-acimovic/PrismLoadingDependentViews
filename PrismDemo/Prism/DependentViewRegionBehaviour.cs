using Infragistics.Windows.Ribbon;
using Prism.Regions;
using PrismDemo.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrismDemo.Prism
{
    public class DependentViewRegionBehaviour : RegionBehavior   
    {

        static Dictionary<object, List<DependentViewInfo>> _dependentViewCache = new Dictionary<object, List<DependentViewInfo>>();
        //on ovdje ne stavlja static, ali onda ce svaka razlicita instanca DependentViewRegionBehaviour (za razlicit region) imati svoj cache!


        public const string BehavoiourKey = "DependentViewRegionBehaviour";

      

        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
             

                foreach (var newView in e.NewItems)
                {
                    var viewList = new List<DependentViewInfo>();

                    if (_dependentViewCache.ContainsKey(newView))
                    {
                        viewList = _dependentViewCache[newView];
                    }

                    else
                    {

                        foreach (var attribute in GetCustomAttributes<DependentViewAttribute>(newView.GetType()))
                        {
                            var info = CreateDependentView(attribute);

                            if (info.View is ISupportDataContext && newView is ISupportDataContext)
                            {
                                ((ISupportDataContext)info.View).DataContext = ((ISupportDataContext)newView).DataContext;
                            }
                            viewList.Add(info);
                        }

                            _dependentViewCache.Add(newView, viewList);

                    }

                    viewList.ForEach(x => Region.RegionManager.Regions[x.TargetRegionName].Add(x.View));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldView in e.OldItems)
                {
                    if (_dependentViewCache.ContainsKey(oldView))
                    {
                        _dependentViewCache[oldView].ForEach(x => Region.RegionManager.Regions[x.TargetRegionName].Remove(x.View));

                        if (!ShouldKeepAlive(oldView))
                            _dependentViewCache.Remove(oldView);
                    }
                }
            }
        }

        private bool ShouldKeepAlive(object oldView)
        {
            IRegionMemberLifetime lifetime = GetItemOrContextLifetime(oldView);
            if (lifetime != null)
                return lifetime.KeepAlive;

            RegionMemberLifetimeAttribute lifetimeAttribute = GetItemOrContextLifetimeAttribute(oldView);
            if (lifetimeAttribute != null)
                return lifetimeAttribute.KeepAlive;

            return true;


        }

        private RegionMemberLifetimeAttribute GetItemOrContextLifetimeAttribute(object oldView)
        {
            var lifetimeAttribute = GetCustomAttributes<RegionMemberLifetimeAttribute>(oldView.GetType()).FirstOrDefault();
            if (lifetimeAttribute != null)
                return lifetimeAttribute;

            var frameworkElement = oldView as FrameworkElement;
            if (frameworkElement != null && frameworkElement.DataContext != null)
            {
                var dataContext = frameworkElement.DataContext;
                var contextLifetimeAttribute =
                    GetCustomAttributes<RegionMemberLifetimeAttribute>(dataContext.GetType()).FirstOrDefault();
                return contextLifetimeAttribute;
            }

            return null;
        }

        private IRegionMemberLifetime GetItemOrContextLifetime(object oldView)
        {
            var regionLifetime = oldView as IRegionMemberLifetime;
            if (regionLifetime != null)
                return regionLifetime;

            var frameworkElement = oldView as FrameworkElement;
            if (frameworkElement != null)
                return frameworkElement.DataContext as IRegionMemberLifetime;

            return null;
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
