using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityGLTF;
using System.Threading.Tasks;
public class APIManager : MonoBehaviour
{
    public static APIManager Instance;

    private static readonly string baseURL = "https://raw.githubusercontent.com/anuj-chouhan/Unity-Ar-Assets/main/HostedStuffs/";
    private readonly string urlImage = $"{baseURL}Image.png";
    private readonly string urlModel = $"{baseURL}Character.glb";
    private readonly string urlText = $"{baseURL}Text.txt";
    private readonly string urlVideo = $"{baseURL}Video.mp4";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ------------------ Public API Methods ------------------
    
    public void GetTextFromServer(Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(DownloadText(urlText, onSuccess, onError));
    }

    public void GetImageFromServer(Action<Sprite> onSuccess = null, Action<string> onError = null)
    {
        StartCoroutine(DownloadImage(urlImage, onSuccess, onError));
    }

    public void GetGLBModel(Action<GameObject> onSuccess, Action<string> onError)
    {
        StartCoroutine(DownloadGLBModel(urlModel, onSuccess, onError));
    }

    public string GetVideoURL()
    {
        return urlVideo;
    }

    public void StopAllFetching()
    {
        StopAllCoroutines();
    }

    // ------------------ Internal Download Methods ------------------

    private IEnumerator DownloadText(string url, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Text Download Failed: " + request.error);
                onError?.Invoke(request.error);
            }
            else
            {
                string text = request.downloadHandler.text;
                Debug.Log("Text Downloaded:\n" + text);
                onSuccess?.Invoke(text);
            }
        }
    }

    private IEnumerator DownloadImage(string url, Action<Sprite> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Image Download Failed: " + request.error);
                onError?.Invoke(request.error);
            }
            else
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(request);
                Sprite FetchedSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                Debug.Log("Image Loaded Successfully.");
                onSuccess?.Invoke(FetchedSprite);
            }
        }
    }

    //private IEnumerator DownloadGLBModel(string url, Action<GameObject> onSuccess, Action<string> onError)
    //{
    //    using (UnityWebRequest request = UnityWebRequest.Get(url))
    //    {
    //        yield return request.SendWebRequest();

    //        if (request.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError("GLB Download Failed: " + request.error);
    //            onError?.Invoke(request.error);
    //            yield break;
    //        }

    //        byte[] glbData = request.downloadHandler.data;

    //        ImportOptions importOptions = new ImportOptions
    //        {
    //            DataLoader = new MemoryLoader(glbData),
    //            AsyncCoroutineHelper = gameObject.AddComponent<AsyncCoroutineHelper>()
    //        };

    //        var importer = new GLTFSceneImporter("model.glb", importOptions);
    //        GameObject result = null;
    //        bool done = false;

    //        importer.LoadSceneAsync().ContinueWith(task =>
    //        {
    //            if (task.Exception == null)
    //                result = importer.LastLoadedScene;
    //            else
    //                Debug.LogException(task.Exception);

    //            done = true;
    //        });

    //        while (!done)
    //            yield return null;

    //        if (result == null)
    //        {
    //            string error = "Failed to load GLB model.";
    //            Debug.LogError(error);
    //            onError?.Invoke(error);
    //            yield break;
    //        }

    //        result.transform.forward = -Vector3.forward;

    //        var animation = result.GetComponentInChildren<Animation>();
    //        if (animation != null && animation.clip != null)
    //            animation.Play();
    //        else
    //            Debug.Log("No animation found in GLB.");

    //        onSuccess?.Invoke(result);
    //    }
    //}  

    private IEnumerator DownloadGLBModel(string url, Action<GameObject> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("GLB Download Failed: " + request.error);
                onError?.Invoke(request.error);
                yield break;
            }

            byte[] glbData = request.downloadHandler.data;

            ImportOptions importOptions = new ImportOptions
            {
                DataLoader = new MemoryLoader(glbData),
                AsyncCoroutineHelper = gameObject.AddComponent<AsyncCoroutineHelper>()
            };

            var importer = new GLTFSceneImporter("memory", importOptions);

            Task loadTask = importer.LoadSceneAsync(-1); // Load default scene index
            while (!loadTask.IsCompleted)
            {
                yield return null;
            }

            if (loadTask.Exception != null)
            {
                Debug.LogError("Failed to load GLB model: " + loadTask.Exception);
                onError?.Invoke("Failed to load GLB model.");
                yield break;
            }

            GameObject loadedModel = importer.LastLoadedScene;

            if (loadedModel == null)
            {
                onError?.Invoke("Loaded scene is null.");
                yield break;
            }

            loadedModel.transform.forward = -Vector3.forward;
            onSuccess?.Invoke(loadedModel);
        }
    }



}
