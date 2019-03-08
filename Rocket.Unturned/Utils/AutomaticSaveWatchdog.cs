﻿using Rocket.API;
using Rocket.API.Logging;
using Rocket.API.Scheduling;
using Rocket.Core.Logging;
using SDG.Unturned;
using System;

namespace Rocket.Unturned.Utils
{
    public class AutomaticSaveWatchdog
    {
        private readonly IHost host;
        private readonly ILogger logger;
        private readonly ITaskScheduler scheduler;
        private DateTime? nextSaveTime;
        public static AutomaticSaveWatchdog Instance;
        private int saveInterval = 30;

        public AutomaticSaveWatchdog(IHost host, ILogger logger, ITaskScheduler scheduler)
        {
            this.host = host;
            this.logger = logger;
            this.scheduler = scheduler;
        }

        public void Start()
        {
            Instance = this;
            bool autoSaveEnabled = true; //U.Settings.Instance.AutomaticSave.Enabled;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once HeuristicUnreachableCode

            if (!autoSaveEnabled)
                return;

            int i = 30; //;U.Settings.Instance.AutomaticSave.Interval;

            if (i < saveInterval)
                logger.LogError("AutomaticSave interval must be at least 30 seconds, changed to 30 seconds");
            else
                saveInterval = i;

            logger.LogInformation("This server will automatically save every {0} seconds", saveInterval);

            var period = TimeSpan.FromSeconds(saveInterval);
            scheduler.SchedulePeriodically(host, RunSave, "Automatic Save", period, period);
        }

        private void RunSave()
        {
            if (!Level.isInitialized)
            {
                return;
            }

            if (Level.isLoading)
            {
                return;
            }

            if (Level.info == null)
            {
                return;
            }

            logger.LogInformation("Saving server");

            try
            {
                SaveManager.save();
            }
            catch (Exception er)
            {
                logger.LogError("Failed to auto-save: ", er);
            }
        }
    }
}