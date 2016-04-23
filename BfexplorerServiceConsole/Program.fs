// Bfexplorer cannot be held responsible for any losses or damages incurred during the use of this betfair bot.
// It is up to you to determine the level of risk you wish to trade under. 
// Do not gamble with money you cannot afford to lose.

open System

open BeloSoft.Data
open BeloSoft.Bfexplorer.Service
open BeloSoft.Bfexplorer.Domain

let showMarketData (market : Market) =
    Console.WriteLine(sprintf "%s, Status: %s, Total matched: %.2f" (market.ToString()) market.MarketStatusText market.TotalMatched)

    market.Selections
    |> Seq.iter (fun selection -> Console.WriteLine(sprintf "\t%s: %.2f | %.2f" selection.Name selection.LastPriceTraded selection.TotalMatched))

[<EntryPoint>]
let main argv = 
    if argv.Length <> 2
    then
        failwith "Please enter your betfair user name and password!"

    let username, password = argv.[0], argv.[1]

    let bfexplorerService = BfexplorerService()

    async {
        let! loginResult = bfexplorerService.Login(username, password)

        if loginResult.IsSuccessResult
        then
            let filter = [ 
                    //Countries [| "GB" |]; 
                    BetEventTypeIds [| 1 |]; MarketTypeCodes [| "MATCH_ODDS" |]; MarketTypeCodes [| "MATCH_ODDS" |];
                    //InPlayOnly false; 
                    InPlayOnly true; 
                    TurnInPlayEnabled true
                ]
            
            let! marketCataloguesResult = bfexplorerService.GetMarketCatalogues(filter, 10)

            if marketCataloguesResult.IsSuccessResult
            then
                let marketCatalogues = 
                    marketCataloguesResult.SuccessResult
                    |> Seq.sortByDescending (fun marketCatalogue -> marketCatalogue.MarketInfo.StartTime)
                    |> Seq.toArray

                marketCatalogues
                |> Array.iter (fun marketCatalogue -> 
                        let marketInfo = marketCatalogue.MarketInfo
                        let betEvent = marketInfo.BetEvent

                        Console.WriteLine(sprintf "%A: %s, eventId: %d, marketId: %s" betEvent.OpenTime betEvent.Name betEvent.Id marketInfo.Id)
                    )

                let marketInfo = marketCatalogues.[0].MarketInfo

                let! marketResult = bfexplorerService.GetMarket(marketInfo)

                if marketResult.IsSuccessResult
                then                    
                    let market = marketResult.SuccessResult

                    showMarketData market

                    let continueLooping = ref true

                    Console.CancelKeyPress.Add (fun _ -> continueLooping := false)

                    while !continueLooping do
                        do! Async.Sleep(5000)

                        let! result = bfexplorerService.UpdateMarketBaseData(market)

                        if result.IsSuccessResult && market.IsUpdated
                        then
                            showMarketData market
                                                    
            do! bfexplorerService.Logout() |> Async.Ignore
    }
    |> Async.RunSynchronously

    0 // return an integer exit code