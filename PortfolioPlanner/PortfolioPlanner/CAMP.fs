namespace PortfolioPlanner

open MathUtils
open DomainTypes

type CAPM(Rf : double, 
          Em : double,
          correlationM : CorrelationMatrix,
          priceStore : HistoricalPriceStore,
          marketSymbol : string) =

    let beta symbol=
        let prices = 
            symbol
            |> priceStore.ProvidePricesFor
        let stdDev =
            match prices with
            | Failure f -> Failure f
            | Success s -> Success (calcStdDev s)
        stdDev

    member __.EpectedReturn symbol : Result<double,ErrorTypes> =
        match beta symbol with
        | Success b -> Success (Rf + (Em - Rf)*b)
        | Failure f -> Failure f 

