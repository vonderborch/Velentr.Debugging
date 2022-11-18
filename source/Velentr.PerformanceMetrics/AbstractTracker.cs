using System;

namespace Velentr.PerformanceMetrics
{
    /// <summary>
    ///     An abstract tracker.
    /// </summary>
    public abstract class AbstractTracker
    {
        /// <summary>
        ///     Specialized constructor for use only by derived class.
        /// </summary>
        /// <param name="maximumSamples">   The maximum samples. </param>
        protected AbstractTracker(int maximumSamples)
        {
            this.MaximumSamples = maximumSamples;
        }

        /// <summary>
        ///     Gets or sets the maximum samples.
        /// </summary>
        /// <value>
        ///     The maximum samples.
        /// </value>
        public int MaximumSamples { get; protected set; }

        /// <summary>
        ///     Updates the given timeSpan.
        /// </summary>
        /// <param name="timeSpan"> The time span. </param>
        public abstract void Update(TimeSpan timeSpan);

        /// <summary>
        ///     Starts a tracking.
        /// </summary>
        public abstract void StartTracking();

        /// <summary>
        ///     Stops a tracking.
        /// </summary>
        public abstract void StopTracking();
    }
}
