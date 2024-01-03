using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private int targetSceneIndex;
    private void Start()
    {
        FighterLoader.Instance.OnAllReplaysFinished += FighterLoader_OnAllReplaysFinished;
        FaderScreenManager.Instance.OnFadedOut += FaderScreenManager_OnFadedOut;
    }

    private void FaderScreenManager_OnFadedOut(object sender, System.EventArgs e)
    {
        SceneManager.LoadScene(targetSceneIndex);
    }

    private void FighterLoader_OnAllReplaysFinished(object sender, System.EventArgs e)
    {
        FaderScreenManager.Instance.FadeOut();
    }
}
