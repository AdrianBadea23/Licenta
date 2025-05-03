using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Scene(String sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
