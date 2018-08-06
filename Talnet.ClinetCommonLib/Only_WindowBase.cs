using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Talent.ClientCommonLib
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Talent.ClientCommonLib"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Talent.ClientCommonLib;assembly=Talent.ClientCommonLib"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:Only_WindowBase/>
    ///
    /// </summary>
    public class Only_WindowBase : Window, IDisposable
    {
        static Only_WindowBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Only_WindowBase), new FrameworkPropertyMetadata(typeof(Only_WindowBase)));
        }

        public UIElement ChildWindowContent
        {
            get { return (UIElement)GetValue(ChildWindowContentProperty); }
            set { SetValue(ChildWindowContentProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ChildWindowContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildWindowContentProperty =
            DependencyProperty.Register("ChildWindowContent", typeof(UIElement), typeof(Only_WindowBase), new PropertyMetadata(default(UIElement)));

        public virtual void Dispose()
        {
            try
            {
                GC.SuppressFinalize(this);
                GC.Collect();
            }
            catch (System.StackOverflowException so)
            {
                string sst = so.Message;
            }
            catch (Exception ex)
            {
                string ss = ex.Message;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            base.OnClosed(e);
        }
    }
}
