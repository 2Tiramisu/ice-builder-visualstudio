﻿// **********************************************************************
//
// Copyright (c) 2009-2015 ZeroC, Inc. All rights reserved.
//
// **********************************************************************

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Windows.Forms;
using System.ComponentModel;

namespace ZeroC.IceVisualStudio
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    public class IceOptionsPage : UIElementDialogPage
    {
        [Category("General")]
        [DisplayName("Ice Home")]
        [Description("Ice Home")]
        public String IceHome
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }

        protected override System.Windows.UIElement Child
        {
            get
            {
                IceHomeEditor editor = new IceHomeEditor();
                editor.optionsPage = this;
                editor.Initialize();
                return editor; 
            }
        }

        public override void SaveSettingsToStorage()
        {
            if(!Package.Instance.GetIceHome().Equals(_value))
            {
                Package.Instance.SetIceHome(_value);
            }
        }

        public override void LoadSettingsFromStorage()
        {
            _value = Package.Instance.GetIceHome();
        }

        private String _value;
    }
}
