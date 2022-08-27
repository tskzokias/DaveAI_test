using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HttpClient
{
    private readonly ISerializationOption serializationOption;

    public HttpClient(ISerializationOption serializationOption)
    {
        this.serializationOption = serializationOption;
    }

    public async Task<TResultType> Get<TResultType>(string url)
    {
        try
        {
            using var www = UnityWebRequest.Get(url);

            www.SetRequestHeader("Content-type", serializationOption.ContentType);
            
            var operation = www.SendWebRequest();
            
            // Wait until fully done
            while (!operation.isDone) 
                await Task.Yield();
            
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError($"Failed: {www.error}");

            var result = serializationOption.Deserialize<TResultType>(www.downloadHandler.text);
            return result;
       
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(Get)} failed: {e.Message}");
            return default;
        }
    }

    public async Task<TResultType> Post<TResultType>(string uri, string customerState)
    {
        try
        {
            // PostData Mandatory fillings
            PostData postdata = new PostData();
            postdata.systemResponse = "sr_init";
            postdata.engagementID = "NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyZXhoaWJpdF9hbGRv";
            postdata.customer_state = customerState;

            var webRequest = new UnityWebRequest(uri, "POST");
            
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(postdata.SaveToString());

            // low-level usage of UnityWebRequest
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("X-I2CE-ENTERPRISE-ID", "dave_expo");
            webRequest.SetRequestHeader("X-I2CE-USER-ID", "74710c52-42a5-3e65-b1f0-2dc39ebe42c2");
            webRequest.SetRequestHeader("X-I2CE-API-KEY", "NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyMTYwNzIyMDY2NiAzNw__");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + webRequest.error);
            }
            else
            {
                Debug.Log("response code: " + webRequest.responseCode);

                // pass your string json here
                var result = serializationOption.Deserialize<TResultType>(webRequest.downloadHandler.text);
                
                return result;
            }

            return default;
        }
        catch (Exception e)
        {
            Debug.Log($"{nameof(Post)} failed: {e.Message}");
            return default;
        }
    }
}
