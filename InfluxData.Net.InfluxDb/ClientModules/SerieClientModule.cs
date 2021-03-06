﻿using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class SerieClientModule : ClientModuleBase, ISerieClientModule
    {
        private readonly ISerieQueryBuilder _serieQueryBuilder;
        private readonly ISerieResponseParser _serieResponseParser;

        public SerieClientModule(IInfluxDbRequestClient requestClient, ISerieQueryBuilder serieQueryBuilder, ISerieResponseParser serieResponseParser)
            : base(requestClient)
        {
            _serieQueryBuilder = serieQueryBuilder;
            _serieResponseParser = serieResponseParser;
        }

        public virtual async Task<IEnumerable<SerieSet>> GetSeriesAsync(string dbName, string measurementName = null, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetSeries(dbName, measurementName, filters);
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query).ConfigureAwait(false);
            var serieSets = _serieResponseParser.GetSerieSets(series);

            return serieSets;
        }

        public virtual async Task<IInfluxDataApiResponse> DropSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            return await DropSeriesAsync(dbName, new List<string>() { measurementName }, filters).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> DropSeriesAsync(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.DropSeries(dbName, measurementNames, filters);
            var response = await base.GetAndValidateQueryAsync(dbName, query).ConfigureAwait(false);

            return response;
        }

        public virtual async Task<IEnumerable<Measurement>> GetMeasurementsAsync(string dbName, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetMeasurements(dbName, filters);
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query).ConfigureAwait(false);
            var measurements = _serieResponseParser.GetMeasurements(series);

            return measurements;
        }

        public virtual async Task<IInfluxDataApiResponse> DropMeasurementAsync(string dbName, string measurementName)
        {
            var query = _serieQueryBuilder.DropMeasurement(dbName, measurementName);
            var response = await base.GetAndValidateQueryAsync(dbName, query).ConfigureAwait(false);

            return response;
        }
    }
}
