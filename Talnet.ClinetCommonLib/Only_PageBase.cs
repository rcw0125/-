using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Talent.ClientCommonLib
{
    [System.ComponentModel.DesignTimeVisible(false)]
    public class Only_PageBase : ContentControl, IShowControl
    {
        public string MenuReamk
        { get; set; }

        public Only_PageBase()
        {
            this.DefaultStyleKey = typeof(Only_PageBase);
            this.Loaded += TLH_PageBase_Loaded;            
        }

        void TLH_PageBase_Loaded(object sender, RoutedEventArgs e)
        {
            Only_ViewModelBase model = this.DataContext as Only_ViewModelBase;
            if (model != null)
            {
                model.MenuID = m_menuid;
                model.MenuName = m_menuName;
                model.MenuReamk = MenuReamk;
            }            
        }

        Grid mainGrid;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainGrid = this.GetTemplateChild("mainGrid") as Grid;
        }        

        public static readonly DependencyProperty ChildWindowContentProperty =
            DependencyProperty.Register("ChildWindowContent", typeof(UIElement), typeof(Only_PageBase),
            new PropertyMetadata(default(UIElement)));

        public UIElement ChildWindowContent
        {
            get {
                return (UIElement)GetValue(ChildWindowContentProperty);
            }
            set {
                SetValue(ChildWindowContentProperty,value);
            }
        }

        private string m_menuid, m_menuName;
        public void Run(string menuId, string menuName)
        {
            m_menuid = menuId;
            m_menuName = menuName;
            Only_ViewModelBase model = this.DataContext as Only_ViewModelBase;
            if (model != null)
            {
                model.MenuID = m_menuid;
                model.MenuName = m_menuName;
                model.MenuReamk = MenuReamk;
            }
        }

        public virtual void Dispose()
        {
            this.Loaded -= TLH_PageBase_Loaded;
        }
    }
}
