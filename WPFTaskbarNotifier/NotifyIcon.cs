using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Drawing = System.Drawing;
using Forms = System.Windows.Forms;


namespace WPFTaskbarNotifier
{
	[ContentProperty("Text")]
	[DefaultEvent("MouseDoubleClick")]
	public class NotifyIcon : FrameworkElement, IAddChild
	{
		#region Events

		public static readonly RoutedEvent MouseDownEvent = EventManager.RegisterRoutedEvent(
			"MouseDown", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

		public static readonly RoutedEvent MouseUpEvent = EventManager.RegisterRoutedEvent(
			"MouseUp", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

		public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent(
			"MouseClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

		public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
			"MouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

		#endregion

		#region Dependency properties

		public static readonly DependencyProperty BalloonTipIconProperty =
			DependencyProperty.Register("BalloonTipIcon", typeof(BalloonTipIcon), typeof(NotifyIcon));

		public static readonly DependencyProperty BalloonTipTextProperty =
			DependencyProperty.Register("BalloonTipText", typeof(string), typeof(NotifyIcon));

		public static readonly DependencyProperty BalloonTipTitleProperty =
			DependencyProperty.Register("BalloonTipTitle", typeof(string), typeof(NotifyIcon));

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotifyIcon));

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(NotifyIcon));

		#endregion

	    bool initialized;

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			InitializeNotifyIcon();
			Dispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
		}

		private void OnDispatcherShutdownStarted(object sender, EventArgs e)
		{
			InternalIcon.Dispose();
		}

		private void InitializeNotifyIcon()
		{
            InternalIcon = new Forms.NotifyIcon { Text = Text, Icon = FromImageSource(Icon), Visible = FromVisibility(Visibility) };
            Bitmap bitmap = new Bitmap(16, 16);//, System.Drawing.Imaging.PixelFormat.Max);
		    Font m_font = new Font("Helvetica", 8);
            Drawing.Brush brush = new SolidBrush(Drawing.Color.Black);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.DrawString("0", m_font, brush, 0, 0);


            InternalIcon.Icon = Drawing.Icon.FromHandle(bitmap.GetHicon());

            InternalIcon.MouseDown += OnMouseDown;
            InternalIcon.MouseUp += OnMouseUp;
            InternalIcon.MouseClick += OnMouseClick;
            InternalIcon.MouseDoubleClick += OnMouseDoubleClick;
            InternalIcon.MouseMove += OnMouseMove;

			initialized = true;
		}

        private void OnMouseMove(object sender, Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseDoubleClickEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, MouseButton.Left));

        }

		private void OnMouseDown(object sender, Forms.MouseEventArgs e)
		{
			OnRaiseEvent(MouseDownEvent, new MouseButtonEventArgs(
				InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		private void OnMouseUp(object sender, Forms.MouseEventArgs e)
		{
			if (e.Button == Forms.MouseButtons.Right)
			{
				ShowContextMenu();
			}
			OnRaiseEvent(MouseUpEvent, new MouseButtonEventArgs(
				InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		public void ShowContextMenu()
		{
			if (ContextMenu != null)
			{
				ContextMenuService.SetPlacement(ContextMenu, PlacementMode.MousePoint);
				ContextMenu.IsOpen = true;
			}
		}

		private void OnMouseDoubleClick(object sender, Forms.MouseEventArgs e)
		{
			OnRaiseEvent(MouseDoubleClickEvent, new MouseButtonEventArgs(
				InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		private void OnMouseClick(object sender, Forms.MouseEventArgs e)
		{
			OnRaiseEvent(MouseClickEvent, new MouseButtonEventArgs(
				InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
		}

		private void OnRaiseEvent(RoutedEvent handler, MouseButtonEventArgs e)
		{
			e.RoutedEvent = handler;
			RaiseEvent(e);
		}

		public BalloonTipIcon BalloonTipIcon
		{
			get { return (BalloonTipIcon)GetValue(BalloonTipIconProperty); }
			set { SetValue(BalloonTipIconProperty, value); }
		}

		public string BalloonTipText
		{
			get { return (string)GetValue(BalloonTipTextProperty); }
			set { SetValue(BalloonTipTextProperty, value); }
		}

		public string BalloonTipTitle
		{
			get { return (string)GetValue(BalloonTipTitleProperty); }
			set { SetValue(BalloonTipTitleProperty, value); }
		}

		public ImageSource Icon
		{
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

	    public Forms.NotifyIcon InternalIcon { get; set; }

	    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (initialized)
			{
				switch (e.Property.Name)
				{
					case "Icon":
						InternalIcon.Icon = FromImageSource(Icon);
						break;
					case "Text":
						InternalIcon.Text = Text;
						break;
					case "Visibility":
						InternalIcon.Visible = FromVisibility(Visibility);
						break;
				}
			}
		}

		public void ShowBalloonTip(int timeout)
		{
			InternalIcon.BalloonTipTitle = BalloonTipTitle;
			InternalIcon.BalloonTipText = BalloonTipText;
			InternalIcon.BalloonTipIcon = (Forms.ToolTipIcon)BalloonTipIcon;
			InternalIcon.ShowBalloonTip(timeout);
		}

		public void ShowBalloonTip(int timeout, string tipTitle, string tipText, BalloonTipIcon tipIcon)
		{
			InternalIcon.ShowBalloonTip(timeout, tipTitle, tipText, (Forms.ToolTipIcon)tipIcon);
		}

		public event MouseButtonEventHandler MouseClick
		{
			add { AddHandler(MouseClickEvent, value); }
			remove { RemoveHandler(MouseClickEvent, value); }
		}

		public event MouseButtonEventHandler MouseDoubleClick
		{
			add { AddHandler(MouseDoubleClickEvent, value); }
			remove { RemoveHandler(MouseDoubleClickEvent, value); }
		}

		public event MouseButtonEventHandler MouseDown
		{
			add { AddHandler(MouseDownEvent, value); }
			remove { RemoveHandler(MouseDownEvent, value); }
		}

		public event MouseButtonEventHandler MouseUp
		{
			add { AddHandler(MouseUpEvent, value); }
			remove { RemoveHandler(MouseUpEvent, value); }
		}

		#region IAddChild Members

		void IAddChild.AddChild(object value)
		{
			throw new InvalidOperationException();
		}

		void IAddChild.AddText(string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			Text = text;
		}

		#endregion

		#region Conversion members

		private static Icon FromImageSource(ImageSource icon)
		{
			if (icon == null)
			{
				return null;
			}
			Uri iconUri = new Uri(icon.ToString());
			return new Icon(Application.GetResourceStream(iconUri).Stream);
		}

		private static bool FromVisibility(Visibility visibility)
		{
			return visibility == Visibility.Visible;
		}

		private MouseButton ToMouseButton(Forms.MouseButtons button)
		{
			switch (button)
			{
				case Forms.MouseButtons.Left:
					return MouseButton.Left;
				case Forms.MouseButtons.Right:
					return MouseButton.Right;
				case Forms.MouseButtons.Middle:
					return MouseButton.Middle;
				case Forms.MouseButtons.XButton1:
					return MouseButton.XButton1;
				case Forms.MouseButtons.XButton2:
					return MouseButton.XButton2;
			}
			throw new InvalidOperationException();
		}

		#endregion
	}
}