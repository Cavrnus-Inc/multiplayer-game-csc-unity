using UnityEngine;

namespace CavrnusSdk.Experimental.Environment
{
    public class CavrnusSyncCameraColor : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private CavrnusPropertyObject<Color> propertyObject;
        
        private void Start()
        {
            if (cam == null)
                cam = Camera.main;
            
            propertyObject.AwaitInitialize(this, _ => {
                propertyObject.BindProperty(this, OnPropertyUpdated);
            });
        }

        private void OnPropertyUpdated(Color color) => cam.backgroundColor = color;
        private void OnDestroy() => propertyObject.DisposeProperty(this, OnPropertyUpdated);
    }
}