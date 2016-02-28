namespace PortfolioPlanner

open DomainTypes
open FSharp.Data
open System

type Stocks = CsvProvider<"http://ichart.finance.yahoo.com/table.csv?s=MSFT">

[<Interface>]
type IPriceLoader = 
    abstract member load : string -> Result<SymbolPrices, ErrorTypes>


//TODO: add more life like data
type MockPriceLoader() = 
    interface IPriceLoader with
        member __.load symbol = 
            match symbol with
            | "MSFT" -> Success {symbol="MSFT"; prices=[
                                            {date=DateTime(1900,01,01); price= 50.123};
                                            {date=DateTime(1900,01,02); price= 50.124};
                                            {date=DateTime(1900,01,03); price= 50.125};
                                            {date=DateTime(1900,01,04); price= 50.126};
                                            {date=DateTime(1900,01,05); price= 50.127}]}
            | "S&P500" -> Success {symbol="S&P500"; prices=[
                                            {date=DateTime(1900,01,01); price= 1550.123};
                                            {date=DateTime(1900,01,02); price= 1550.124};
                                            {date=DateTime(1900,01,03); price= 1550.125};
                                            {date=DateTime(1900,01,04); price= 1550.126};
                                            {date=DateTime(1900,01,05); price= 1550.127}]}
            | "GOOG" -> Success {symbol="GOOG"; prices=[
                                            {date=DateTime(1900,01,01); price= 80.123};
                                            {date=DateTime(1900,01,02); price= 80.124};
                                            {date=DateTime(1900,01,03); price= 80.125};
                                            {date=DateTime(1900,01,04); price= 80.126};
                                            {date=DateTime(1900,01,05); price= 80.127}]}
            | _ -> Failure PriceLoadingError


type YahooPriceLoader() =
    member __.count = 100

    interface IPriceLoader with
        member __.load symbol = 
            let url = "http://ichart.finance.yahoo.com/table.csv?s="
            let stockData =
                try
                    let prices = 
                        Stocks.Load(url + symbol).Take(__.count).Rows 
                        |> Seq.map(fun r -> {date=r.Date; price=(double)r.Open}) 
                        |> Seq.toList
                    Success {symbol=symbol; prices=prices}
                with
                    | :? System.Net.WebException as ex -> printfn "%s" ex.Message; Failure PriceLoadingError                        
            stockData               
            
