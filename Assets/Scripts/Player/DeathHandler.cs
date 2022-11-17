using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class DeathHandler : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;
    /* Animator */
    private static readonly int Die = Animator.StringToHash("Die");
    
    private void Start()
    {
        gameOverCanvas.enabled = false;
    }

    public void HandleDeath()
    {
        gameOverCanvas.enabled = true;
        Time.timeScale = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
        GetComponent<Animator>().SetTrigger(Die);
    }
}
