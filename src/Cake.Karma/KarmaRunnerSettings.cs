﻿using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Karma
{
    /// <summary>
    /// Available Karma logging levels.
    /// </summary>
    public enum KarmaLogLevel
    {
        /// <summary>
        /// No logging.
        /// </summary>
        Disable,
        /// <summary>
        /// Log errors.
        /// </summary>
        Error,
        /// <summary>
        /// Log warnings.
        /// </summary>
        Warn,
        /// <summary>
        /// Log info.
        /// </summary>
        Info,
        /// <summary>
        /// Log diagnostics.
        /// </summary>
        Debug
    }



    /// <summary>
    /// Available Karma reporters.
    /// </summary>
    public enum KarmaReporter
    {
        /// <summary>
        /// Dots.
        /// </summary>
        Dots,
        /// <summary>
        /// Progress.
        /// </summary>
        Progress,
        /// <summary>
        /// JUnit.
        /// </summary>
        JUnit,
        /// <summary>
        /// Growl.
        /// </summary>
        Growl,
        /// <summary>
        /// Coverage.
        /// </summary>
        Coverage
    }



    /// <summary>
    /// Run modes.
    /// </summary>
    public enum KarmaRunMode
    {
        /// <summary>
        /// Run the command using local Karma.
        /// </summary>
        Local,
        /// <summary>
        /// Run the command using global Karma.
        /// </summary>
        Global
    }
    


    /// <summary>
    /// Settings common across all Karma commands.
    /// </summary>
    public class KarmaSettings : ToolSettings
    {
        /// <summary>
        /// The default Karma CLI file, if one is not specified during local run mode.
        /// </summary>
        public const string DefaultLocalKarmaCli = "node_modules/karma-cli/bin/karma";

        internal virtual string Command { get; } = "init";

        /// <summary>
        /// Run karma locally or globally. Defaults to KarmaRunMode.Global.
        /// </summary>
        public KarmaRunMode RunMode { get; set; } = KarmaRunMode.Global;

        /// <summary>
        /// Path to Karma CLI. Use this if you want to utilise a local Karma deployment, otherwise Global use is attempted.
        /// </summary>
        public FilePath LocalKarmaCli { get; set; }
        
        /// <summary>
        /// The conf.js file to use.
        /// </summary>
        public FilePath ConfigFile { get; set; }
        
        /// <summary>
        /// Level of logging.
        /// </summary>
        public KarmaLogLevel? LogLevel { get; set; }

        /// <summary>
        /// Use colors when reporting or printing logs.
        /// Defaults to false.
        /// </summary>
        public bool Colors { get; set; }

        /// <summary>
        /// Do not use colors when reporting or printing logs.
        /// Defaults to false.
        /// </summary>
        public bool NoColors { get; set; }


        internal void Evaluate(ProcessArgumentBuilder args)
        {
            args.AppendQuoted(ConfigFile.ToString());

            if (LogLevel.HasValue)
            {
                args.AppendSwitch("--log-level", GetLogLevelString());
            }

            if (Colors)
            {
                args.Append("--colors");
            }
            if (NoColors)
            {
                args.Append("--no-colors");
            }

            EvaluateCore(args);
        }

        /// <summary>
        /// Evaluate further settings.
        /// </summary>
        protected virtual void EvaluateCore(ProcessArgumentBuilder args)
        {
        }

        private string GetLogLevelString()
        {
            return LogLevel.Value.ToString().ToLower();
        }
    }


    
    /// <summary>
    /// Settings shared across start and run commands.
    /// </summary>
    public abstract class KarmaSharedSettings : KarmaSettings
    {
        /// <summary>
        /// Port where the server is listening.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// Fail on empty test suite.
        /// Defaults to false.
        /// </summary>
        public bool FailOnEmptyTestSuite { get; set; }

        /// <summary>
        /// Do not fail on empty test suite.
        /// Defaults to false.
        /// </summary>
        public bool NoFailOnEmptyTestSuite { get; set; }

        /// <summary>
        /// Applies the settings to the process arguments ready for execution.
        /// </summary>
        /// <param name="args">The builder to apply the settings to.</param>
        protected override void EvaluateCore(ProcessArgumentBuilder args)
        {
            base.EvaluateCore(args);

            if (Port.HasValue)
            {
                args.AppendSwitch("--port", Port.Value.ToString());
            }

            if (FailOnEmptyTestSuite)
            {
                args.Append("--fail-on-empty-test-suite");
            }
            if (NoFailOnEmptyTestSuite)
            {
                args.Append("--no-fail-on-empty-test-suite");
            }
        }
    }



    /// <summary>
    /// Settings specific to karma run.
    /// </summary>
    public class KarmaRunSettings : KarmaSharedSettings
    {
        internal override string Command { get; } = "run";

        /// <summary>
        /// Do not re-glob all the patterns.
        /// Defaults to false.
        /// </summary>
        public bool NoRefresh { get; set; }

        /// <summary>
        /// Applies the settings to the process arguments ready for execution.
        /// </summary>
        /// <param name="args">The builder to apply the settings to.</param>
        protected override void EvaluateCore(ProcessArgumentBuilder args)
        {
            base.EvaluateCore(args);

            if (NoRefresh)
            {
                args.Append("--no-refresh");
            }
        }
    }



    /// <summary>
    /// Settings specific to karma start.
    /// </summary>
    public class KarmaStartSettings : KarmaSharedSettings
    {
        internal override string Command { get; } = "start";

        /// <summary>
        /// Auto watch source files and run on change.
        /// </summary>
        public bool AutoWatch { get; set; }

        /// <summary>
        /// Do not watch source files.
        /// </summary>
        public bool NoAutoWatch { get; set; }

        /// <summary>
        /// Detach the server.
        /// </summary>
        public bool Detached { get; set; }

        /// <summary>
        /// List of reporters.
        /// </summary>
        public ICollection<KarmaReporter> Reporters { get; set; }

        /// <summary>
        /// List of browsers to start.
        /// </summary>
        public ICollection<string> Browsers { get; set; }

        /// <summary>
        /// Kill browser if does not capture in given time (ms).
        /// </summary>
        public int? CaptureTimeout { get; set; }

        /// <summary>
        /// Run the test when the browsers captured and exit.
        /// </summary>
        public bool SingleRun { get; set; }

        /// <summary>
        /// Disable single-run.
        /// </summary>
        public bool NoSingleRun { get; set; }

        /// <summary>
        /// Report tests that are slower then given time (ms).
        /// </summary>
        public int? ReportSlowerThan { get; set; }

        /// <summary>
        /// Applies the settings to the process arguments ready for execution.
        /// </summary>
        /// <param name="args">The builder to apply the settings to.</param>
        protected override void EvaluateCore(ProcessArgumentBuilder args)
        {
            base.EvaluateCore(args);

            if (AutoWatch)
            {
                args.Append("--auto-watch");
            }

            if (NoAutoWatch)
            {
                args.Append("--no-auto-watch");
            }

            if (Detached)
            {
                args.Append("--detached");
            }

            if (SingleRun)
            {
                args.Append("--single-run");
            }

            if (NoSingleRun)
            {
                args.Append("--no-single-run");
            }

            if (CaptureTimeout.HasValue)
            {
                args.AppendSwitch("--capture-timeout", CaptureTimeout.Value.ToString());
            }

            if (ReportSlowerThan.HasValue)
            {
                args.AppendSwitch("--report-slower-than", ReportSlowerThan.Value.ToString());
            }

            if (Reporters != null && Reporters.Count > 0)
            {
                args.AppendSwitch("--reporters", string.Join(",", Reporters.Select(_ => _.ToString().ToLower())));
            }

            if (Browsers != null && Browsers.Count > 0)
            {
                args.AppendSwitch("--browsers", string.Join(",", Browsers));
            }
        }
    }
}
