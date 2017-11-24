namespace Serpent.Chain.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// A simple message handler (without cancellation token)
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface ISimpleMessageHandler<in TMessageType>
    {
        /// <summary>
        /// The method to invoke when a message is published
        /// </summary>
        /// <param name="message">
        /// The message
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task HandleMessageAsync(TMessageType message);
    }
}