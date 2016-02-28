namespace PortfolioPlanner

open System

module DomainTypes =    
    
    type DatePrice = {
        date : DateTime;
        price : double
    }  
      
    type SymbolPrices = {
        symbol : string;
        prices : DatePrice list
    }

    type PortfolioAsset = {
        symbol : string;
        expectedReturn : double;
        stdDeviation : double;
        weight : double 
    }

    type Correlation = {
        symbolPair : string * string;
        correlation : double
    }

    type GenericReturn<'a> = 
        | Generic        

    type Result<'TSuccess, 'TFailure> = 
        | Success of 'TSuccess
        | Failure of 'TFailure

    type ErrorTypes =
        | PriceLoadingError 
        | SymbolNotFoundError
        | DivisionByZero

    let bind switchFunction twoTrackInput = 
        match twoTrackInput with
        | Success s -> switchFunction s
        | Failure f -> Failure f
