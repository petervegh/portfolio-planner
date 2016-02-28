namespace PortfolioPlanner

open FsUnit
open NUnit.Framework
open System
open DomainTypes
open ReturnCalculators
open PortfolioLoader
open MathUtils

//TODO: Rename module/refactor tests to separate modules
[<TestFixture>]
module HistoricalPriceLoaderTests =  

    let checkStatus (input : Result<SymbolPrices, ErrorTypes>) = 
        match input with
        | Success l -> true
        | _ -> false

    let getData (input : Result<SymbolPrices, ErrorTypes>) = 
        match input with
        | Success l -> (l.symbol, l.prices)
        | _ -> ("", [])
       
        
    [<Test>]
    let ``When a YahooPriceLoader.load MSFT is run I should get back 100 historical prices or an Error``() =
        let yahooPriceLoader = new YahooPriceLoader() :> IPriceLoader
        "MSFT" |> yahooPriceLoader.load |> checkStatus |> should be True

    [<Test>]
    let ``When a YahooPriceLoader.load SILLYSYMBOL is run I should get back PriceLoadingError``() =
        let yahooPriceLoader = new YahooPriceLoader() :> IPriceLoader
        let expected : Result<SymbolPrices, ErrorTypes> = Failure PriceLoadingError
        "SILLYSYMBOL" |> yahooPriceLoader.load |> should equal expected
                
    [<Test>]
    let ``When a HistoricalPriceLoader.providePriceFor MSFT is run I should get back a Success status``() =        
        let yahooPriceLoader = new YahooPriceLoader() :> IPriceLoader
        let historicalPriceStore = new HistoricalPriceStore(yahooPriceLoader, ["MSFT"; "GOOG"; "SILLYCRAP"])
        "MSFT" |> historicalPriceStore.ProvidePricesFor |> checkStatus |> should be True        

    [<Test>]
    let ``When a HistoricalPriceLoader.providePriceFor MSFT is run I should get back a Success and symbol is MSFT status``() =        
        let yahooPriceLoader = new YahooPriceLoader() :> IPriceLoader
        let historicalPriceStore = new HistoricalPriceStore(yahooPriceLoader, ["MSFT"; "GOOG"; "SILLYCRAP"])
        "MSFT" |> historicalPriceStore.ProvidePricesFor |> getData |> fst |> should equal "MSFT"   
        
    [<Test>]
    let ``When price1=(DateTime("2016/02/13"), 0.05) and price2=(DateTime("2016/02/14"), 0.04) then logReturn should equal 1``() =
        let price1 = {date=DateTime(2016,02,13); price=0.05}
        let price2 = {date=DateTime(2016,02,14); price=0.04}
        let expected = {date=DateTime(2016,02,14); price= -0.22314355131420985}
        logReturn (price1,price2) |> should equal expected

    //TODO: give these proper descriptions pretty please...
    [<Test>]
    let ``When...``() = 
        let input = 
            {symbol="MSFT"; prices=[{date=DateTime(2016,02,12); price=1.0};{date=DateTime(2016,02,13); price=2.0};{date=DateTime(2016,02,14); price=3.0}]}
        let expected = 
            ("MSFT", [{date=DateTime(2016,02,13); price=0.69314718055994529}; {date=DateTime(2016,02,14); price=0.40546510810816438}])
        calculateLogReturn input |> should equal expected

    [<Test>]
    let ``When.....``() =         
        let expected = [{symbol="MSFT"; expectedReturn=18.0; stdDeviation=40.0; weight=0.5};
                        {symbol="GOOG"; expectedReturn=23.0; stdDeviation=50.0; weight=0.5}]
        loadPortfolioCsv("../Data/Portfolio.csv") |> should equal expected

    //This will keep failing
    [<Test>]
    let ``When......``() =  
        let symbols = ["MSFT"; "GOOG"; "YHOO"]
        let yahooPriceLoader = new YahooPriceLoader() :> IPriceLoader
        let historicalPriceStore = new HistoricalPriceStore(yahooPriceLoader, symbols)
        let correlationMatrix = new CorrelationMatrix(symbols, historicalPriceStore)
        let correlation = correlationMatrix.Correlation ("MSFT","GOOG")
        let expected : Result<Correlation,ErrorTypes> = Success {symbolPair=("MSFT","GOOG"); correlation=0.95716688128692962}
        correlation |> should equal expected

    [<Test>]
    let ``When.......``() =  
        let input = 
            {symbol="MSFT"; prices=[{date=DateTime(2016,02,13); price=1.0}; {date=DateTime(2016,02,14); price=2.0}; {date=DateTime(2016,02,15); price=3.0}]}
        let result = calcStdDev input 
        result |> should equal 0.816496580927726

    [<Test>]
    let ``When........``() =  
        let input = 
            {symbol="MSFT"; prices=[]}
        let result = calcStdDev input 
        result |> should equal 0.0

    [<Test>]
    let ``When I pass in all the params CAPM will give me the expected return of the Asset``() =
        let Rf = 0.05
        let Em = 0.1
        let marketSymbol = "S&P500"
        let mockPriceLoader = new MockPriceLoader() :> IPriceLoader
        let historicalPriceStore = new HistoricalPriceStore(mockPriceLoader, ["MSFT"; "GOOG"; "S&P500"])
        let corM = new CorrelationMatrix(["MSFT";"GOOG";"S&P500"], historicalPriceStore)
        let capm = new CAPM(Rf, Em, corM, historicalPriceStore, marketSymbol)
        let expected : Result<double,ErrorTypes> = Success 0.05007071068
        let result = capm.EpectedReturn "MSFT"
        result |> should equal expected

        //((PortfolioPlanner.DomainTypes.Result<double,PortfolioPlanner.DomainTypes.ErrorTypes>.Success)(expected))