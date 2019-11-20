export interface AppConfig {
   /// Current configured coin, an entry with same value must exists in 'coins' property in order to fetch configuration parameters
   currentCoin: string;
   coins: { [network: string]: CoinConfiguration };
}

export interface CoinConfiguration {
   /// Name of the network (e.g. "Stratis Main").
   networkName: string;

   /// Network Coin Symbol (e.g. STRAT)
   symbol: string;

   /// Explorer url
   url: string;

   /// Name of the theme that will be applied for this network (corresponding themes styles has to be deployed in assets/themes/ folder)
   themeName: string;

   /// Base URL of the API endpoint used to fetch data.
   apiBaseUrl: string;
}
