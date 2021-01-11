using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerDynamicEffects : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessing;
    [SerializeField] private float defaultVignette;
    [SerializeField] private float lerpSpeed;

    private PlayerMovement playerMovement;
    private Vignette vignette;
    private bool startTeleport;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Post Processing effect
        postProcessing.profile.TryGetSettings(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        TeleportEffects();
    }

    private void TeleportEffects()
    {
        // DO on start of teleport
        if (playerMovement.isTeleporting && !startTeleport)
        {
            startTeleport = true;
            StartCoroutine(changeValueOverTime(0.75f, 0.05f, playerMovement.teleportTimeLimit * playerMovement.teleportingSlowMotion));
        }
        else if (!playerMovement.isTeleporting)
            startTeleport = false;

        // Lerp to vignette
        if (Mathf.Abs(defaultVignette * 2 - vignette.intensity.value) > 0.02f && playerMovement.isTeleporting)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, defaultVignette * 2, lerpSpeed * Time.deltaTime);

        // Lerp to 0
        if (Mathf.Abs(defaultVignette - vignette.intensity.value) > 0.02f && !playerMovement.isTeleporting)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, defaultVignette, lerpSpeed * Time.deltaTime);

    }

    IEnumerator changeValueOverTime(float fromVal, float toVal, float duration)
    {
        Color color = playerMovement.teleportDragObj.GetComponent<SpriteRenderer>().color;

        float counter = 0f;

        while (counter < duration)
        {
            // Check if player is still teleporting
            if (!playerMovement.isTeleporting) break;

            if (Time.timeScale == 0)
                counter += Time.unscaledDeltaTime;
            else
                counter += Time.deltaTime;

            float val = Mathf.Lerp(fromVal, toVal, counter / duration);

            color.a = val;
            playerMovement.teleportDragObj.GetComponent<SpriteRenderer>().color = color;

            yield return null;
        }
    }

}
