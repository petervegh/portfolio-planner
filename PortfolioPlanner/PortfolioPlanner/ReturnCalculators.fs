namespace PortfolioPlanner

open DomainTypes
open System

module ReturnCalculators =
    
    let logReturn pricePair =
        let priceTm1 = (fst pricePair).price
        let priceT = (snd pricePair).price
        let calc =
            match priceTm1 with
            | 0.0 -> Failure DivisionByZero
            | _ -> Success {date=(snd pricePair).date; price=log(( (priceT / priceTm1) ))}              
        calc 

    let calculateLogReturn (symbolPrices : SymbolPrices) =
        let logReturns =
            symbolPrices.prices 
            |> List.toSeq
            |> Seq.pairwise
            |> Seq.map(fun r -> logReturn r)
            |> Seq.toList
            
        let errors = 
            logReturns
            |> List.filter( fun r -> match r with 
                                     | Failure f -> true
                                     | _ -> false)
        let returns = 
            logReturns
            |> List.filter( fun r -> match r with 
                                     | Success s -> true
                                     | _ -> false)
            |> List.map (fun r -> match r with
                                  | Success s -> s
                                  | Failure f -> {date=DateTime(1900,01,01); price=0.0})
        if errors.Length > 0 then
            Failure DivisionByZero
        else
            Success (symbolPrices.symbol, returns)
        
