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

    public GameObject planeSpawn;
    public GameObject houseToSpawn;

    private GameObject plane;
    private Vector3 previousLocation;
    public List<Vector3> locations;
    private float latestLocX = 0f;
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
                plane = Instantiate(planeSpawn, new Vector3(0f, 0f, 0f), new Quaternion());
                int counter = 0;
                foreach (UserObjectClass user in users.users)
                {
                    if (counter == locations.Count)
                    {
                        latestLocX = latestLocX + 15;
                        plane = Instantiate(planeSpawn, new Vector3(latestLocX, 0f, 0f), new Quaternion());
                        counter = 0;
                    }
                    DoXForUser(user, counter);
                    counter++;
                }
            }
        }
    }

    void DoXForUser(UserObjectClass user, int locationCounter)
    {
        if (locationCounter > locations.Count)
        {
            return;
        }

        Vector3 location = locations[locationCounter];
        Quaternion newrotation = new Quaternion();
        var house = Instantiate(houseToSpawn, new Vector3(), newrotation);
        house.transform.SetParent(plane.transform);
        house.transform.localPosition = location;
        var textmesh = house.GetComponentInChildren<TextMeshPro>();
        textmesh.SetText(user.name);
    }
}
