using NQuotes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMT_Scalper
{
    /// <summary>
    /// XMT-Scalper v. 2.4.2
    ///
    ///                                      by Capella
    ///                             http://www.worldwide-invest.org
    ///                                  Copyright 2011 - 2013
    /// 
    /// This code was traduct by Abdallah Hacid https://www.facebook.com/ab.hacid.
    /// This traduction is make in Csharp style with NQuotes library in order to capitalize on dotnet framework and 
    /// Visual studio infrastructure.
    /// 
    /// Project Hosting for Open Source Software on Github : https://github.com/abhacid/cAlgoBot
    /// </summary>
    /// 

    public class XMTScalper : MqlApiWithStdLib
    {

        //----------------------- Include files ------------------------------------------------------------

        // Note: If the below files are stored in the installation directory of MT4 then the files should be
        // written with " " around their names. If you however prefer to have the include files in the same 
        // directory as this EA, then the files below shoul be surropunded by < > instead.
        //# include "stdlib.mqh"        // "stdlib.mqh" or "<sdlib.mqh> 
        //# include "stderror.mqh"      // "stderror.mqh" or <stderror.mqh>

        //----------------------- External Globals ----------------------------------------------------------------
        // All globals should here have their name starting with a CAPITAL character

        [ExternVariable] public string Configuration             = "==== Configuration ====";
        [ExternVariable] public bool ReverseTrade                = false;    // If true, then trade in opposite direction
        [ExternVariable] public int Magic                        = -1;       // If set to a number less than 0 it will calculate MagicNumber automatically

        [ExternVariable] public string OrderCmt                  = "XMT-Scalper 2.46"; // Trade comments that appears in the Trade and Account History tab
        [ExternVariable] public bool ECN_Mode                    = false;    // True for brokers that don't accept SL and TP to be sent at the same time as the order
        [ExternVariable] public bool Debug                       = false;    // Print huge log files with info, only for debugging purposes
        [ExternVariable] public bool Verbose                     = false;    // Additional information printed in the chart

        [ExternVariable] public string TradingSettings           = "==== Trade settings ====";
        [ExternVariable] public double MaxSpread                 = 30.0;     // Max allowed spread in points (1 / 10 pip)
        [ExternVariable] public int MaxExecution                = 0;        // Max allowed average execution time in ms (0 means no restrictions)
        [ExternVariable] public int MaxExecutionMinutes          = 5;        // How often in minutes should fake orders be sent to measure execution speed
        [ExternVariable] public double StopLoss                  = 60;       // StopLoss from as many points. Default 60 (= 6 pips)
        [ExternVariable] public double TakeProfit                = 100;      // TakeProfit from as many points. Default 100 (= 10 pip)
        [ExternVariable] public double AddPriceGap               = 0;        // Additional price gap in points added to SL and TP in order to avoid Error 130
        [ExternVariable] public double TrailingStart             = 20;       // Start trailing profit from as so many points. 
        [ExternVariable] public double Commission                = 0;        // Some broker accounts charge commission in USD per 1.0 lot. Commission in dollar
        [ExternVariable] public int Slippage                     = 3;        // Maximum allowed Slippage in points
        [ExternVariable] public double MinimumUseStopLevel       = 0;        // Minimum stop level. Stoplevel to use will be max value of either this value or broker stoplevel 

        [ExternVariable] public string VolatilitySettings        = "==== Volatility Settings ====";
        [ExternVariable] public bool UseDynamicVolatilityLimit   = true;     // Calculate VolatilityLimit based on INT (spread * VolatilityMultiplier)
        [ExternVariable] public double VolatilityMultiplier      = 125;      // Dynamic value, only used if UseDynamicVolatilityLimit is set to true
        [ExternVariable] public double VolatilityLimit           = 180;      // Fix value, only used if UseDynamicVolatilityLimit is set to false
        [ExternVariable] public bool UseVolatilityPercentage     = true;     // If true, then price must break out more than a specific percentage
        [ExternVariable] public double VolatilityPercentageLimit = 0;        // Percentage of how much iHigh-iLow difference must differ from VolatilityLimit. 0 is risky, 60 means a safe value

        [ExternVariable] public string UseIndicatorSet           = "=== Indicators: 1 = Moving Average, 2 = BollingerBand, 3 = Envelopes";
        [ExternVariable] public int _UseIndicatorSwitch          = 1;        // Switch User indicators. 
        [ExternVariable] public int Indicatorperiod              = 3;        // Period in bars for indicators
        [ExternVariable] public double _BBDeviation              = 2.0;      // Deviation for the iBands indicator
        [ExternVariable] public double EnvelopesDeviation        = 0.07;     // Deviation for the iEnvelopes indicator
        [ExternVariable] public int OrderExpireSeconds           = 3600;     // Orders are deleted after so many seconds

        [ExternVariable] public string Money_Management          = "==== Money Management ====";
        [ExternVariable] public bool MoneyManagement             = true;     // If true then calculate lotsize automaticallay based on Risk, if False then use ManualLotsize below
        [ExternVariable] public double MinLots                   = 0.01;     // Minimum lot-size to trade with
        [ExternVariable] public double MaxLots                   = 100.0;    // Maximum allowed lot-size to trade with
        [ExternVariable] public double Risk                      = 2.0;      // Risk setting in percentage, For 10.000 in Equity 10% Risk and 60 StopLoss lotsize = 16.66
        [ExternVariable] public double ManualLotsize             = 0.1;      // Manual lotsize to trade with if MoneyManagement above is set to false
        [ExternVariable] public double MinMarginLevel            = 100;      // Lowest allowed Margin level for new positions to be opened. 

        [ExternVariable] public string Screen_Shooter            = "==== Screen Shooter ====";
        [ExternVariable] public bool TakeShots                   = false;    // Save screen shots on STOP orders?
        [ExternVariable] public int DelayTicks                   = 1;        // Delay so many ticks after new bar
        [ExternVariable] public int ShotsPerBar                  = 1;        // How many screen shots per bar

        [ExternVariable] public string DisplayGraphics           = "=== Display Graphics ==="; // Colors for Display at upper left
        [ExternVariable] public int Heading_Size                 = 13;       // Font size for headline
        [ExternVariable] public int Text_Size                    = 12;       // Font size for texts
        [ExternVariable] public Color Color_Heading              = Color.Lime;   // Color for text lines
        [ExternVariable] public Color Color_Section1             = Color.Yellow;            // -"-
        [ExternVariable] public Color Color_Section2             = Color.Aqua;              // -"-
        [ExternVariable] public Color Color_Section3             = Color.Orange;            // -"-
        [ExternVariable] public Color Color_Section4             = Color.Magenta;         // -"-

        //--------------------------- Globals --------------------------------------------------------------
        // All globals should here have their name starting with a CAPITAL character

        string EA_version = "XMT-Scalper v. 2.46";

        int BrokerDigits                        = 0;    // Nnumber of digits that the broker uses for this currency pair
        int GlobalError                         = 0;    // To keep track on number of added errors
        DateTime _lastTime                      = DateTime.MinValue;    // For measuring tics
        int TickCounter                         = 0;    // Counting tics
        int UpTo30Counter                       = 0;    // For calculating average spread
        int _executionTickCount                 = -1;   // For Execution speed, -1 means no speed
        int Avg_execution                       = 0;    // Average Execution speed
        int Execution_samples                   = 0;    // For calculating average Execution speed
        DateTime _startTime;                            // Initial time

        int Leverage;                                   // Account Leverage in percentage
        double _lotBase;                                // Amount of money in base currency for 1 lot

        int Err_unchangedvalues;                        // Error count for unchanged values (modify to the same values)
        int Err_busyserver;                             // Error count for busy server
        int Err_lostconnection;                         // Error count for lost connection
        int Err_toomanyrequest;                         // Error count for too many requests
        int Err_invalidprice;                           // Error count for invalid price
        int Err_invalidstops;                           // Error count for invalid SL and/or TP
        int Err_invalidtradevolume;                     // Error count for invalid lot size
        int Err_pricechange;                            // Error count for change of price
        int Err_brokerbuzy;                             // Error count for broker is buzy
        int Err_requotes;                               // Error count for requotes
        int Err_toomanyrequests;                        // Error count for too many requests
        int Err_trademodifydenied;                      // Error count for modify orders is denied
        int Err_tradecontextbuzy;                       // error count for trade context is buzy

        int SkippedTicks = 0;                           // Used for simulation of latency during backtests, how many tics that should be skipped
        int Ticks_samples = 0;                          // Used for simulation of latency during backtests, number of tick samples

        int Tot_closed_pos;                             // Number of closed positions for this EA
        int Tot_Orders;                                 // Number of open orders disregarding of magic and pairs
        int Tot_open_pos;                               // Number of open positions for this EA

        double Tot_open_profit;                         // A summary of the current open profit/loss for this EA
        double Tot_open_lots;                           // A summary of the current open lots for this EA
        double Tot_open_swap;                           // A summary of the current charged swaps of the open positions for this EA
        double Tot_open_commission;                     // A summary of the currebt charged commission of the open positions for this EA

        double G_equity;                                // Current equity for this EA
        double Changedmargin;                           // Free margin for this account

        double Tot_closed_lots;                         // A summary of the current closed lots for this EA
        double Tot_closed_profit;                       // A summary of the current closed profit/loss for this EA 
        double Tot_closed_swap;                         // A summary of the current closed swaps for this EA
        double Tot_closed_comm;                         // A summary of the current closed commission for this EA

        double G_balance = 0;                           // Balance for this EA
        double[] Array_spread = new double[30];         // Store spreads for the last 30 tics
        double _lotSize;                                // Lotsize

        double _highest=double.MinValue;                                 // LotSize indicator value
        double _lowest = double.MaxValue;                                  // Lowest indicator value

        double _stopLevel;                               // Broker StopLevel
        double StopOut;                                 // Broker stoput percentage

        double LotStep;                                 // Broker LotStep
        double MarginForOneLot;                         // Margin required for 1 lot
        double Avg_tickspermin;                         // Used for simulation of latency during backtests
        double MarginFree;                              // Free margin in percentage

        DateTime _lastOpenTimeBar;
        int _doShot = -1;
        double _oldTimePhase = 3000000;
        static int indexOfScreenShot = 0;


        /// <summary>
        /// Program initialization
        /// </summary>
        /// <returns></returns>
        public override int init()
        {
            // Print short message at the start of initalization
            Print("====== Initialization of ", EA_version, " ======");

            // If we have any objects on the screen then clear the screen
            deleteDisplay();   // clear the chart
            Comment("");    // clear the chart	

            // Reset time for Execution control
            _startTime = TimeLocal();

            // Reset error variable
            GlobalError = -1;

            // Get the broker decimals
            BrokerDigits = Digits;

            // Get Leverage
            Leverage = AccountLeverage();

            // Calculate StopLevel as max of either STOPLEVEL or FREEZELEVEL
            _stopLevel = MathMax(MarketInfo(Symbol(), MODE_FREEZELEVEL), MarketInfo(Symbol(), MODE_STOPLEVEL));
            // Then calculate the StopLevel as max of either this StopLevel or MinimumUseStopLevel
            _stopLevel = MathMax(MinimumUseStopLevel, _stopLevel);

            // Get stoput level and re-calculate as fraction
            StopOut = AccountStopoutLevel();

            // Calculate LotStep
            LotStep = MarketInfo(Symbol(), MODE_LOTSTEP);

            // Check to confirm that indicator switch is valid choices, if not force to 1 (Moving Average)
            if (_UseIndicatorSwitch < 1 || _UseIndicatorSwitch > 4)
                _UseIndicatorSwitch = 1;

            // If indicator switch is set to 4, using iATR, tben UseVolatilityPercentage cannot be used, so force it to false
            if (_UseIndicatorSwitch == 4)
                UseVolatilityPercentage = false;

            // Adjust SL and TP to broker StopLevel if they are less than this StopLevel
            StopLoss = MathMax(StopLoss, _stopLevel);
            TakeProfit = MathMax(TakeProfit, _stopLevel);

            // Re-calculate variables 
            VolatilityPercentageLimit = VolatilityPercentageLimit / 100 + 1;
            VolatilityMultiplier = VolatilityMultiplier / 10;
            ArrayInitialize(Array_spread, 0);
            VolatilityLimit = VolatilityLimit * Point;
            Commission = normalizebrokerdigits(Commission * Point);
            TrailingStart = TrailingStart * Point;
            _stopLevel = _stopLevel * Point;
            AddPriceGap = AddPriceGap * Point;

            // If we have set MaxLot and/or MinLots to more/less than what the broker allows, then adjust it accordingly
            if (MinLots < MarketInfo(Symbol(), MODE_MINLOT))
                MinLots = MarketInfo(Symbol(), MODE_MINLOT);
            if (MaxLots > MarketInfo(Symbol(), MODE_MAXLOT))
                MaxLots = MarketInfo(Symbol(), MODE_MAXLOT);
            if (MaxLots < MinLots)
                MaxLots = MinLots;

            // Calculate margin required for 1 lot
            MarginForOneLot = MarketInfo(Symbol(), MODE_MARGINREQUIRED);

            // Amount of money in base currency for 1 lot
            _lotBase = MarketInfo(Symbol(), MODE_LOTSIZE);

            // Also make sure that if the risk-percentage is too low or too high, that it's adjusted accordingly
            recalculatewrongrisk();

            // Calculate intitial LotSize 
            _lotSize = calculateLotSize();

            // If magic number is set to a value less than 0, then calculate MagicNumber automatically
            if (Magic < 0)
                Magic = magicnumber();

            // If Execution speed should be measured, then adjust maxexecution from minutes to seconds	 
            if (MaxExecution > 0)
                MaxExecutionMinutes = MaxExecution * 60;

            // Print initial info 
            printdetails();

            // Check through all closed and open orders to get stats
            checkThroughAllClosedOrders();
            checkThroughAllOpenOrders();
            // Show info in graphics
            showGraphInfo();

            // Print short message at the end of initialization
            Print("========== Initialization complete! ===========\n");

            // Finally call the main trading subroutine
            start();

            return (0);
        }

        /// <summary>
        /// Program deinitialization
        /// </summary>
        /// <returns></returns>
        public override int deinit()
        {
            string text = "";

            // Print summarize of broker errors
            printsumofbrokererrors();

            // Delete all objects on the screen
            deleteDisplay();
            //( Check through all closed orders
            checkThroughAllClosedOrders();
            // If we're running as backtest, then print some result
            if (IsTesting() == true)
            {
                Print("Total closed lots = ", DoubleToStr(Tot_closed_lots, 2));
                Print("Total closed swap = ", DoubleToStr(Tot_closed_swap, 2));
                Print("Total closed commission = ", DoubleToStr(Tot_closed_comm, 2));

                // If we run backtests and simulate latency, then print result
                if (MaxExecution > 0)
                {
                    text = text + "During backtesting " + SkippedTicks + " number of ticks was ";
                    text = text + "skipped to simulate latency of up to " + MaxExecution + " ms";
                    printandcomment(text);
                }
            }

            // Print short message when EA has been deinitialized
            Print(EA_version, " has been deinitialized!");

            return (0);
        }

        /// <summary>
        /// Program start
        /// </summary>
        /// <returns></returns>
        public override int start()
        {
            // We must wait til we have enough of bar data before we call trading routine
            if (iBars(Symbol(), PERIOD_M1) > Indicatorperiod)
            {
                showSymbolInfos();

                trade();

                // Check through all closed and open orders to get stats
                checkThroughAllClosedOrders();
                checkThroughAllOpenOrders();

                showGraphInfo();
            }
            else
                Print("Please wait until enough of bar data has been gathered!");

            return (0);
        }

        /// <summary>
        /// 
        /// </summary>
        void showSymbolInfos()
        {
            Print("Symbol=", Symbol());
            Print("Low day price=", MarketInfo(Symbol(), MODE_LOW));
            Print("High day price=", MarketInfo(Symbol(), MODE_HIGH));
            Print("The last incoming tick time=", (MarketInfo(Symbol(), MODE_TIME)));
            Print("Last incoming bid price=", MarketInfo(Symbol(), MODE_BID));
            Print("Last incoming ask price=", MarketInfo(Symbol(), MODE_ASK));
            Print("Point size in the quote currency=", MarketInfo(Symbol(), MODE_POINT));
            Print("Digits after decimal point=", MarketInfo(Symbol(), MODE_DIGITS));
            Print("Spread value in points=", MarketInfo(Symbol(), MODE_SPREAD));
            Print("Stop level in points=", MarketInfo(Symbol(), MODE_STOPLEVEL));
            Print("Lot size in the base currency=", MarketInfo(Symbol(), MODE_LOTSIZE));
            Print("Tick value in the deposit currency=", MarketInfo(Symbol(), MODE_TICKVALUE));
            Print("Tick size in points=", MarketInfo(Symbol(), MODE_TICKSIZE));
            Print("Swap of the buy order=", MarketInfo(Symbol(), MODE_SWAPLONG));
            Print("Swap of the sell order=", MarketInfo(Symbol(), MODE_SWAPSHORT));
            Print("Market starting date (for futures)=", MarketInfo(Symbol(), MODE_STARTING));
            Print("Market expiration date (for futures)=", MarketInfo(Symbol(), MODE_EXPIRATION));
            Print("Trade is allowed for the symbol=", MarketInfo(Symbol(), MODE_TRADEALLOWED));
            Print("Minimum permitted amount of a lot=", MarketInfo(Symbol(), MODE_MINLOT));
            Print("Step for changing lots=", MarketInfo(Symbol(), MODE_LOTSTEP));
            Print("Maximum permitted amount of a lot=", MarketInfo(Symbol(), MODE_MAXLOT));
            Print("Swap calculation method=", MarketInfo(Symbol(), MODE_SWAPTYPE));
            Print("Profit calculation mode=", MarketInfo(Symbol(), MODE_PROFITCALCMODE));
            Print("Margin calculation mode=", MarketInfo(Symbol(), MODE_MARGINCALCMODE));
            Print("Initial margin requirements for 1 lot=", MarketInfo(Symbol(), MODE_MARGININIT));
            Print("Margin to maintain open orders calculated for 1 lot=", MarketInfo(Symbol(), MODE_MARGINMAINTENANCE));
            Print("Hedged margin calculated for 1 lot=", MarketInfo(Symbol(), MODE_MARGINHEDGED));
            Print("Free margin required to open 1 lot for buying=", MarketInfo(Symbol(), MODE_MARGINREQUIRED));
            Print("Order freeze level in points=", MarketInfo(Symbol(), MODE_FREEZELEVEL));
        }

        /// <summary>
        /// This is the main trading subroutine
        /// </summary>
        void trade()
        {
            string textstring;
            //string pair;
            string indy;

            bool select;
            bool wasOrderModified = false;

#pragma warning disable CS0219 // Variable is assigned but its value is never used
            bool ordersenderror;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            bool isBidGreaterThanIma = false;
            bool isBidGreaterThaniBand = false;
            bool isBidGreaterThanEnvelopes = false;
            bool isBidGreaterThanIndy;

            int orderticket;
            DateTime orderExpireTime = DateTime.MinValue;
            int pricedirection;
            int counter1;
            int counter2;

            double _ask;
            double _bid;
            double _askPlusDistance;
            double _bidMinusDistance;

            double _volatilityPercentage = 0;

            double orderprice;
            double orderstoploss;
            double ordertakeprofit;

            double ihigh;
            double ilow;

            double _iMALow = 0;
            double _iMAHigh = 0;
            double _iMADiff;

            double _ibandsupper = 0;
            double _ibandslower = 0;
            double _ibandsdiff;

            double _envelopesupper = 0;
            double _envelopeslower = 0;
            double envelopesdiff;

            double volatility;
            double spread;
            double avgSpread;
            double realAvgSpread;
            double fakeprice;
            double askpluscommission;
            double bidminuscommission;

            double skipticks;

            double am = 0.000000001;  // Set variable to a very small number

            double marginlevel;

            // Get the Free Margin
            MarginFree = AccountFreeMargin();
            // Calculate Margin level
            if (AccountMargin() != 0)
                am = AccountMargin();
            marginlevel = AccountEquity() / am * 100;

            // Free Margin is less than the value of MinMarginLevel, so no trading is allowed
            if (marginlevel < MinMarginLevel)
            {
                Comment("Warning! Free Margin " + DoubleToStr(marginlevel, 2) + " is lower than MinMarginLevel!");
                Alert("Warning! Free Margin " + DoubleToStr(marginlevel, 2) + " is lower than MinMarginLevel!");
                return;
            }

            // Previous time was less than current time, initiate tick counter
            if (_lastTime < Time[0])
            {
                // For simulation of latency during backtests, consider only 10 samples at most.
                if (Ticks_samples < 10)
                    Ticks_samples++;
                Avg_tickspermin = Avg_tickspermin + (TickCounter - Avg_tickspermin) / Ticks_samples;
                // Set previopus time to current time and reset tick counter
                _lastTime = Time[0];
                TickCounter = 0;
            }
            // Previous time was NOT less than current time, so increase tick counter with 1
            else
                TickCounter++;

            // If backtesting and MaxExecution is set let's skip a proportional number of ticks them in order to 
            // reproduce the effect of latency on this EA
            if (IsTesting() && MaxExecution != 0 && _executionTickCount != -1)
            {
                skipticks = MathRound(Avg_tickspermin * MaxExecution / (60 * 1000));
                if (SkippedTicks >= skipticks)
                {
                    _executionTickCount = -1;
                    SkippedTicks = 0;
                }
                else
                {
                    SkippedTicks++;
                }
            }

            // Get Ask and Bid for the currency
            _ask = MarketInfo(Symbol(), MODE_ASK);
            _bid = MarketInfo(Symbol(), MODE_BID);

            // Calculate the channel of Volatility based on the difference of iHigh and iLow during current bar
            ihigh = iHigh(Symbol(), PERIOD_M1, 0);
            ilow = iLow(Symbol(), PERIOD_M1, 0);
            volatility = ihigh - ilow;

            // Reset printout string
            indy = "";

            // Calculate a channel on Moving Averages, and check if the price is outside of this channel. 
            if (_UseIndicatorSwitch == 1 || _UseIndicatorSwitch == 4)
            {
                _iMALow = iMA(Symbol(), PERIOD_M1, Indicatorperiod, 0, MODE_LWMA, PRICE_LOW, 0);
                _iMAHigh = iMA(Symbol(), PERIOD_M1, Indicatorperiod, 0, MODE_LWMA, PRICE_HIGH, 0);
                _iMADiff = _iMAHigh - _iMALow;
                isBidGreaterThanIma = _bid >= _iMALow + _iMADiff / 2.0;
                indy = "iMA_low: " + dbl2StrBrokerDigits(_iMALow) + ", iMA_high: " + dbl2StrBrokerDigits(_iMAHigh) + ", iMA_diff: " + dbl2StrBrokerDigits(_iMADiff);
            }

            // Calculate a channel on BollingerBands, and check if the price is outside of this channel
            if (_UseIndicatorSwitch == 2)
            {
                _ibandsupper = iBands(Symbol(), PERIOD_M1, Indicatorperiod, _BBDeviation, 0, PRICE_OPEN, MODE_UPPER, 0);
                _ibandslower = iBands(Symbol(), PERIOD_M1, Indicatorperiod, _BBDeviation, 0, PRICE_OPEN, MODE_LOWER, 0);
                _ibandsdiff = _ibandsupper - _ibandslower;
                isBidGreaterThaniBand = _bid >= _ibandslower + _ibandsdiff / 2.0;
                indy = "iBands_upper: " + dbl2StrBrokerDigits(_ibandsupper) + ", iBands_lower: " + dbl2StrBrokerDigits(_ibandslower) + ", iBands_diff: " + dbl2StrBrokerDigits(_ibandsdiff);
            }

            // Calculate a channel on Envelopes, and check if the price is outside of this channel
            if (_UseIndicatorSwitch == 3)
            {
                _envelopesupper = iEnvelopes(Symbol(), PERIOD_M1, Indicatorperiod, MODE_LWMA, 0, PRICE_OPEN, EnvelopesDeviation, MODE_UPPER, 0);
                _envelopeslower = iEnvelopes(Symbol(), PERIOD_M1, Indicatorperiod, MODE_LWMA, 0, PRICE_OPEN, EnvelopesDeviation, MODE_LOWER, 0);
                envelopesdiff = _envelopesupper - _envelopeslower;
                isBidGreaterThanEnvelopes = _bid >= _envelopeslower + envelopesdiff / 2.0;
                indy = "iEnvelopes_upper: " + dbl2StrBrokerDigits(_envelopesupper) + ", iEnvelopes_lower: " + dbl2StrBrokerDigits(_envelopeslower) + ", iEnvelopes_diff: " + dbl2StrBrokerDigits(envelopesdiff);
            }

            // Reset breakout variable as false
            isBidGreaterThanIndy = false;

            // Reset pricedirection for no indication of trading direction 
            pricedirection = 0;

            // If we're using iMA as indicator, then check if there's a breakout
            if (_UseIndicatorSwitch == 1 && isBidGreaterThanIma == true)
            {
                isBidGreaterThanIndy = true;
                _highest = _iMAHigh;
                _lowest = _iMALow;
            }

            // If we're using iBands as indicator, then check if there's a breakout
            else if (_UseIndicatorSwitch == 2 && isBidGreaterThaniBand == true)
            {
                isBidGreaterThanIndy = true;
                _highest = _ibandsupper;
                _lowest = _ibandslower;
            }

            // If we're using iEnvelopes as indicator, then check if there's a breakout
            else if (_UseIndicatorSwitch == 3 && isBidGreaterThanEnvelopes == true)
            {
                isBidGreaterThanIndy = true;
                _highest = _envelopesupper;
                _lowest = _envelopeslower;
            }

            // Calculate spread	
            spread = _ask - _bid;

            // Calculate lot size
            _lotSize = calculateLotSize();

            // calculatwe orderexpiretime
            if (OrderExpireSeconds != 0)
                orderExpireTime = TimeCurrent().AddSeconds(OrderExpireSeconds);
            else
                orderExpireTime = DateTime.MinValue;

            // Calculate average true spread, which is the average of the spread for the last 30 tics
            ArrayCopy(Array_spread, Array_spread, 0, 1, 29);
            Array_spread[29] = spread;

            if (UpTo30Counter < 30)
                UpTo30Counter++;

            double sumOfSpreads = 0;
            int spreadIndex = 29;
            for (int index = 0; index < UpTo30Counter; index++)
            {
                sumOfSpreads += Array_spread[spreadIndex];
                spreadIndex--;
            }

            // Calculate an average of spreads based on the spread from the last 30 tics
            avgSpread = sumOfSpreads / UpTo30Counter;

            // Calculate price and spread considering commission
            askpluscommission = normalizebrokerdigits(_ask + Commission);
            bidminuscommission = normalizebrokerdigits(_bid - Commission);
            realAvgSpread = avgSpread + Commission;

            // Recalculate the VolatilityLimit if it's set to dynamic. It's based on the average of spreads + commission
            if (UseDynamicVolatilityLimit == true)
                VolatilityLimit = realAvgSpread * VolatilityMultiplier;

            //	If the variables below have values it means that we have enough of data from broker server. 
            if ((volatility!=0) && (VolatilityLimit!=0) && (_lowest!=0) && (_highest!=0) && (_UseIndicatorSwitch != 4))
            {
                // The Volatility is outside of the VolatilityLimit, so we can now open a trade
                if (volatility > VolatilityLimit)
                {
                    // Calculate how much it differs
                    _volatilityPercentage = volatility / VolatilityLimit;
                    // In case of UseVolatilityPercentage == true then also check if it differ enough of percentage
                    if ((UseVolatilityPercentage == false) || (UseVolatilityPercentage == true && _volatilityPercentage > VolatilityPercentageLimit))
                    {
                        if (_bid < _lowest)
                            if (ReverseTrade == false)
                                pricedirection = -1; // BUY or BUYSTOP
                            else // ReverseTrade == true
                                pricedirection = 1; // SELL or SELLSTOP
                        else if (_bid > _highest)
                            if (ReverseTrade == false)
                                pricedirection = 1;  // SELL or SELLSTOP
                            else // ReverseTrade == true
                                pricedirection = -1; // BUY or BUYSTOP
                    }
                }
                // The Volatility is less than the VolatilityLimit 
                else
                    _volatilityPercentage = 0;
            }

            // Out of money 
            if (AccountEquity() <= 0.0)
            {
                Comment("ERROR -- Account Equity is " + DoubleToStr(MathRound(AccountEquity()), 0));
                return;
            }

            // Reset Execution time	
            _executionTickCount = -1;

            // Reset counters
            counter1 = 0;
            counter2 = 0;

            // Loop through all open orders (if any) to either modify them or delete them
            for (int index = 0; index < OrdersTotal(); index++)
            {
                select = OrderSelect(index, SELECT_BY_POS, MODE_TRADES);

                // We've found an that matches the magic number and is open
                if (OrderMagicNumber() == Magic && OrderCloseTime() == DateTime.MinValue)
                {
                    // If the order doesn't match the currency pair from the chart then check next open order
                    if (OrderSymbol() != Symbol())
                    {
                        // Increase counter
                        counter2++;
                        continue;
                    }
                    // Select order by type of order
                    switch (OrderType())
                    {
                        // We've found a matching BUY-order
                        case OP_BUY:
                            // Start endless loop
                            while (true)
                            {
                                // Update prices from the broker
                                RefreshRates();
                                // Set SL and TP
                                orderstoploss = OrderStopLoss();
                                ordertakeprofit = OrderTakeProfit();
                                //	Ok to modify the order if its TP is less than the price+commission+StopLevel AND price+StopLevel-TP greater than trailingStart			
                                if (ordertakeprofit < normalizebrokerdigits(askpluscommission + TakeProfit * Point + AddPriceGap) && askpluscommission + TakeProfit * Point + AddPriceGap - ordertakeprofit > TrailingStart)
                                {
                                    // Set SL and TP
                                    orderstoploss = normalizebrokerdigits(_bid - StopLoss * Point - AddPriceGap);
                                    ordertakeprofit = normalizebrokerdigits(askpluscommission + TakeProfit * Point + AddPriceGap);
                                    // Send an OrderModify command with adjusted SL and TP
                                    if (orderstoploss != OrderStopLoss() && ordertakeprofit != OrderTakeProfit())
                                    {
                                        // Start Execution timer
                                        _executionTickCount = GetTickCount();
                                        // Try to modify order
                                        wasOrderModified = OrderModify(OrderTicket(), 0, orderstoploss, ordertakeprofit, orderExpireTime, Color.Lime);
                                    }
                                    // Order was modified with new SL and TP
                                    if (wasOrderModified == true)
                                    {
                                        // Calculate Execution speed
                                        _executionTickCount = GetTickCount() - _executionTickCount;
                                        // If we have choosen to take snapshots and we're not backtesting, then do so
                                        if (TakeShots && !IsTesting())
                                            takesnapshot();
                                        // Break out from while-loop since the order now has been modified
                                        break;
                                    }
                                    // Order was not modified
                                    else
                                    {
                                        // Reset Execution counter
                                        _executionTickCount = -1;
                                        // Add to errors
                                        errormessages();
                                        // Print if debug or verbose
                                        if (Debug || Verbose)
                                            Print("Order could not be modified because of ", ErrorDescription(GetLastError()));
                                        // Order has not been modified and it has no StopLoss
                                        if (orderstoploss == 0)
                                            // Try to modify order with a safe hard SL that is 3 pip from current price
                                            wasOrderModified = OrderModify(OrderTicket(), 0, NormalizeDouble(Bid - 30, BrokerDigits), 0, DateTime.MinValue, Color.Red);
                                    }
                                }
                                // Break out from while-loop since the order now has been modified
                                break;
                            }
                            // count 1 more up
                            counter1++;
                            // Break out from switch
                            break;

                        // We've found a matching SELL-order	
                        case OP_SELL:
                            // Start endless loop
                            while (true)
                            {
                                // Update broker prices
                                RefreshRates();
                                // Set SL and TP
                                orderstoploss = OrderStopLoss();
                                ordertakeprofit = OrderTakeProfit();
                                // Ok to modify the order if its TP is greater than price-commission-StopLevel AND TP-price-commission+StopLevel is greater than trailingstart
                                if (ordertakeprofit > normalizebrokerdigits(bidminuscommission - TakeProfit * Point - AddPriceGap) && ordertakeprofit - bidminuscommission + TakeProfit * Point - AddPriceGap > TrailingStart)
                                {
                                    // set SL and TP
                                    orderstoploss = normalizebrokerdigits(_ask + StopLoss * Point + AddPriceGap);
                                    ordertakeprofit = normalizebrokerdigits(bidminuscommission - TakeProfit * Point - AddPriceGap);
                                    // Send an OrderModify command with adjusted SL and TP
                                    if (orderstoploss != OrderStopLoss() && ordertakeprofit != OrderTakeProfit())
                                    {
                                        // Start Execution timer
                                        _executionTickCount = GetTickCount();
                                        wasOrderModified = OrderModify(OrderTicket(), 0, orderstoploss, ordertakeprofit, orderExpireTime, Color.Orange);
                                    }
                                    // Order was modiified with new SL and TP
                                    if (wasOrderModified == true)
                                    {
                                        // Calculate Execution speed
                                        _executionTickCount = GetTickCount() - _executionTickCount;
                                        // If we have choosen to take snapshots and we're not backtesting, then do so							
                                        if (TakeShots && !IsTesting())
                                            takesnapshot();
                                        // Break out from while-loop since the order now has been modified
                                        break;
                                    }
                                    // Order was not modified
                                    else
                                    {
                                        // Reset Execution counter
                                        _executionTickCount = -1;
                                        // Add to errors
                                        errormessages();
                                        // Print if debug or verbose
                                        if (Debug || Verbose)
                                            Print("Order could not be modified because of ", ErrorDescription(GetLastError()));
                                        // Lets wait 1 second before we try to modify the order again
                                        Sleep(1000);
                                        // Order has not been modified and it has no StopLoss
                                        if (orderstoploss == 0)
                                            // Try to modify order with a safe hard SL that is 3 pip from current price
                                            wasOrderModified = OrderModify(OrderTicket(), 0, NormalizeDouble(Ask + 30, BrokerDigits), 0, DateTime.MinValue, Color.Red);
                                    }
                                }
                                // Break out from while-loop since the order now has been modified
                                break;
                            }
                            // count 1 more up
                            counter1++;
                            // Break out from switch
                            break;

                        // We've found a matching BUYSTOP-order					
                        case OP_BUYSTOP:
                            // Price must NOT be larger than indicator in order to modify the order, otherwise the order will be deleted			
                            if (isBidGreaterThanIndy == false)
                            {
                                // Calculate how much Price, SL and TP should be modified
                                orderprice = normalizebrokerdigits(_ask + _stopLevel + AddPriceGap);
                                orderstoploss = normalizebrokerdigits(orderprice - spread - StopLoss * Point - AddPriceGap);
                                ordertakeprofit = normalizebrokerdigits(orderprice + Commission + TakeProfit * Point + AddPriceGap);
                                // Start endless loop
                                while (true)
                                {
                                    // Ok to modify the order if price+StopLevel is less than orderprice AND orderprice-price-StopLevel is greater than trailingstart
                                    if (orderprice < OrderOpenPrice() && OrderOpenPrice() - orderprice > TrailingStart)
                                    {

                                        // Send an OrderModify command with adjusted Price, SL and TP 
                                        if (orderstoploss != OrderStopLoss() && ordertakeprofit != OrderTakeProfit())
                                        {
                                            RefreshRates();
                                            // Start Execution timer
                                            _executionTickCount = GetTickCount();
                                            wasOrderModified = OrderModify(OrderTicket(), orderprice, orderstoploss, ordertakeprofit, DateTime.MinValue, Color.Lime);
                                        }
                                        // Order was modified
                                        if (wasOrderModified == true)
                                        {
                                            // Calculate Execution speed
                                            _executionTickCount = GetTickCount() - _executionTickCount;
                                            // Print if debug or verbose
                                            if (Debug || Verbose)
                                                Print("Order executed in " + _executionTickCount + " ms");
                                        }
                                        // Order was not modified
                                        else
                                        {
                                            // Reset Execution counter
                                            _executionTickCount = -1;
                                            // Add to errors
                                            errormessages();
                                        }
                                    }
                                    // Break out from endless loop
                                    break;
                                }
                                // Increase counter
                                counter1++;
                            }
                            // Price was larger than the indicator
                            else
                                // Delete the order
                                select = OrderDelete(OrderTicket());
                            // Break out from switch
                            break;

                        // We've found a matching SELLSTOP-order				
                        case OP_SELLSTOP:
                            // Price must be larger than the indicator in order to modify the order, otherwise the order will be deleted
                            if (isBidGreaterThanIndy == true)
                            {
                                // Calculate how much Price, SL and TP should be modified
                                orderprice = normalizebrokerdigits(_bid - _stopLevel - AddPriceGap);
                                orderstoploss = normalizebrokerdigits(orderprice + spread + StopLoss * Point + AddPriceGap);
                                ordertakeprofit = normalizebrokerdigits(orderprice - Commission - TakeProfit * Point - AddPriceGap);
                                // Endless loop
                                while (true)
                                {
                                    // Ok to modify order if price-StopLevel is greater than orderprice AND price-StopLevel-orderprice is greater than trailingstart
                                    if (orderprice > OrderOpenPrice() && orderprice - OrderOpenPrice() > TrailingStart)
                                    {
                                        // Send an OrderModify command with adjusted Price, SL and TP
                                        if (orderstoploss != OrderStopLoss() && ordertakeprofit != OrderTakeProfit())
                                        {
                                            RefreshRates();
                                            // Start Execution counter
                                            _executionTickCount = GetTickCount();
                                            wasOrderModified = OrderModify(OrderTicket(), orderprice, orderstoploss, ordertakeprofit, DateTime.MinValue, Color.Orange);
                                        }
                                        // Order was modified							
                                        if (wasOrderModified == true)
                                        {
                                            // Calculate Execution speed
                                            _executionTickCount = GetTickCount() - _executionTickCount;
                                            // Print if debug or verbose
                                            if (Debug || Verbose)
                                                Print("Order executed in " + _executionTickCount + " ms");
                                        }
                                        // Order was not modified
                                        else
                                        {
                                            // Reset Execution counter
                                            _executionTickCount = -1;
                                            // Add to errors
                                            errormessages();
                                        }
                                    }
                                    // Break out from endless loop
                                    break;
                                }
                                // count 1 more up
                                counter1++;
                            }
                            // Price was NOT larger than the indicator, so delete the order
                            else
                                select = OrderDelete(OrderTicket());

                            break;
                    } // end of switch
                }  // end if OrderMagicNumber
            } // end for loopcount2 - end of loop through open orders

            // Calculate and keep track on global error number 
            if (GlobalError >= 0 || GlobalError == -2)
            {
                double bidpart = NormalizeDouble(_bid / Point, 0);
                double askpart = NormalizeDouble(_ask / Point, 0);
                if (bidpart % 10 != 0 || askpart % 10 != 0)
                    GlobalError = -1;
                else
                {
                    if (GlobalError >= 0 && GlobalError < 10)
                        GlobalError++;
                    else
                        GlobalError = -2;
                }
            }

            // Reset error-variable
            ordersenderror = false;

            // Before executing new orders, lets check the average Execution time.
            if (pricedirection != 0 && MaxExecution > 0 && Avg_execution > MaxExecution)
            {
                pricedirection = 0; // Ignore the order opening triger
                if (Debug || Verbose)
                    Print("Server is too Slow. Average Execution: " + Avg_execution);
            }

            // Set default price adjustment
            _askPlusDistance = _ask + _stopLevel;
            _bidMinusDistance = _bid - _stopLevel;

            // If we have no open orders AND a price breakout AND average spread is less or equal to max allowed spread AND we have no errors THEN proceed
            if (counter1 == 0 && pricedirection != 0 && normalizebrokerdigits(realAvgSpread) <= normalizebrokerdigits(MaxSpread * Point) && GlobalError == -1)
            {
                // If we have a price breakout downwards (Bearish) then send a BUYSTOP order
                if (pricedirection == -1 || pricedirection == 2) // Send a BUYSTOP
                {
                    // Calculate a new price to use
                    orderprice = _ask + _stopLevel;
                    // SL and TP is not sent with order, but added afterwords in a OrderModify command
                    if (ECN_Mode == true)
                    {
                        // Set prices for OrderModify of BUYSTOP order
                        orderprice = _askPlusDistance;
                        orderstoploss = 0;
                        ordertakeprofit = 0;
                        // Start Execution counter
                        _executionTickCount = GetTickCount();
                        // Send a BUYSTOP order without SL and TP
                        orderticket = OrderSend(Symbol(), OP_BUYSTOP, _lotSize, orderprice, Slippage, orderstoploss, ordertakeprofit, OrderCmt, Magic, DateTime.MinValue, Color.Lime);
                        // OrderSend was executed successfully
                        if (orderticket > 0)
                        {
                            // Calculate Execution speed
                            _executionTickCount = GetTickCount() - _executionTickCount;
                            if (Debug || Verbose)
                                Print("Order executed in " + _executionTickCount + " ms");
                            // If we have choosen to take snapshots and we're not backtesting, then do so			
                            if (TakeShots && !IsTesting())
                                takesnapshot();
                        }  // end if ordersend
                           // OrderSend was NOT executed
                        else
                        {
                            ordersenderror = true;
                            _executionTickCount = -1;
                            // Add to errors
                            errormessages();
                        }
                        // OrderSend was executed successfully, so now modify it with SL and TP				
                        if (OrderSelect(orderticket, SELECT_BY_TICKET))
                        {
                            RefreshRates();
                            // Set prices for OrderModify of BUYSTOP order
                            orderprice = OrderOpenPrice();
                            orderstoploss = normalizebrokerdigits(orderprice - spread - StopLoss * Point - AddPriceGap);
                            ordertakeprofit = normalizebrokerdigits(orderprice + TakeProfit * Point + AddPriceGap);
                            // Start Execution timer
                            _executionTickCount = GetTickCount();
                            // Send a modify order for BUYSTOP order with new SL and TP
                            wasOrderModified = OrderModify(OrderTicket(), orderprice, orderstoploss, ordertakeprofit, orderExpireTime, Color.Lime);
                            // OrderModify was executed successfully
                            if (wasOrderModified == true)
                            {
                                // Calculate Execution speed
                                _executionTickCount = GetTickCount() - _executionTickCount;
                                if (Debug || Verbose)
                                    Print("Order executed in " + _executionTickCount + " ms");
                                // If we have choosen to take snapshots and we're not backtesting, then do so			
                                if (TakeShots && !IsTesting())
                                    takesnapshot();
                            } // end successful ordermodiify
                              // Order was NOT modified
                            else
                            {
                                ordersenderror = true;
                                _executionTickCount = -1;
                                // Add to errors
                                errormessages();
                            } // end if-else					
                        }  // end if ordermodify					
                    } // end if ECN_Mode

                    // No ECN-mode, SL and TP can be sent directly
                    else
                    {
                        RefreshRates();
                        // Set prices for BUYSTOP order
                        orderprice = _askPlusDistance;//ask+StopLevel
                        orderstoploss = normalizebrokerdigits(orderprice - spread - StopLoss * Point - AddPriceGap);
                        ordertakeprofit = normalizebrokerdigits(orderprice + TakeProfit * Point + AddPriceGap);
                        // Start Execution counter
                        _executionTickCount = GetTickCount();
                        // Send a BUYSTOP order with SL and TP 
                        orderticket = OrderSend(Symbol(), OP_BUYSTOP, _lotSize, orderprice, Slippage, orderstoploss, ordertakeprofit, OrderCmt, Magic, orderExpireTime, Color.Lime);
                        if (orderticket > 0) // OrderSend was executed suxxessfully
                        {
                            // Calculate Execution speed
                            _executionTickCount = GetTickCount() - _executionTickCount;
                            if (Debug || Verbose)
                                Print("Order executed in " + _executionTickCount + " ms");
                            // If we have choosen to take snapshots and we're not backtesting, then do so			
                            if (TakeShots && !IsTesting())
                                takesnapshot();
                        } // end successful ordersend
                          // Order was NOT sent
                        else
                        {
                            ordersenderror = true;
                            // Reset Execution timer
                            _executionTickCount = -1;
                            // Add to errors
                            errormessages();
                        } // end if-else
                    } // end no ECN-mode
                } // end if pricedirection == -1 or 2

                // If we have a price breakout upwards (Bullish) then send a SELLSTOP order
                if (pricedirection == 1 || pricedirection == 2)
                {
                    // Set prices for SELLSTOP order with zero SL and TP
                    orderprice = _bidMinusDistance;
                    orderstoploss = 0;
                    ordertakeprofit = 0;
                    // SL and TP cannot be sent with order, but must be sent afterwords in a modify command
                    if (ECN_Mode)
                    {
                        // Start Execution timer
                        _executionTickCount = GetTickCount();
                        // Send a SELLSTOP order without SL and TP 
                        orderticket = OrderSend(Symbol(), OP_SELLSTOP, _lotSize, orderprice, Slippage, orderstoploss, ordertakeprofit, OrderCmt, Magic, DateTime.MinValue, Color.Orange);
                        // OrderSend was executed successfully
                        if (orderticket > 0)
                        {
                            // Calculate Execution speed
                            _executionTickCount = GetTickCount() - _executionTickCount;
                            if (Debug || Verbose)
                                Print("Order executed in " + _executionTickCount + " ms");
                            // If we have choosen to take snapshots and we're not backtesting, then do so			
                            if (TakeShots && !IsTesting())
                                takesnapshot();
                        }  // end if ordersend
                           // OrderSend was NOT executed
                        else
                        {
                            ordersenderror = true;
                            _executionTickCount = -1;
                            // Add to errors
                            errormessages();
                        }
                        // If the SELLSTOP order was executed successfully, then select that order
                        if (OrderSelect(orderticket, SELECT_BY_TICKET))
                        {
                            RefreshRates();
                            // Set prices for SELLSTOP order with modified SL and TP
                            orderprice = OrderOpenPrice();
                            orderstoploss = normalizebrokerdigits(orderprice + spread + StopLoss * Point + AddPriceGap);
                            ordertakeprofit = normalizebrokerdigits(orderprice - TakeProfit * Point - AddPriceGap);
                            // Start Execution timer
                            _executionTickCount = GetTickCount();
                            // Send a modify order with adjusted SL and TP
                            wasOrderModified = OrderModify(OrderTicket(), OrderOpenPrice(), orderstoploss, ordertakeprofit, orderExpireTime, Color.Orange);
                        }
                        // OrderModify was executed successfully
                        if (wasOrderModified == true)
                        {
                            // Calculate Execution speed
                            _executionTickCount = GetTickCount() - _executionTickCount;
                            // Print debug info
                            if (Debug || Verbose)
                                Print("Order executed in " + _executionTickCount + " ms");
                            // If we have choosen to take snapshots and we're not backtesting, then do so	
                            if (TakeShots && !IsTesting())
                                takesnapshot();
                        } // end if ordermodify was executed successfully
                          // Order was NOT executed
                        else
                        {
                            ordersenderror = true;
                            // Reset Execution timer
                            _executionTickCount = -1;
                            // Add to errors
                            errormessages();
                        }
                    }
                    else // No ECN-mode, SL and TP can be sent directly
                    {
                        RefreshRates();
                        // Set prices for SELLSTOP order	with SL and TP		
                        orderprice = _bidMinusDistance;
                        orderstoploss = normalizebrokerdigits(orderprice + spread + StopLoss * Point + AddPriceGap);
                        ordertakeprofit = normalizebrokerdigits(orderprice - TakeProfit * Point - AddPriceGap);
                        // Start Execution timer
                        _executionTickCount = GetTickCount();
                        // Send a SELLSTOP order with SL and TP
                        orderticket = OrderSend(Symbol(), OP_SELLSTOP, _lotSize, orderprice, Slippage, orderstoploss, ordertakeprofit, OrderCmt, Magic, orderExpireTime, Color.Orange);
                        // If OrderSend was executed successfully
                        if (orderticket > 0)
                        {
                            // Calculate exection speed for that order
                            _executionTickCount = GetTickCount() - _executionTickCount;
                            // Print debug info
                            if (Debug || Verbose)
                                Print("Order executed in " + _executionTickCount + " ms");
                            if (TakeShots && !IsTesting())
                                takesnapshot();
                        } // end successful ordersend
                          // OrderSend was NOT executed successfully
                        else
                        {
                            ordersenderror = true;
                            // Nullify Execution timer
                            _executionTickCount = 0;
                            // Add to errors
                            errormessages();
                        } // end if-else
                    } // end no ECN-mode
                } // end pricedirection == 0 or 2			
            } // end if execute new orders


            // If we have no samples, every MaxExecutionMinutes a new OrderModify Execution test is done
            if (MaxExecution!=0 && _executionTickCount == -1 && (TimeLocal()-_startTime).TotalMinutes % MaxExecutionMinutes == 0)
            {
                // When backtesting, simulate random Execution time based on the setting
                if (IsTesting())
                {
                    if (MaxExecution != 0)
                    {
                        MathSrand(TimeLocal().Second);
                        _executionTickCount = (int)(MathRand() / (32767 / (double)MaxExecution));
                    }
                }
                else
                {
                    // Unless backtesting, lets send a fake order to check the OrderModify Execution time, 
                    // To be sure that the fake order never is executed, st the price to twice the current price
                    fakeprice = _ask * 2.0;
                    // Send a BUYSTOP order
                    orderticket = OrderSend(Symbol(), OP_BUYSTOP, _lotSize, fakeprice, Slippage, 0, 0, OrderCmt, Magic, DateTime.MinValue, Color.Lime);
                    _executionTickCount = GetTickCount();
                    // Send a modify command where we adjust the price with +1 pip
                    wasOrderModified = OrderModify(orderticket, fakeprice + 10 * Point, 0, 0, DateTime.MinValue, Color.Lime);
                    // Calculate Execution speed
                    _executionTickCount = GetTickCount() - _executionTickCount;
                    // Delete the order
                    select = OrderDelete(orderticket);
                }
            }

            // Do we have a valid Execution sample? Update the average Execution time.
            if (_executionTickCount >= 0)
            {
                // Consider only 10 samples at most.
                if (Execution_samples < 10)
                    Execution_samples++;
                // Calculate average Execution speed
                Avg_execution = Avg_execution + (_executionTickCount - Avg_execution) / Execution_samples;
            }

            // Check initialization 
            if (GlobalError >= 0)
                Comment("Robot is initializing...");
            else
            {
                // Error
                if (GlobalError == -2)
                    Comment("ERROR -- Instrument " + Symbol() + " prices should have " + BrokerDigits + " fraction digits on broker account");
                // No errors, ready to print 
                else
                {
                    textstring = TimeToStr(TimeCurrent()) + " Tick: " + adjust00instring(TickCounter);
                    // Only show / print this if Debug OR Verbose are set to true
                    if (Debug || Verbose)
                    {
                        textstring = textstring + "\n*** DEBUG MODE *** \nCurrency pair: " + Symbol() + ", Volatility: " + dbl2StrBrokerDigits(volatility)
                        + ", VolatilityLimit: " + dbl2StrBrokerDigits(VolatilityLimit) + ", VolatilityPercentage: " + dbl2StrBrokerDigits(_volatilityPercentage);
                        textstring = textstring + "\nPriceDirection: " + StringSubstr("BUY NULLSELLBOTH", 4 * pricedirection + 4, 4) + ", Expire: "
                        + TimeToStr(orderExpireTime, TIME_MINUTES) + ", Open orders: " + counter1;
                        textstring = textstring + "\nBid: " + dbl2StrBrokerDigits(_bid) + ", Ask: " + dbl2StrBrokerDigits(_ask) + ", " + indy;
                        textstring = textstring + "\nAvgSpread: " + dbl2StrBrokerDigits(avgSpread) + ", RealAvgSpread: " + dbl2StrBrokerDigits(realAvgSpread)
                        + ", Commission: " + dbl2StrBrokerDigits(Commission) + ", Lots: " + DoubleToStr(_lotSize, 2) + ", Execution: " + _executionTickCount + " ms";
                        if (normalizebrokerdigits(realAvgSpread) > normalizebrokerdigits(MaxSpread * Point))
                        {
                            textstring = textstring + "\n" + "The current spread (" + dbl2StrBrokerDigits(realAvgSpread)
                            + ") is higher than what has been set as MaxSpread (" + dbl2StrBrokerDigits(MaxSpread * Point) + ") so no trading is allowed right now on this currency pair!";
                        }
                        if (MaxExecution > 0 && Avg_execution > MaxExecution)
                        {
                            textstring = textstring + "\n" + "The current Avg Execution (" + Avg_execution + ") is higher than what has been set as MaxExecution ("
                            + MaxExecution + " ms), so no trading is allowed right now on this currency pair!";
                        }
                        Comment(textstring);
                        // Only print this if we have a any orders  OR have a price breakout OR Verbode mode is set to true
                        if (counter1 != 0 || pricedirection != 0)
                            printformattedstring(textstring);
                    }
                } // end if-else
            } // end check initialization

            // Check for stray market orders without SL
            check4StrayTrades();

        } 

        /// <summary>
        /// 
        /// </summary>
        void check4StrayTrades()
        {
            int loop;
            int totals;
            bool modified = true;
            bool selected;
            double ordersl;
            double newsl;

            // New SL to use for modifying stray market orders is max of either current SL or 10 points
            newsl = MathMax(StopLoss, 10);
            // Get number of open orders
            totals = OrdersTotal();

            // Loop through all open orders from first to last
            for (loop = 0; loop < totals; loop++)
            {
                // Select on order
                if (OrderSelect(loop, SELECT_BY_POS, MODE_TRADES))
                {
                    // Check if it matches the MagicNumber and chart symbol
                    if (OrderMagicNumber() == Magic && OrderSymbol() == Symbol())    // If the orders are for this EA
                    {
                        ordersl = OrderStopLoss();
                        // Continue as long as the SL for the order is 0.0 
                        while (ordersl == 0.0)
                        {
                            if (OrderType() == OP_BUY)
                            {
                                // Set new SL 10 points away from current price
                                newsl = Bid - newsl * Point;
                                modified = OrderModify(OrderTicket(), OrderOpenPrice(), NormalizeDouble(newsl, Digits), OrderTakeProfit(), DateTime.MinValue, Color.Blue);
                            }
                            else if (OrderType() == OP_SELL)
                            {
                                // Set new SL 10 points away from current price
                                newsl = Ask + newsl * Point;
                                modified = OrderModify(OrderTicket(), OrderOpenPrice(), NormalizeDouble(newsl, Digits), OrderTakeProfit(), DateTime.MinValue, Color.Blue);
                            } // If the order without previous SL was modified wit a new SL
                            if (modified == true)
                            {
                                // Select that modified order, set while condition variable to that true value and exit while-loop
                                selected = OrderSelect(1/*modified*/, SELECT_BY_TICKET, MODE_TRADES);
                                ordersl = OrderStopLoss();
                                break;
                            }
                            // If the order could not be modified
                            else // if ( modified == false )
                            {
                                // Wait 1/10 second and then fetch new prices
                                Sleep(100);
                                RefreshRates();
                                // Print debug info
                                if (Debug || Verbose)
                                    Print("Error trying to modify stray order with a SL!");
                                // Add to errors
                                errormessages();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Convert a decimal number to a text string
        /// </summary>
        /// <param name="par_a"></param>
        /// <returns></returns>
        string dbl2StrBrokerDigits(double par_a)
        {
            return (DoubleToStr(par_a, BrokerDigits));
        }

        /// <summary>
        /// Adjust numbers with as many decimals as the broker uses
        /// </summary>
        /// <param name="par_a"></param>
        /// <returns></returns>
        double normalizebrokerdigits(double par_a)
        {
            return (NormalizeDouble(par_a, BrokerDigits));
        }

        /// <summary>
        /// Adjust textstring with zeros at the end
        /// </summary>
        /// <param name="par_a"></param>
        /// <returns></returns>
        string adjust00instring(int par_a)
        {
            if (par_a < 10)
                return ("00" + par_a);
            if (par_a < 100)
                return ("0" + par_a);
            return ("" + par_a);
        }

        /// <summary>
        /// Print out formatted textstring 
        /// </summary>
        /// <param name="par_a"></param>
        void printformattedstring(string par_a)
        {
            int difference;
            int a = -1;

            while (a < StringLen(par_a))
            {
                difference = a + 1;
                a = StringFind(par_a, "\n", difference);
                if (a == -1)
                {
                    Print(StringSubstr(par_a, difference));
                    return;
                }
                Print(StringSubstr(par_a, difference, a - difference));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        double multiplicator()
        {
            // Calculate lot multiplicator for Account Currency. Assumes that account currency is any of the 8 majors.
            // If the account currency is of any other currency, then calculate the multiplicator as follows:
            // If base-currency is USD then use the BID-price for the currency pair USDXXX; or if the 
            // counter currency is USD the use 1 / BID-price for the currency pair XXXUSD, 
            // where XXX is the abbreviation for the account currency. The calculated lot-size should 
            // then be multiplied with this multiplicator.
            double multiplicator = 1.0;
            int length;
            string appendix = "";

            if (AccountCurrency() == "USD")
                return (multiplicator);
            length = StringLen(Symbol());
            if (length != 6)
                appendix = StringSubstr(Symbol(), 6, length - 6);
            if (AccountCurrency() == "EUR")
                multiplicator = 1.0 / MarketInfo("EURUSD" + appendix, MODE_BID);
            if (AccountCurrency() == "GBP")
                multiplicator = 1.0 / MarketInfo("GBPUSD" + appendix, MODE_BID);
            if (AccountCurrency() == "AUD")
                multiplicator = 1.0 / MarketInfo("AUDUSD" + appendix, MODE_BID);
            if (AccountCurrency() == "NZD")
                multiplicator = 1.0 / MarketInfo("NZDUSD" + appendix, MODE_BID);
            if (AccountCurrency() == "CHF")
                multiplicator = MarketInfo("USDCHF" + appendix, MODE_BID);
            if (AccountCurrency() == "JPY")
                multiplicator = MarketInfo("USDJPY" + appendix, MODE_BID);
            if (AccountCurrency() == "CAD")
                multiplicator = MarketInfo("USDCAD" + appendix, MODE_BID);
            if (multiplicator == 0)
                multiplicator = 1.0; // If account currency is neither of EUR, GBP, AUD, NZD, CHF, JPY or CAD we assumes that it is USD
            return (multiplicator);
        }

        /// <summary>
        /// Magic Number - calculated from a sum of account number + ASCII-codes from currency pair
        /// </summary>
        /// <returns></returns>
        int magicnumber()
        {
            string a;
            string b;
            int c;
            int d;
            int i;
            string par = "EURUSDJPYCHFCADAUDNZDGBP";
            string sym = Symbol();

            a = StringSubstr(sym, 0, 3);
            b = StringSubstr(sym, 3, 3);
            c = StringFind(par, a, 0);
            d = StringFind(par, b, 0);
            i = 999999999 - AccountNumber() - c - d;
            if (Debug == true)
                Print("MagicNumber: ", i);
            return (i);
        }

        /// <summary>
        /// Main routine for making a screenshoot / printscreen
        /// </summary>
        void takesnapshot()
        {
            double shotinterval;
            double timerShot;

            if (ShotsPerBar > 0)
                shotinterval = MathRound((double)(60 * Period()) / ShotsPerBar);
            else
                shotinterval = 60 * Period();

            timerShot = MathFloor((TimeCurrent() - Time[0]).Ticks / shotinterval);

            if (Time[0] != _lastOpenTimeBar)
            {
                _lastOpenTimeBar = Time[0];
                _doShot = DelayTicks;
            }
            else if (timerShot > _oldTimePhase)
                makescreenshot("i");

            _oldTimePhase = timerShot;

            if (_doShot == 0)
                makescreenshot("");

            if (_doShot >= 0)
                _doShot -= 1;
        }

        /// <summary>
        /// add leading zeros that the resulting string has 'digits' length.
        /// </summary>
        /// <param name="par_number"></param>
        /// <param name="par_digits"></param>
        /// <returns></returns>
        string maketimestring(int par_number, int par_digits)
        {
            string result;

            result = DoubleToStr(par_number, 0);
            while (StringLen(result) < par_digits)
                result = "0" + result;

            return (result);
        }

        /// <summary>
        /// Make a screenshoot / printscreen
        /// </summary>
        /// <param name="par_sx"></param>
        void makescreenshot(string par_sx = "")
        {

            indexOfScreenShot++;
            string fn = "SnapShot" + Symbol() + Period() + "\\" + Year() + "-" + maketimestring(Month(), 2) + "-" + maketimestring(Day(), 2)
            + " " + maketimestring(Hour(), 2) + "_" + maketimestring(Minute(), 2) + "_" + maketimestring(Seconds(), 2) + " " + indexOfScreenShot + par_sx + ".gif";

            if (!WindowScreenShot(fn, 640, 480))
                Print("ScreenShot error: ", ErrorDescription(GetLastError()));
        }

        /// <summary>
        /// Calculate LotSize based on Equity, Risk (in %) and StopLoss in points
        /// </summary>
        /// <returns></returns>
        double calculateLotSize()
        {
            string textstring;
            double availablemoney;
            double lotsize;
            double maxlot;
            double minlot;

            int lotdigit=0;

            if (LotStep == 1)
                lotdigit = 0;
            if (LotStep == 0.1)
                lotdigit = 1;
            if (LotStep == 0.01)
                lotdigit = 2;

            // Get available money as Equity
            availablemoney = AccountEquity();

            // Maximum allowed Lot by the broker according to Equity. And we don't use 100% but 98%
            maxlot = MathMin(MathFloor(availablemoney * 0.98 / MarginForOneLot / LotStep) * LotStep, MaxLots);

            // Minimum allowed Lot by the broker
            minlot = MinLots;

            // Lot according to Risk. Don't use 100% but 98% (= 102) to avoid 
            lotsize = MathMin(MathFloor(Risk / 102 * availablemoney / (StopLoss + AddPriceGap) / LotStep) * LotStep, MaxLots);
            lotsize = lotsize * multiplicator();
            lotsize = NormalizeDouble(lotsize, lotdigit);

            // Empty textstring
            textstring = "";

            // Use manual fix LotSize, but if necessary adjust to within limits
            if (MoneyManagement == false)
            {
                // Set LotSize to manual LotSize
                lotsize = ManualLotsize;
                // Check if ManualLotsize is greater than allowed LotSize
                if (ManualLotsize > maxlot)
                {
                    lotsize = maxlot;
                    textstring = "Note: Manual LotSize is too high. It has been recalculated to maximum allowed " + DoubleToStr(maxlot, 2);
                    Print(textstring);
                    Comment(textstring);
                    ManualLotsize = maxlot;
                }
                else if (ManualLotsize < minlot)
                    lotsize = minlot;
            }
            return (lotsize);
        }

        /// <summary>
        /// Re-calculate a new Risk if the current one is too low or too high
        /// </summary>
        void recalculatewrongrisk()
        {
            string textstring;
            double availablemoney;
            double maxlot;
            double minlot;
            double maxrisk;
            double minrisk;

            // Get available amount of money as Equity
            availablemoney = AccountEquity();
            // Maximum allowed Lot by the broker according to Equity
            maxlot = MathFloor(availablemoney / MarginForOneLot / LotStep) * LotStep;
            // Maximum allowed Risk by the broker according to maximul allowed Lot and Equity
            maxrisk = MathFloor(maxlot * (_stopLevel + StopLoss) / availablemoney * 100 / 0.1) * 0.1;
            // Minimum allowed Lot by the broker
            minlot = MinLots;
            // Minimum allowed Risk by the broker according to minlots_broker
            minrisk = MathRound(minlot * StopLoss / availablemoney * 100 / 0.1) * 0.1;
            // Empty textstring
            textstring = "";

            if (MoneyManagement == true)
            {
                // If Risk% is greater than the maximum risklevel the broker accept, then adjust Risk accordingly and print out changes
                if (Risk > maxrisk)
                {
                    textstring = textstring + "Note: Risk has manually been set to " + DoubleToStr(Risk, 1) + " but cannot be higher than " + DoubleToStr(maxrisk, 1) + " according to ";
                    textstring = textstring + "the broker, StopLoss and Equity. It has now been adjusted accordingly to " + DoubleToStr(maxrisk, 1) + "%";
                    Risk = maxrisk;
                    printandcomment(textstring);
                }
                // If Risk% is less than the minimum risklevel the broker accept, then adjust Risk accordingly and print out changes
                if (Risk < minrisk)
                {
                    textstring = textstring + "Note: Risk has manually been set to " + DoubleToStr(Risk, 1) + " but cannot be lower than " + DoubleToStr(minrisk, 1) + " according to ";
                    textstring = textstring + "the broker, StopLoss, AddPriceGap and Equity. It has now been adjusted accordingly to " + DoubleToStr(minrisk, 1) + "%";
                    Risk = minrisk;
                    printandcomment(textstring);
                }
            }
            // Don't use MoneyManagement, use fixed manual LotSize
            else // MoneyManagement == false
            {
                // Check and if necessary adjust manual LotSize to external limits
                if (ManualLotsize < MinLots)
                {
                    textstring = "Manual LotSize " + DoubleToStr(ManualLotsize, 2) + " cannot be less than " + DoubleToStr(MinLots, 2) + ". It has now been adjusted to " + DoubleToStr(MinLots, 2);
                    ManualLotsize = MinLots;
                    printandcomment(textstring);
                }
                if (ManualLotsize > MaxLots)
                {
                    textstring = "Manual LotSize " + DoubleToStr(ManualLotsize, 2) + " cannot be greater than " + DoubleToStr(MaxLots, 2) + ". It has now been adjusted to " + DoubleToStr(MinLots, 2);
                    ManualLotsize = MaxLots;
                    printandcomment(textstring);
                }
                // Check to see that manual LotSize does not exceeds maximum allowed LotSize	
                if (ManualLotsize > maxlot)
                {
                    textstring = "Manual LotSize " + DoubleToStr(ManualLotsize, 2) + " cannot be greater than maximum allowed LotSize. It has now been adjusted to " + DoubleToStr(maxlot, 2);
                    ManualLotsize = maxlot;
                    printandcomment(textstring);
                }
            }
        }

        /// <summary>
        /// Print out broker details and other info
        /// </summary>
        void printdetails()
        {
            string marginText = "";
            string stopoutText = "";
            string fixedLots = "";
            int type;
            double freeMarginMode;
            int stopoutmode;
            double newsl;

            newsl = MathMax(StopLoss, 10);
            type = (IsDemo() ? 1:0) + (IsTesting() ? 1: 0);
            freeMarginMode = AccountFreeMarginMode();
            stopoutmode = AccountStopoutMode();

            if (freeMarginMode == 0)
                marginText = "that floating profit/loss is not used for calculation.";
            else if (freeMarginMode == 1)
                marginText = "both floating profit and loss on open positions.";
            else if (freeMarginMode == 2)
                marginText = "only profitable values, where current loss on open positions are not included.";
            else if (freeMarginMode == 3)
                marginText = "only loss values are used for calculation, where current profitable open positions are not included.";

            if (stopoutmode == 0)
                stopoutText = "percentage ratio between margin and equity.";
            else if (stopoutmode == 1)
                stopoutText = "comparison of the free margin level to the absolute value.";

            if (MoneyManagement == true)
                fixedLots = " (automatically calculated lots).";
            if (MoneyManagement == false)
                fixedLots = " (fixed manual lots).";

            Print("Broker name: ", AccountCompany());
            Print("Broker server: ", AccountServer());
            Print("Account type: ", StringSubstr("RealDemoTest", 4 * type, 4));
            Print("Initial account equity: ", AccountEquity(), " ", AccountCurrency());
            Print("Broker digits: ", BrokerDigits);
            Print("Broker StopLevel / freezelevel (max): ", _stopLevel);
            Print("Broker StopOut level: ", StopOut, "%");
            Print("Broker Point: ", DoubleToStr(Point, BrokerDigits), " on ", AccountCurrency());
            Print("Broker account Leverage in percentage: ", Leverage);
            Print("Broker credit value on the account: ", AccountCredit());
            Print("Broker account margin: ", AccountMargin());
            Print("Broker calculation of free margin allowed to open positions considers " + marginText);
            Print("Broker calculates StopOut level as " + stopoutText);
            Print("Broker requires at least ", MarginForOneLot, " ", AccountCurrency(), " in margin for 1 lot.");
            Print("Broker set 1 lot to trade ", _lotBase, " ", AccountCurrency());
            Print("Broker minimum allowed LotSize: ", MinLots);
            Print("Broker maximum allowed LotSize: ", MaxLots);
            Print("Broker allow lots to be resized in ", LotStep, " steps.");
            Print("Risk: ", Risk, "%");
            Print("Risk adjusted LotSize: ", DoubleToStr(_lotSize, 2) + fixedLots);
        }

        /// <summary>
        /// Print and show comment of text
        /// </summary>
        /// <param name="par_text"></param>
        void printandcomment(string par_text)
        {
            Print(par_text);
            Comment(par_text);
        }

        /// <summary>
        /// Summarize error messages that comes from the broker server
        /// </summary>
        void errormessages()
        {
            int error = GetLastError();

            switch (error)
            {
                // Unchanged values
                case 1: // ERR_SERVER_BUSY:
                    {
                        Err_unchangedvalues++;
                        break;
                    }
                // Trade server is busy
                case 4: // ERR_SERVER_BUSY:
                    {
                        Err_busyserver++;
                        break;
                    }
                case 6: // ERR_NO_CONNECTION:
                    {
                        Err_lostconnection++;
                        break;
                    }
                case 8: // ERR_TOO_FREQUENT_REQUESTS:
                    {
                        Err_toomanyrequest++;
                        break;
                    }
                case 129: // ERR_INVALID_PRICE:
                    {
                        Err_invalidprice++;
                        break;
                    }
                case 130: // ERR_INVALID_STOPS:
                    {
                        Err_invalidstops++;
                        break;
                    }
                case 131: // ERR_INVALID_TRADE_VOLUME:
                    {
                        Err_invalidtradevolume++;
                        break;
                    }
                case 135: // ERR_PRICE_CHANGED:
                    {
                        Err_pricechange++;
                        break;
                    }
                case 137: // ERR_BROKER_BUSY:
                    {
                        Err_brokerbuzy++;
                        break;
                    }
                case 138: // ERR_REQUOTE:
                    {
                        Err_requotes++;
                        break;
                    }
                case 141: // ERR_TOO_MANY_REQUESTS:
                    {
                        Err_toomanyrequests++;
                        break;
                    }
                case 145: // ERR_TRADE_MODIFY_DENIED:
                    {
                        Err_trademodifydenied++;
                        break;
                    }
                case 146: // ERR_TRADE_CONTEXT_BUSY:
                    {
                        Err_tradecontextbuzy++;
                        break;
                    }
            }
        }

        /// <summary>
        /// Print out and comment summarized messages from the broker
        /// </summary>
        void printsumofbrokererrors()
        {
            string txt;
            int totalerrors;

            txt = "Number of times the brokers server reported that ";

            totalerrors = Err_unchangedvalues + Err_busyserver + Err_lostconnection + Err_toomanyrequest + Err_invalidprice
           + Err_invalidstops + Err_invalidtradevolume + Err_pricechange + Err_brokerbuzy + Err_requotes + Err_toomanyrequests
            + Err_trademodifydenied + Err_tradecontextbuzy;

            if (Err_unchangedvalues > 0)
                printandcomment(txt + "SL and TP was modified to existing values: " + DoubleToStr(Err_unchangedvalues, 0));
            if (Err_busyserver > 0)
                printandcomment(txt + "it is buzy: " + DoubleToStr(Err_busyserver, 0));
            if (Err_lostconnection > 0)
                printandcomment(txt + "the connection is lost: " + DoubleToStr(Err_lostconnection, 0));
            if (Err_toomanyrequest > 0)
                printandcomment(txt + "there was too many requests: " + DoubleToStr(Err_toomanyrequest, 0));
            if (Err_invalidprice > 0)
                printandcomment(txt + "the price was invalid: " + DoubleToStr(Err_invalidprice, 0));
            if (Err_invalidstops > 0)
                printandcomment(txt + "invalid SL and/or TP: " + DoubleToStr(Err_invalidstops, 0));
            if (Err_invalidtradevolume > 0)
                printandcomment(txt + "invalid lot size: " + DoubleToStr(Err_invalidtradevolume, 0));
            if (Err_pricechange > 0)
                printandcomment(txt + "the price has changed: " + DoubleToStr(Err_pricechange, 0));
            if (Err_brokerbuzy > 0)
                printandcomment(txt + "the broker is buzy: " + DoubleToStr(Err_brokerbuzy, 0));
            if (Err_requotes > 0)
                printandcomment(txt + "requotes " + DoubleToStr(Err_requotes, 0));
            if (Err_toomanyrequests > 0)
                printandcomment(txt + "too many requests " + DoubleToStr(Err_toomanyrequests, 0));
            if (Err_trademodifydenied > 0)
                printandcomment(txt + "modifying orders is denied " + DoubleToStr(Err_trademodifydenied, 0));
            if (Err_tradecontextbuzy > 0)
                printandcomment(txt + "trade context is buzy: " + DoubleToStr(Err_tradecontextbuzy, 0));
            if (totalerrors == 0)
                printandcomment("There was no error reported from the broker server!");
        }

        /// <summary>
        /// 
        /// </summary>
        void checkThroughAllOpenOrders()
        {
            int pos;
            double tmp_order_lots;
            double tmp_order_price;

            // Get total number of open orders
            Tot_Orders = OrdersTotal();

            // Reset counters 
            Tot_open_pos = 0;
            Tot_open_profit = 0;
            Tot_open_lots = 0;
            Tot_open_swap = 0;
            Tot_open_commission = 0;
            G_equity = 0;
            Changedmargin = 0;

            // Loop through all open orders from first to last
            for (pos = 0; pos < Tot_Orders; pos++)
            {
                // Select on order
                if (OrderSelect(pos, SELECT_BY_POS, MODE_TRADES))
                {

                    // Check if it matches the MagicNumber
                    if (OrderMagicNumber() == Magic && OrderSymbol() == Symbol())    // If the orders are for this EA
                    {
                        // Calculate sum of open orders, open profit, swap and commission
                        Tot_open_pos++;
                        tmp_order_lots = OrderLots();
                        Tot_open_lots += tmp_order_lots;
                        tmp_order_price = OrderOpenPrice();
                        Tot_open_profit += OrderProfit();
                        Tot_open_swap += OrderSwap();
                        Tot_open_commission += OrderCommission();
                        Changedmargin += tmp_order_lots * tmp_order_price;
                    }
                }
            }
            // Calculate Balance and Equity for this EA and not for the entire account
            G_equity = G_balance + Tot_open_profit + Tot_open_swap + Tot_open_commission;

        }

        /// <summary>
        /// 
        /// </summary>
        void checkThroughAllClosedOrders()
        {
            int pos;
            int openTotal = OrdersHistoryTotal();

            // Reset counters
            Tot_closed_pos = 0;
            Tot_closed_lots = 0;
            Tot_closed_profit = 0;
            Tot_closed_swap = 0;
            Tot_closed_comm = 0;
            G_balance = 0;

            // Loop through all closed orders
            for (pos = 0; pos < openTotal; pos++)
            {
                // Select one order
                if (OrderSelect(pos, SELECT_BY_POS, MODE_HISTORY))    // Loop through the history pool of closed and deleted orders 
                {
                    // If the MagicNumber matches
                    if (OrderMagicNumber() == Magic && OrderSymbol() == Symbol())    // If the orders are for this EA
                    {
                        // Fetch order info
                        Tot_closed_lots += OrderLots();
                        Tot_closed_profit += OrderProfit();
                        Tot_closed_swap += OrderSwap();
                        Tot_closed_comm += OrderCommission();
                        // Count number of closed total orders for this EA
                        Tot_closed_pos++;
                    }
                }
            }
            G_balance = Tot_closed_profit + Tot_closed_swap + Tot_closed_comm;
        }

        /// <summary>
        /// 
        /// </summary>
        void showGraphInfo()
        {
            string line1;
            string line2;
            string line3;
            string line4;
            string line5;
            //	string line6;
            string line7;
            //	string line8;
            string line9;
            string line10;
            int textspacing = 10;
            int linespace;

            // Prepare for Display	
            line1 = EA_version;
            line2 = "Open: " + DoubleToStr(Tot_open_pos, 0) + " positions, " + DoubleToStr(Tot_open_lots, 2) + " lots with value: " + DoubleToStr(Tot_open_profit, 2);
            line3 = "Closed: " + DoubleToStr(Tot_closed_pos, 0) + " positions, " + DoubleToStr(Tot_closed_lots, 2) + " lots with value: " + DoubleToStr(Tot_closed_profit, 2);
            line4 = "EA Balance: " + DoubleToStr(G_balance, 2) + ", Swap: " + DoubleToStr(Tot_open_swap, 2) + ", Commission: " + DoubleToStr(Tot_open_commission, 2);
            line5 = "EA Equity: " + DoubleToStr(G_equity, 2) + ", Swap: " + DoubleToStr(Tot_closed_swap, 2) + ", Commission: " + DoubleToStr(Tot_closed_comm, 2);
            // line6	
            line7 = "                               ";
            //	line8 = "";
            line9 = "Free margin: " + DoubleToStr(MarginFree, 2) + ", Min allowed Margin level: " + DoubleToStr(MinMarginLevel, 2) + "%";
            line10 = "Margin value: " + DoubleToStr(Changedmargin, 2);

            // Display graphic information on the chart
            linespace = textspacing;
            display("line1", line1, Heading_Size, 3, linespace, Color_Heading, 0);
            linespace = textspacing * 2 + Text_Size * 1 + 3 * 1;
            //	linespace = textspacing * 2 + Text_Size * 2 + 3 * 2;	// Next line should look like this
            display("line2", line2, Text_Size, 3, linespace, Color_Section1, 0);
            linespace = textspacing * 2 + Text_Size * 2 + 3 * 2 + 20;
            display("line3", line3, Text_Size, 3, linespace, Color_Section2, 0);
            linespace = textspacing * 2 + Text_Size * 3 + 3 * 3 + 40;
            display("line4", line4, Text_Size, 3, linespace, Color_Section3, 0);
            linespace = textspacing * 2 + Text_Size * 4 + 3 * 4 + 40;
            display("line5", line5, Text_Size, 3, linespace, Color_Section3, 0);
            //	linespace = textspacing * 2 + Text_Size * 5 + 3 * 5 + 60;		
            //	Display ( "line6", line6, Text_Size, 3, linespace, Color_Section4, 0 ); 
            linespace = textspacing * 2 + Text_Size * 5 + 3 * 5 + 40;
            display("line7", line7, Text_Size, 3, linespace, Color_Section4, 0);
            //	linespace = textspacing * 2 + Text_Size * 7 + 3 * 7 + 60;		
            //	Display ( "line8", line8, Text_Size, 3, linespace, Color_Section4, 0 );  
            linespace = textspacing * 2 + Text_Size * 6 + 3 * 6 + 40;
            display("line9", line9, Text_Size, 3, linespace, Color_Section4, 0);
            linespace = textspacing * 2 + Text_Size * 7 + 3 * 7 + 40;
            display("line10", line10, Text_Size, 3, linespace, Color_Section4, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj_name"></param>
        /// <param name="object_text"></param>
        /// <param name="object_text_fontsize"></param>
        /// <param name="object_x_distance"></param>
        /// <param name="object_y_distance"></param>
        /// <param name="object_textcolor"></param>
        /// <param name="object_corner_value"></param>
        void display(string obj_name, string object_text, int object_text_fontsize, int object_x_distance, int object_y_distance, Color object_textcolor, int object_corner_value)
        {
            ObjectCreate(obj_name, OBJ_LABEL, 0, DateTime.MinValue, 0, DateTime.MinValue, 0);
            ObjectSet(obj_name, OBJPROP_CORNER, object_corner_value);
            ObjectSet(obj_name, OBJPROP_XDISTANCE, object_x_distance);
            ObjectSet(obj_name, OBJPROP_YDISTANCE, object_y_distance);
            ObjectSetText(obj_name, object_text, object_text_fontsize, "Tahoma", object_textcolor);
        }
       
        /// <summary>
        /// 
        /// </summary>
        void deleteDisplay()
        {
            ObjectsDeleteAll();
        }
    }


}
