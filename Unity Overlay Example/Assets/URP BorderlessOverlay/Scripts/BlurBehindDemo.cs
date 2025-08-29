using System;
using System.Runtime.InteropServices;

namespace BlurBehindDemo
{
	public enum AccentState
	{
		ACCENT_DISABLED = 0,
		ACCENT_ENABLE_GRADIENT = 1,
		ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
		ACCENT_ENABLE_BLURBEHIND = 3,
		ACCENT_INVALID_STATE = 4
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct AccentPolicy
	{
		public AccentState AccentState;
		public int AccentFlags;
		public int GradientColor;
		public int AnimationId;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct WindowCompositionAttributeData
	{
		public WindowCompositionAttribute Attribute;
		public IntPtr Data;
		public int SizeOfData;
	}

	internal enum WindowCompositionAttribute
	{
		// ...
		WCA_ACCENT_POLICY = 19
		// ...
	}

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public class CompositeWindow
	{
		[DllImport("user32.dll")]
		internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

		public void SetBlurEnabled(IntPtr windowHandler, AccentState targetState)
		{
			EnableBlur(windowHandler, targetState);
		}

		internal void EnableBlur(IntPtr windowHandler, AccentState targetState)
		{
			//var windowHelper = new WindowInteropHelper(this);

			var accent = new AccentPolicy();
			accent.AccentState = targetState;
			accent.GradientColor = 0x00000000;
			var accentStructSize = Marshal.SizeOf(accent);

			var accentPtr = Marshal.AllocHGlobal(accentStructSize);
			Marshal.StructureToPtr(accent, accentPtr, false);

			var data = new WindowCompositionAttributeData();
			data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
			data.SizeOfData = accentStructSize;
			data.Data = accentPtr;

			SetWindowCompositionAttribute(windowHandler, ref data);

			Marshal.FreeHGlobal(accentPtr);
		}


	}
}