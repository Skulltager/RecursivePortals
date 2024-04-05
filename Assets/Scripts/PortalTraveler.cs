using UnityEngine;

public class PortalTraveler : MonoBehaviour
{
    [SerializeField] private GameObject clone = default;
    [SerializeField] private Rigidbody rigidBody = default;

    private Portal fromPortal;
    private Portal toPortal;

    private bool isActive;
    private float previousDotProductValue;
    private Vector3 lastPositionCalculated;

    private void Awake()
    {
        clone.SetActive(false);
        isActive = false;
    }

    private void Update()
    {
        if (!isActive)
            return;

        UpdateClonePosition();

        if (DidMoveThroughPortal())
            SwapPositionWithClone();
    }

    private void FixedUpdate()
    {
        if (!isActive)
            return;

        UpdateClonePosition();

        if (DidMoveThroughPortal())
            SwapPositionWithClone();
    }

    private bool DidMoveThroughPortal()
    {
        Vector3 forward = fromPortal.transform.right;
        Vector3 distance = fromPortal.transform.position - transform.position;
        float currentValue = Vector3.Dot(forward, distance);
        float previousValue = previousDotProductValue;
        
        previousDotProductValue = currentValue;
        if (previousValue >= 0 && currentValue >= 0)
            return false;

        if (previousValue < 0 && currentValue < 0)
            return false;

        return true;
    }

    private void SwapPositionWithClone()
    {
        Vector3 clonePosition = clone.transform.position;
        Quaternion cloneRotation = clone.transform.rotation;

        clone.transform.position = transform.position;
        clone.transform.rotation = transform.rotation;

        transform.position = clonePosition;
        transform.rotation = cloneRotation;

        rigidBody.velocity = toPortal.transform.rotation * (Quaternion.Inverse(fromPortal.transform.rotation) * rigidBody.velocity);
        Portal tempPortal = fromPortal;
        fromPortal = toPortal;
        toPortal = tempPortal;
    }

    private void UpdateClonePosition()
    {
        Transform oldParent = transform.parent;
        transform.parent = fromPortal.transform;

        clone.transform.SetParent(toPortal.transform);
        clone.transform.localPosition = transform.localPosition;
        clone.transform.localRotation = transform.localRotation;
        clone.transform.parent = transform;
        clone.transform.localScale = new Vector3(1, 1, 1);
        clone.transform.SetParent(null);
        transform.parent = oldParent;
    }

    public void StartPortalTravel(Portal fromPortal, Portal toPortal)
    {
        this.fromPortal = fromPortal;
        this.toPortal = toPortal;
        isActive = true;
        clone.SetActive(true);

        UpdateClonePosition();

        Vector3 forward = fromPortal.transform.right;
        Vector3 distance = fromPortal.transform.position - transform.position;
        
        previousDotProductValue = Vector3.Dot(forward, distance);
    }

    public void StopPortalTravel()
    {
        isActive = false;
        clone.SetActive(false);
        clone.transform.SetParent(transform);
    }
}