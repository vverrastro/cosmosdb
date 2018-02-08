using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Cosmosdb.Web.Configuration;
using Cosmosdb.Web.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cosmosdb.Web
{
    public static class DocumentDBRepository<T> where T : class
    {
        private static string DatabaseId { get; set; }
        private static string CollectionId { get; set; }
        private static DocumentClient client;

        public static void InitializeDB( DatabaseOptions options ) {
            DatabaseId = options.Database;
            CollectionId = options.Collection;
            client = new DocumentClient( new Uri( options.Endpoint ), options.AuthKey );
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private static async Task CreateDatabaseIfNotExistsAsync() {
            try {
                await client.ReadDatabaseAsync( UriFactory.CreateDatabaseUri( DatabaseId ) );
            }
            catch ( DocumentClientException e ) {
                if ( e.StatusCode == System.Net.HttpStatusCode.NotFound ) {
                    await client.CreateDatabaseAsync( new Database { Id = DatabaseId } );
                }
                else {
                    throw;
                }
            }
        }

        private static async Task CreateCollectionIfNotExistsAsync() {
            try {
                await client.ReadDocumentCollectionAsync( UriFactory.CreateDocumentCollectionUri( DatabaseId, CollectionId ) );
            }
            catch ( DocumentClientException e ) {
                if ( e.StatusCode == System.Net.HttpStatusCode.NotFound ) {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri( DatabaseId ),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 } );
                }
                else {
                    throw;
                }
            }
        }

        public static async Task<IEnumerable<T>> GetItemsAsync( Expression<Func<T, bool>> predicate ) {
            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId))
                .Where(predicate)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>());
            }
            return results;
        }

        public static async Task<Document> CreateItemAsync( T item ) {
            return await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), item);
        }

        public static async Task<Document> UpdateItemAsync( string id, T item ) {
            return await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id), item);
        }

        public static async Task<T> GetItemAsync( string id ) {
            try
            {
                Document document = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id));
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

    }
}
