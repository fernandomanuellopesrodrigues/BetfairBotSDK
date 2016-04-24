// Bfexplorer cannot be held responsible for any losses or damages incurred during the use of this betfair bot.
// It is up to you to determine the level of risk you wish to trade under. 
// Do not gamble with money you cannot afford to lose.

using BeloSoft.Bfexplorer.Domain;
using BeloSoft.Bfexplorer.Trading;
using Microsoft.FSharp.Core;
using System;

namespace MyCsharpBot
{
    /// <summary>
    /// BotCreator
    /// </summary>
    public abstract class BotCreator : FSharpFunc<Tuple<Market, Selection, BotParameters, IBfexplorerService>, Bot>
    {
    }

    /// <summary>
    /// MyMarketBotCreator
    /// </summary>
    public sealed class MyMarketBotCreator : BotCreator
    {
        public override Bot Invoke(Tuple<Market, Selection, BotParameters, IBfexplorerService> tupledArg)
        {
            Market market = tupledArg.Item1;
            Selection selection = tupledArg.Item2;
            BotParameters botParameters = tupledArg.Item3;
            IBfexplorerService bfexplorerService = tupledArg.Item4;

            return new MyMarketBot(market, (MyMarketBotParameters)botParameters, bfexplorerService);
        }
    }

    /// <summary>
    /// UpdateSpPricesMarketBotCreator
    /// </summary>
    public sealed class UpdateSpPricesMarketBotCreator : BotCreator
    {
        public override Bot Invoke(Tuple<Market, Selection, BotParameters, IBfexplorerService> tupledArg)
        {
            Market market = tupledArg.Item1;
            Selection selection = tupledArg.Item2;
            BotParameters botParameters = tupledArg.Item3;
            IBfexplorerService bfexplorerService = tupledArg.Item4;

            return new UpdateSpPricesMarketBot(market, (UpdateSpPricesBotParameters)botParameters, bfexplorerService);
        }
    }

    /// <summary>
    /// ShowSelectionSpPricesBotCreator
    /// </summary>
    public sealed class ShowSelectionSpPricesBotCreator : BotCreator
    {
        public override Bot Invoke(Tuple<Market, Selection, BotParameters, IBfexplorerService> tupledArg)
        {
            Market market = tupledArg.Item1;
            Selection selection = tupledArg.Item2;
            BotParameters botParameters = tupledArg.Item3;
            IBfexplorerService bfexplorerService = tupledArg.Item4;

            return new ShowSelectionSpPricesBot(market, selection, botParameters, bfexplorerService);
        }
    }

    /// <summary>
    /// BfexplorerBotCreator
    /// </summary>
    public sealed class BfexplorerBotCreator : IBotCreator
    {
        private static BotDescriptor[] myBots = {
                new BotDescriptor(new BotId(200, "C# - My Csharp Test Bot"), new MyMarketBotParameters()),
                new BotDescriptor(new BotId(201, "C# - Update SP prices"), new UpdateSpPricesBotParameters()),
                new BotDescriptor(new BotId(202, "C# - Show Selection SP prices"), new BotParameters())
            };

        private static BotCreator[] myBotsCreators = {
                new MyMarketBotCreator(),
                new UpdateSpPricesMarketBotCreator(),
                new ShowSelectionSpPricesBotCreator()
            };

        public BotDescriptor[] Bots
        {
            get { return myBots; }
        }

        public bool GetIsMyBot(BotId botId)
        {
            // Check valid range of your bot/s id/s. Bfexplorer BotId starts from 0, use your unique number range for bot identification.
            //return botId.Id >= 200 && botId.Id <= 209; // If you have got 10 bots
            return botId.Id >= 200 && botId.Id <= 202; 
        }

        public FSharpOption<FSharpFunc<Tuple<Market, Selection, BotParameters, IBfexplorerService>, Bot>> GetBotCreator(BotId botId)
        {
            if (GetIsMyBot(botId))
            {
                var func = myBotsCreators[botId.Id - 200];

                return FSharpOption<FSharpFunc<Tuple<Market, Selection, BotParameters, IBfexplorerService>, Bot>>.Some(func);
            }
            else
            {
                return FSharpOption<FSharpFunc<Tuple<Market, Selection, BotParameters, IBfexplorerService>, Bot>>.None;
            }
        }

        private static Bot CreateMyMarketBot(Tuple<Market, Selection, BotParameters, IBfexplorerService> data)
        {
            return new MyMarketBot(data.Item1, data.Item3 as MyMarketBotParameters, data.Item4) as Bot;
        }
    }
}