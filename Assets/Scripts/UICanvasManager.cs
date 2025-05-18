using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UICanvasManager : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonRetry;
    [SerializeField] private Button buttonReset;
    [SerializeField] private Button buttonLoad;
    [SerializeField] private TMP_Dropdown dropdownDataType;
    [Header("Panels")]
    [SerializeField] private GameObject panelStart;
    [SerializeField] private GameObject panelDemonstration;    
    [Header("CPanels")]
    [SerializeField] private GameObject cPanelTxt;
    [SerializeField] private GameObject cPanelImage;
    [SerializeField] private GameObject cPanelVideo;
    [SerializeField] private GameObject cPanelThreeDModel;
    [SerializeField] private GameObject cPanelFailedToLoadData;
    [SerializeField] private GameObject cPanelLoadingScreen;
    [Header("Other")]
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private Image fetchedImage;
    [SerializeField] private TextMeshProUGUI fetchedText;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject threeDModelParent;

    private Action _loadButtonAction;

    private enum Panels
    {
        Starting,
        Demonstration
    }

    private void Start()
    {
        dropdownDataType.onValueChanged.AddListener(CPanelDemonstrationManager);
        buttonLoad.onClick.AddListener(() => 
        {            
            if (_loadButtonAction != null)
            {
                ShowHideLoadingScreen(true);
                _loadButtonAction();
            }                    
        });

        buttonRetry.onClick.AddListener(ResetScene);
        buttonReset.onClick.AddListener(ResetScene);
        buttonStart.onClick.AddListener(() => ManagePanels(Panels.Demonstration));
        

        ShowHideLoadingScreen(false);
        CPanelDemonstrationManager(0);
        ManagePanels(Panels.Starting);
    }

    private void ManagePanels(Panels Panel)
    {
        panelStart.SetActive(false);
        panelDemonstration.SetActive(false);

        switch (Panel)
        {
            case Panels.Starting:
                panelStart.SetActive(true);
                break;
            case Panels.Demonstration:
                panelDemonstration.SetActive(true);
                break;
        }
    }
    private void ResetScene()
    {
        APIManager.Instance.StopAllFetching();
        
        fetchedImage.sprite = null;
        fetchedImage.gameObject.SetActive(false);
        
        fetchedText.text = string.Empty;

        videoDisplay.gameObject.SetActive(false);
        videoPlayer.url = string.Empty;
        videoPlayer.Stop();

        foreach (Transform child in threeDModelParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CPanelDemonstrationManager(int Value)
    {
        ResetScene();
        cPanelTxt.SetActive(false);
        cPanelImage.SetActive(false);
        cPanelVideo.SetActive(false);
        cPanelThreeDModel.SetActive(false);
        cPanelFailedToLoadData.SetActive(false);
        APIManager.Instance.StopAllFetching();
        _loadButtonAction = null;

        if (Value == 0)
        {
            cPanelTxt.SetActive(true);

            _loadButtonAction = () =>
            {
                APIManager.Instance.GetTextFromServer(
                onSuccess: FetchedText =>
                {
                    ShowHideLoadingScreen(false);
                    fetchedText.text = FetchedText;
                },
                onError: ErrorMessage =>
                {
                    ShowHideLoadingScreen(false);
                    cPanelFailedToLoadData.SetActive(true);
                });
            };
        }
        else if (Value == 1)
        {
            cPanelImage.SetActive(true);

            _loadButtonAction = () =>
            {
                APIManager.Instance.GetImageFromServer(
                onSuccess: FetchedImage =>
                {
                    ShowHideLoadingScreen(false);
                    fetchedImage.gameObject.SetActive(true);
                    fetchedImage.sprite = FetchedImage;
                    fetchedImage.SetNativeSize();
                },
                onError: ErrorMessage =>
                {
                    ShowHideLoadingScreen(false);
                    cPanelFailedToLoadData.SetActive(true);
                });
            };
        }
        else if (Value == 2)
        {
            cPanelVideo.SetActive(true);

            _loadButtonAction = () =>
            {
                ShowHideLoadingScreen(false);
                videoDisplay.gameObject.SetActive(true);
                videoPlayer.url = APIManager.Instance.GetVideoURL();
                videoPlayer.Play();
            };
        }
        else if (Value == 3)
        {
            cPanelThreeDModel.SetActive(true);

            _loadButtonAction = () =>
            {
                APIManager.Instance.GetGLBModel(
                onSuccess: FetchedModel =>
                {
                    FetchedModel.transform.forward = -Vector3.forward;
                    FetchedModel.transform.SetParent(threeDModelParent.transform);
                    ShowHideLoadingScreen(false);
                },
                onError: ErrorMessage =>
                {
                    ShowHideLoadingScreen(false);
                    cPanelFailedToLoadData.SetActive(true);
                });
            };
        }
    }

    private void ShowHideLoadingScreen(bool Active)
    {
        cPanelLoadingScreen.SetActive(Active);
    }

}
