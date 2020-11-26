using System.Collections.Generic;
using ClickQuest.Items;
using ClickQuest.Enemies;
using ClickQuest.Places;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClickQuest.Account
{
    public static partial class Database
    {
        public static List<Material> Materials {get; set;}
        public static List<Monster> Monsters { get; set; }
        public static List<Region> Regions{ get; set; }

        public static void Load()
        {
            Materials = new List<Material>();
            Monsters = new List<Monster>();
            Regions = new List<Region>();

            LoadMaterials();
            LoadMonsters();
            LoadRegions();
        }

        public static void LoadMaterials()
        {
         //   Materials= JsonConvert.DeserializeObject<List<Material>>(Path.Combine(Environment.CurrentDirectory, @"Data\", "Materials.json"));
            
            var parsedObject = JObject.Parse(Path.Combine(Environment.CurrentDirectory, @"Data\", "Materials.json"));
            var jArray = (JArray) parsedObject["materials"];

            for(var i=0; i<jArray.Count; i++)
            {
                var id = int.Parse(parsedObject["materials"][i]["Id"].ToString());
                var name = parsedObject["materials"][i]["Name"].ToString();
                var value = int.Parse(parsedObject["materials"][i]["Value"].ToString());
                var newMaterial = new Material(id,name,value);
                Materials.Add(newMaterial);
            }
            // Test: output materials list
        }
        public static void LoadMonsters()
        {
            
        }
        public static void LoadRegions()
        {
            
        }
    }
}