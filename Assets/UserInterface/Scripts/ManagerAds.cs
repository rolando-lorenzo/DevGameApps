using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerAds : MonoBehaviour {

    public Text CoinOrLive;
    private GameObject btnViewAds;
    private GameObject btnAddLive;
    private GameObject btnRestLive;

    private int CoinLive;

    // Use this for initialization
	void Start () {
        btnAddLive.GetComponent<Button>().onClick.AddListener(AddCoinLive);
        btnRestLive.GetComponent<Button>().onClick.AddListener(RestCoinLive);
        btnViewAds.GetComponent<Button>().onClick.AddListener(ShowAds);
    }

    // Update is called once per frame
    private void Awake()
    {
        btnAddLive = GameObject.Find("btn_addlive");
        btnRestLive = GameObject.Find("btn_restlive");
        btnViewAds = GameObject.Find("btn_viewads");

        AdsManagement.Instance.InitAds();
        CoinLive = PlayerPrefs.GetInt("coinlive", 0);
        CoinOrLive.text = "" + CoinLive;
        verifyCoin();
    }

    private void Update()
    {
        verifyCoin();
    }

    private void verifyCoin()
    {
        if (CoinLive == 0)
        {
            btnViewAds.SetActive(true);
        }
        else
        {
            btnViewAds.SetActive(false);
        }
    }

    public void AddCoinLive()
    {
        CoinLive += 1;
        PlayerPrefs.SetInt("coinlive", CoinLive);
        CoinOrLive.text = "" + CoinLive;
    }

    public void RestCoinLive()
    {
        CoinLive -= 1;
        if (CoinLive < 0)
        {
            CoinLive = 0;
        }

        PlayerPrefs.SetInt("coinlive", CoinLive);
        CoinOrLive.text = "" + CoinLive;
    }

    public void ShowAds()
    {
        AdsManagement.Instance.ShowVideo();
        ViewAds();
    }

    public void ViewAds()
    {
        if (AdsManagement.Instance.CoinLive != null)
        {
            CoinLive = CoinLive + Int32.Parse(AdsManagement.Instance.CoinLive);
            PlayerPrefs.SetInt("coinlive", CoinLive);
            CoinOrLive.text = "" + CoinLive;
        }
        else
        {
            StartCoroutine(WaitForAdsVideo());
        }
    }

    private IEnumerator WaitForAdsVideo()
    {
        while (AdsManagement.Instance.CoinLive == null)
        {
            yield return null;
        }

        ViewAds();
    }
}
