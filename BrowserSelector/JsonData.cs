using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace BrowserSelector
{
    [DataContract]
    public class JsonData
    {
        [DataMember]
        public JsonDataItem[] Items { get; set;}
        
        public void Save(Stream stream)
        {
            serializer.WriteObject(stream, this);
        }

        public static JsonData Load(Stream stream)
        {
            return (JsonData) serializer.ReadObject(stream);
        }

        static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(JsonData));
    }

    [DataContract]
    public class JsonDataItem
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ExePath { get; set; }

        [DataMember(IsRequired=false)]
        public string Params { get; set; }

        [DataMember(Name="Regex")]
        public string RegexString { get; set; }

        [IgnoreDataMember]
        Regex regex;
        [IgnoreDataMember]
        public Regex Regex 
        {
            get 
            {
                if ( regex == null )
                    regex = new Regex(RegexString, RegexOptions.Compiled);
                return regex; 
            } 
        }

        [DataMember(IsRequired=false)]
        public bool IsSupportMultiUrl { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}", Name, ExePath, Params, RegexString);                
        }
    }
}
