using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyUmp
{
    /// <summary>
    /// Dispatches actions onto Unity's main thread.
    /// </summary>
    internal sealed class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> queue = new Queue<Action>();
        private static MainThreadDispatcher instance;

        /// <summary>
        /// Enqueues an action to run on the main thread.
        /// </summary>
        public static void Post(Action action)
        {
            if (action == null)
            {
                return;
            }

            EnsureInstance();
            lock (queue)
            {
                queue.Enqueue(action);
            }
        }

        /// <summary>
        /// Ensures a dispatcher instance exists in the scene.
        /// </summary>
        private static void EnsureInstance()
        {
            if (instance != null)
            {
                return;
            }

            var go = new GameObject(UnityNativeBridgeConstants.MainThreadDispatcherObjectName);
            DontDestroyOnLoad(go);
            instance = go.AddComponent<MainThreadDispatcher>();
        }

        /// <summary>
        /// Executes queued actions on the main thread.
        /// </summary>
        private void Update()
        {
            Action[] pending;
            lock (queue)
            {
                if (queue.Count == 0)
                {
                    return;
                }

                pending = queue.ToArray();
                queue.Clear();
            }

            foreach (var action in pending)
            {
                action?.Invoke();
            }
        }
    }
}
