﻿#pragma checksum "..\..\..\..\Vk\SendToVKWnd.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "04CC80276F014AFD4A12924768E0CACA11751617"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WPFLocalization;


namespace LaserWar.Vk {
    
    
    /// <summary>
    /// SendToVKWnd
    /// </summary>
    public partial class SendToVKWnd : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblUser_ID;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblUser_Photo;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblUser_Name;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblUser_Surname;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblUser_Country;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblUser_City;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbGroups;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\..\Vk\SendToVKWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.WebBrowser brsrGetTToken;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/LaserWar;component/vk/sendtovkwnd.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Vk\SendToVKWnd.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 5 "..\..\..\..\Vk\SendToVKWnd.xaml"
            ((LaserWar.Vk.SendToVKWnd)(target)).Loaded += new System.Windows.RoutedEventHandler(this.SendToVKWnd_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblUser_ID = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.lblUser_Photo = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblUser_Name = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.lblUser_Surname = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.lblUser_Country = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.lblUser_City = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.cmbGroups = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 9:
            this.brsrGetTToken = ((System.Windows.Controls.WebBrowser)(target));
            
            #line 75 "..\..\..\..\Vk\SendToVKWnd.xaml"
            this.brsrGetTToken.Navigated += new System.Windows.Navigation.NavigatedEventHandler(this.brsrGetTToken_Navigated);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

