﻿#pragma checksum "..\..\..\..\..\..\..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "7FDD1AA2587BB8D1BD48A8380C0BA51A9B9DC8A629DB15B72C65236FB5428D5F"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
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
using WPFApp;


namespace WPFApp {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\..\..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.StatusBar StatusBarStatus;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\..\..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBoxIp;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\..\..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border BorderStatus;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\..\..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TextBoxLogged;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemDocumentation;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\..\..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem MenuItemAbout;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\..\..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ListBoxOldMeasures;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFApp;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\..\..\MainWindow.xaml"
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
            this.StatusBarStatus = ((System.Windows.Controls.Primitives.StatusBar)(target));
            return;
            case 2:
            this.TextBoxIp = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.BorderStatus = ((System.Windows.Controls.Border)(target));
            return;
            case 4:
            this.TextBoxLogged = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.MenuItemDocumentation = ((System.Windows.Controls.MenuItem)(target));
            
            #line 20 "..\..\..\..\..\..\..\..\MainWindow.xaml"
            this.MenuItemDocumentation.Click += new System.Windows.RoutedEventHandler(this.MenuItemDocumentation_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.MenuItemAbout = ((System.Windows.Controls.MenuItem)(target));
            
            #line 21 "..\..\..\..\..\..\..\..\MainWindow.xaml"
            this.MenuItemAbout.Click += new System.Windows.RoutedEventHandler(this.MenuItemAbout_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 26 "..\..\..\..\..\..\..\..\MainWindow.xaml"
            ((System.Windows.Controls.TextBox)(target)).TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 8:
            this.ListBoxOldMeasures = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
