using Xunit; 
using FaucetSite.Lib;

namespace stratfaucet
{

    public class ThrottlingTest
    {

        IWalletUtils walletUtils;
        Recipient recipient;
        public ThrottlingTest () {
            Throttling.Init();
            walletUtils = new WalletUtils();
            recipient = new Recipient {

            };
        }


        [Fact]
        public void WillNotSendWhenRequestingIPHasBeenUsed()
        {
           Throttling.IPAddressesSeen.Enqueue("ipaddress");
           recipient.ip_address = "ipaddress";
           Assert.False(walletUtils.newRecipient(recipient));
        }

        [Fact]
        public void WillNotSendWhenReceivingAddressHasBeenUsed(){
           Throttling.AddressesSeen.Enqueue("address");
           recipient.address = "address";
           Assert.False(walletUtils.newRecipient(recipient));
        }

    }

}
