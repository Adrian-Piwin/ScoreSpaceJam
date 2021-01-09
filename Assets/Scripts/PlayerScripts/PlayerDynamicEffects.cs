using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerDynamicEffects : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessing;
    [SerializeField] private float defaultChromaticAberration;
    [SerializeField] private float defaultVignette;
    [SerializeField] private float lerpSpeed;

    private PlayerMovement playerMovement;
    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Post Processing effect
        postProcessing.profile.TryGetSettings(out chromaticAberration);
        postProcessing.profile.TryGetSettings(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        TeleportEffects();
    }

    private void TeleportEffects()
    {

        // Lerp to vignette
        if (Mathf.Abs(defaultVignette * 2 - vignette.intensity.value) > 0.02f && playerMovement.isTeleporting)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, defaultVignette * 2, lerpSpeed);

        // Lerp to 0
        if (Mathf.Abs(defaultVignette - vignette.intensity.value) > 0.02f && !playerMovement.isTeleporting)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, defaultVignette, lerpSpeed);

        /*
        // Lerp to ChromaticAberration
        if (Mathf.Abs(defaultChromaticAberration * 2 - chromaticAberration.intensity.value) > 0.02f && playerMovement.isTeleporting)
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, defaultChromaticAberration * 2, lerpSpeed);

        // Lerp to 0
        if (Mathf.Abs(defaultChromaticAberration - chromaticAberration.intensity.value) > 0.02f && !playerMovement.isTeleporting)
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, defaultChromaticAberration, lerpSpeed);
        */
    }

}
