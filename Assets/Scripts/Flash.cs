using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Flash : MonoBehaviour {

    // How long (in seconds) it takes for the red to come in.
    public float fadeInTime = .1f;

    // How long (in seconds) it takes for the red to disappear.
    public float fadeOutTime = 1;

    // The maximum alpha value the red color reaches (ranges from 0 to 1).
    public float maxAlpha = .5f;

    // How long (in seconds) it takes to fade the screen to black upon death.
    public float deathTime = 3f;

    // The Image component of the Canvas.
    private Image image;

    // Stores whatever coroutine is running currently, or null if none.
    private Coroutine storedCoroutine = null;

	// Use this for initialization
	void Start () {
        // Get the Image component.
        image = GetComponent<Image>();
	}

    // Starts a coroutine that flashes the screen red.
    public void DamageFlash()
    {
        // If a flashing coroutine is already playing...
        if (storedCoroutine != null)
            // Stop it prematurely (we'll interrupt it with this one).
            StopCoroutine(storedCoroutine);
        
        // Start the new coroutine and store it.
        storedCoroutine = StartCoroutine(DamageRoutine());
    }

    // The corutine played by DamageFlash.
    IEnumerator DamageRoutine()
    {
        // Whenever we yield this wait obj, the coroutine will sleep for 25 milliseconds.
        WaitForSeconds wait = new WaitForSeconds(.025f);

        // Begin fading in. The while loop runs for fadeInTime seconds,
        float anchorTime = Time.time;
        while (Time.time < anchorTime + fadeInTime) {
            // Grab the current image color.
            Color c = image.color;

            // Linearly interpolate alpha between 0 and maxAlpha, given the current elapsed time and fadeInTime.
            c.a = Mathf.Lerp(0, maxAlpha, (Time.time - anchorTime) / fadeInTime);

            // Set image color.
            image.color = c;

            // Sleep.
            yield return wait;
        }

        // Begin fading out. The while loop runs for fadeOutTime seconds.
        anchorTime = Time.time;
        while (Time.time < anchorTime + fadeOutTime) {
            // Grab the current image color.
            Color c = image.color;

            // Linearly interpolate alpha between maxAlpha and 0, given the current elapsed time and fadeOutTime.
            c.a = maxAlpha - Mathf.Lerp(0, maxAlpha, (Time.time - anchorTime) / fadeOutTime);

            // Set image color.
            image.color = c;

            // Sleep.
            yield return wait;
        }

        // When the coroutine has finished, remove it from storage.
        storedCoroutine = null;
    }

    // Starts a coroutine that fades the screen to black.
    public void DeathFade()
    {
        storedCoroutine = StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        // If a damage flash is happening, first wait for it to finish.
        WaitForSeconds wait = new WaitForSeconds(.025f);
        while (storedCoroutine != null)
            yield return wait;

        // Set the initial time and also create a new black color that's initially transparent.
        float anchorTime = Time.time;
        Color c = Color.black;
        c.a = 0; // 'a' represents its alpha value.

        // Run this loop for deathTime seconds.
        while (Time.time < anchorTime + deathTime) {
            // Linearly interpolate alpha between 0 and 1, given the elapsed time and deathTime.
            c.a = Mathf.Lerp(0, 1, (Time.time - anchorTime) / deathTime);

            // Set the image color.
            image.color = c;

            // Adjust the volume with a similar interpolation (except inverted so it gets softer).
            AudioListener.volume = 1 - Mathf.Lerp(0, 1, (Time.time - anchorTime) / deathTime);

            // Sleep.
            yield return wait;
            
        }

        // Restart up the scene anew. Don't forget to reset the sound too!
        SceneManager.LoadScene("Lab");
        AudioListener.volume = 1;
    }
}
