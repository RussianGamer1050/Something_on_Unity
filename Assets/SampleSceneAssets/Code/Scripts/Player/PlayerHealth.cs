using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public CanvasGroup blackScreen; // Reference to a CanvasGroup for fade effect
    public float fadeDuration = 1f; // Duration of the fade effect

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        // Disable player controls or trigger game over
        GetComponent<CharacterController>().enabled = false;
        StartCoroutine(EndGame());
    }

    private System.Collections.IEnumerator EndGame()
    {
        // Fade to black
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            blackScreen.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        blackScreen.alpha = 1f;

        // Stop the game
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }
}
