﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace IceCustomProject
{
    class Project : FlavoredProjectBase
    {
        /// <summary>
        /// This is were all QI for interface on the inner object should happen. 
        /// Then set the inner project wait for InitializeForOuter to be called to do
        /// the real initialization
        /// </summary>
        /// <param name="innerIUnknown"></param>
        protected override void SetInnerProject(IntPtr innerIUnknown)
        {
            object objectForIUnknown = null;
            objectForIUnknown = Marshal.GetObjectForIUnknown(innerIUnknown);
            if (base.serviceProvider == null)
            {
                base.serviceProvider = this.Package;
            }
            base.SetInnerProject(innerIUnknown);
            _cfgProvider = objectForIUnknown as IVsProjectFlavorCfgProvider;
        }

        protected override void Close()
        {
            base.Close();
            if (_cfgProvider != null)
            {
                if (Marshal.IsComObject(_cfgProvider))
                {
                    Marshal.ReleaseComObject(_cfgProvider);
                }
                _cfgProvider = null;
            }
        }

        /// <summary>
        ///  By overriding GetProperty method and using propId parameter containing one of 
        ///  the values of the __VSHPROPID2 enumeration, we can filter, add or remove project
        ///  properties. 
        ///  
        ///  For example, to add a page to the configuration-dependent property pages, we
        ///  need to filter configuration-dependent property pages and then add a new page 
        ///  to the existing list. 
        /// </summary>
        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            if (propId == (int)__VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList)
            {
                // Get a semicolon-delimited list of clsids of the configuration-dependent
                // property pages.
                ErrorHandler.ThrowOnFailure(base.GetProperty(itemId, propId, out property));

                // Add the CustomPropertyPage property page.
                property += ';' + typeof(PropertyPage).GUID.ToString("B");

                return VSConstants.S_OK;
            }
            return base.GetProperty(itemId, propId, out property);
        }

        public int CreateProjectFlavorCfg(IVsCfg pBaseProjectCfg, out IVsProjectFlavorCfg ppFlavorCfg)
        {
            IVsProjectFlavorCfg cfg = null;
            if(_cfgProvider != null)
            {
                _cfgProvider.CreateProjectFlavorCfg(pBaseProjectCfg, out cfg);
            }

            ProjectConfiguration configuration = new ProjectConfiguration();
            configuration.Initialize(this, pBaseProjectCfg, cfg);
            ppFlavorCfg = (IVsProjectFlavorCfg)configuration;

            return VSConstants.S_OK;
        }

        protected IVsProjectFlavorCfgProvider _cfgProvider = null;
        internal Microsoft.VisualStudio.Shell.Package Package { get; set; }
    }
}