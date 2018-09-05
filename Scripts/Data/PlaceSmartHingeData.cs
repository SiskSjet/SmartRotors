using ParallelTasks;
using Sandbox.ModAPI;

namespace AutoMcD.SmartRotors.Data {
    /// <summary>
    ///     A <see cref="WorkData" /> type for <see cref="ParallelTasks" />.
    /// </summary>
    public class PlaceSmartHingeData : WorkData {
        /// <summary>
        ///     Initializes a new instance of <see cref="PlaceSmartHingeData" /> work data.
        /// </summary>
        /// <param name="head"></param>
        public PlaceSmartHingeData(IMyAttachableTopBlock head) {
            Head = head;
        }

        /// <summary>
        ///     The Entity which should place a new hinge.
        /// </summary>
        public IMyAttachableTopBlock Head { get; }
    }
}