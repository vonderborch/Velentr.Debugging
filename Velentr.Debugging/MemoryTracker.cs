using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Velentr.Collections.Collections;
using Velentr.Math.ByteConversion;

namespace Velentr.Debugging
{
    /// <summary>
    ///     A memory tracker.
    /// </summary>
    /// <seealso cref="AbstractTracker" />
    public class MemoryTracker : AbstractTracker
    {
        /// <summary>
        ///     (Immutable) the samples.
        /// </summary>
        private readonly SizeLimitedPool<long> _samples;

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
        public MemoryTracker(int maximumSamples = 100, bool automaticallyStartTracking = false, int sleepTime = 1000)
            : base(maximumSamples)
        {
            this._samples = new SizeLimitedPool<long>();
            this._sleepTime = sleepTime;
            this.ContinueTrackingForever = automaticallyStartTracking;
            this._cancellationToken = new CancellationTokenSource();
            this._updateThread = new Task(UpdateInternal, this._cancellationToken.Token);
            if (automaticallyStartTracking)
            {
                this._updateThread.Start();
            }
        }

        /// <summary>
        ///     Gets or sets the memory usage bytes.
        /// </summary>
        /// <value>
        ///     The memory usage bytes.
        /// </value>
        public long MemoryUsageBytes { get; private set; }

        /// <summary>
        ///     Gets the memory usage kB.
        /// </summary>
        /// <value>
        ///     The memory usage kB.
        /// </value>
        public double MemoryUsageKb => this.MemoryUsageBytes.ToSizeUnit(SizeUnits.KB);

        /// <summary>
        ///     Gets the memory usage megabytes.
        /// </summary>
        /// <value>
        ///     The memory usage megabytes.
        /// </value>
        public double MemoryUsageMb => this.MemoryUsageBytes.ToSizeUnit(SizeUnits.MB);

        /// <summary>
        ///     Gets the memory usage gigabytes.
        /// </summary>
        /// <value>
        ///     The memory usage gigabytes.
        /// </value>
        public double MemoryUsageGb => this.MemoryUsageBytes.ToSizeUnit(SizeUnits.GB);

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
                this._samples.AddItem(Process.GetCurrentProcess().WorkingSet64);

                this.MemoryUsageBytes = this._samples.Sum() / this._samples.Count;
                Thread.Sleep(this._sleepTime);
            } while (this.ContinueTrackingForever);
        }
    }
}
