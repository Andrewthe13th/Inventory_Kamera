using System.Drawing;
using System.Runtime.InteropServices;

namespace GenshinGuide
{
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		private int _Left;
		private int _Top;
		private int _Right;
		private int _Bottom;

		public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
		{
		}
		public RECT(int Left, int Top, int Right, int Bottom)
		{
			_Left = Left;
			_Top = Top;
			_Right = Right;
			_Bottom = Bottom;
		}

		public int X
		{
			get => _Left;
			set => _Left = value;
		}
		public int Y
		{
			get => _Top;
			set => _Top = value;
		}
		public int Left
		{
			get => _Left;
			set => _Left = value;
		}
		public int Top
		{
			get => _Top;
			set => _Top = value;
		}
		public int Right
		{
			get => _Right;
			set => _Right = value;
		}
		public int Bottom
		{
			get => _Bottom;
			set => _Bottom = value;
		}
		public int Height
		{
			get => _Bottom - _Top;
			set => _Bottom = value + _Top;
		}
		public int Width
		{
			get => _Right - _Left;
			set => _Right = value + _Left;
		}
		public Point Location
		{
			get => new Point(Left, Top);
			set
			{
				_Left = value.X;
				_Top = value.Y;
			}
		}
		public Size Size
		{
			get => new Size(Width, Height);
			set
			{
				_Right = value.Width + _Left;
				_Bottom = value.Height + _Top;
			}
		}

		public static implicit operator Rectangle(RECT Rectangle) => new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
		public static implicit operator RECT(Rectangle Rectangle) => new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
		public static bool operator ==(RECT Rectangle1, RECT Rectangle2) => Rectangle1.Equals(Rectangle2);
		public static bool operator !=(RECT Rectangle1, RECT Rectangle2) => !Rectangle1.Equals(Rectangle2);

		public override string ToString()
		{
			return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public bool Equals(RECT Rectangle)
		{
			return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
		}

		public override bool Equals(object Object)
		{
			switch (Object)
			{
				case RECT rect:
					return Equals(rect);
				case Rectangle rectangle:
					return Equals(new RECT(rectangle));
			}

			return false;
		}
	}
}
