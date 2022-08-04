using System;

namespace Velentr.Debugging
{
    /// <summary>
    ///     A performance tracker.
    /// </summary>
    /// <seealso cref="AbstractTracker" />
    public class PerformanceTracker : AbstractTracker
    {
        /// <summary>
        ///     The CPU tracker.
        /// </summary>
        public CpuTracker CpuTracker;

        /// <summary>
        ///     The FPS tracker.
        /// </summary>
        public FpsTracker FpsTracker;

        /// <summary>
        ///     The memory tracker.
        /// </summary>
        public MemoryTracker MemoryTracker;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="maximumSamples">                       (Optional) The maximum samples. </param>
        /// <param name="enableCpuTracker">
        ///     (Optional) True to enable, false to
        ///     disable the CPU tracker.
        /// </param>
        /// <param name="cpuSleepTime">                         (Optional) The CPU sleep time. </param>
        /// <param name="automaticallyEnableCpuTracking">
        ///     (Optional) True to automatically enable
        ///     CPU tracking.
        /// </param>
        /// <param name="enableMemoryTracker">
        ///     (Optional) True to enable, false to
        ///     disable the memory tracker.
        /// </param>
        /// <param name="automaticallyEnableMemoryTracking">
        ///     (Optional) True to automatically enable
        ///     memory tracking.
        /// </param>
        /// <param name="memorySleepTime">                      (Optional) The memory sleep time. </param>
        /// <param name="enableFpsTracker">
        ///     (Optional) True to enable, false to
        ///     disable the FPS tracker.
        /// </param>
        public PerformanceTracker(
            int maximumSamples = 100
          , bool enableCpuTracker = true
          , int cpuSleepTime = 1000
          , bool automaticallyEnableCpuTracking = false
          , bool enableMemoryTracker = true
          , bool automaticallyEnableMemoryTracking = false
          , int memorySleepTime = 1000
          , bool enableFpsTracker = false
        )
            : base(maximumSamples)
        {
            if (enableCpuTracker)
            {
                this.CpuTracker = new CpuTracker(maximumSamples, automaticallyEnableCpuTracking, cpuSleepTime);
            }

            if (enableMemoryTracker)
            {
                this.MemoryTracker = new MemoryTracker(maximumSamples, automaticallyEnableMemoryTracking, memorySleepTime);
            }

            if (enableFpsTracker)
            {
                this.FpsTracker = new FpsTracker(maximumSamples);
            }
        }

        /// <summary>
        ///     Updates the given timeSpan.
        /// </summary>
        /// <param name="timeSpan"> The time span. </param>
        /// <seealso cref="AbstractTracker.Update(TimeSpan)" />
        public override void Update(TimeSpan timeSpan)
        {
            this.MemoryTracker?.Update(timeSpan);
            this.CpuTracker?.Update(timeSpan);
            this.FpsTracker?.Update(timeSpan);
        }

        /// <summary>
        ///     Starts a tracking.
        /// </summary>
        /// <seealso cref="AbstractTracker.StartTracking()" />
        public override void StartTracking()
        {
            this.MemoryTracker?.StartTracking();
            this.CpuTracker?.StartTracking();
        }

        /// <summary>
        ///     Stops a tracking.
        /// </summary>
        /// <seealso cref="AbstractTracker.StopTracking()" />
        public override void StopTracking()
        {
            this.MemoryTracker?.StopTracking();
            this.CpuTracker?.StopTracking();
        }
    }
}
