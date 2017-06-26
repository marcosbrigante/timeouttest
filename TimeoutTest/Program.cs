using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using AssetPortfolioManagerAddin;
using log4net;

namespace TimeoutTest
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var log = LogManager.GetLogger("Logger");

            var date = new DateTime(2017, 06, 26);
            const string fund = "Ibiuna Hedge STR FIM";
            var stopwatch = new Stopwatch();

            log.Info($"Starting test using fund {fund} and date {date:dd-MM-yyyy}");

            for (var i = 0; i < 200; i++)
            {
                var provider = RiskFactory.GetInstance();
                stopwatch.Reset();

                try
                {
                    log.Info($"Loading overview");
                    stopwatch.Start();
                    var overview = provider.LoadOverview(date, fund, false, false);
                    stopwatch.Stop();
                    log.Info($"Overview loaded {overview.Rows.Count} rows in {stopwatch.Elapsed.Seconds} seconds");
                    log.Info($"Ended loading overview");

                }
                catch (Exception ex)
                {
                    log.Error("Error loading overview", ex);
                }

                stopwatch.Reset();

                try
                {
                    log.Info($"Loading cashflow");
                    stopwatch.Start();
                    var cashflow = provider.LoadTradingDeskCashFlows(date, date.AddDays(120), fund);
                    stopwatch.Stop();
                    log.Info($"Cashflow loaded {cashflow.Rows.Count} rows in {stopwatch.Elapsed.Seconds} seconds");
                    log.Info($"Ended loading cashflow");
                }
                catch (Exception ex)
                {
                    log.Info($"Error loading cashflow", ex);
                }

                if (provider != null)
                {
                    RiskFactory.ReleaseRisk(ref provider);
                    RiskFactory.DisposeFreeInstances();
                }

                var sleepInSeconds = 10;
                log.Info($"Waiting {sleepInSeconds} seconds to next turn");
                Thread.Sleep(TimeSpan.FromSeconds(sleepInSeconds));
                
            }

            log.Info($"Ended test using fund {fund} and date {date:dd-mm-yyyy}");
        }
    }
}
