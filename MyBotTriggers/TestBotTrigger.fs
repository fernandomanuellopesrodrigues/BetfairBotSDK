// Bfexplorer cannot be held responsible for any losses or damages incurred during the use of this betfair bot.
// It is up to you to determine the level of risk you wish to trade under. 
// Do not gamble with money you cannot afford to lose.

module TestBotTrigger

open BeloSoft.Data
open BeloSoft.Bfexplorer.Domain
open BeloSoft.Bfexplorer.Trading

/// <summary>
/// TestBotTrigger
/// </summary>
type TestBotTrigger(market : Market, selection : Selection, botName : string, botTriggerParameters : BotTriggerParameters, myBfexplorer : IMyBfexplorer) =
    interface IBotTrigger with

        /// <summary>
        /// Execute
        /// </summary>
        member this.Execute() =
            let executeBot = defaultArg (botTriggerParameters.GetParameter<bool>("execute")) false
            
            if executeBot
            then
                let executeOnSelection = market.Selections.[1]
                let myBotParameters = [ { MyBotParameter.Name = "OpenBetPosition.Stake"; MyBotParameter.Value = 2.0 } ]

                ExecuteActionBotOnSelectionWithParameters (executeOnSelection, myBotParameters)
            else
                EndExecution

        /// <summary>
        /// EndExecution
        /// </summary>
        member this.EndExecution() =
            ()