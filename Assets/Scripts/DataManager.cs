using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class UserList
{
    public List<UserObjectClass> users;
}

[System.Serializable]
public class UserObjectClass
{
    public int id;
    public string name;
    public string city;
    public string profession;
}

public class DataManager : MonoBehaviour
{
    public string unserialized_data;
    public UserList users;

    public GameObject houseToSpawn;
    void Start()
    {
        // A correct website page.
        StartCoroutine(GetRequest("http://127.0.0.1:5000/"));
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    unserialized_data = webRequest.error;
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    unserialized_data = webRequest.error;
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    unserialized_data = webRequest.downloadHandler.text;
                    users = JsonUtility.FromJson<UserList>(unserialized_data);
                    break;
            }
            if (users.users.Count > 0)
            {
                foreach (UserObjectClass user in users.users)
                {
                    DoXForUser(user);
                }
            }
        }
    }

    void DoXForUser(UserObjectClass user)
    {
        Vector3 location = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0f, 0f), Random.Range(-5.0f, 5.0f));
        Quaternion newrotation = new Quaternion();
        var house = Instantiate(houseToSpawn, location, newrotation);
        var textmesh = house.GetComponentInChildren<TextMeshPro>();
        textmesh.SetText(user.name);
    }
}
