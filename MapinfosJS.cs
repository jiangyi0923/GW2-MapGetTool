﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// <auto-generated />
// https://app.quicktype.io/#l=cs&r=json2csharp
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using GW2_mapinfogeter;
//
//    var mapinfosJs = MapinfosJs.FromJson(jsonString);

namespace GW2MapGetTool
{
    public partial class MapinfosJs
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("min_level", Required = Required.Always)]
        public long MinLevel { get; set; }

        [JsonProperty("max_level", Required = Required.Always)]
        public long MaxLevel { get; set; }

        [JsonProperty("default_floor", Required = Required.Always)]
        public long DefaultFloor { get; set; }

        [JsonProperty("label_coord")]
        public long[] LabelCoord { get; set; }

        [JsonProperty("map_rect", Required = Required.Always)]
        public long[][] MapRect { get; set; }

        [JsonProperty("continent_rect", Required = Required.Always)]
        public long[][] ContinentRect { get; set; }

        [JsonProperty("points_of_interest", Required = Required.Always)]
        public Dictionary<string, PointsOfInterest> PointsOfInterest { get; set; }

        [JsonProperty("tasks", Required = Required.Always)]
        public Dictionary<string, Sector> Tasks { get; set; }

        [JsonProperty("skill_challenges", Required = Required.Always)]
        public SkillChallenge[] SkillChallenges { get; set; }

        [JsonProperty("sectors", Required = Required.Always)]
        public Dictionary<string, Sector> Sectors { get; set; }

        [JsonProperty("adventures", Required = Required.Always)]
        public Adventure[] Adventures { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty("mastery_points")]
        public MasteryPoint[] MasteryPoints { get; set; }
    }

    public partial class Adventure
    {
        [JsonProperty("coord", Required = Required.Always)]
        public double[] Coord { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("description", Required = Required.Always)]
        public string Description { get; set; }
    }

    public partial class MasteryPoint
    {
        [JsonProperty("coord")]
        public double[] Coord { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }
    }

    public partial class PointsOfInterest
    {
        [JsonProperty("name", Required = Required.Default)]
        public string Name { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("floor", Required = Required.Always)]
        public long Floor { get; set; }

        [JsonProperty("coord", Required = Required.Always)]
        public double[] Coord { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty("chat_link", Required = Required.Always)]
        public string ChatLink { get; set; }
    }

    public partial class Sector
    {
        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("level", Required = Required.Always)]
        public long Level { get; set; }

        [JsonProperty("coord", Required = Required.Always)]
        public double[] Coord { get; set; }

        [JsonProperty("bounds", Required = Required.Always)]
        public double[][] Bounds { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public long Id { get; set; }

        [JsonProperty("chat_link", Required = Required.Always)]
        public string ChatLink { get; set; }

        [JsonProperty("objective", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Objective { get; set; }
    }

    public partial class SkillChallenge
    {
        [JsonProperty("coord", Required = Required.Always)]
        public double[] Coord { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }
    }

    public partial class MapinfosJs
    {
        public static MapinfosJs FromJson(string json) => JsonConvert.DeserializeObject<MapinfosJs>(json, GW2MapGetTool.Converter2.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MapinfosJs self) => JsonConvert.SerializeObject(self, GW2MapGetTool.Converter2.Settings);
    }

    internal static class Converter2
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}