using System;

namespace Utils.Core.SerializedSubClass
{
	/// <summary>
	/// An attribute that overrides the name of the type displayed in the SubclassSelector popup.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public sealed class SubClassMenuAttribute : Attribute
	{
		private static readonly char[] separators = new char[] { '/' };
		public string MenuName { get; }
		public int Order { get; }

		public SubClassMenuAttribute(string menuName, int order = 0)
		{
			MenuName = menuName;
			Order = order;
		}

		/// <summary>
		/// Returns the menu name split by the '/' separator.
		/// </summary>
		public string[] GetSplittedMenuName()
		{
			return !string.IsNullOrWhiteSpace(MenuName) ? MenuName.Split(separators, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
		}

		/// <summary>
		/// Returns the display name without the path.
		/// </summary>
		public string GetTypeNameWithoutPath()
		{
			string[] splittedDisplayName = GetSplittedMenuName();
			return (splittedDisplayName.Length != 0) ? splittedDisplayName[splittedDisplayName.Length - 1] : null;
		}
	}
}