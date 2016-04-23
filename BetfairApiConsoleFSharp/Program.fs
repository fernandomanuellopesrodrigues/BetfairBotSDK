// Bfexplorer cannot be held responsible for any losses or damages incurred during the use of this betfair bot.
// It is up to you to determine the level of risk you wish to trade under. 
// Do not gamble with money you cannot afford to lose.

open System
open BeloSoft.Data
open BeloSoft.Betfair.API
open BeloSoft.Betfair.API.Models

[<EntryPoint>]
let main argv = 
    if argv.Length <> 2
    then
        failwith "Please enter your betfair user name and password!"        

    let username, password = argv.[0], argv.[1]

    let betfairServiceProvider = BetfairServiceProvider(BetfairApiServer.GBR)
    
    async {
        let! loginResult = betfairServiceProvider.AccountOperations.Login(username, password)

        if loginResult.IsSuccessResult
        then
            let filter = 
                createMarketFilterParameters()
                |> withMarketFilterParameter (MarketCountries [| "GB" |])
                |> withMarketFilterParameter (EventTypeIds [| 1 |])
                |> withMarketFilterParameter (MarketTypeCodes [| "MATCH_ODDS" |])
                |> withMarketFilterParameter (InPlayOnly false)
            
            let! marketCataloguesResult = betfairServiceProvider.BrowsingOperations.GetMarketCatalogues(filter, 10, 
                                            marketProjection = [| MarketProjection.EVENT; MarketProjection.MARKET_START_TIME; MarketProjection.COMPETITION; MarketProjection.RUNNER_DESCRIPTION; MarketProjection.MARKET_DESCRIPTION |], 
                                            sort = MarketSort.MAXIMUM_TRADED)

            if marketCataloguesResult.IsSuccessResult
            then
                let marketCatalogues = marketCataloguesResult.SuccessResult

                marketCatalogues
                |> Seq.iter (fun marketCatalogue -> 
                        let betEvent = marketCatalogue.event

                        Console.WriteLine(sprintf "%A: %s, eventId: %s, marketId: %s" betEvent.openDate betEvent.name betEvent.id marketCatalogue.marketId)
                    )

            do! betfairServiceProvider.AccountOperations.Logout() |> Async.Ignore
    }
    |> Async.RunSynchronously

    0 // return an integer exit code
