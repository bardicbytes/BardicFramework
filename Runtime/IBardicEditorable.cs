namespace BardicBytes.BardicFramework
{
    public interface IBardicEditorable
    {
#if UNITY_EDITOR
        /// <summary>
        /// new string[0]
        /// </summary>
        string[] EditorFieldNames { get; }
        /// <summary>
        /// true
        /// </summary>
        bool DrawOtherFields { get; }
#endif
    }
}
