namespace BB.BardicFramework.Cameras
{
    public class CursorCameraController : CameraController
    {
        protected override void OnValidate()
        {
            activateOnAwake = true;
            base.OnValidate();
        }

        protected override void Awake()
        {
            manager.ActivateCamera(this);
        }
    }
}
