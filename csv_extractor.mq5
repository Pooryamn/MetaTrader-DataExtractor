//+------------------------------------------------------------------+
//|  mytest.mq5                                                      |
//|  Copyright 2020, Poorya Mohammadi Nasab                          |
//|  https://github.com/Pooryamn                                     |
//+------------------------------------------------------------------+
#property copyright "Copyright 2020, Poorya Mohammadi"
#property link      "https://github.com/Pooryamn"
#property version   "1.00"

#property script_show_inputs

#include <Poorya\CheckHistory.mqh>
#include <Poorya\String.mqh>
#include <Files\FileTxt.mqh>
#include <Files\FileBin.mqh>

input string					Main_Properties		= "";		// Main properties:
input string					SymbolsList				= "all";	// * List of symbols (or "all" MarketWatch symbols)
input string					TimeFramesList			= "M1";	// * List of TFs (or "all" MT4 TFs)
input int						BarsToDownload			= 2;		// * Bars to download (0 - all visible in MT)
input int                  timePeriod           = 2;


#define OFFLINE_HEADER_SIZE 148 // LONG_VALUE + 64 + 12 + 4 * LONG_VALUE + 13 * LONG_VALUE
#define OFFLINE_RECORD_SIZE 44  // 5 * DOUBLE_VALUE + LONG_VALUE

int OnInit()
  {
//--- create timer
   EventSetTimer(timePeriod);
   
//---
   return(INIT_SUCCEEDED);
  }
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
//--- destroy timer
   EventKillTimer();
   
  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
void OnTick()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| Timer function                                                   |
//+------------------------------------------------------------------+
void OnTimer()
  {
//---
   uint start = GetTickCount();

//--- Get symbols from list
	string strSymbols = SymbolsList;
	if ( strSymbols == "all" || strSymbols == "" )
	{
		strSymbols = "";
		for ( int s = SymbolsTotal( true )-1; s >= 0; s -- )
		{
			StringAdd( strSymbols, SymbolName( s, true ) );
			if ( s != 0 ) StringAdd( strSymbols, "," );
		}
	}

	string	SymbolsName[];
	int		SymbolsCount = StringToArray( strSymbols, SymbolsName );
	if ( SymbolsCount <= 0 )
	{
		Alert( "Invalid SymbolsList (\"", SymbolsList, "\")!" );
		return;
	}

//--- Get TFs from list
	string strTimeFrames = TimeFramesList;
	if ( strTimeFrames == "all" || strTimeFrames == "" ) strTimeFrames = "M1,M5,M15,M30,H1,H4,D1";

	string	PeriodsName[];
	int		PeriodsCount = StringToArray( strTimeFrames, PeriodsName );
	if ( PeriodsCount <= 0 )
	{
		Alert( "Invalid TimeFramesList (\"", TimeFramesList, "\")!" );
		return;
	}

//--- Get bars count
	int BarsCount = BarsToDownload, MaxBars = TerminalInfoInteger( TERMINAL_MAXBARS );
	if ( BarsCount <= 0 || BarsCount > MaxBars ) BarsCount = MaxBars;

//--- 
	int files_count = 0;
	for ( int s = 0; s < SymbolsCount; s ++ )
	{
		for ( int p = 0; p < PeriodsCount; p ++ )
		{
		   Comment( "Downloading history and writing files: ", DoubleToString( (PeriodsCount*s+p)/double(SymbolsCount*PeriodsCount)*100.0, 1 ), "% complete..." );
			CheckLoadHistory( SymbolsName[s], StringToPeriod( PeriodsName[p] ), BarsCount );
			if ( WriteHistoryToFile( SymbolsName[s], StringToPeriod( PeriodsName[p] ), BarsCount ) ) files_count ++;
		}
	}

//---
	Alert( "History export finished within ", DoubleToString( (GetTickCount() - start)/1000.0, 1 ), " sec! ", IntegerToString( files_count ), " files have been written to:\n", 
				TerminalInfoString( TERMINAL_DATA_PATH ), "\\MQL5\\Files\\History (" + AccountInfoString( ACCOUNT_SERVER ) + ")\\" );
	Comment( "" );
	
  }
//+------------------------------------------------------------------+

bool WriteHistoryToFile( string symbol, ENUM_TIMEFRAMES period, int bars_count )
{
	uint start = GetTickCount();

	CFileTxt FileTxt;

	MqlRates rates_array[];
	ArraySetAsSeries( rates_array, true );

	int copy_count = CopyRates( symbol, period, 0, bars_count, rates_array );
	if ( copy_count < 0 ) return(false);

	int digits =(int)SymbolInfoInteger( symbol, SYMBOL_DIGITS );
	int period_s = PeriodSeconds( period ) / 60;

	// Open a file
	FileTxt.Open( "History (" + AccountInfoString( ACCOUNT_SERVER ) + ")\\" + symbol + PeriodToString ( period   ) + ".txt",FILE_WRITE|FILE_UNICODE);
	//FileBin.Open( "History (" + AccountInfoString( ACCOUNT_SERVER ) + ")\\" + symbol + IntegerToString( period_s ) + ".hst", FILE_ANSI|FILE_WRITE );

//---
   int		version		= 400;
   string	c_copyright	= "Copyright 2012-2013, komposter";
   int		i_unused		[13];

//---
	for ( int i = copy_count-1; i > 0; i -- )
	{
		string str_write = "";
		/*StringConcatenate( str_write
									,      TimeToString( rates_array[i].time, TIME_MINUTES )
									, ",", IntegerToString(SymbolInfoInteger(symbol,SYMBOL_VOLUME))
									, ",", DoubleToString(SymbolInfoDouble(symbol,SYMBOL_BID))
									, ",", DoubleToString(SymbolInfoDouble(symbol,SYMBOL_ASK))
									, ",", DoubleToString(SymbolInfoDouble(symbol,SYMBOL_PRICE_CHANGE))
									, ",", DoubleToString( rates_array[i].open	, digits )
									, ",", DoubleToString( rates_array[i].high	, digits )
									, ",", DoubleToString( rates_array[i].low		, digits )
									, ",", DoubleToString( rates_array[i].close	, digits )
									, ",", DoubleToString( rates_array[i].tick_volume, 0 )
									, "\n",symbol
									 );*/
		str_write += TimeToString( rates_array[i].time, TIME_DATE );
	   str_write += ",";							 
	   str_write += TimeToString( rates_array[i].time, TIME_MINUTES );
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_BID)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_ASK)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_LAST)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_LASTHIGH)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_LASTLOW)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_PRICE_CHANGE)) ;
	   str_write += ",";
	   str_write += IntegerToString(SymbolInfoInteger(symbol,SYMBOL_SESSION_DEALS)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_SESSION_VOLUME)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_SESSION_OPEN)) ;
	   str_write += ",";
	   str_write += DoubleToString(SymbolInfoDouble(symbol,SYMBOL_SESSION_CLOSE)) ;
	   str_write += ",";
	   str_write += symbol ;
	   str_write += "";
	   
		FileTxt.WriteString( str_write );

//---
	}

	Print( symbol, ", ", EnumToString( period ), ": ", IntegerToString( copy_count ), " bars have been written to \"", FileTxt.FileName(), " within ", DoubleToString( (GetTickCount() - start)/1000.0, 1 ), " sec!" );
	FileTxt.Close();

	return(true);
}
