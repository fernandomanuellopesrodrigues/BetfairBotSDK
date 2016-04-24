﻿#r @"C:\Program Files (x86)\BeloSoft\Bfexplorer\BeloSoft.Data.dll"
#r @"C:\Program Files (x86)\BeloSoft\Bfexplorer\BeloSoft.Bfexplorer.Domain.dll"

open BeloSoft.Data
open BeloSoft.Bfexplorer.Domain

let getCanCloseTheMarket(market : Market) =
    not(market.Selections |> Seq.exists (fun selection -> selection.IsWatched))

let bfexplorer : IBfexplorerConsole = nil

async {
    let marketsToClose = bfexplorer.OpenMarkets |> List.filter getCanCloseTheMarket

    if not marketsToClose.IsEmpty
    then
        bfexplorer.CloseMarkets(marketsToClose)
}
|> Async.RunSynchronously