namespace PortfolioPlanner

open DomainTypes
open RDotNet
open RProvider
open RProvider.stats

//TOOD: think about stocks where trading dates do not match? 
type CorrelationMatrix(symbols : string list, historicalPriceStore : HistoricalPriceStore) =
    
    //TODO: make this tail recursive
    let rec pairUp list =
        match list with    
        | head::tail -> List.map(fun elem -> (head, elem)) tail @ pairUp tail   
        | [] -> []

    let pricesFor symbol = 
        symbol 
        |> historicalPriceStore.ProvidePricesFor
        |> function 
           | Success sym -> sym.prices
           | Failure _ -> []
        |> List.map(fun prices -> prices.price)


    let keyValueStore = 
        symbols
        |> pairUp
        |> List.map(fun symbolPairs -> 
            (symbolPairs,R.cor(pricesFor(fst symbolPairs), pricesFor(snd symbolPairs)).AsNumeric().[0]))
        |> Map.ofList

    member __.Correlation symbolPair = 
        let correlation = function            
            | None -> Failure SymbolNotFoundError
            | Some corr -> Success {symbolPair=symbolPair; correlation=corr}

        correlation(keyValueStore.TryFind symbolPair)