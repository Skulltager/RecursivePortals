using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    [SerializeField] private MeshRenderer portalScreen = default;
    [SerializeField] private Portal linkedPortal = default;
    [SerializeField] private Camera portalCamera = default;
    [SerializeField] private int maxRenderDepth = default;

    private RenderTexture renderTexture;
    private Vector3 originalScale;
    private Vector3 originalLocation;

    private Coroutine animateHealthBar;
    
    private void Awake()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        portalCamera.enabled = false;
    }

    private void Start()
    {
        portalScreen.material.SetTexture("_MainTex", renderTexture);
        linkedPortal.portalCamera.targetTexture = renderTexture;
        originalScale = portalScreen.transform.localScale;
        originalLocation = portalScreen.transform.localPosition;
    }

    private void OnEnable()
    {
        PortalManager.instance.AddPortal(this);
    }

    private void OnDisable()
    {
        PortalManager.instance.RemovePortal(this);
    }

    private void RenderFromPortalCamera(Camera sourceCamera, RenderTexture targetTexture, int depth)
    {
        Transform cameraParent = sourceCamera.transform.parent;
        sourceCamera.transform.parent = linkedPortal.transform;

        portalCamera.transform.localPosition = sourceCamera.transform.localPosition;
        portalCamera.transform.localRotation = sourceCamera.transform.localRotation;

        sourceCamera.transform.parent = cameraParent;

        SetNearClipPlane();
        portalScreen.enabled = false;
        portalCamera.Render();
        portalScreen.enabled = true;
    }

    private void SetNearClipPlane()
    {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf

        int dot = (int) Mathf.Sign(Vector3.Dot(transform.right, transform.position - portalCamera.transform.position));
        Vector3 camSpacePos = portalCamera.worldToCameraMatrix.MultiplyPoint(transform.position);
        Vector3 camSpaceNormal = portalCamera.worldToCameraMatrix.MultiplyVector(transform.right) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + portalCamera.nearClipPlane;
    
        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > Camera.main.nearClipPlane)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            portalCamera.projectionMatrix = portalCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            portalCamera.projectionMatrix = Camera.main.projectionMatrix;
        }
    }

    private void UpdateRenderTexture()
    {
        if (renderTexture.width == Screen.width && renderTexture.height == Screen.height)
            return;
        
        renderTexture.Release();

        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        linkedPortal.portalCamera.targetTexture = renderTexture;
        portalScreen.material.SetTexture("_MainTex", renderTexture);
    }

    public void RenderPortal(Camera otherCamera, int depth)
    {
        if (depth == maxRenderDepth)
            return;

        depth++;
        UpdateRenderTexture();
        linkedPortal.RenderFromPortalCamera(otherCamera, renderTexture, depth);
    }

    private void OnTriggerEnter(Collider other)
    {
        PortalTraveler portalTraveler = other.GetComponent<PortalTraveler>();
        if (portalTraveler == null)
            return;

        portalTraveler.StartPortalTravel(this, linkedPortal);
    }

    private void OnTriggerExit(Collider other)
    {
        PortalTraveler portalTraveler = other.GetComponent<PortalTraveler>();
        if (portalTraveler == null)
            return;

        portalTraveler.StopPortalTravel();
    }

    public void UpdatePortalRendererSize(Camera mainCamera)
    {
        //// Code from Coding Adventure Portals by Sebastian Lague 
        float halfHeight = mainCamera.nearClipPlane * Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * mainCamera.aspect;
        float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, mainCamera.nearClipPlane).magnitude;
        
        Transform screenT = portalScreen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot(transform.right, transform.position - mainCamera.transform.position) > 0;
        screenT.localScale = new Vector3(dstToNearClipPlaneCorner, screenT.localScale.y, screenT.localScale.z);
        screenT.localPosition = Vector3.right * dstToNearClipPlaneCorner * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
    }

    private bool IsPointInBoxCollider(Vector3 point, BoxCollider box)
    {
        point = box.transform.InverseTransformPoint(point) - box.center;

        float halfX = (box.size.x * 0.5f);
        float halfY = (box.size.y * 0.5f);
        float halfZ = (box.size.z * 0.5f);
        if (point.x < halfX && point.x > -halfX &&
           point.y < halfY && point.y > -halfY &&
           point.z < halfZ && point.z > -halfZ)
            return true;
        else
            return false;
    }

}