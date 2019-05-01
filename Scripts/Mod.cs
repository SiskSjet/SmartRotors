﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sandbox.ModAPI;
using Sisk.Utils.Logging;
using Sisk.Utils.Logging.DefaultHandler;
using Sisk.Utils.Net;
using VRage;
using VRage.Game.Components;

namespace Sisk.SmartRotors {
    /// <summary>
    ///     Main session component which register Logging, Network and SunTracker components.
    /// </summary>
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Mod : MySessionComponentBase {
        public const string NAME = "SmartRotors";

        // important: change to info | warning | error or none before publishing this mod.
        private const LogEventLevel DEFAULT_LOG_EVENT_LEVEL = LogEventLevel.All;

        private const string LOG_FILE_TEMPLATE = "{0}.log";
        private const ushort NETWORK_ID = 51511;
        private static readonly string LogFile = string.Format(LOG_FILE_TEMPLATE, NAME);

        /// <summary>
        ///     Initializes a new instance of the <see cref="Mod" /> session component.
        /// </summary>
        public Mod() {
            Static = this;
            Defs = new Defs();
            Controls = new Controls();
            InitializeLogging();
        }

        /// <summary>
        ///     Helper for modifying vanilla terminal controls and actions.
        /// </summary>
        public Controls Controls { get; private set; }

        /// <summary>
        ///     Holds Subtype ids for blocks in this mod.
        /// </summary>
        public Defs Defs { get; private set; }

        /// <summary>
        ///     Language used to localize this mod.
        /// </summary>
        public MyLanguagesEnum? Language { get; private set; }

        /// <summary>
        ///     Logger used for logging.
        /// </summary>
        public ILogger Log { get; private set; }

        /// <summary>
        ///     Network to handle syncing.
        /// </summary>
        public Network Network { get; private set; }

        /// <summary>
        ///     The static instance.
        /// </summary>
        public static Mod Static { get; private set; }

        /// <summary>
        ///     The sun tracker component.
        /// </summary>
        public SunTracker SunTracker { get; private set; }

        /// <summary>
        ///     Used to format the <see cref="LogEvent" /> entries.
        /// </summary>
        /// <param name="level">The <see cref="LogEventLevel" /> for current event.</param>
        /// <param name="message">The <see cref="LogEvent" /> message.</param>
        /// <param name="timestamp">The timestamp of the <see cref="LogEvent" />.</param>
        /// <param name="scope">The scope of the <see cref="LogEvent" />.</param>
        /// <param name="method">The called method of this <see cref="LogEvent" />.</param>
        /// <returns></returns>
        private static string LogFormatter(LogEventLevel level, string message, DateTime timestamp, Type scope, string method) {
            return $"[{timestamp:HH:mm:ss:fff}] [{new string(level.ToString().Take(1).ToArray())}] [{scope}->{method}()]: {message}";
        }

        /// <summary>
        ///     Initialize some components that are only available just before the start.
        /// </summary>
        public override void BeforeStart() {
            using (Log.BeginMethod(nameof(BeforeStart))) {
                if (Network == null || Network.IsServer) {
                    InitializeSunTracker();
                }
            }
        }

        /// <summary>
        ///     Load mod settings and create localizations.
        /// </summary>
        public override void LoadData() {
            LoadLocalization();

            if (MyAPIGateway.Multiplayer.MultiplayerActive) {
                InitializeNetwork();
            }

            MyAPIGateway.Gui.GuiControlRemoved += OnGuiControlRemoved;
        }

        /// <summary>
        ///     Save mod settings and fire OnSave event.
        /// </summary>
        public override void SaveData() {
            Log.Flush();
        }

        /// <summary>
        ///     Unloads all data.
        /// </summary>
        protected override void UnloadData() {
            Log?.EnterMethod(nameof(UnloadData));
            MyAPIGateway.Gui.GuiControlRemoved -= OnGuiControlRemoved;

            if (Network != null) {
                Log?.Info("Cap network connections");
                Network.Close();
                Network = null;
            }

            if (Log != null) {
                Log.Info("Logging stopped");
                Log.Flush();
                Log.Close();
                Log = null;
            }

            if (Controls != null) {
                Controls = null;
            }

            if (Defs != null) {
                Defs = null;
            }

            Static = null;
        }

        /// <summary>
        ///     Initialize the logging system.
        /// </summary>
        private void InitializeLogging() {
            Log = Logger.ForScope<Mod>();
            if (MyAPIGateway.Multiplayer.IsServer) {
                Log.Register(new WorldStorageHandler(LogFile, LogFormatter, DEFAULT_LOG_EVENT_LEVEL, 0));
            } else {
                Log.Register(new GlobalStorageHandler(LogFile, LogFormatter, DEFAULT_LOG_EVENT_LEVEL, 0));
            }

            using (Log.BeginMethod(nameof(InitializeLogging))) {
                Log.Info("Logging initialized");
            }
        }

        /// <summary>
        ///     Initialize the network system.
        /// </summary>
        private void InitializeNetwork() {
            using (Log.BeginMethod(nameof(InitializeNetwork))) {
                Log.Info("Initialize Network");
                Network = new Network(NETWORK_ID);
                Log.Info($"IsClient {Network.IsClient}, IsServer: {Network.IsServer}, IsDedicated: {Network.IsDedicated}");
                Log.Info("Network initialized");
            }
        }

        /// <summary>
        ///     Initialize a sun tracker component.
        /// </summary>
        private void InitializeSunTracker() {
            using (Log.BeginMethod(nameof(InitializeSunTracker))) {
                SunTracker = new SunTracker();
                Log.Info($"{nameof(SunTracker)} initialized");
            }
        }

        /// <summary>
        ///     Load localizations for this mod.
        /// </summary>
        private void LoadLocalization() {
            var path = Path.Combine(ModContext.ModPathData, "Localization");
            var supportedLanguages = new HashSet<MyLanguagesEnum>();
            MyTexts.LoadSupportedLanguages(path, supportedLanguages);

            var currentLanguage = supportedLanguages.Contains(MyAPIGateway.Session.Config.Language) ? MyAPIGateway.Session.Config.Language : MyLanguagesEnum.English;
            if (Language != null && Language == currentLanguage) {
                return;
            }

            Language = currentLanguage;
            var languageDescription = MyTexts.Languages.Where(x => x.Key == currentLanguage).Select(x => x.Value).FirstOrDefault();
            if (languageDescription != null) {
                var cultureName = string.IsNullOrWhiteSpace(languageDescription.CultureName) ? null : languageDescription.CultureName;
                var subcultureName = string.IsNullOrWhiteSpace(languageDescription.SubcultureName) ? null : languageDescription.SubcultureName;

                MyTexts.LoadTexts(path, cultureName, subcultureName);
            }
        }

        /// <summary>
        ///     Event triggered on gui control removed.
        ///     Used to detect if Option screen is closed and then to reload localization.
        /// </summary>
        /// <param name="obj"></param>
        private void OnGuiControlRemoved(object obj) {
            if (obj.ToString().EndsWith("ScreenOptionsSpace")) {
                LoadLocalization();
            }
        }
    }
}