using Microsoft.Xna.Framework;

namespace Velentr.PerformanceMetrics
{
    /// <summary>
    ///     A game instance that tracks performance.
    /// </summary>
    public class PerformanceTrackedGame : Game
    {
        /// <summary>
        ///     (Immutable) the decimals.
        /// </summary>
        private const string _decimals = "0.000";

        /// <summary>
        ///     The base title.
        /// </summary>
        private readonly string _baseTitle = "Velentr.DevEnv";

        /// <summary>
        ///     The frame counter.
        /// </summary>
        private readonly FpsTracker _frameCounter = new(10);

        /// <summary>
        ///     The performance.
        /// </summary>
        private readonly PerformanceTracker _performance = new(10, enableFpsTracker: true);

        /// <summary>
        ///     Constructor.
        /// </summary>
        public PerformanceTrackedGame() : base()
        {
            this._baseTitle = $"{{5}} | {{4}} | FPS: {{0:{_decimals}}} | TPS: {{1:{_decimals}}} | CPU: {{2:{_decimals}}}% | Memory: {{3:{_decimals}}} MB";
        }

        /// <summary>
        ///     Gets or sets the framework version.
        /// </summary>
        /// <value>
        ///     The framework version.
        /// </value>
        public string FrameworkVersion { get; set; } = "N/A";

        /// <summary>
        ///     Gets or sets the game name.
        /// </summary>
        /// <value>
        ///     The game name.
        /// </value>
        public string GameName { get; set; } = "N/A";

        /// <summary>
        ///     Updates the given gameTime.
        /// </summary>
        /// <param name="gameTime"> The game time. </param>
        protected override void Update(GameTime gameTime)
        {
            this._performance.Update(gameTime.ElapsedGameTime);
            base.Update(gameTime);
        }

        /// <summary>
        ///     Draws the given game time.
        /// </summary>
        /// <param name="gameTime"> The game time. </param>
        protected override void Draw(GameTime gameTime)
        {
            this._frameCounter.Update(gameTime.ElapsedGameTime);
            this.Window.Title = string.Format(
                                              this._baseTitle,
                                              this._frameCounter.AverageFramesPerSecond,
                                              this._performance.FpsTracker.AverageFramesPerSecond,
                                              this._performance.CpuTracker.CpuPercent,
                                              this._performance.MemoryTracker.MemoryUsageMb,
                                              this.FrameworkVersion,
                                              this.GameName
                                             );

            base.Draw(gameTime);
        }
    }
}
