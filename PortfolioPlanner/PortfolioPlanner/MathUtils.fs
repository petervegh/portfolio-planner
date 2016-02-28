namespace PortfolioPlanner

open DomainTypes

module MathUtils =
    let pow2 x = pown x 2

    let stdDev (symbolPrices : SymbolPrices) =
        let sum = 
            symbolPrices.prices
            |> List.fold(fun acc elem -> elem.price + acc) 0.0
        let mean = sum / (float)symbolPrices.prices.Length
        let nominator = 
            symbolPrices.prices
            |> List.fold(fun acc elem -> pow2(elem.price - mean) + acc) 0.0
        sqrt(nominator / (float)symbolPrices.prices.Length)

    let calcStdDev (symbolPrices : SymbolPrices) = 
        match symbolPrices.prices with
        | [] -> 0.0
        | _ -> stdDev symbolPrices
        
        

