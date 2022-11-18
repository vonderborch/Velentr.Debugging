using System;
#if FNA
using System.IO;
using System.Runtime.InteropServices;
#endif

namespace Velentr.PerformanceMetrics
{
    /// <summary>
    ///     A game performance wrapper.
    /// </summary>
    public class GamePerformanceWrapper : IDisposable
    {
#if FNA

        /// <summary>
        ///     Sets default DLL directories.
        /// </summary>
        /// <param name="directoryFlags">   The directory flags. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDefaultDllDirectories(int directoryFlags);

        /// <summary>
        ///     Adds a DLL directory.
        /// </summary>
        /// <param name="lpPathName">   Full pathname of the file. </param>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void AddDllDirectory(string lpPathName);

        /// <summary>
        ///     Sets DLL directory.
        /// </summary>
        /// <param name="lpPathName">   Full pathname of the file. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        ///     (Immutable) the load library search default dirs.
        /// </summary>
        private const int LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;
#endif

#if FNA

        /// <summary>
        ///     (Immutable) the framework version.
        /// </summary>
        private const string FrameworkVersion = "FNA";
#elif MONOGAME
        /// <summary>
        /// (Immutable) the framework version.
        /// </summary>
        const string FrameworkVersion = "Monogame";
#else
        /// <summary>
        /// (Immutable) the framework version.
        /// </summary>
        const string FrameworkVersion = "Generic";
#endif

        /// <summary>
        ///     (Immutable) the game.
        /// </summary>
        private readonly PerformanceTrackedGame game;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="game"> The game. </param>
        public GamePerformanceWrapper(PerformanceTrackedGame game, string gameName)
        {
            this.game = game;
            this.game.FrameworkVersion = FrameworkVersion;
            this.game.GameName = gameName;
        }

        /// <summary>
        ///     Runs this object.
        /// </summary>
        [STAThread]
        public void Run()
        {
#if FNA
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                try
                {
                    SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
                    AddDllDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Environment.Is64BitProcess ? "x64" : "x86"));
                }
                catch
                {
                    // Pre-Windows 7, KB2533623
                    SetDllDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Environment.Is64BitProcess ? "x64" : "x86"));
                }
            }
#endif

            this.game.Run();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        ///     resources.
        /// </summary>
        public void Dispose()
        {
            this.game.Dispose();
        }
    }
}
