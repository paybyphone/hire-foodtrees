using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using CsvHelper;
using Newtonsoft.Json;


namespace Internal.DataPopulator
{
    class Program
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        static void Main(string[] args)
        {
            var trees = ReadTreesFromCSV("foodtrees.csv");
            try
            {
                UploadFoodTreesToDynamo(trees);
            }
            catch (AmazonDynamoDBException e) { Console.WriteLine(e.Message); }
            catch (AmazonServiceException e) { Console.WriteLine(e.Message); }
            catch (Exception e) { Console.WriteLine(e.Message); }

            Console.WriteLine("To continue, press Enter");
            Console.ReadLine();
        }

        private static IEnumerable<IDictionary<string, object>> ReadTreesFromCSV(string fileName)
        {
            var csv = new CsvReader(new StreamReader(fileName));
            var records = csv.GetRecords(typeof(object)).ToList();
            var dict = records.Select(r =>
            {
                var idict = ((ExpandoObject) r);
                IDictionary<string, object> d = idict;
                return d;
            }).ToList();
            return dict;
        }

        private static void UploadFoodTreesToDynamo(IEnumerable<IDictionary<string, object>> trees)
        {
            Table treesCatalog = Table.LoadTable(client, "hire-foodtrees");
            var batchWrite = treesCatalog.CreateBatchWrite();

            foreach (var treeDict in trees)
            {
                var json = JsonConvert.SerializeObject(treeDict);
                var tree = Document.FromJson(json);
                batchWrite.AddDocumentToPut(tree);
            }

            Console.WriteLine("Performing batch write in UploadFoodTreesToDynamo()");
            batchWrite.Execute();
        }
    }
}
