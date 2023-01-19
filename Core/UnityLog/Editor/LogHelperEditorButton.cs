using UnityEditor;

namespace Utils.Core
{
	public class LogHelperEditorButton : Editor
	{
		[MenuItem("Utils/Open Editor Log")]
		private static void Open()
		{
			LogHelper.OpenLog();
		}
	}
}