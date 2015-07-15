
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

    extern public string Configuration = "==== Configuration ====";
    extern public bool ReverseTrade = false;    // If true, then trade in opposite direction
    extern public int Magic = -1;				// If set to a number less than 0 it will calculate MagicNumber automatically

int init()
{
	nquotes_setup("Robots.XMTScalper", "Robots.XMTScalper");
	nquotes_set_property_bool("ReverseTrade", ReverseTrade);
	nquotes_set_property_int("Magic", Magic);

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
