namespace BB.BardicFramework.Cameras
{
    public interface IOffsetCameraController
    {
        void ApplyOffset(CamOffset newOffset);
        void ApplyDefaultOffset();
    }
}
