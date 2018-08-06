using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Talent.ClientCommonLib
{
    public class Only_ViewModelBase : ViewModelBase, IDataErrorInfo
    {
        public Only_ViewModelBase()
        {
            _childWindow = new ChildWindowViewModel();
            _childWindow.ChildWindowTitle = "数据管理";
            _childWindow.ChildWindowHeight = double.NaN;
            _childWindow.ChildWindowWidth = double.NaN;
            _childWindow.ChildWindowHandelEvnet += _childWindow_ChildWindowHandleEvent;


            _messageWindow = new ChildWindowViewModel();
            _messageWindow.ChildWindowTitle = "系统提示";
            _messageWindow.ChildWindowHeight = double.NaN;
            _messageWindow.ChildWindowWidth = double.NaN;
            _messageWindow.ChildWindowHandelEvnet += _messageWindow_ChildWindowHandleEvent;



        }



        #region 子窗体
        private ChildWindowViewModel _childWindow;
        public ChildWindowViewModel ChildWindow
        {
            get
            {
                return _childWindow;
            }
            set
            {
                _childWindow = value;
                this.RaisePropertyChanged("ChildWindow");
            }
        }
        #endregion


        #region 消息
        private ChildWindowViewModel _messageWindow;
        public ChildWindowViewModel MessageWindow
        {
            get
            {
                return _messageWindow;
            }
            set
            {
                _messageWindow = value;
                this.RaisePropertyChanged("MessageWindow");
            }
        }
        #endregion
        #region tipmsg
        private bool _showTip = false;
        public bool ShowTip
        {
            get
            {
                return _showTip;
            }
            set
            {
                _showTip = value;
                RaisePropertyChanged("ShowTip");
            }
        }

        private string _tipText = "请稍后...";
        public string TipText
        {
            get
            {
                return _tipText;
            }
            set
            {
                _tipText = value;
                RaisePropertyChanged("TipText");
            }
        }
        #endregion
        #region 正在加载
        private bool _showBusy = false;
        public bool ShowBusy
        {
            get
            {
                return _showBusy;
            }
            set
            {
                _showBusy = value;
                RaisePropertyChanged("ShowBusy");
            }
        }

        private string _busyText = "正在加载";
        public string BusyText
        {
            get
            {
                return _busyText;
            }
            set
            {
                _busyText = value;
                RaisePropertyChanged("BusyText");
            }
        }
        #endregion

        public ICommand TextCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand(() =>
                {
                    ChildWindow.ShowChildWindow = true;
                });
            }
        }

        public string MenuID
        { get; set; }

        public string MenuName
        { get; set; }

        public string MenuReamk
        { get; set; }
        public void ShowMessage(string title, string msg, bool showok, bool showcancel, string par,Thickness thickness)
        {
            if (MessageWindow.ShowChildWindow)
                return;
            this.MessageWindow.ChildWindowTitle = title;
            this.MessageWindow.ChildWindowContent = msg;
            this.MessageWindow.ShowMessageOkBnt = showok ? Visibility.Visible : Visibility.Collapsed;
            this.MessageWindow.ShowMessageCancelBnt = showcancel ? Visibility.Visible : Visibility.Collapsed;
            this.MessageWindow.Parameter = par;
            this.MessageWindow.Margin = thickness;
            this.MessageWindow.ShowChildWindow = true;
        }

        public void ShowMessage(string title, string msg, bool showok, bool showcancel, string par)
        {
            ShowMessage(title, msg, showok, showcancel, par, new Thickness());
        }

        public void ShowMessage(string title, string msg, bool showok, bool showcancel)
        {
            ShowMessage(title, msg, showok, showcancel, "");
        }

        #region 弹出窗体事件通知
        void _messageWindow_ChildWindowHandleEvent(object sender, ChildWindowCommandType type)
        {
            _messageWindow.ShowChildWindow = false;
            MessageWindowEvent(type);
        }

        void _childWindow_ChildWindowHandleEvent(object sender, ChildWindowCommandType type)
        {
            ChildWindowEvent(type);
        }

        protected virtual void ChildWindowEvent(ChildWindowCommandType type)
        { }

        protected virtual void MessageWindowEvent(ChildWindowCommandType type)
        { }
        #endregion
        #region 验证
        private string _error = string.Empty;
        public string Error
        {
            get { return _error; }
        }

        public bool ISSucceed
        {
            get
            {
                if (_dataErrors.Count > 0)
                    return false;
                else
                    return true;
            }
        }

        private Dictionary<string, string> _dataErrors = new Dictionary<string, string>();
        public string this[string columnName]
        {
            get
            {
                if (_dataErrors.ContainsKey(columnName))
                    return _dataErrors[columnName];
                else
                    return null;
            }
        }

        protected void AddError(string columnName, string errorMsg)
        {
            _dataErrors[columnName] = errorMsg;
        }

        protected void ClearError(string columnName)
        {
            _dataErrors.Remove(columnName);
        }
        #endregion
    }

    public enum ChildWindowCommandType
    {
        /// <summary>
        /// 确定
        /// </summary>
        OK,
        /// <summary>
        /// 关闭
        /// </summary>
        Close,
        /// <summary>
        /// 取消
        /// </summary>
        Cancel
    }

    public class ChildWindowViewModel : INotifyPropertyChanged
    {
        #region Command事件
        public delegate void ChildWindowHandle(object sender, ChildWindowCommandType type);
        public event ChildWindowHandle ChildWindowHandelEvnet;
        public ICommand ChildWindowCommand
        {
            get
            {
                return new GalaSoft.MvvmLight.Command.RelayCommand<string>((arg) =>
                {
                    switch (arg.ToLower())
                    {
                        case "ok":
                            ChildWindowEvent(ChildWindowCommandType.OK);
                            break;
                        case "cancel":
                            ChildWindowEvent(ChildWindowCommandType.Cancel);
                            break;
                        default:
                            ChildWindowEvent(ChildWindowCommandType.Close);
                            break;
                    }
                });
            }
        }

        protected virtual void ChildWindowEvent(ChildWindowCommandType type)
        {
            if (ChildWindowHandelEvnet != null)
            {
                ChildWindowHandelEvnet(this, type);
            }
        }
        #endregion

        private bool _showChildWindow = false;
        /// <summary>
        /// 是否显示弹出框
        /// </summary>
        public bool ShowChildWindow
        {
            get
            {
                return _showChildWindow;
            }
            set
            {
                _showChildWindow = value;
                PropertyChangedMethod("ShowChildWindow");
            }
        }

        private double _childWindowHeight = 200;
        /// <summary>
        /// 弹出框高度
        /// </summary>
        public double ChildWindowHeight
        {
            get { return _childWindowHeight; }
            set
            {
                _childWindowHeight = value;
                PropertyChangedMethod("ChildWindowHeight");
            }
        }
        private double _childWindowWidth = 300;
        /// <summary>
        /// 弹出框宽度
        /// </summary>
        public double ChildWindowWidth
        {
            get { return _childWindowWidth; }
            set
            {
                _childWindowWidth = value;
                PropertyChangedMethod("ChildWindowWidth");
            }
        }
        private string _childWindowTitle = "系统提示";
        /// <summary>
        /// 弹出框标题
        /// </summary>
        public string ChildWindowTitle
        {
            get { return _childWindowTitle; }
            set
            {
                _childWindowTitle = value;
                PropertyChangedMethod("ChildWindowTitle");
            }
        }
        private object _childWindowContent;
        /// <summary>
        /// 弹出框内容
        /// </summary>
        public object ChildWindowContent
        {
            get
            {
                return _childWindowContent;
            }
            set
            {
                _childWindowContent = value;
                PropertyChangedMethod("ChildWindowContent");
            }
        }

        private Visibility _showMessageOkBnt = Visibility.Visible;
        /// <summary>
        /// 是否显示OK按钮
        /// </summary>
        public Visibility ShowMessageOkBnt
        {
            get { return _showMessageOkBnt; }
            set
            {
                _showMessageOkBnt = value;
                PropertyChangedMethod("ShowMessageOkBnt");
            }
        }
        private Visibility _showMessageCancelBnt = Visibility.Visible;
        /// <summary>
        /// 是否显示取消按钮
        /// </summary>
        public Visibility ShowMessageCancelBnt
        {
            get { return _showMessageCancelBnt; }
            set
            {
                _showMessageCancelBnt = value;
                PropertyChangedMethod("ShowMessageCancelBnt");
            }
        }

        private object _windowDataContext;
        /// <summary>
        /// 弹出内容的数据上下文
        /// </summary>
        public object WindowDataContext
        {
            get
            {
                return _windowDataContext;
            }
            set
            {
                _windowDataContext = value;
                PropertyChangedMethod("WindowDataContext");
            }
        }

        private Thickness margin;
        /// <summary>
        /// 水平和垂直的边距
        /// </summary>
        public Thickness Margin
        {
            get
            {
                return margin;
            }
            set
            {
                margin = value;
                PropertyChangedMethod("Margin");
            }
        }
        public string Parameter
        { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void PropertyChangedMethod(string pname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(pname));
        }
    }
}
