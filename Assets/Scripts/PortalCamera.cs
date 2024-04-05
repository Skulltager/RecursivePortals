using UnityEngine;

public class PortalCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCamera = default;
    
    void OnPreCull()
    {
        for (int i = 0; i < PortalManager.instance.portals.Count; i++)
            PortalManager.instance.portals[i].UpdatePortalRendererSize(mainCamera);
        
        for (int i = 0; i < PortalManager.instance.portals.Count; i++)
            PortalManager.instance.portals[i].RenderPortal(mainCamera, 0);
    }
}