using UnityEngine;

public class SceneManager : MonoBehaviour
{
    #region Singleton / DnD on Load
    public static SceneManager Instance;

    void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }
    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    #endregion

    public void LoadScene(string scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}