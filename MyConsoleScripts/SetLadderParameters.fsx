#r @"C:\Program Files (x86)\BeloSoft\Bfexplorer\BeloSoft.Data.dll"
#r @"C:\Program Files (x86)\BeloSoft\Bfexplorer\BeloSoft.Bfexplorer.Domain.dll"

open BeloSoft.Data
open BeloSoft.Bfexplorer.Domain

let bfexplorer : IBfexplorerConsole = nil

bfexplorer.SetBestPricesDepth(3)
bfexplorer.SetShowFullMarketDepth(false)
bfexplorer.SetShowVirtualBets(false)
