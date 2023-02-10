using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Networking;
using System.Net.NetworkInformation;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;

namespace Utils.Core.SceneLockTool
{
	[InitializeOnLoad]
	public class SceneLockEditorTool : EditorWindow
	{

		public const string SceneLockUrl = "https://www.knucklehead-studios.com/SceneLockManagement/SceneLockInterfacer.php";
		private readonly int INDENT_SPACE = 16;
		private const int TEXT_FONT_SIZE = 12;
		private static readonly Color32 selectionBackgroundColor = new Color32(38, 38, 38, 255);

		private static EditorWindow window = null;


		private const string GET_REPLY_KEY = "get_reply_key";
		private const string GET_USER_KNOWN = "get_user_known";
		private const string SET_NEW_USER = "set_new_user";
		private const string NEW_USERNAME = "new_username";
		private const string GET_PROJECT_KNOWN = "get_project_known";
		private const string SET_NEW_PROJECT = "set_new_project";
		private const string GET_PROJECT_ID = "get_project_id";
		private const string GET_PROJECT_SCENES = "get_project_scenes";
		private const string SET_PROJECT_SCENES = "set_project_scenes";
		private const string GET_SCENE_LOCK_DATA = "get_scene_lock_data";
		private const string SUBMIT_NEW_SCENE_LOCK = "set_scene_lock";
		private const string RELEASE_SCENE_LOCK = "release_scene_lock";
		private const string PRODUCT_NAME = "product_name";

		private static Dictionary<string, RunningWebRequest> runningRequests = new Dictionary<string, RunningWebRequest>();
		private static List<RunningWebRequest> runningSceneLockRequests = new List<RunningWebRequest>();
		private static Dictionary<string, SceneLockSceneObject> sceneLockDictionary = new Dictionary<string, SceneLockSceneObject>();
		private static List<string> AllScenesInProject = new List<string>();

		private static bool BGInitialized = false;
		private static string previousScenePath = "";
		private static Vector2 scrollPos = new Vector2();
		private static int openedToolbarMenu = 0;

		[SerializeField]
		private static bool hasDonePopup = false;

		private bool initialized = false;
		private bool userIsKnown = false;
		private bool shouldCreateNewUser = false;
		private bool submittedNewUserData = false;
		private bool startedProjectCheck = false;
		private bool projectIsKnown = false;
		private bool shouldCreateNewProject = false;
		private bool submittedNewProjectData = false;
		private bool projectIDIsRequested = false;
		private RunningWebRequest GetProjectIDRequest = null;
		private string newUserName = "username";
		private string deviceID = "";
		private string projectID = "";
		private List<string> projectScenes = new List<string>();
		private bool hasRequestedProjectScenes = false;
		private RunningWebRequest GetProjectScenesRequest = null;
		private string searchText = "";
		private bool allScenesFoldoutIsOpen = false;

		Texture2D bgColor = null;
		Texture2D sceneLockBG = null;
		Texture2D sceneLockOwnedBG = null;
		Texture2D sceneLockNotOwnedBG = null;
		Texture2D sceneMissingBG = null;

		static SceneLockEditorTool()
		{
			BGInitialized = false;

			EditorApplication.playModeStateChanged += PlayModeSceneChangedEvent;

			EditorSceneManager.sceneOpened += SceneLoadEvent;
			EditorSceneManager.sceneClosing += SceneCloseEvent;

			EditorSceneManager.sceneSaving += OnSceneSavingEvent;

			EditorApplication.update += DoFirstInit;
		}

		[MenuItem("Utils/Scene lock tool &s")]
		public static void Open()
		{
			window = GetWindow<SceneLockEditorTool>("Scene lock tool");
			window.autoRepaintOnSceneChange = true;
			window.minSize = new Vector2(850, 600);
			window.Show();
		}

		private void OnEnable()
		{
			ResetVariables();
			RefreshInterface();
		}

		private void OnDisable()
		{
			ResetVariables();
		}

		private void OnGUI()
		{
			if (!initialized)
			{
				RefreshInterface();
			}

			//Check running requests
			CheckRunningRequests();

			if (runningRequests.Count > 0)
			{
				return;
			}

			GUIStyle header = new GUIStyle();
			header.fontStyle = FontStyle.Bold;
			header.fontSize = 14;
			header.normal.textColor = EditorStyles.boldLabel.normal.textColor;

			string[] displayToolbarOptions = new string[] { "All scenes in build settings", "All scenes in project" };

			EditorGUILayout.BeginHorizontal();
			{
				DrawCurrentOpenScene();

				GUILayout.FlexibleSpace();

				EditorGUILayout.BeginVertical();
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Label("Filter: ");
							searchText = GUILayout.TextField(searchText, GUILayout.MinWidth(300));
							if (GUILayout.Button("Clear filter"))
							{
								searchText = "";
							}
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();

						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Refresh interface"))
						{
							EditorGUILayout.EndHorizontal();
							Open();
							RefreshInterface();
							// additional pop to prevent pesky push/pop error
						}
					}
					EditorGUILayout.EndHorizontal();
					openedToolbarMenu = GUILayout.Toolbar(openedToolbarMenu, displayToolbarOptions, GUILayout.MinWidth(window.minSize.x / 3), GUILayout.Height(30));
					GUILayout.Space(20);

					int scenelockOwnedCount = 0;
					int sceneLockNotOwnedCount = 0;
					foreach (var item in sceneLockDictionary)
					{
						if (item.Value.HasSceneLockActive)
						{
							if(item.Value.SceneLock.OwnerDeviceID == GetDeviceID())
								scenelockOwnedCount++;
							else
								sceneLockNotOwnedCount++;
						}
					}

					if(sceneLockDictionary.Count > 0)
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							EditorGUILayout.BeginVertical();
							{
								GUILayout.Label("Current scene locks owned by you: ", header);
								GUILayout.Label("Current scene locks not owned by you: ", header);
								GUILayout.Label("Total scene locks: ", header);
							}
							EditorGUILayout.EndVertical();
							EditorGUILayout.BeginVertical();
							{
								GUILayout.Label(scenelockOwnedCount.ToString(), header);
								GUILayout.Label(sceneLockNotOwnedCount.ToString(), header);
								GUILayout.Label((scenelockOwnedCount + sceneLockNotOwnedCount).ToString(), header);
							}
							EditorGUILayout.EndVertical();
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();
					}
					else
					{
						GUILayout.Label("No active scene locks in " + GetProjectName(), header);
					}
				}
				EditorGUILayout.EndVertical();

			}
			EditorGUILayout.EndHorizontal();

			scrollPos = (Vector2)EditorGUILayout.BeginScrollView((Vector2)scrollPos);
			if (!userIsKnown)
			{
				if (shouldCreateNewUser && !submittedNewUserData)
				{
					GUILayout.Label("First time setup: ");
					GUILayout.Space(10);
					GUILayout.Label("Create new user");
					GUI.enabled = false;
					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label("Device ID: ");
						GUILayout.Label(GetDeviceID());
					}
					EditorGUILayout.EndHorizontal();
					GUI.enabled = true;
					newUserName = Sanitize(EditorGUILayout.TextField(newUserName));

					bool enabled = newUserName != "username" && newUserName.Length > 3 && newUserName.Length < 32;
					GUI.enabled = enabled;
					if (!enabled)
					{
						GUIStyle style = new GUIStyle();
						style.normal.textColor = Color.red;
						GUILayout.Label("Character name cannot be 'username' and must be longer than 3 characters and no longer than 32", style);
					}
					if (GUILayout.Button("Submit character name"))
					{
						SubmitNewUser((status, result) => Debug.Log("Create user result: " + status + " " + result), newUserName);
					}
					GUI.enabled = true;
				}
				GUILayout.Label("No user known");
			}
			else // user is known
			{
				if (!projectIsKnown)
				{
					if (!startedProjectCheck)
					{
						GetProjectKnown((status, result) =>
						{
							// empty
						});
						startedProjectCheck = true;
					}

					// register project
					//GUILayout.Label("project is not known");
					if (shouldCreateNewProject && !submittedNewProjectData)
					{
						GUILayout.Label("First time setup: ");
						GUILayout.Space(10);
						GUILayout.Label("Submit new project");
						GUI.enabled = false;
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Label("Project name: ");
							GUILayout.Label(GetProjectName());
						}
						EditorGUILayout.EndHorizontal();
						GUI.enabled = true;

						if (GUILayout.Button("Submit new project to database"))
						{
							SubmitNewProject((status, result) =>
							{
								// left empty
								Debug.Log($"new project submit result: {status}");
							});
						}
					}
				}
				else
				{
					// continue
					//GUILayout.Label("project is known");

					if (!projectIDIsRequested)
					{
						GetProjectIDRequest = GetProjectID((status, result) =>
						{
							if (status == WebRequestResult.Success)
							{
								string r = GetProjectIDRequest.Result;
								char c = char.Parse("-");
								string[] s = r.Split(c);
								string rs = s[1].Replace("result", "");
								projectID = rs;
							}
						});
						projectIDIsRequested = true;
					}
					//GUILayout.Label($"projectID: {projectID}");

					if (projectID.Length > 0)
					{
						// project ID is known
						if (!hasRequestedProjectScenes)
						{
							GetProjectScenesRequest = GetProjectScenes((status, result) =>
							{
								if (status == WebRequestResult.Success)
								{
									string r = GetProjectScenesRequest.Result;
									string[] sceneNames = r.Split(char.Parse("?"));
									projectScenes = new List<string>();
									projectScenes.AddRange(sceneNames);
								}
							});

							hasRequestedProjectScenes = true;
						}

						switch (openedToolbarMenu)
						{
							case 0:
								DrawCurrentBuildSettingsScenes();
								break;
							case 1:
								DrawAllScenesGroup();
								break;
							default:
								break;
						}
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}

		private void DrawTestDisplay(string[] entries)
		{
			float width = 200;
			float height = 200;

			float windowWidth = position.size.x;

			List<string> filteredList = new List<string>();

			foreach (var item in entries)
			{
				string sceneName = PathToFile(item);
				bool contentContains = sceneName.IndexOf(Sanitize(searchText), StringComparison.OrdinalIgnoreCase) >= 0;
				if (searchText == "" || searchText.Length <= 0 || contentContains)
				{
					filteredList.Add(item);
				}
			}

			int maxWidthFit = Mathf.FloorToInt(windowWidth / width);
			maxWidthFit = Mathf.Min(maxWidthFit, filteredList.Count);
			int contentToFill = filteredList.Count;
			int columnCount = contentToFill / maxWidthFit;
			bool addOneMoreRow = contentToFill % maxWidthFit != 0;

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical();
				{ 
					int drawnEntries = 0;

					for (int i = 0; i < columnCount; i++)
					{
						EditorGUILayout.BeginHorizontal();
						for (int y = 0; y < maxWidthFit; y++)
						{
							TestDisplaySingleBlock(width, height, filteredList[drawnEntries]);
							drawnEntries++;
						}
						EditorGUILayout.EndHorizontal();
					}

					if (drawnEntries > 0)
					{
						EditorGUILayout.BeginHorizontal();
						for (int y = 0; y < contentToFill % maxWidthFit; y++)
						{
							TestDisplaySingleBlock(width, height, filteredList[drawnEntries]);
							drawnEntries++;
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void TestDisplaySingleBlock(float width, float height, string path = "Hello")
		{
			GUIStyle header = new GUIStyle();
			header.fontStyle = FontStyle.Bold;
			header.fontSize = 14;
			header.normal.textColor = EditorStyles.boldLabel.normal.textColor;

			if(bgColor == null)
			{
				ResetTextures();
			}

			Rect area = EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(height), GUILayout.Width(width), GUILayout.ExpandWidth(false));
			{
				GUI.DrawTexture(area, bgColor);

				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();

					Color backgroundColor = GUI.backgroundColor;

					GUI.backgroundColor = selectionBackgroundColor;

					Rect rect = EditorGUILayout.BeginVertical(GUI.skin.box);
					{
						var go = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

						EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width));
						{
							string text = go != null ? go.name : path;
							
							GUILayout.FlexibleSpace();

							GUILayout.Label(text, header, GUILayout.MaxWidth(width));

							GUILayout.FlexibleSpace();

							GUI.enabled = go != null;
							if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
								SelectGameObject(go);
							GUI.enabled = true;

						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
					GUI.backgroundColor = backgroundColor;
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();


				bool hasSceneLock = false;
				bool isLockOwner = false;
				SceneLockSceneObject _lock = null;
				if (sceneLockDictionary.TryGetValue(Path.GetFileNameWithoutExtension(path), out _lock))
				{
					if (_lock.HasSceneLockActive)
					{
						hasSceneLock = true;
						if (_lock.SceneLock.OwnerDeviceID == GetDeviceID())
							isLockOwner = true;
						else
							isLockOwner = false;
					}
					else
						hasSceneLock = false;
				}
				else
				{
					// no lock present
					hasSceneLock = false;
				}

				Color lockColor = (!hasSceneLock) ? Color.green * 0.5f : (isLockOwner) ? Color.yellow * 0.5f : Color.red * 0.5f;
				Texture2D lockBG = (!hasSceneLock) ? sceneLockBG : (isLockOwner) ? sceneLockOwnedBG : sceneLockNotOwnedBG;
				if (_lock == null)
					lockBG = sceneMissingBG;

				Rect sceneLockArea = EditorGUILayout.BeginVertical(GUILayout.Height(height - 35));
				{
					GUI.DrawTexture(sceneLockArea, lockBG);
					GUILayout.Space(5);
					EditorGUILayout.BeginHorizontal(GUILayout.Height(height - 45));
					{
						GUILayout.Space(5);
						Rect scenelockInnerArea = EditorGUILayout.BeginHorizontal(GUILayout.Height(height - 45));
						{
							GUI.DrawTexture(scenelockInnerArea, bgColor);
							Rect test = EditorGUILayout.BeginVertical();
							{
								Vector2 pos = scenelockInnerArea.position;
								Vector2 size = scenelockInnerArea.size;
								size.x -= 10;
								size.y -= 100;
								pos.x += 5;
								pos.y += 95;
								Rect result = new Rect(pos.x, pos.y, size.x, size.y);

								EditorGUILayout.BeginHorizontal();
								{
									GUILayout.Space(5);
									EditorGUILayout.BeginVertical();
									{
										GUILayout.Label("Scene lock status:", header);

										GUIStyle lockText = new GUIStyle();
										lockText.fontStyle = FontStyle.Bold;
										lockText.fontSize = 14;
										Color c = lockColor;
										c.a = 1f;
										lockText.normal.textColor = c;

										if (_lock != null)
											GUILayout.Label((hasSceneLock) ? "Locked" : "Open", lockText);
										else
										{
											c = Color.red * 0.5f;
											c.a = 1f;
											lockText.normal.textColor = c;
											GUILayout.Label("Missing from DB", lockText);
										}
									}
									EditorGUILayout.EndVertical();
								}
								EditorGUILayout.EndHorizontal();

								//GUILayout.FlexibleSpace();

								EditorGUILayout.BeginVertical();
								{
									if(_lock != null)
									{
										if (_lock.HasSceneLock)
										{
											// locked
											GUILayout.Label("Owner: " + _lock.SceneLock.OwnerName);
											GUILayout.Label("Time:  " + _lock.SceneLock.LockTime);

											if (_lock.SceneLock.OwnerDeviceID == GetDeviceID())
											{
												if (GUI.Button(result, "Release scene lock"))
												{
													// delete lock
													ReleaseSceneLock((status, result) => { InitializeSceneLocks(); }, _lock);
												}
											}
											else
											{
												if (GUI.Button(result, "Force remove scene lock"))
												{
													// delete and claim lock
													ReleaseSceneLock((status, result) => { InitializeSceneLocks(); }, _lock);
												}
											}
										}
										else
										{
											GUILayout.Label("Owner: -");
											GUILayout.Label("Time: -");
											// no lock
											if (GUI.Button(result, "Claim scene lock"))
											{
												// claim lock call
												SubmitNewSceneLock((status, result) => { InitializeSceneLocks(); }, Path.GetFileNameWithoutExtension(path));
											}
										}
									}
									else
									{
										GUILayout.Label("Scene is missing in DB.");
										// no lock
										if (GUI.Button(result, "Upload scene to DB"))
										{
											SubmitIndividualScene((status, result) =>
											{
												hasRequestedProjectScenes = false;
											},
											Path.GetFileNameWithoutExtension(path));
										}
									}
								}
								EditorGUILayout.EndVertical();
								GUILayout.Space(20);
							}
							EditorGUILayout.EndVertical();
						}
						EditorGUILayout.EndHorizontal();

						
						GUILayout.Space(5);
					}
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(3);

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("Load scene", GUILayout.Width(width - 25)))
						{
							if (EditorSceneManager.GetActiveScene().isDirty)
							{
								if (EditorUtility.DisplayDialog("Scene change warning", "Current scene has unsaved changes. Are you sure you want to leave?", "Save scene, then go", $"Cancel scene loading"))
								{
									// proceed as expected
									EditorSceneManager.SaveOpenScenes();
									EditorSceneManager.OpenScene(path);
								}
								else
								{
									// nothing
								}
							}
							else
							{
								EditorSceneManager.OpenScene(path);
							}
						}
						GUILayout.FlexibleSpace();
					}
					EditorGUILayout.EndHorizontal();
					GUILayout.Space(4);
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();
		}

		private void DrawCurrentBuildSettingsScenes()
		{
			GUILayout.Space(20);

			bool hasMissingScenes = false;

			foreach (var item in EditorBuildSettings.scenes)
			{
				if(!sceneLockDictionary.ContainsKey(Path.GetFileNameWithoutExtension(item.path)))
				{
					hasMissingScenes = true;
					break;
				}
			}

			if (hasMissingScenes)
			{
				if (GUILayout.Button("Upload all scenes from build settings to DB and link to project"))
				{
					string scenes = "scenes";
					foreach (var item in EditorBuildSettings.scenes)
					{
						string path = item.path;
						string sceneName = Path.GetFileNameWithoutExtension(path);
						scenes += "?" + sceneName;
					}
					SubmitProjectScenes((status, result) =>
					{
						hasRequestedProjectScenes = false;
					// empty
				}, scenes);
				}
			}

			List<string> s = new List<string>();
			foreach (var item in EditorBuildSettings.scenes)
			{
				s.Add(item.path);
			}

			DrawTestDisplay(s.ToArray());
		}

		private void DrawCurrentOpenScene()
		{
			GUIStyle header = new GUIStyle();
			header.fontStyle = FontStyle.Bold;
			header.fontSize = 14;
			header.normal.textColor = EditorStyles.boldLabel.normal.textColor;

			Scene currentScene = EditorSceneManager.GetActiveScene();
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(5);
				GUILayout.Label("Current open scene: ", header);
				EditorGUILayout.EndHorizontal();

				TestDisplaySingleBlock(300, 200, currentScene.path);

				//DrawSceneDisplay(currentScene.path, currentScene.buildIndex);
			}
			EditorGUILayout.EndVertical();
		}

		private void DrawAllScenesGroup()
		{
			if (AllScenesInProject.Count > 0)
			{
				EditorGUI.indentLevel++;
				GUILayout.Space(20);

				bool hasMissingScenes = false;

				foreach (var item in AllScenesInProject)
				{
					if (!sceneLockDictionary.ContainsKey(Path.GetFileNameWithoutExtension(item)))
					{
						hasMissingScenes = true;
						break;
					}
				}

				if (hasMissingScenes)
				{
					if (GUILayout.Button("Upload all scenes from project to DB and link to project"))
					{
						string scenes = "scenes";
						foreach (var item in AllScenesInProject)
						{
							scenes += "?" + PathToFile(item);
						}
						SubmitProjectScenes((status, result) =>
						{
							hasRequestedProjectScenes = false;
							InitializeSceneLocks();
						// empty
					}, scenes);
					}
				}

				DrawTestDisplay(AllScenesInProject.ToArray());
			}
		}

		

		private static void DoFirstInit()
		{
			ResetStatics();
			InitializeAllScenesList();
			InitializeSceneLocks();
			EditorApplication.update += WaitUntillSceneLockRequestsAreDone;

			EditorApplication.update -= DoFirstInit;
		}

		private static void PlayModeSceneChangedEvent(PlayModeStateChange obj)
		{
			if(obj == PlayModeStateChange.ExitingPlayMode)
			{
				DoPopupOnSceneSaveWhenNotLockOwner();
			}
		}

		private static void SceneLoadEvent(Scene scene, OpenSceneMode mode)
		{
			if (Application.isPlaying)
				return;
			BGInitialized = false;
			if (!BGInitialized)
			{
				ResetStatics();
				InitializeAllScenesList();
				InitializeSceneLocks();
			}
			EditorApplication.update += WaitUntillSceneLockRequestsAreDone;
		}

		private static void OnSceneSavingEvent(Scene scene, string path)
		{
			SetHasDonePopup(false);
			DoPopupOnSceneSaveWhenNotLockOwner();
		}

		private static void SceneCloseEvent(Scene scene, bool removingScene)
		{
			if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
				return;
			SetHasDonePopup(false);
			previousScenePath = scene.path;
		}

		private static bool HasDonePopup()
		{
			if (EditorPrefs.HasKey("hasDonePopup"))
			{
				return EditorPrefs.GetBool("hasDonePopup");
			}
			else
			{
				SetHasDonePopup(false);
				return false;
			}
		}

		private static void SetHasDonePopup(bool val)
		{
			EditorPrefs.SetBool("hasDonePopup", val);
		}

		private static void WaitUntillSceneLockRequestsAreDone()
		{
			CheckRunningRequests(false);

			if (SceneLocksAreInitialized())
			{
				string currentScene = EditorSceneManager.GetActiveScene().name;
				if (sceneLockDictionary.ContainsKey(currentScene))
				{
					// spawn popup
					SceneLockSceneObject scl = sceneLockDictionary[currentScene];
					if (scl.HasSceneLock && scl.SceneLock.OwnerDeviceID != GetDeviceID())
					{
						string text = $"{currentScene} has an active scene lock that is owned by {scl.SceneLock.OwnerName}. \nThey claimed the lock at {scl.SceneLock.LockTime}. \nCheck with them if the scene lock is still active before saving and/or commiting any changes to it.";
						if (previousScenePath != string.Empty && previousScenePath.Length > 2)
						{
							if (!HasDonePopup() && !EditorApplication.isPlayingOrWillChangePlaymode)
							{
								if (EditorUtility.DisplayDialog("Scene lock warning", text, "Acknowledge", $"Go back to {Path.GetFileNameWithoutExtension(previousScenePath)}"))
								{
									// proceed as expected
								}
								else
								{
									EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);
								}
								SetHasDonePopup(true);
							}
						}
						else
						{
							if (!HasDonePopup() && !EditorApplication.isPlayingOrWillChangePlaymode)
							{
								EditorUtility.DisplayDialog("Scene lock warning", text, "Acknowledge");
								SetHasDonePopup(true);
							}

						}
					}
					else if (scl.HasSceneLock && scl.SceneLock.OwnerDeviceID == GetDeviceID())
					{
						string text = $"{currentScene} has an active scene lock that is owned by you. \nYou claimed the lock at {scl.SceneLock.LockTime}. \nPlease remember to lift the scene lock when you're done with it.";
						if (!HasDonePopup() && !EditorApplication.isPlayingOrWillChangePlaymode)
						{
							EditorUtility.DisplayDialog("Scene lock warning", text, "Acknowledge");
							SetHasDonePopup(true);
						}
					}
				}
				EditorApplication.update -= WaitUntillSceneLockRequestsAreDone;
			}
		}

		private static void DoPopupOnSceneSaveWhenNotLockOwner()
		{
			if (sceneLockDictionary != null)
			{
				string currentScene = EditorSceneManager.GetActiveScene().name;
				if (sceneLockDictionary.ContainsKey(currentScene))
				{
					// spawn popup
					SceneLockSceneObject scl = sceneLockDictionary[currentScene];
					if (scl.HasSceneLock && scl.SceneLock.OwnerDeviceID != GetDeviceID())
					{
						string text = $"The current scene has a scene lock that is not owned by you. \n\n{currentScene} has an active scene lock that is owned by {scl.SceneLock.OwnerName}. \nThey claimed the lock at {scl.SceneLock.LockTime}. \nCheck with them if the scene lock is still active before saving and/or commiting any changes to it.";

						//if (!HasDonePopup() && !EditorApplication.isPlayingOrWillChangePlaymode)
						//{
							if (EditorUtility.DisplayDialog("Scene lock warning", text, "Acknowledge"))
							{
								// proceed as expected
							}
							SetHasDonePopup(true);
						//}
					}
				}
			}
		}

		private static void CheckRunningRequests(bool drawResults = true)
		{
			bool hasProcess = false;
			Action toRemoves = null;
			foreach (var item in runningRequests)
			{
				if (!hasProcess)
				{
					if (item.Value.IsDone)
					{
						toRemoves += () => runningRequests.Remove(item.Key);
						//Debug.Log(item.Value.LoadingText + " " + item.Value.Result);
						if (!drawResults)
						{
							toRemoves?.Invoke();
							return;
						}
						else
							hasProcess = true;
					}

					if (item.Value.IsExpired())
					{
						toRemoves += () => runningRequests.Remove(item.Key);
						//Debug.Log(item.Value.LoadingText + " " + item.Value.Result);
						if (!drawResults)
						{
							toRemoves?.Invoke();
							return;
						}
						else
							hasProcess = true;
					}
					if (drawResults)
					{
						GUILayout.Label(item.Value.LoadingText);
						GUILayout.Label("Time left: " + item.Value.GetRemainingTime());
						hasProcess = true;
					}
				}
			}
			toRemoves?.Invoke();
			if (drawResults)
			{
				if (hasProcess)
				{
					if (window == null)
						Open();
					window.Repaint();
				}
			}
		}

		private static void InitializeAllScenesList()
		{
			AllScenesInProject = new List<string>();
			if (AllScenesInProject.Count == 0)
			{
				string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
				foreach (string guid in guids)
				{
					string scenePath = AssetDatabase.GUIDToAssetPath(guid);
					string sceneName = Path.GetFileNameWithoutExtension(scenePath);
					if (!scenePath.Contains("ThirdParty") && !AllScenesInProject.Contains(PathToFile(scenePath)))
						AllScenesInProject.Add(scenePath);
				}
			}
		}

		private static void InitializeSceneLocks()
		{
			sceneLockDictionary = new Dictionary<string, SceneLockSceneObject>();
			runningSceneLockRequests = new List<RunningWebRequest>();
			foreach (var item in AllScenesInProject)
			{
				GetSceneLockData((status, result) =>
				{
					if (status == WebRequestResult.Success)
					{
						if (!sceneLockDictionary.ContainsKey(PathToFile(item)))
						{
							SceneLockSceneObject sc = GetSceneLockSceneObjectFromPHPResult(result);
							sceneLockDictionary.Add(PathToFile(item), sc);
						}
					}
				}, PathToFile(item));

			}
		}

		private static bool SceneLocksAreInitialized()
		{
			foreach (var item in runningSceneLockRequests)
			{
				if (!item.IsDone)
					return false;
			}
			return true;
		}

		private static RunningWebRequest CreateNewWebRequest(string key, string loadingText, List<IMultipartFormSection> wwwForm, Action<WebRequestResult, string> callback, string url = SceneLockUrl, Action onSuccess = null, Action onFail = null, Action onNetwork = null, Action onNull = null)
		{
			if (runningRequests.ContainsKey(key))
				return runningRequests[key];

			wwwForm.Add(new MultipartFormDataSection(GET_REPLY_KEY, key));
			wwwForm.Add(new MultipartFormDataSection(PRODUCT_NAME, GetProjectName()));

			UnityWebRequest req = UnityWebRequest.Post(url, wwwForm);

			req.SendWebRequest();
			RunningWebRequest rReq = new RunningWebRequest(req, key, callback, loadingText, onSuccess, onFail, onNetwork, onNull);
			runningRequests.Add(key, rReq);
			EditorApplication.update += runningRequests[key].Run;
			return rReq;
		}

		private RunningWebRequest GetUserKnown(Action<WebRequestResult, string> callback)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(GET_USER_KNOWN, deviceID));

			return CreateNewWebRequest(nameof(GetUserKnown), "Checking if user exists...", wwwForm, callback, onSuccess: () => userIsKnown = true, onFail: () => { shouldCreateNewUser = true; userIsKnown = false; submittedNewUserData = false; });
		}

		private RunningWebRequest SubmitNewUser(Action<WebRequestResult, string> callback, string newUsername)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(SET_NEW_USER, deviceID));
			wwwForm.Add(new MultipartFormDataSection(NEW_USERNAME, newUsername));

			return CreateNewWebRequest(nameof(SubmitNewUser), "Creating new user...", wwwForm, callback, onSuccess: () => { userIsKnown = true; }, onFail: () => { shouldCreateNewUser = true; userIsKnown = false; submittedNewUserData = false; });
		}

		private RunningWebRequest GetProjectKnown(Action<WebRequestResult, string> callback)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(GET_PROJECT_KNOWN, GetProjectName()));

			return CreateNewWebRequest(nameof(GetProjectKnown), "Checking if project exists...", wwwForm, callback, onSuccess: () => { projectIsKnown = true; shouldCreateNewProject = false; }, onFail: () => { projectIsKnown = false; shouldCreateNewProject = true; });
		}

		private RunningWebRequest SubmitNewProject(Action<WebRequestResult, string> callback)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(SET_NEW_PROJECT, GetProjectName()));

			return CreateNewWebRequest(nameof(SubmitNewProject), "Creating new project...", wwwForm, callback, onSuccess: () => { projectIsKnown = true; submittedNewProjectData = true; }, onFail: () => { projectIsKnown = false; submittedNewProjectData = false; });
		}

		private RunningWebRequest GetProjectID(Action<WebRequestResult, string> callback)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(GET_PROJECT_ID, GetProjectName()));

			return CreateNewWebRequest(nameof(GetProjectID), "Getting project id...", wwwForm, callback, onSuccess: () => { }, onFail: () => { });
		}

		private RunningWebRequest GetProjectScenes(Action<WebRequestResult, string> callback)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(GET_PROJECT_SCENES, projectID));

			return CreateNewWebRequest(nameof(GetProjectScenes), "Getting project scenes...", wwwForm, callback, onSuccess: () => { }, onFail: () => { });
		}

		private RunningWebRequest SubmitProjectScenes(Action<WebRequestResult, string> callback, string scenes)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(SET_PROJECT_SCENES, projectID));

			wwwForm.Add(new MultipartFormDataSection("project_scene_names", scenes));

			return CreateNewWebRequest(nameof(SubmitProjectScenes), "Submitting project scenes...", wwwForm, callback, onSuccess: () => { InitializeSceneLocks(); }, onFail: () => { });
		}

		private RunningWebRequest SubmitIndividualScene(Action<WebRequestResult, string> callback, string scene)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(SET_PROJECT_SCENES, projectID));
			wwwForm.Add(new MultipartFormDataSection("project_scene_names", "scenes?" + scene));

			return CreateNewWebRequest(nameof(SubmitIndividualScene), $"Submitting project scene {scene}...", wwwForm, callback, onSuccess: () => { InitializeSceneLocks(); }, onFail: () => { });
		}

		private static RunningWebRequest GetSceneLockData(Action<WebRequestResult, string> callback, string scene)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(GET_SCENE_LOCK_DATA, scene));

			RunningWebRequest req = CreateNewWebRequest(nameof(GetSceneLockData) + scene, $"Gathering scene lock data for scene {scene}...", wwwForm, callback, onSuccess: () => { }, onFail: () => { });
			runningSceneLockRequests.Add(req);
			return req;
		}

		//TODO: this
		private RunningWebRequest SubmitNewSceneLock(Action<WebRequestResult, string> callback, string scene)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(SUBMIT_NEW_SCENE_LOCK, scene));
			wwwForm.Add(new MultipartFormDataSection("user_device", GetDeviceID()));

			RunningWebRequest req = CreateNewWebRequest(nameof(SubmitNewSceneLock) + scene, $"Submitting scene lock data for scene {scene}...", wwwForm, callback, onSuccess: () => { }, onFail: () => { InitializeSceneLocks(); });
			return req;
		}

		private RunningWebRequest ReleaseSceneLock(Action<WebRequestResult, string> callback, SceneLockSceneObject sceneLock)
		{
			List<IMultipartFormSection> wwwForm = new List<IMultipartFormSection>();
			wwwForm.Add(new MultipartFormDataSection(RELEASE_SCENE_LOCK, sceneLock.SceneLock.SceneLockID));

			RunningWebRequest req = CreateNewWebRequest(nameof(ReleaseSceneLock) + sceneLock.SceneLock.SceneLockID, $"Releasing scene lock data for sceneLockID {sceneLock.SceneLock.SceneLockID}...", wwwForm, callback, onSuccess: () => { InitializeSceneLocks(); }, onFail: () => { InitializeSceneLocks(); });
			return req;
		}

		private void RefreshInterface()
		{
			ResetVariables();
			InitializeAllScenesList();

			deviceID = GetDeviceID();
			GetUserKnown((status, result) =>
			{
				// left empty
			});
			InitializeSceneLocks();

			initialized = true;
		}

		private void DrawSceneDisplay(string path, int index)
		{
			string sceneName = Path.GetFileNameWithoutExtension(path);
			bool contentContains = sceneName.IndexOf(Sanitize(searchText), StringComparison.OrdinalIgnoreCase) >= 0;
			if (searchText == "" || searchText.Length <= 0 || contentContains)
			{
				// cross reference to existing project scenes
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					EditorGUILayout.BeginHorizontal();
					{
						bool hasScene = projectScenes.Contains(Sanitize(sceneName));

						GUILayout.Label(index.ToString());
						GUILayout.FlexibleSpace();

						EditorGUILayout.BeginHorizontal();
						{
							var obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
							DrawElement(obj);
							GUILayout.FlexibleSpace();
						}
						EditorGUILayout.EndHorizontal();

						//GUILayout.FlexibleSpace();
						GUILayout.Label(path);
						//GUILayout.FlexibleSpace();

						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.FlexibleSpace();
							if (GUILayout.Button("Load scene"))
							{
								if(EditorSceneManager.GetActiveScene().isDirty)
								{
									if (EditorUtility.DisplayDialog("Scene change warning", "Current scene has unsaved changes. Are you sure you want to leave?", "Save scene, then go", $"Cancel scene loading"))
									{
										// proceed as expected
										EditorSceneManager.SaveOpenScenes();
										EditorSceneManager.OpenScene(path);
									}
									else
									{
										// nothing
									}
								}
								else
								{
									EditorSceneManager.OpenScene(path);
								}
							}

							if (!projectScenes.Contains(sceneName))
							{
								if (GUILayout.Button("Submit " + sceneName + " to database"))
								{
									SubmitIndividualScene((status, result) =>
									{
										hasRequestedProjectScenes = false;
									},
									sceneName);
								}
							}
							else
							{
								GUI.enabled = !hasScene;
								GUILayout.Label("Scene already in database");
								GUI.enabled = true;
							}
						}
						EditorGUILayout.EndHorizontal();
						//IndentedLabel(item.path);
					}
					EditorGUILayout.EndHorizontal();
					if (sceneLockDictionary.TryGetValue(sceneName, out SceneLockSceneObject _lock))
					{
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Space(INDENT_SPACE);
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Label("Locked: ");
								EditorGUILayout.BeginHorizontal();
								{
									GUI.enabled = false;
									GUILayout.Toggle(_lock.HasSceneLock, "");
									GUI.enabled = true;
									GUILayout.FlexibleSpace();
								}
								EditorGUILayout.EndHorizontal();
							}
							EditorGUILayout.EndHorizontal();
							if (_lock.HasSceneLock)
							{
								// locked
								GUILayout.Label("Owner: " + _lock.SceneLock.OwnerName);
								GUILayout.Label("Locked since " + _lock.SceneLock.LockTime);

								if (_lock.SceneLock.OwnerDeviceID == GetDeviceID())
								{
									if (GUILayout.Button("Release scene lock"))
									{
										// delete lock
										ReleaseSceneLock((status, result) => { InitializeSceneLocks(); }, _lock);
									}
								}
								else
								{
									if (GUILayout.Button("Force remove scene lock"))
									{
										// delete and claim lock
										ReleaseSceneLock((status, result) => { InitializeSceneLocks(); }, _lock);
									}
								}
							}
							else
							{
								// no lock
								if (GUILayout.Button("Claim scene lock"))
								{
									// claim lock call
									SubmitNewSceneLock((status, result) => { InitializeSceneLocks(); }, sceneName);
								}
							}
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndVertical();
			}
		}

		private static string GetDeviceID()
		{
			string physicalAddress = "";
			//if (GamePlatform.Platform == BuildPlatform.Android)
			//	physicalAddress = SystemInfo.deviceUniqueIdentifier;

			NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

			foreach (NetworkInterface adapter in networkInterfaces)
			{
				if (adapter.Description == "en0")
				{
					physicalAddress = adapter.GetPhysicalAddress().ToString();
					break;
				}
				else
				{
					physicalAddress = adapter.GetPhysicalAddress().ToString();

					if (physicalAddress != "")
					{
						break;
					};
				}
			}

			byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(physicalAddress);
			StringBuilder sb = new StringBuilder();
			using (MD5 md5 = MD5.Create())
			{
				byte[] hash = md5.ComputeHash(stringBytes);

				foreach (Byte b in hash)
				{
					sb.Append(b.ToString("X2"));
				}
			}

			physicalAddress = sb.ToString();

			return physicalAddress;
		}

		private static string GetProjectName()
		{
			return Sanitize(Application.productName);
		}

		private void IndentedLabel(string content, GUILayoutOption[] layoutOptions = null)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(Mathf.Max(0, EditorGUI.indentLevel * INDENT_SPACE - 1f));
			GUILayout.Label(content, layoutOptions);
			EditorGUILayout.EndHorizontal();
		}

		private static SceneLockSceneObject GetSceneLockSceneObjectFromPHPResult(string result)
		{
			SceneLockSceneObject obj = new SceneLockSceneObject();
			string[] results = result.Split(char.Parse("?"));

			obj.projectID = results[1];
			obj.projectName = results[2];

			// has scene lock
			if (results[3].Contains("1"))
			{
				obj.HasSceneLock = true;
				string SceneLockID = results[4];
				string ProjectID = results[1];
				string SceneID = results[5];
				string OwnerID = results[7];
				string OwnerName = results[8];
				string LockTime = results[9];
				string OwnerDeviceID = results[10];
				obj.SceneLock = new SceneLock(SceneLockID, ProjectID, SceneID, OwnerID, OwnerName, OwnerDeviceID, LockTime);
			}
			return obj;
		}

		private void ResetVariables()
		{
			// TODO: fill this in entirely
			userIsKnown = false;
			shouldCreateNewUser = false;
			submittedNewUserData = false;
			startedProjectCheck = false;
			projectIsKnown = false;
			shouldCreateNewProject = false;
			submittedNewProjectData = false;
			projectIDIsRequested = false;
			GetProjectIDRequest = null;
			newUserName = "username";
			deviceID = "";
			projectID = "";
			searchText = "";
			hasRequestedProjectScenes = false;
			GetProjectScenesRequest = null;
			ResetTextures();

			ResetStatics();
		}

		private void ResetTextures()
		{
			bgColor = new Texture2D(1, 1);
			bgColor.SetPixel(0, 0, new Color(0.25f, 0.25f, 0.25f));
			bgColor.Apply();

			sceneLockBG = new Texture2D(1, 1);
			sceneLockBG.SetPixel(0, 0, Color.green * 0.5f);
			sceneLockBG.Apply();

			sceneLockOwnedBG = new Texture2D(1, 1);
			sceneLockOwnedBG.SetPixel(0, 0, Color.yellow * 0.5f);
			sceneLockOwnedBG.Apply();

			sceneLockNotOwnedBG = new Texture2D(1, 1);
			sceneLockNotOwnedBG.SetPixel(0, 0, Color.red * 0.5f);
			sceneLockNotOwnedBG.Apply();

			sceneMissingBG = new Texture2D(1, 1);
			sceneMissingBG.SetPixel(0, 0, Color.magenta * 0.5f);
			sceneMissingBG.Apply();
		}

		private static void ResetStatics()
		{
			runningRequests = new Dictionary<string, RunningWebRequest>();
			sceneLockDictionary = new Dictionary<string, SceneLockSceneObject>();
			AllScenesInProject = new List<string>();
		}

		public static string Sanitize(string _in)
		{
			System.Text.RegularExpressions.Regex illegalCharacters = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9_]");

			string result = _in;

			if (illegalCharacters.IsMatch(_in))
			{
				result = illegalCharacters.Replace(result, "");
			}

			return result;
		}

		protected Rect DrawElement(UnityEngine.Object go)
		{
			Color backgroundColor = GUI.backgroundColor;

			GUI.backgroundColor = selectionBackgroundColor;

			Rect rect = EditorGUILayout.BeginVertical(GUI.skin.box);

			EditorGUILayout.BeginHorizontal();
			GUIStyle textStyle = EditorStyles.label;
			textStyle.fontSize = TEXT_FONT_SIZE;
			string text = go != null ? go.name : "Object was destroyed";
			GUILayout.Label(text, textStyle);

			GUILayout.FlexibleSpace();

			GUI.enabled = go != null;
			if (GUI.Button(rect, GUIContent.none, GUIStyle.none))
				SelectGameObject(go);
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			GUI.backgroundColor = backgroundColor;

			return rect;
		}

		private static void SelectGameObject(UnityEngine.Object go)
		{
			EditorGUIUtility.PingObject(go);
		}

		private static string PathToFile(string path)
		{
			return Path.GetFileNameWithoutExtension(path);
		}
	}
}

namespace Utils.Core.SceneLockTool
{
	[System.Serializable]
	public class SceneLockSceneObject
	{
		public SceneLock SceneLock = null;
		public string projectID = "";
		public string projectName = "";
		public bool HasSceneLock = false;
		public bool HasSceneLockActive => SceneLock != null;
	}

	[System.Serializable]
	public class SceneLock
	{
		public string SceneLockID { get; private set; }
		public string ProjectID { get; private set; }
		public string SceneID { get; private set; }
		public string OwnerID { get; private set; }
		public string OwnerName { get; private set; }
		public string OwnerDeviceID { get; private set; }
		public string LockTime { get; private set; }

		public SceneLock(string sceneLockID, string projectID, string sceneID, string ownerID, string ownerName, string ownerDeviceID, string lockTime)
		{
			this.SceneLockID = sceneLockID;
			this.ProjectID = projectID;
			this.SceneID = sceneID;
			this.OwnerID = ownerID;
			this.OwnerName = ownerName;
			this.LockTime = lockTime;
			this.OwnerDeviceID = ownerDeviceID;
		}
	}
}
