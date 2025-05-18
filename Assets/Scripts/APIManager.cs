using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityGLTF;
using System.Threading.Tasks;
using GLTFast;
using System.Net.Http;

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
            Shader.Find("GLTF/GLTFStandard");
            Shader.Find("GLTF/GLTFMetallicRoughness");
            // Or manually load from Resources if needed:
            Resources.Load<Shader>("Shaders/GLTFStandard");


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

    public async void GetGLBModel(Action<GameObject> onSuccess, Action<string> onError)
    {
        //StartCoroutine(DownloadGLBModel(urlModel, onSuccess, onError));
        await DownloadGLBModelAsync(urlModel, onSuccess, onError);
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

            var importer = new GLTFSceneImporter("model.glb", importOptions);
            GameObject result = null;
            bool done = false;

            importer.LoadSceneAsync().ContinueWith(task =>
            {
                if (task.Exception == null)
                    result = importer.LastLoadedScene;
                else
                    Debug.LogException(task.Exception);

                done = true;
            });

            while (!done)
                yield return null;

            if (result == null)
            {
                string error = "Failed to load GLB model.";
                Debug.LogError(error);
                onError?.Invoke(error);
                yield break;
            }

            result.transform.forward = -Vector3.forward;

            var animation = result.GetComponentInChildren<Animation>();
            if (animation != null && animation.clip != null)
                animation.Play();
            else
                Debug.Log("No animation found in GLB.");

            onSuccess?.Invoke(result);
        }
    }

    public async Task DownloadGLBModelAsync(string url, Action<GameObject> onSuccess, Action<string> onError)
    {
        try
        {
            using UnityWebRequest www = UnityWebRequest.Get(url);
            www.downloadHandler = new DownloadHandlerBuffer();

            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone)
                await Task.Yield();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                onError?.Invoke($"Download error: {www.error}");
                return;
            }

            byte[] glbData = www.downloadHandler.data;

            var gltf = new GltfImport();
            bool success = await gltf.LoadGltfBinary(glbData, null, new ImportSettings());

            if (!success)
            {
                onError?.Invoke("Failed to load GLB data.");
                return;
            }

            GameObject modelGO = new GameObject("GLB_Model");
            bool instSuccess = await gltf.InstantiateMainSceneAsync(modelGO.transform);

            if (!instSuccess)
            {
                onError?.Invoke("Failed to instantiate GLB scene.");
                return;
            }

            // Optional: Try to play animation if available
            var animator = modelGO.GetComponent<Animation>() ?? modelGO.GetComponentInChildren<Animation>();
            if (animator != null && animator.clip != null)
            {
                animator.Play();
            }

            onSuccess?.Invoke(modelGO);
        }
        catch (Exception ex)
        {
            onError?.Invoke($"Exception: {ex.Message}");
        }
    }
}