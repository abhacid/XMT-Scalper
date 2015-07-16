
#property copyright "(c) 2011 - 2014 Capella"
#property link "http://www.worldwide-invest.org"

#import "nquotes/nquoteslib.ex4"
	int nquotes_setup(string className, string assemblyName);
	int nquotes_init();
	int nquotes_start();
	int nquotes_deinit();

	int nquotes_set_property_bool(string name, bool value);
	int nquotes_set_property_int(string name, int value);
	int nquotes_set_property_double(string name, double value);
	int nquotes_set_property_datetime(string name, datetime value);
	int nquotes_set_property_color(string name, color value);
	int nquotes_set_property_string(string name, string value);
	int nquotes_set_property_adouble(string name, double& value[], int count=WHOLE_ARRAY, int start=0);

	bool nquotes_get_property_bool(string name);
	int nquotes_get_property_int(string name);
	double nquotes_get_property_double(string name);
	datetime nquotes_get_property_datetime(string name);
	color nquotes_get_property_color(string name);
	string nquotes_get_property_string(string name);
	int nquotes_get_property_array_size(string name);
	int nquotes_get_property_adouble(string name, double& value[]);
#import

	extern string Configuration             = "==== Configuration ====";
    extern bool ReverseTrade                = false;    // If true, then trade in opposite direction
    extern int Magic                        = -1;       // If set to a number less than 0 it will calculate MagicNumber automatically
    extern string OrderCmt                  = "XMT-Scalper 2.46"; // Trade comments that appears in the Trade and Account History tab
    extern bool ECN_Mode                    = false;    // True for brokers that don't accept SL and TP to be sent at the same time as the order
    extern bool Debug                       = false;    // Print huge log files with info, only for debugging purposes
    extern bool Verbose                     = false;    // Additional information printed in the chart

    extern string TradingSettings           = "==== Trade settings ====";
    extern double MaxSpread                 = 30.0;     // Max allowed spread in points (1 / 10 pip)
    extern int MaxExecution                 = 0;        // Max allowed average execution time in ms (0 means no restrictions)
    extern int MaxExecutionMinutes          = 5;        // How often in minutes should fake orders be sent to measure execution speed
    extern double StopLoss                  = 60;       // StopLoss from as many points. Default 60 (= 6 pips)
    extern double TakeProfit                = 100;      // TakeProfit from as many points. Default 100 (= 10 pip)
    extern double AddPriceGap               = 0;        // Additional price gap in points added to SL and TP in order to avoid Error 130
    extern double TrailingStart             = 20;       // Start trailing profit from as so many points. 
    extern double Commission                = 0;        // Some broker accounts charge commission in USD per 1.0 lot. Commission in dollar
    extern int Slippage                     = 3;        // Maximum allowed Slippage in points
    extern double MinimumUseStopLevel       = 0;        // Minimum stop level. Stoplevel to use will be max value of either this value or broker stoplevel 

    extern string VolatilitySettings        = "==== Volatility Settings ====";
    extern bool UseDynamicVolatilityLimit   = true;     // Calculate VolatilityLimit based on INT (spread * VolatilityMultiplier)
    extern double VolatilityMultiplier      = 125;      // Dynamic value, only used if UseDynamicVolatilityLimit is set to true
    extern double VolatilityLimit           = 180;      // Fix value, only used if UseDynamicVolatilityLimit is set to false
    extern bool UseVolatilityPercentage     = true;     // If true, then price must break out more than a specific percentage
    extern double VolatilityPercentageLimit = 0;        // Percentage of how much iHigh-iLow difference must differ from VolatilityLimit. 0 is risky, 60 means a safe value

    extern string UseIndicatorSet           = "=== Indicators: 1 = Moving Average, 2 = BollingerBand, 3 = Envelopes";
    extern int _UseIndicatorSwitch          = 1;        // Switch User indicators. 
    extern int Indicatorperiod              = 3;        // Period in bars for indicators
    extern double _BBDeviation              = 2.0;      // Deviation for the iBands indicator
    extern double EnvelopesDeviation        = 0.07;     // Deviation for the iEnvelopes indicator
    extern int OrderExpireSeconds           = 3600;     // Orders are deleted after so many seconds

    extern string Money_Management          = "==== Money Management ====";
    extern bool MoneyManagement             = true;     // If true then calculate lotsize automaticallay based on Risk, if False then use ManualLotsize below
    extern double MinLots                   = 0.01;     // Minimum lot-size to trade with
    extern double MaxLots                   = 100.0;    // Maximum allowed lot-size to trade with
    extern double Risk                      = 2.0;      // Risk setting in percentage, For 10.000 in Equity 10% Risk and 60 StopLoss lotsize = 16.66
    extern double ManualLotsize             = 0.1;      // Manual lotsize to trade with if MoneyManagement above is set to false
    extern double MinMarginLevel            = 100;      // Lowest allowed Margin level for new positions to be opened. 

    extern string Screen_Shooter            = "==== Screen Shooter ====";
    extern bool TakeShots                   = false;    // Save screen shots on STOP orders?
    extern int DelayTicks                   = 1;        // Delay so many ticks after new bar
    extern int ShotsPerBar                  = 1;        // How many screen shots per bar

    extern string DisplayGraphics           = "=== Display Graphics ==="; // Colors for Display at upper left
    extern int Heading_Size                 = 13;       // Font size for headline
    extern int Text_Size                    = 12;       // Font size for texts
    extern color Color_Heading              = Black;   // Color for text lines
    extern color Color_Section1             = DarkSlateGray;            // -"-
    extern color Color_Section2             = DimGray;              // -"-
    extern color Color_Section3             = MidnightBlue;            // -"-
    extern color Color_Section4             = SeaGreen;         // -"-



int init()
{
	nquotes_setup("Robots.XMTScalper", "Robots.XMTScalper");

	nquotes_set_property_bool("ReverseTrade", ReverseTrade);
	nquotes_set_property_int("Magic", Magic);
	nquotes_set_property_string("OrderCmt", OrderCmt);
	nquotes_set_property_bool("ECN_Mode", ECN_Mode);
	nquotes_set_property_bool("Debug", Debug);
	nquotes_set_property_bool("Verbose", Verbose);

	nquotes_set_property_string("TradingSettings", TradingSettings);
	nquotes_set_property_double("MaxSpread", MaxSpread);
	nquotes_set_property_int("MaxExecution", MaxExecution);
	nquotes_set_property_int("MaxExecutionMinutes", MaxExecutionMinutes);
	nquotes_set_property_double("StopLoss", StopLoss);
	nquotes_set_property_double("TakeProfit", TakeProfit);
	nquotes_set_property_double("AddPriceGap", AddPriceGap);
	nquotes_set_property_double("TrailingStart", TrailingStart);
	nquotes_set_property_double("Commission", Commission);
	nquotes_set_property_int("Slippage", Slippage);
	nquotes_set_property_double("MinimumUseStopLevel", MinimumUseStopLevel);

	nquotes_set_property_string("VolatilitySettings",VolatilitySettings);
	nquotes_set_property_bool("UseDynamicVolatilityLimit", UseDynamicVolatilityLimit);
	nquotes_set_property_double("VolatilityMultiplier", VolatilityMultiplier);
	nquotes_set_property_double("VolatilityLimit", VolatilityLimit);
	nquotes_set_property_bool("UseVolatilityPercentage", UseVolatilityPercentage);
	nquotes_set_property_double("VolatilityPercentageLimit", VolatilityPercentageLimit);

	nquotes_set_property_string("UseIndicatorSet", UseIndicatorSet);
	nquotes_set_property_int("_UseIndicatorSwitch", _UseIndicatorSwitch);
	nquotes_set_property_int("Indicatorperiod", Indicatorperiod);
	nquotes_set_property_double("_BBDeviation", _BBDeviation);
	nquotes_set_property_double("EnvelopesDeviation", EnvelopesDeviation);
	nquotes_set_property_int("OrderExpireSeconds", OrderExpireSeconds);

	nquotes_set_property_string("Money_Management", Money_Management);
	nquotes_set_property_bool("MoneyManagement", MoneyManagement);
	nquotes_set_property_double("MinLots", MinLots);
	nquotes_set_property_double("MaxLots", MaxLots);
	nquotes_set_property_double("Risk", Risk);
	nquotes_set_property_double("ManualLotsize", ManualLotsize);
	nquotes_set_property_double("MinMarginLevel", MinMarginLevel);

	nquotes_set_property_string("Screen_Shooter", Screen_Shooter);
	nquotes_set_property_bool("TakeShots", TakeShots);
	nquotes_set_property_int("DelayTicks", DelayTicks);
	nquotes_set_property_int("ShotsPerBar", ShotsPerBar);

	nquotes_set_property_string("DisplayGraphics", DisplayGraphics);
	nquotes_set_property_int("Heading_Size", Heading_Size);
	nquotes_set_property_int("Text_Size", Text_Size);
	nquotes_set_property_color("Color_Heading", Color_Heading);
	nquotes_set_property_color("Color_Section1", Color_Section1); 
	nquotes_set_property_color("Color_Section2", Color_Section2);
	nquotes_set_property_color("Color_Section3", Color_Section3);
	nquotes_set_property_color("Color_Section4", Color_Section4); 

	return (nquotes_init());
}

int start()
{
	return (nquotes_start());
}

int deinit()
{
	return (nquotes_deinit());
}
