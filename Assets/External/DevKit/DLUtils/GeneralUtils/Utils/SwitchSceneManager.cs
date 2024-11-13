using UnityEngine.SceneManagement;

public class SwitchSceneManager
{
    public static void SceneChangerUtils(string sceneName)
    {
        PHUtils.SendMessageToFlutter(ConstDependency.SceneLoaded, sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
