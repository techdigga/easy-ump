using System;
using UnityEditor;
using UnityEngine;

namespace EasyUmp.Editor
{
    internal enum EditorPopupMode
    {
        Always = 0,
        OncePerSession = 1
    }

    /// <summary>
    /// Minimal editor popup to simulate UMP callbacks.
    /// </summary>
    internal sealed class EasyUmpEditorPopup : EditorWindow
    {
        private static bool shownThisSession;
        private static EasyUmpEditorPopup instance;

        private string operationName;
        private Action onSuccess;
        private Action<UmpError> onFailure;
        private bool callbackInvoked;

        public static bool Show(string opName, Action success, Action<UmpError> failure)
        {
            var mode = UmpSettings.instance.EditorPopupMode;
            if (mode == EditorPopupMode.OncePerSession && shownThisSession)
            {
                return false;
            }

            shownThisSession = true;
            if (instance == null)
            {
                instance = CreateInstance<EasyUmpEditorPopup>();
                instance.titleContent = new GUIContent("Easy UMP (Editor)");
                instance.minSize = new Vector2(320, 120);
            }

            instance.operationName = opName;
            instance.onSuccess = success;
            instance.onFailure = failure;
            instance.ShowUtility();
            instance.Focus();
            return true;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Easy UMP Editor Simulation", EditorStyles.boldLabel);
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField("Operation", operationName ?? "Unknown");
            EditorGUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Success"))
            {
                CloseAndInvokeSuccess();
            }
            if (GUILayout.Button("Fail"))
            {
                CloseAndInvokeFailure();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void CloseAndInvokeSuccess()
        {
            var cb = onSuccess;
            callbackInvoked = true;
            Cleanup();
            cb?.Invoke();
        }

        private void CloseAndInvokeFailure()
        {
            var cb = onFailure;
            callbackInvoked = true;
            Cleanup();
            cb?.Invoke(new UmpError
            {
                Code = -1,
                Message = ErrorMessages.EditorSimulatedFailure,
                Domain = "EasyUmpEditor"
            });
        }

        private void OnDisable()
        {
            if (!callbackInvoked && (onSuccess != null || onFailure != null))
            {
                CloseAndInvokeFailure();
            }
        }

        private void Cleanup()
        {
            onSuccess = null;
            onFailure = null;
            Close();
        }
    }
}
