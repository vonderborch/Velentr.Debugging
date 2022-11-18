using System;
using System.Linq;

using Velentr.Collections;

namespace Velentr.PerformanceMetrics
{
    /// <summary>
    ///     The FPS tracker.
    /// </summary>
    /// <seealso cref="AbstractTracker" />
    public class FpsTracker : AbstractTracker
    {
        /// <summary>
        ///     (Immutable) the samples.
        /// </summary>
        private readonly SizeLimitedPool<double> _samples;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="maximumSamples">   (Optional) The maximum samples. </param>
        public FpsTracker(int maximumSamples = 100)
            : base(maximumSamples)
        {
            this._samples = new SizeLimitedPool<double>(maximumSamples);
        }

        /// <summary>
        ///     Gets or sets the total number of frames.
        /// </summary>
        /// <value>
        ///     The total number of frames.
        /// </value>
        public long TotalFrames { get; private set; }

        /// <summary>
        ///     Gets or sets the total number of seconds.
        /// </summary>
        /// <value>
        ///     The total number of seconds.
        /// </value>
        public double TotalSeconds { get; private set; }

        /// <summary>
        ///     Gets or sets the average frames per second.
        /// </summary>
        /// <value>
        ///     The average frames per second.
        /// </value>
        public double AverageFramesPerSecond { get; private set; }

        /// <summary>
        ///     Gets or sets the current frames per second.
        /// </summary>
        /// <value>
        ///     The current frames per second.
        /// </value>
        public double CurrentFramesPerSecond { get; private set; }

        /// <summary>
        ///     Updates the given timeSpan.
        /// </summary>
        /// <param name="timeSpan"> The time span. </param>
        /// <seealso cref="AbstractTracker.Update(TimeSpan)" />
        public override void Update(TimeSpan timeSpan)
        {
            this.CurrentFramesPerSecond = 1.0f / timeSpan.TotalSeconds;

            this._samples.AddItem(this.CurrentFramesPerSecond);
            this.AverageFramesPerSecond = this._samples.Average(i => i);

            this.TotalFrames++;
            this.TotalSeconds += timeSpan.TotalSeconds;
        }

        /// <summary>
        ///     Starts a tracking.
        /// </summary>
        /// <seealso cref="AbstractTracker.StartTracking()" />
        public override void StartTracking()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Stops a tracking.
        /// </summary>
        /// <seealso cref="AbstractTracker.StopTracking()" />
        public override void StopTracking()
        {
            throw new NotImplementedException();
        }
    }
}
