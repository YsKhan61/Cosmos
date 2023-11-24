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

    // Use UniTask to run the update loop in a thread
    // void Start()
    // {
    //     UniTask.Run(() =>
    //     {
    //         while (true)
    //         {
    //             fighterShip.forward = Vector3.forward;
    //         }
    //     });
    // }


}
