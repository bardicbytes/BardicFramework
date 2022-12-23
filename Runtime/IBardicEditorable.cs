namespace BardicBytes.BardicFramework
{
    public interface IBardicEditorable
    {
        /// <summary>
        /// new string[0]
        /// </summary>
        string[] EditorFieldNames { get; }
        /// <summary>
        /// true
        /// </summary>
        bool DrawOtherFields { get; }
    }
}
