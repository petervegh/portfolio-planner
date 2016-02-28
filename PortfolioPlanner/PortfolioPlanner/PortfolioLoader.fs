namespace PortfolioPlanner

open DomainTypes
open FSharp.Data

module PortfolioLoader =

    type PortfolioCsvLoader = CsvProvider<"Data\PortfolioTemplate.csv">
    
    //TODO: return Result<'TSuccess, 'TFailure>
    let loadPortfolioCsv (path:string) =
        PortfolioCsvLoader.Load(path).Rows
        |> Seq.map(fun row -> {symbol=row.Asset; expectedReturn=(double)row.ExpectedReturn;
                               stdDeviation=(double)row.StdDeviation; weight=(double)row.Weight})
        |> Seq.toList
        
    



