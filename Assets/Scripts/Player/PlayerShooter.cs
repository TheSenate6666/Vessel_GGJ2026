using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    public Transform shootingPoint; // The point where the arrow spawns
    public GameObject arrowPrefab;
    
    public GameObject beamPrefab;   // The Arrow prefab
    
    [Header("Settings")]
    public int dmg = 10;            // Your damage variable

    // This function is called by your Animation Event
    public void ShootArrow()
    {
        if (shootingPoint == null || arrowPrefab == null) return;

        // Create the arrow
        Debug.DrawRay(shootingPoint.position, shootingPoint.right * 2f, Color.yellow, 1f);
        GameObject arrow = Instantiate(arrowPrefab, shootingPoint.position, shootingPoint.rotation);
        
        // Pass the damage to the arrow script
       
        
    }


    public void ShootBeam()
    {
        if (shootingPoint == null || arrowPrefab == null) return;

        // Create the arrow
        Debug.DrawRay(shootingPoint.position, shootingPoint.right * 2f, Color.yellow, 1f);
        GameObject arrow = Instantiate(beamPrefab, shootingPoint.position, shootingPoint.rotation);
        
        // Pass the damage to the arrow script
       
        
    }
}