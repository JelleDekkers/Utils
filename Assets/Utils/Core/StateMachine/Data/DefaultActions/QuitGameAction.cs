using UnityEngine;
using Utils.Core.Services;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utils.Core.Flow.DefaultActions
{
    public class QuitGameAction : StateAction
    {
#if UNITY_EDITOR
        [SerializeField] private bool exitEditorPlayMode = false;
#endif
        private CoroutineService coroutineService; 

        public void InjectDependencies(CoroutineService coroutineService)
        {
            this.coroutineService = coroutineService;
        }

        public override void OnStarted()
        {
            coroutineService.StopAllCoroutines();
            Application.Quit();

#if UNITY_EDITOR
            if (exitEditorPlayMode && EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
#endif
        }
    }
}
