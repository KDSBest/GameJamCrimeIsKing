using UnityEngine;

public class CameraConfig : MonoBehaviour
{
    public Moba_Camera mCam;



    // Use this for initialization
    void Start()
    {
        this.mCam.settings.rotation.lockRotationY = false;
        this.mCam.settings.rotation.lockRotationX = false;
        this.mCam.SetCameraZoom(this.mCam.settings.zoom.defaultZoom);
        this.mCam.SetCameraRotation(-45, 0);
        this.mCam.settings.useBoundaries = false;
        this.mCam.settings.rotation.constRotationRate = false;
        this.mCam.settings.zoom.constZoomRate = false;
        this.mCam.settings.movement.defualtHeight = 15;
        this.mCam.settings.movement.useLockTargetHeight = true;
        this.mCam.settings.movement.edgeHoverMovement = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
