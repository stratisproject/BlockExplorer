using System;
using FaucetSite.Lib;

namespace FaucetSite.Jobs {

  public class SendCoinJob {

      public async static void Execute(WalletUtils walletUtils ) {
        //   Console.WriteLine("SendCoinJob.Execute");
          foreach(Recipient recp in  Throttling.Transactions.Values)
          {
            // Console.WriteLine("Sending Transaction {0}  ",  recp.address);
            if(!recp.is_sent && !recp.is_error)
            {
               await walletUtils.SendCoin(recp);
            } 
          }
        
        Throttling.Manage();
      }
  }

}