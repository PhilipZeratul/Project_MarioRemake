using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public List<SceneReference> sceneList = new List<SceneReference>();


    private void Start()
    {
        SceneManager.LoadSceneAsync(sceneList[0].ScenePath, LoadSceneMode.Additive);
    }
}
