public interface ILookingReceiver
{
    /// <summary>
    /// Should be called when the object is being looked at
    /// </summary>
    void LookingAt();

    /// <summary>
    /// Should be called when the object is no longer being looked at
    /// </summary>
    void NotLookingAt();
}
