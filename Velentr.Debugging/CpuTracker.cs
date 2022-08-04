using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Velentr.Collections.Collections;
using Velentr.Math;

namespace Velentr.Debugging
{
    /// <summary>
    ///     A CPU tracker.
    /// </summary>
    /// <seealso cref="AbstractTracker" />
    public class CpuTracker : AbstractTracker
    {
        /// <summary>
        ///     (Immutable) the CPU tracker.
        /// </summary>
        private readonly PerformanceCounter _cpuTracker;

        /// <summary>
        ///     (Immutable) the samples.
        /// </summary>
        private readonly SizeLimitedPool<double> _samples;

        /// <summary>
        ///     (Immutable) the sleep time.
        /// </summary>
        private readonly int _sleepTime;

        /// <summary>
        ///     The cancellation token.
        /// </summary>
        private CancellationTokenSource _cancellationToken;

        /// <summary>
        ///     The update thread.
        /// </summary>
        private Task _updateThread;

        /// <summary>
        ///     True to continue tracking forever.
        /// </summary>
        public bool ContinueTrackingForever;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="maximumSamples">               (Optional) The maximum samples. </param>
        /// <param name="automaticallyStartTracking">   (Optional) True to automatically start tracking. </param>
        /// <param name="sleepTime">                    (Optional) The sleep time. </param>
        public CpuTracker(int maximumSamples = 100, bool automaticallyStartTracking = false, int sleepTime = 1000)
            : base(maximumSamples)
        {
            this._cpuTracker = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName, true);
            this._samples = new SizeLimitedPool<double>(maximumSamples);
            this._sleepTime = MathHelpers.Clamp(sleepTime, 1000);
            this.ContinueTrackingForever = automaticallyStartTracking;
            this._cancellationToken = new CancellationTokenSource();
            this._updateThread = new Task(UpdateInternal, this._cancellationToken.Token);
            if (automaticallyStartTracking)
            {
                this._updateThread.Start();
            }
        }

        /// <summary>
        ///     Gets or sets the CPU percent.
        /// </summary>
        /// <value>
        ///     The CPU percent.
        /// </value>
        public double CpuPercent { get; private set; }

        /// <summary>
        ///     Updates the given timeSpan.
        /// </summary>
        /// <param name="timeSpan"> The time span. </param>
        /// <seealso cref="AbstractTracker.Update(TimeSpan)" />
        public override void Update(TimeSpan timeSpan)
        {
            if (this._updateThread.IsCompleted)
            {
                this._updateThread = new Task(UpdateInternal, this._cancellationToken.Token);
                this._updateThread.Start();
            }
            else if (this._updateThread.Status == TaskStatus.Created)
            {
                this._updateThread.Start();
            }
        }

        /// <summary>
        ///     Starts a tracking.
        /// </summary>
        /// <seealso cref="AbstractTracker.StartTracking()" />
        public override void StartTracking()
        {
            if (this._updateThread.Status != TaskStatus.Running
                && this._updateThread.Status != TaskStatus.WaitingForActivation
                && this._updateThread.Status != TaskStatus.WaitingForChildrenToComplete
                && this._updateThread.Status != TaskStatus.WaitingToRun)
            {
                if (this._updateThread.IsCanceled)
                {
                    this._cancellationToken = new CancellationTokenSource();
                }

                this._updateThread = new Task(UpdateInternal, this._cancellationToken.Token);
                this._updateThread.Start();
            }
        }

        /// <summary>
        ///     Stops a tracking.
        /// </summary>
        /// <seealso cref="AbstractTracker.StopTracking()" />
        public override void StopTracking()
        {
            this._cancellationToken.Cancel();
        }

        /// <summary>
        ///     Updates the internal.
        /// </summary>
        private void UpdateInternal()
        {
            do
            {
                this._cpuTracker.NextValue();
                Thread.Sleep(this._sleepTime); // wait a second to get a valid reading
                this._samples.AddItem(this._cpuTracker.NextValue() / (double) Environment.ProcessorCount);

                this.CpuPercent = this._samples.Sum() / this._samples.Count;
            } while (this.ContinueTrackingForever);
        }
    }
}
