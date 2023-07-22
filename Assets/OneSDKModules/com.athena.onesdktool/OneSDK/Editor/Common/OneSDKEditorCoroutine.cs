﻿using System.Collections;
using UnityEditor;

namespace OneSDK.Editor
{
    /// <summary>
    /// A coroutine that can update based on editor application update.
    /// </summary>
    public class OneSDKEditorCoroutine
    {
        private readonly IEnumerator enumerator;

        private OneSDKEditorCoroutine(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
        }

        /// <summary>
        /// Creates and starts a coroutine.
        /// </summary>
        /// <param name="enumerator">The coroutine to be started</param>
        /// <returns>The coroutine that has been started.</returns>
        public static OneSDKEditorCoroutine StartCoroutine(IEnumerator enumerator)
        {
            var coroutine = new OneSDKEditorCoroutine(enumerator);
            coroutine.Start();
            return coroutine;
        }

        private void Start()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        /// <summary>
        /// Stops the coroutine.
        /// </summary>
        public void Stop()
        {
            if (EditorApplication.update == null) return;

            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            // Coroutine has ended, stop updating.
            if (!enumerator.MoveNext())
            {
                Stop();
            }
        }
    }
}
