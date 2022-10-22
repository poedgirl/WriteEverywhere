﻿namespace WriteEverywhere.TransportLines
{
    public struct WTSLine
    {
        public ushort lineId;
        public bool regional;
        public bool test;

        public WTSLine(ushort lineId, bool regional, bool test = false)
        {
            this.lineId = lineId;
            this.regional = regional;
            this.test = test;
        }
        public WTSLine(StopInformation stop) : this(stop.m_lineId, stop.m_regionalLine) { }
        public bool ZeroLine => lineId == 0 && !regional;

        private int ToRefId() => regional ? 256 + lineId : lineId;
        public int ToExternalRefId() => (regional ? 256 + lineId : lineId) * (test ? -1 : 1);
        public uint GetUniqueStopId(ushort stopId) => (uint)(ToRefId() << 16) | stopId;

        public override string ToString() => $"{lineId}@{(regional ? "REG" : "CITY")}";
    }
}

