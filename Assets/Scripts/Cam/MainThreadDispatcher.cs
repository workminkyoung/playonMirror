using System;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
public class MainThreadDispatcher : MonoBehaviour
{
    // Using a thread-safe collection
    private ConcurrentQueue<byte[]> actions = new ConcurrentQueue<byte[]>();

    // The instance to access from other threads
    private static MainThreadDispatcher instance;

    private Texture2D previewSource;

    Vector2 previewSize = new Vector2(1920, 1280);

    public void Setting()
    {
        previewSource = new Texture2D(0, 0);
        // Assign the current instance
        instance = this;
    }

    // This method can be called from other threads to queue up actions
    public static void InvokeOnMainThread(byte[] action)
    {
        if (instance == null)
        {
            //throw new Exception("MainThreadDispatcher instance has not been created yet.");
            Debug.Log("MainThreadDispatcher instance has not been created yet.");
            return;
        }

        instance.actions.Enqueue(action);
    }

    private void Update()
    {
        if (instance == null)
            return;

        // Execute all actions that have been queued up
        while (actions.TryDequeue(out var action))
        {
            //Texture2D canvas = new Texture2D(2, 2);
            previewSource.LoadImage(action);
            DSLRManager.Instance.OnLoadPreview?.Invoke(previewSource);
        }
    }
}