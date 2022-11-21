using UnityEngine;

/// <summary>
/// AUTHOR: @Daniel K.
/// </summary>
public class DeathHandler : MonoBehaviour
{
    /* EXPOSED FIELDS: */
    [Header("CANVAS: ")]
    [SerializeField] private Canvas gameOverCanvas;
    
    /* HIDDEN FIELDS: */
    // Animator Hash
    private static readonly int Die = Animator.StringToHash("Die");
    
    /* METHODS: */
    private void Start()
    {
        gameOverCanvas.enabled = false;
    }
    
    /// Method is called on player's death. Enables the Game Over canvas.
    public void HandleDeath()
    {
        gameOverCanvas.enabled = true;
        Time.timeScale = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        
        GetComponent<Animator>().SetTrigger(Die);
    }
}
