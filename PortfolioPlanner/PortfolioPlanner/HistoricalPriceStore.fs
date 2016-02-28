namespace PortfolioPlanner

open DomainTypes

type HistoricalPriceStore(priceLoader : IPriceLoader, symbols) =
    let priceLoader = priceLoader
    let symbols = symbols  
    
    let keyValueStore = 
        symbols 
        |> List.map(fun s -> priceLoader.load s)
        |> List.filter(fun s -> match s with
                                | Success symbolPrices -> true        
                                | Failure symbolPrices -> false)
        |> List.map(fun res -> match res with 
                                | Success l -> (l.symbol, l.prices)
                                | _ -> ("",[])) 
        |> Map.ofList                                
        
    member __.ProvidePricesFor symbol = 
        let getPrices = function            
            | None -> Failure SymbolNotFoundError
            | Some prices -> Success {symbol=symbol; prices=prices}

        getPrices(keyValueStore.TryFind symbol)


        
