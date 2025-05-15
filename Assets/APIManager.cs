using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class APIManagerMedia : MonoBehaviour
{
    [System.Serializable]
    public class ContentItem
    {
        public string type;
        public string name;
        public string url;
        public float[] position;
    }

    [System.Serializable]
    public class ContentListWrapper
    {
        public ContentItem[] contents;
    }

    string url = "https://raw.githubusercontent.com/anuj-chouhan/Unity-Ar-Assets/main/HostedStuffs/data.json";

    [Header("UI References")]
    public TextMeshProUGUI uiText;
    public Image uiImage;
    public VideoPlayer videoPlayer;

    private void Start() //But this is not!!! while i'm passing the same URL
    {
        StartCoroutine(FetchAndDisplayContent());
    }

    private IEnumerator FetchAndDisplayContent()
    {
        UnityWebRequest jsonRequest = UnityWebRequest.Get(url);
        yield return jsonRequest.SendWebRequest();

        if (jsonRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load JSON: " + jsonRequest.error);
            yield break;
        }

        string json = jsonRequest.downloadHandler.text;
        ContentListWrapper contentList = JsonUtility.FromJson<ContentListWrapper>(json);

        if (contentList == null || contentList.contents == null)
        {
            Debug.LogError("Failed to parse JSON or empty contents");
            yield break;
        }

        foreach (var content in contentList.contents)
        {
            switch (content.type.ToLower())
            {
                case "text":
                    yield return StartCoroutine(LoadText(content));
                    break;
                case "image":
                    yield return StartCoroutine(LoadImage(content));
                    break;
                case "video":
                    LoadVideo(content);
                    break;
                default:
                    Debug.LogWarning($"Unknown content type: {content.type}");
                    break;
            }
        }
    }

    private IEnumerator LoadText(ContentItem content)
    {
        UnityWebRequest textRequest = UnityWebRequest.Get(content.url);
        yield return textRequest.SendWebRequest();

        if (textRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to load text '{content.name}': " + textRequest.error);
        }
        else
        {
            string loadedText = textRequest.downloadHandler.text;
            Debug.Log($"Loaded text ({content.name}):\n{loadedText}");
            uiText.text = loadedText;
        }
    }

    private IEnumerator LoadImage(ContentItem content)
    {
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(content.url);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to load image '{content.name}': " + textureRequest.error);
        }
        else
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(textureRequest);
            uiImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            Debug.Log($"Loaded image ({content.name})");
        }
    }

    private void LoadVideo(ContentItem content)
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer reference not assigned.");
            return;
        }

        videoPlayer.url = content.url;
        videoPlayer.Play();
        Debug.Log($"Playing video ({content.name}) from URL: {content.url}");
    }
}



//using System.Collections;
//using UnityEngine;
//using UnityEngine.Networking;

//public class APIManager : MonoBehaviour
//{

//    [System.Serializable]
//    public class ContentItem
//    {
//        public string type;
//        public string name;
//        public string url;
//        public float[] position;
//    }

//    [System.Serializable]
//    public class ContentListWrapper
//    {
//        public ContentItem[] contents;
//    }


//    private string apiUrl = "https://raw.githubusercontent.com/anuj-chouhan/Unity-Ar-Assets/main/data.json";

//    void Start()
//    {
//        StartCoroutine(FetchAndDisplayText());
//    }

//    IEnumerator FetchAndDisplayText()
//    {
//        UnityWebRequest jsonRequest = UnityWebRequest.Get(apiUrl);
//        yield return jsonRequest.SendWebRequest();

//        if (jsonRequest.result != UnityWebRequest.Result.Success)
//        {
//            Debug.LogError("Failed to load JSON: " + jsonRequest.error);
//            yield break;
//        }

//        string json = jsonRequest.downloadHandler.text;

//        // Parse JSON using JsonUtility
//        ContentListWrapper contentList = JsonUtility.FromJson<ContentListWrapper>(json);

//        if (contentList == null || contentList.contents == null)
//        {
//            Debug.LogError("Failed to parse JSON or empty contents");
//            yield break;
//        }

//        foreach (var content in contentList.contents)
//        {
//            if (content.type.ToLower() == "text")
//            {
//                UnityWebRequest textRequest = UnityWebRequest.Get(content.url);
//                yield return textRequest.SendWebRequest();

//                if (textRequest.result != UnityWebRequest.Result.Success)
//                {
//                    Debug.LogError("Failed to load text: " + textRequest.error);
//                }
//                else
//                {
//                    string loadedText = textRequest.downloadHandler.text;
//                    Debug.Log($"Loaded text ({content.name}):\n{loadedText}");

//                    print(loadedText);
//                }
//            }
//        }
//    }
//}
