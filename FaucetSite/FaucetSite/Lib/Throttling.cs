using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace FaucetSite.Lib
{
    public class Throttling
    {
        public static ConcurrentDictionary<string, Recipient> Transactions;
        public static ConcurrentQueue<string> AddressesSeen;
        public static ConcurrentQueue<string> IPAddressesSeen;
        private static string ERROR_SEND = "An error occurred when sending your coins";
        private static string ERROR_TIMEOUT = "Sending coins timed out";

        public static void Init()
        {
            Transactions = new ConcurrentDictionary<string, Recipient>();
            AddressesSeen = new ConcurrentQueue<string>();
            IPAddressesSeen = new ConcurrentQueue<string>();
        }

        public static Recipient WaitForTransaction(string address)
        {
            int timeout = 30;
            int waitCount = 1;
            while (waitCount < timeout)
            {
                Recipient recipient;
                if (Transactions.TryGetValue(address, out recipient))
                {

                    if (recipient.is_error)
                    {
                        Transactions.Remove(recipient.address, out Recipient rec);
                        throw new FaucetException(ERROR_SEND);
                    }
                    else if (recipient.is_sent)
                    {
                        return recipient;
                    }
                    else
                    {
                        Console.WriteLine("Waiting for transaction");
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    throw new FaucetException(ERROR_SEND);
                }
                waitCount++;
            }

            throw new FaucetException(ERROR_TIMEOUT);
        }

        public static void Manage()
        {

            foreach (var transaction in Transactions.Values)
            {
                if (transaction.is_error || transaction.is_sent)
                {
                    Transactions.TryRemove(transaction.address, out Recipient rec);
                }
            }

            if (AddressesSeen.Count > 100)
            {
                for (int i = 0; i < (AddressesSeen.Count - 100); i++)
                {
                    AddressesSeen.TryDequeue(out string addr);
                }
            }

            if (IPAddressesSeen.Count > 100)
            {
         
                for (int i = 0; i < (IPAddressesSeen.Count - 100); i++)
                {
                    IPAddressesSeen.TryDequeue(out string addr);
                }
            }
        }

    }

}

