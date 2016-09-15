using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace Example.DataAccess
{
    public class TreeRepository : ITreeRepository
    {
        private readonly AmazonDynamoDBClient _dynamoDbClient;

        public TreeRepository(AmazonDynamoDBClient dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public IEnumerable<TreeDTO> GetTreesByFoodNames(string foodName)
        {
            string tableName = "hire-foodtrees";
            var table = Table.LoadTable(_dynamoDbClient, tableName);

            ScanFilter scanFilter = new ScanFilter();
            scanFilter.AddCondition("FOOD_TREE_VARIETIES", ScanOperator.Contains, foodName);

            Search search = table.Scan(scanFilter);
            List<Document> documentList = new List<Document>();
            var result = new List<TreeDTO>();
            do
            {
                documentList = search.GetNextSet();
                foreach (var document in documentList)
                {
                    var json = document.ToJson();
                    var tree = JsonConvert.DeserializeObject<TreeDTO>(json);
                    result.Add(tree);
                }
            } while (!search.IsDone);
            return result;
        }
    }

    public interface ITreeRepository
    {
        IEnumerable<TreeDTO> GetTreesByFoodNames(string foodName);
    }

    public class TreeDTO
    {
        public string MAPID { get; set; }
        public string YEAR_CREATED { get; set; }
        public string NAME { get; set; }
        public string MERGED_ADDRESS { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string NUMBER_OF_PLOTS { get; set; }
        public string NUMBER_OF_FOOD_TREES { get; set; }
        public string NOTES { get; set; }
        public string FOOD_TREE_VARIETIES { get; set; }
        public string JURISDICTION { get; set; }
    }
}