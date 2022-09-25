﻿using Kwytto.Data;
using Kwytto.Utils;
using System.Xml.Serialization;
using WriteEverywhere.Xml;

namespace WriteEverywhere.Data
{
    [XmlRoot("HighwayShieldsData")]
    public class WTSHighwayShieldsData : DataExtensionBase<WTSHighwayShieldsData>
    {
        public override string SaveId => "K45_WTS_HighwayShieldsData";
        [XmlElement]
        public SimpleXmlDictionary<string, HighwayShieldDescriptor> CityDescriptors = new SimpleXmlDictionary<string, HighwayShieldDescriptor>();
        [XmlIgnore]
        public SimpleXmlDictionary<string, HighwayShieldDescriptor> GlobalDescriptors = new SimpleXmlDictionary<string, HighwayShieldDescriptor>();
    }

}
