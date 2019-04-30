using ParallelTasks;
using Sandbox.ModAPI;

namespace AutoMcD.SmartRotors.Data {
    /// <summary>
    ///     A <see cref="WorkData" /> type for <see cref="ParallelTasks" />.
    /// </summary>
    public class PlaceSmartHingeData : WorkData {
        /// <summary>
        ///     Information about the data completion status.
        /// </summary>
        public enum DataResult {
            Running,
            Success,
            Failed
        }

        /// <summary>
        ///     Initializes a new instance of <see cref="PlaceSmartHingeData" /> work data.
        /// </summary>
        /// <param name="head"></param>
        public PlaceSmartHingeData(IMyAttachableTopBlock head) {
            Head = head;
            Result = DataResult.Running;
        }

        /// <summary>
        ///     The Entity which should place a new hinge.
        /// </summary>
        public IMyAttachableTopBlock Head { get; }

        /// <summary>
        ///     Gets the result of this data.
        /// </summary>
        public DataResult Result { get; private set; }

        /// <summary>
        ///     Flag this data as failed.
        /// </summary>
        public new void FlagAsFailed() {
            Result = DataResult.Failed;
        }

        /// <summary>
        ///     Flag this data as succeeded.
        /// </summary>
        public void FlagAsSucceeded() {
            Result = DataResult.Success;
        }
    }
}