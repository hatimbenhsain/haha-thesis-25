using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VLB;

public class LightBeamFollow : MonoBehaviour
{
    public Transform lightBeamTransform;
    public Transform handTransform;
    public float lerpSpeed;
    public float deltaY;

    private float prevShotIndex;
    public Transform fingertipTransform;
    public bool hovering;
    public Color option1Color;
    public Color option2Color;
    public Color option3Color;
    public float targetIntensity;
    public float targetParticleSize;
    public float lightLerpSpeed;
    public Light spotLight;
    public VolumetricLightBeamHD volumetricLightBeamHD;
    public VolumetricDustParticles volumetricDustParticles;
    private Color initialColor;
    private float initialIntensity;
    private float initialParticleSize;
    // Start is called before the first frame update
    public Transform[] transforms;
    private CuddleCameraManager cuddleCameraManager;
    void Awake()
    {
        cuddleCameraManager = FindObjectOfType<CuddleCameraManager>();
        spotLight = GetComponent<Light>();
        volumetricLightBeamHD = GetComponent<VolumetricLightBeamHD>();
        volumetricDustParticles = GetComponent<VolumetricDustParticles>();
        deltaY = lightBeamTransform.position.y - handTransform.position.y;
        //StartCoroutine(ResetLightBeam());
        initialColor = spotLight.color;
        initialIntensity = spotLight.intensity;
        initialParticleSize = volumetricDustParticles.size;
    }

    // Update is called once per frame
    void Update()
    {
        if (handTransform != null)
        {
            Vector3 direction = handTransform.position - lightBeamTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            lightBeamTransform.rotation = Quaternion.Lerp(lightBeamTransform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
        }

        if (!hovering)
        {
            // Reset light beam to initial state if not hovering over any option box
            if (spotLight != null){
                spotLight.color = Color.Lerp(spotLight.color, initialColor, lightLerpSpeed * Time.deltaTime);
                spotLight.intensity = Mathf.Lerp(spotLight.intensity, initialIntensity, lightLerpSpeed * Time.deltaTime);
            } 
            
            if (volumetricDustParticles != null){
                volumetricDustParticles.size = Mathf.Lerp(volumetricDustParticles.size, initialParticleSize, lightLerpSpeed * Time.deltaTime);
                    }
        }
        if (cuddleCameraManager.shotIndex != prevShotIndex)
        {
            prevShotIndex = cuddleCameraManager.shotIndex;
            StartCoroutine(ResetLightBeam());
        }
        prevShotIndex = cuddleCameraManager.shotIndex;

    }
    
    public void ActivateLightBeam(int colorIndex){
        Color targetColor = Color.white;
        switch (colorIndex)
        {
            case 0:
                targetColor = option1Color;
                break;
            case 1:
                targetColor = option2Color;
                break;
            case 2:
                targetColor = option3Color;
                break;
            default:
                targetColor = initialColor;
                break;
        }
        if (spotLight!=null){
            spotLight.color = Color.Lerp(spotLight.color, targetColor, lightLerpSpeed * Time.deltaTime);
            spotLight.intensity = Mathf.Lerp(spotLight.intensity, targetIntensity, lightLerpSpeed * Time.deltaTime);
        }
        if (volumetricDustParticles != null){
            volumetricDustParticles.size = Mathf.Lerp(volumetricDustParticles.size, targetParticleSize, lightLerpSpeed * Time.deltaTime);
        }
      }

    public IEnumerator ResetLightBeam(){
        yield return new WaitForSeconds(0.2f);
        transform.position = transforms[cuddleCameraManager.shotIndex].position;
    }
}
