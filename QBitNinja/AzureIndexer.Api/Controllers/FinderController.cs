namespace AzureIndexer.Api.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using NBitcoin;

    [Route("api/v1/finder")]
    public class FinderController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly WhatIsIt finder;
        private readonly TimeSpan expiration = TimeSpan.FromHours(24.0);

        public FinderController(ChainIndexer chain, QBitNinjaConfiguration config, IMapper mapper, WhatIsIt finder)
        {
            this.mapper = mapper;
            this.finder = finder;
            this.Configuration = config;
            this.Chain = chain;
        }

        public ChainIndexer Chain { get; set; }

        public QBitNinjaConfiguration Configuration { get; set; }

        public Network Network => this.Configuration.Indexer.Network;

        [HttpGet]
        [Route("{identifier}")]
        public async Task<object> WhatIsIt(string identifier)
        {
            var result = await this.finder.Find(identifier, this.mapper);
            return result ?? "{\"message\": \"Good question Holmes!\"}";
        }
    }
}
