using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TestController : MonoBehaviour
{
    public Button[] buttons;
    InputField placeholderText;
    string[] customerStates = new string[3];
    private string audioUrl;

    private void Start()
    {
        customerStates[0] = "cs_men_products";
        customerStates[1] = "cs_men_explore";
        customerStates[2] = "cs_about_collection";

        placeholderText = GameObject.Find("PlaceHolderText").GetComponent<InputField>();
    }

    public void OnButton1Press(string customerState)
    {
        PostRequest(customerStates[0]);
    }

    public void OnButton2Press(string customerState)
    {
        PostRequest(customerStates[1]);
    }

    public void OnButton3Press(string customerState)
    {
        PostRequest(customerStates[2]);
    }

    [ContextMenu("Test Get")]
    public async void TestGet()
    {
        var httpClient = new HttpClient(new JsonSerializationOption());

        var result = await httpClient.Get<User>(RequestConstant.uri);
    }

    [ContextMenu("Test Post")]
    public async void PostRequest(string cs)
    {
        placeholderText.text = "Loading...";

        var httpClient = new HttpClient(new JsonSerializationOption());

        var result = await httpClient.Post<Root>(RequestConstant.uri, cs);
        placeholderText.text = result.placeholder;
            
        audioUrl = result.response_channels.voice;
        StartCoroutine(PlaySoundFromURL(audioUrl));

    }

    IEnumerator PlaySoundFromURL(string url)
    {
        AudioSource audio = GetComponent<AudioSource>();
        WWW www = new WWW(url);

        while (!www.isDone)
            yield return null;

        audio.clip = www.GetAudioClip(false, false);
        audio.Play();
    }
}
