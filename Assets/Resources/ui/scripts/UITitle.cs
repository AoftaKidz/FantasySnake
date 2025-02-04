using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UITitle : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void OnStart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
           PlayGame();
        }
    }
}
