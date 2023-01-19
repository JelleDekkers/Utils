using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LogHelperEditorButton : Editor
{
	[MenuItem("Utils/Open Editor Log")]
	private static void Open()
	{
		LogHelper.OpenLog();
	}

}
