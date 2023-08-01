using System;
using System.Collections;

namespace Description
{
    /// <summary>
    /// Interface IDescriptionClient provides methods related to generating descriptions.
    /// </summary>
    public interface IDescriptionClient
    {
        /// <summary>
        /// Explains the concept of <paramref name="content"/>.
        /// </summary>
        /// <param name="content">Text that is explained in this method.</param>
        /// <param name="callback">An action that gets executed with the description. Pass a callback that needs to be executed based on the description.</param>
        public IEnumerator Explain(string content, Action<string[]> callback);
    }
}