using UnityEngine;

// [ExecuteAlways]
public class CockpitHUD : MonoBehaviour
{
    
    [Header("Fighter Ship")]
    public Transform fighterShip;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fighterShip.forward = Vector3.forward;
    }
}
