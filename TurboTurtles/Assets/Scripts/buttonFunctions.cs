using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HPPlus()
    {
        if (gamemanager.instance.points > 0 && gamemanager.instance.playerScript.HP < 200)
        {
            gamemanager.instance.playerScript.HPOrig += 10;
            gamemanager.instance.playerScript.HP = gamemanager.instance.playerScript.HPOrig;

            gamemanager.instance.points--;

            gamemanager.instance.stateUnpause();
        }
    }

    public void SpeedPlus()
    {
        if (gamemanager.instance.points > 0 && gamemanager.instance.playerScript.speed < 20)
        {
            gamemanager.instance.playerScript.speedOrig += 1;

            gamemanager.instance.points--;

            gamemanager.instance.stateUnpause();
        }
    }

    public void JumpPlus()
    {
        if (gamemanager.instance.points > 0 && gamemanager.instance.playerScript.jumpMax < 10)
        {
            gamemanager.instance.playerScript.jumpMax += 1;

            gamemanager.instance.points--;

            gamemanager.instance.stateUnpause();
        }
    }

}
