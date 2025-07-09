using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityGLTF;
using System.Threading.Tasks;
using GLTFast;


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
        //await DownloadGLBModelAsync(urlModel, onSuccess, onError);
        //StartCoroutine(DownloadGLBModelCoroutine(urlModel, onSuccess, onError));
        StartCoroutine(DownloadGLBModelCoroutine("https://sandbox-immarsify-data-bucket.s3.ap-south-1.amazonaws.com/raj9116/testing-by-anuj/model/untitled.glb", onSuccess, onError));
    }

    public string GetVideoURL()
    {
        Debug.Log("The Video Is Loaded");
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
    private IEnumerator DownloadGLBModelCoroutine(string url, Action<GameObject> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("GLTF source URL is empty");
            onError?.Invoke("URL Is Empty");
            yield break;
        }


        var gltf = new GltfImport();
        var loadTask = gltf.Load(url);

        while (!loadTask.IsCompleted)
        {
            yield return null;
        }

        if (!loadTask.Result)
        {
            onError?.Invoke("Failed to load GLB data.");
            yield break;
        }

        GameObject modelGO = new GameObject("GLB_Model");

        var instTask = gltf.InstantiateMainSceneAsync(modelGO.transform);
        while (!instTask.IsCompleted)
        {
            yield return null;
        }

        if (!instTask.Result)
        {
            onError?.Invoke("Failed to instantiate GLB scene.");
            yield break;
        }

        Debug.Log("3D Model Is Loaded");

        // Optional: Try to play animation if available
        UnityEngine.Animation animator = modelGO.GetComponent<UnityEngine.Animation>() ?? modelGO.GetComponentInChildren<UnityEngine.Animation>();
        if (animator != null && animator.clip != null)
        {
            animator.Play();
        }

        onSuccess?.Invoke(modelGO);
    }

    //private IEnumerator DownloadGLBModelCoroutine(string url, Action<GameObject> onSuccess, Action<string> onError)
    //{
    //    using UnityWebRequest www = UnityWebRequest.Get(url);
    //    www.downloadHandler = new DownloadHandlerBuffer();
    //    yield return www.SendWebRequest();

    //    if (www.result != UnityWebRequest.Result.Success)
    //    {
    //        onError?.Invoke($"Download error: {www.error}");
    //        yield break;
    //    }

    //    byte[] glbData = www.downloadHandler.data;

    //    var gltf = new GLTFast.GltfImport(); // Ensure you have GLTFast imported and used properly

    //    var loadTask = gltf.LoadGltfBinary(glbData, null, new GLTFast.ImportSettings());
    //    while (!loadTask.IsCompleted)
    //        yield return null;

    //    if (!loadTask.Result)
    //    {
    //        onError?.Invoke("Failed to load GLB data.");
    //        yield break;
    //    }

    //    GameObject modelGO = new GameObject("GLB_Model");

    //    var instTask = gltf.InstantiateMainSceneAsync(modelGO.transform);
    //    while (!instTask.IsCompleted)
    //        yield return null;

    //    if (!instTask.Result)
    //    {
    //        onError?.Invoke("Failed to instantiate GLB scene.");
    //        yield break;
    //    }

    //    Debug.Log("3D Model Is Loaded");

    //    // Optional: Try to play animation if available
    //    var animator = modelGO.GetComponent<Animation>() ?? modelGO.GetComponentInChildren<Animation>();
    //    if (animator != null && animator.clip != null)
    //    {
    //        animator.Play();
    //    }

    //    onSuccess?.Invoke(modelGO);
    //}

    //public async Task DownloadGLBModelAsync(string url, Action<GameObject> onSuccess, Action<string> onError)
    //{
    //    try
    //    {
    //        using UnityWebRequest www = UnityWebRequest.Get(url);
    //        www.downloadHandler = new DownloadHandlerBuffer();

    //        var asyncOp = www.SendWebRequest();
    //        while (!asyncOp.isDone)
    //            await Task.Yield();

    //        if (www.result != UnityWebRequest.Result.Success)
    //        {
    //            onError?.Invoke($"Download error: {www.error}");
    //            return;
    //        }

    //        byte[] glbData = www.downloadHandler.data;

    //        var gltf = new GltfImport();
    //        bool success = await gltf.LoadGltfBinary(glbData, null, new ImportSettings());

    //        if (!success)
    //        {
    //            onError?.Invoke("Failed to load GLB data.");
    //            return;
    //        }

    //        GameObject modelGO = new GameObject("GLB_Model");
    //        bool instSuccess = await gltf.InstantiateMainSceneAsync(modelGO.transform);

    //        if (!instSuccess)
    //        {
    //            onError?.Invoke("Failed to instantiate GLB scene.");
    //            return;
    //        }

    //        Debug.Log("3D Model Is Loaded");

    //        // Optional: Try to play animation if available
    //        var animator = modelGO.GetComponent<Animation>() ?? modelGO.GetComponentInChildren<Animation>();
    //        if (animator != null && animator.clip != null)
    //        {
    //            animator.Play();
    //        }

    //        onSuccess?.Invoke(modelGO);
    //    }
    //    catch (Exception ex)
    //    {
    //        onError?.Invoke($"Exception: {ex.Message}");
    //    }
    //}

}