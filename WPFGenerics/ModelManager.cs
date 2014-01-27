using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFGenerics
{
    /// <summary>
    /// Class to manage all viewmodels registered during creation
    /// Registration is done via the IManageable interface
    /// </summary>
    public static class ViewModelManager
    {
        private static List<ViewModelBase> viewModels;
        /// <summary>
        /// Add a viewmodel to the collection
        /// </summary>
        /// <param name="viewmodel">Viewmodel to add</param>
        public static void AddViewModel(ViewModelBase viewmodel)
        {
            if (viewModels == null)
                viewModels = new List<ViewModelBase>();
            // Check if a viewmodel of the same type was already added
            var refviewmodel = viewModels.FirstOrDefault(v => v.GetType() == viewmodel.GetType());
            if (refviewmodel == null)
                viewModels.Add(viewmodel);
            else
                refviewmodel = viewmodel;
        }
        /// <summary>
        /// Get a specified viewmodel using the type given
        /// </summary>
        /// <param name="type">Type of the viewmodel</param>
        /// <returns>The viewmodel found</returns>
        public static ViewModelBase GetViewModel(Type type)
        {
            if (type == null)
                return null;

            ViewModelBase viewModel = null;
            if (viewModels != null)
                viewModel = viewModels.FirstOrDefault(v => v.GetType() == type);

            return viewModel;
        }
        /// <summary>
        /// Save settings of all viewmodels stored
        /// </summary>
        public static void SaveSettings()
        {
            if (viewModels == null) return;
            foreach (var vm in viewModels)
            {
                var manageable = vm as IManageable;
                if (manageable != null)
                    manageable.SaveSettings();
            }
        }
    }
}

