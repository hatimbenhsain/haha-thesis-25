using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class MainAct11CinematicsManager : MonoBehaviour
{
    public ObiRopeExtrudedRenderer MCRope;
    public MeshRenderer MCMeshRenderer;
    public Transform lidOrigin;
    public Rigidbody lidRigidbody; 
    public bool hideOrgansAtStart;

    // Start is called before the first frame update
    void Start()
    {
        if (MCRope != null && hideOrgansAtStart)
        {
            MCRope.enabled = false;
        }

        if (MCMeshRenderer != null && hideOrgansAtStart)
        {
            MCMeshRenderer.enabled = false;
        }
    }
 

    public void ActivateMCOrgan()
    {
        if (MCRope != null)
        {
            MCRope.enabled = true;
        }

        if (MCMeshRenderer != null)
        {
            MCMeshRenderer.enabled = true;
        }
    }
    public void ShootLid(){
        StartCoroutine(ShootLidCoroutine(1f, 0.0002f, 10f));
        
    }

    public IEnumerator ShootLidCoroutine(float duration, float shakeIntensity, float forceAmount)
    {

        Vector3 originalPosition = lidOrigin.localPosition; // Store the original position of the lid
        float elapsedTime = 0f;

        // Shake the lidOrigin for the specified duration
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Generate random shake offsets
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetZ = Random.Range(-shakeIntensity, shakeIntensity);

            // Apply the shake offsets to the lidOrigin's position
            lidOrigin.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            yield return null;
        }

        // Reset the lidOrigin's position to its original position
        //lidOrigin.localPosition = originalPosition;

        lidRigidbody.isKinematic = false; // Set the Rigidbody to be non-kinematic to allow physics interactions
        // Apply force in the positive X direction to the Rigidbody
        lidRigidbody.AddForce(Vector3.right * forceAmount*6f, ForceMode.Impulse);
        lidRigidbody.AddTorque(Vector3.right * forceAmount*0.3f, ForceMode.Impulse); // Add torque to the Rigidbody for rotation
        lidRigidbody.AddTorque(Vector3.back * forceAmount*0.1f, ForceMode.Impulse); // Add torque to the Rigidbody for rotation
        lidRigidbody.AddTorque(Vector3.down * forceAmount*0.1f, ForceMode.Impulse); // Add torque to the Rigidbody for rotation
    }
}
