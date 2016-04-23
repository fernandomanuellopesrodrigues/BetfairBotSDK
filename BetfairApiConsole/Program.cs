// Bfexplorer cannot be held responsible for any losses or damages incurred during the use of this betfair bot.
// It is up to you to determine the level of risk you wish to trade under. 
// Do not gamble with money you cannot afford to lose.

using BeloSoft.Data;
using BeloSoft.Betfair.API;
using BeloSoft.Betfair.API.Models;

using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using System.Threading;
using System;

namespace BetfairApiConsole
{
    class Program
    {
        /// <summary>
        /// ExecuteAsyncTask
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        static T ExecuteAsyncTask<T>(FSharpAsync<T> task)
        {
            return FSharpAsync.RunSynchronously(task, FSharpOption<int>.None, FSharpOption<CancellationToken>.None);
        }

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new Exception("Please enter your betfair user name and password!");
            }

            var username = args[0];
            var password = args[1];

            var betfairServiceProvider = new BetfairServiceProvider(BetfairApiServer.GBR);

            var loginResult = ExecuteAsyncTask(betfairServiceProvider.AccountOperations.Login(username, password));

            if (loginResult.IsSuccess)
            {
                var filter = new MarketFilter();

                filter.eventTypeIds = new int[] { 1 };
                filter.marketCountries = new string[] { "GB" };
                filter.inPlayOnly = false;
                filter.turnInPlayEnabled = true;
                filter.marketTypeCodes = new string[] { "MATCH_ODDS" };

                var marketCataloguesResult = ExecuteAsyncTask(betfairServiceProvider.BrowsingOperations.GetMarketCatalogues(filter, 10, 
                    FSharpOption<MarketProjection[]>.Some(new MarketProjection[] { MarketProjection.EVENT, MarketProjection.MARKET_START_TIME, MarketProjection.COMPETITION, MarketProjection.RUNNER_DESCRIPTION, MarketProjection.MARKET_DESCRIPTION }), FSharpOption<MarketSort>.Some(MarketSort.MAXIMUM_TRADED), FSharpOption<string>.None));

                if (marketCataloguesResult.IsSuccessResult)
                {
                    var marketCatalogues = marketCataloguesResult.SuccessResult;

                    foreach (var marketCatalogue in marketCatalogues)
                    {
                        var betEvent = marketCatalogue.@event;

                        Console.WriteLine($"{betEvent.openDate}: {betEvent.name}, eventId: {betEvent.id}, marketId: {marketCatalogue.marketId}");
                    }
                }

                ExecuteAsyncTask(betfairServiceProvider.AccountOperations.Logout());
            }
        }
    }
}
