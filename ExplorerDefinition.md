# Stratis Azure Indexer

## Architecture
The Stratis Azure Indexer is part of the Block Explorer Architecture which roughly consists of the following components:

 - Stratis Azure Indexer : Consists of one or more nodes maintaining a set of indexes in the Azure cloud.
 - Block Explorer Back-End: Reads the indexes from the Azure cloud, transforms the data and presents it to the front-end.
 - Block Explorer Front-End: The clients that connect to the back-end and render the block explorer information on the UI.

The first step in designing the Stratis Azure Indexer is to identify the back-end features that would be required to support the UI. Based on the anticipated back-end indexing requirements we can then proceed to define the indexes.

## Block explorer back-end features to be supported by the indexes

This section lists the anticipated back-end requirements and suggests the indexes to use for each requirement.
### Search by Address / View Address Input/Output Transactions
   - Use the TransactionsByScript table.
### Search by TxHash / View Transaction Information
   - Use the TransactionsByHash container.
### Search by Block / View Block Information
   - Use the BlocksByHeight container.
### Find/view contract
   - Use the ContractsByAddress table and the TransactionByHash container

## Indexes required

This section lists the indexes that are anticipated to be required by the block explorer back-end.

### BlocksByHeight Container
Stores block header and transaction hashes indexes by block height:  

- Block Height (PK)
- Block Header 
- Transaction Hashes

### TransactionsByHash Container

Stores the transaction data indexed by transaction hash:
- Transaction Hash (PK)
- Transaction Number 
- Transaction Data
- Block Height

### TransactionsByScript Table

Used to determine which transactions funded or spent a script:
- RowKey = Script Hash : Block Height (zero padded) : Transaction Hash
- PartitionKey = First two hex characters of script hash
- Transaction Number
- BalanceAfter

### SpendingTransactionByUTXO Table

Determines if an output has been spent and identifies the input:
- RowKey = Output Transaction Hash : Output Number
- PartitionKey = First two hex characters of Output Transaction Hash
- Input Transaction Hash
- Input Number

### ContractsByAddress

Used to get a list of all smart contracts.
- RowKey = Address
- PartitionKey = First three characters of Address
- Creation Transaction Hash

## Indexer Design

### Design Approach
- The indexing function will be performed by one or more nodes.  The Azure Service Bus (queues and/or topics) will be used to co-ordinate work among collaborating nodes.
- A special locator blob, held in the Azure store, will track the progress of the indexing.
- If a node looks at the locator blob and determines that the index is behind the current tip, and remaining works is below a certain level, it will take exclusive access of the locator blob, advance the locator by a small amount (roughly corresponding to say 1 to 5  minutes of work), set up work queues items with the corresponding work to be done and then release the locator.
- Nodes will take work items from the queue and update corresponding indexes. Once the work is done work items will then be removed from the queue.
- Different networks will be supported, although each node will be allocated to only one network. Tables will be separated by means of a network-specific-prefix used on table names. Different Azure accounts will be used for test, regression test and production.
- Block headers and transactions will be stored in separate containers. Blocks will be indexed by height and transactions by transaction hash. Storage format will be JSON.
- If nodes can't find the index tip in their chain (by using the special locator) then they will check the de-indexing criteria and perform de-indexing as required. Otherwise they
  will not participate in processing work items and instead focus on catching up with the chain.

### De-Indexing

Nodes will use the following criteria to determine if de-indexing is required:
- Scannign the locator blocks it finds a locator block that is not in the chain.
- The work done up to that block is less than the work done up to the current chain tip.

If the above holds true then the node will first gain exclusive access to the locator blob.
It will then scan all indexed blocks from the last common block to determine which block is not in the chain. 
All blocks starting with such a block will be de-indexed but starting at the most recent blocks. 
The necessary work items will be added to the queue and the special locator blob updated and released accordingly.
