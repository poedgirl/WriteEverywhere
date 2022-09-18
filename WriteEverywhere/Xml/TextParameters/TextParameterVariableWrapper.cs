﻿using SpriteFontPlus;
using SpriteFontPlus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using WriteEverywhere.Rendering;

namespace WriteEverywhere.Xml
{
    public class TextParameterVariableWrapper
    {
        public readonly string m_originalCommand;

        internal TextParameterVariableWrapper(string input, TextRenderingClass renderingClass = TextRenderingClass.Any)
        {
            m_originalCommand = input;
            var parameterPath = CommandLevel.GetParameterPath(input);
            if (parameterPath.Length > 0)
            {
                VariableType varType = VariableType.Invalid;
                try
                {
                    varType = (VariableType)Enum.Parse(typeof(VariableType), parameterPath[0]);
                }
                catch { }
                if (!varType.Supports(renderingClass))
                {
                    return;
                }
                switch (varType)
                {
                    case VariableType.SegmentTarget:
                        if (parameterPath.Length >= 3 && byte.TryParse(parameterPath[1], out byte targIdx) && targIdx <= 4)
                        {
                            try
                            {
                                if (Enum.Parse(typeof(VariableSegmentTargetSubType), parameterPath[2]) is VariableSegmentTargetSubType tt
                                    && tt.ReadData(parameterPath.Skip(3).ToArray(), ref subtype, ref numberFormat, ref stringFormat, ref prefix, ref suffix))
                                {
                                    index = targIdx;
                                    type = VariableType.SegmentTarget;
                                }
                            }
                            catch { }
                        }
                        break;
                    case VariableType.CurrentSegment:
                        if (parameterPath.Length >= 2)
                        {
                            try
                            {
                                if (Enum.Parse(typeof(VariableSegmentTargetSubType), parameterPath[1]) is VariableSegmentTargetSubType tt
                                    && tt.ReadData(parameterPath.Skip(2).ToArray(), ref subtype, ref numberFormat, ref stringFormat, ref prefix, ref suffix))
                                {
                                    type = VariableType.CurrentSegment;
                                }
                            }
                            catch { }
                        }
                        break;
                    case VariableType.CityData:
                        if (parameterPath.Length >= 2)
                        {
                            try
                            {
                                if (Enum.Parse(typeof(VariableCitySubType), parameterPath[1]) is VariableCitySubType tt
                                    && tt.ReadData(parameterPath.Skip(2).ToArray(), ref subtype, ref numberFormat, ref stringFormat, ref prefix, ref suffix))
                                {
                                    type = VariableType.CityData;
                                    break;
                                }
                            }
                            catch { }
                        }
                        break;
                    case VariableType.CurrentBuilding:
                        if (parameterPath.Length >= 2)
                        {
                            try
                            {
                                if (Enum.Parse(typeof(VariableBuildingSubType), parameterPath[1]) is VariableBuildingSubType tt
                                    && tt.ReadData(parameterPath.Skip(2).ToArray(), ref subtype, ref numberFormat, ref stringFormat, ref prefix, ref suffix))
                                {
                                    type = VariableType.CurrentBuilding;
                                    break;
                                }
                            }
                            catch { }
                        }
                        break;
                    case VariableType.CurrentVehicle:
                        if (parameterPath.Length >= 2)
                        {
                            try
                            {
                                if (Enum.Parse(typeof(VariableVehicleSubType), parameterPath[1]) is VariableVehicleSubType tt
                                    && tt.ReadData(parameterPath.Skip(2).ToArray(), ref subtype, ref numberFormat, ref stringFormat, ref prefix, ref suffix))
                                {
                                    type = VariableType.CurrentVehicle;
                                    break;
                                }
                            }
                            catch { }
                        }
                        break;
                }
            }
        }


        private VariableType type = VariableType.Invalid;
        private byte index = 0;
        private Enum subtype = VariableSegmentTargetSubType.None;
        private string numberFormat = "0";
        private string stringFormat = "";
        private string prefix = "";
        private string suffix = "";



        public BasicRenderInformation GetTargetText(BoardInstanceXml instance, BoardTextDescriptorGeneralXml textDescriptor, DynamicSpriteFont targetFont, ushort refId, int secRefId, int tercRefId, out IEnumerable<BasicRenderInformation> multipleOutput)
        {
            string targetStr = m_originalCommand;
            switch (instance)
            {
                case OnNetInstanceCacheContainerXml cc:
                    targetStr = GetTargetTextForNet(cc, refId, textDescriptor, out multipleOutput);
                    break;
                //case BoardInstanceBuildingXml bd:
                //    targetStr = GetTargetTextForBuilding(bd, refId, textDescriptor, out multipleOutput);
                //    break;
                //case LayoutDescriptorVehicleXml ve:
                //    targetStr = GetTargetTextForVehicle(refId, textDescriptor, out multipleOutput);
                //    break;
                //case BoardPreviewInstanceXml bp:
                //    switch (instance.RenderingClass)
                //    {
                //        case TextRenderingClass.None:
                //            break;
                //        case TextRenderingClass.RoadNodes:
                //            break;
                //        case TextRenderingClass.Buildings:
                //            targetStr = GetTargetTextForBuilding(bp, refId, textDescriptor, out multipleOutput);
                //            break;
                //        case TextRenderingClass.PlaceOnNet:
                //            targetStr = GetTargetTextForNet(bp, refId, textDescriptor, out multipleOutput);
                //            break;
                //        case TextRenderingClass.Vehicle:
                //            targetStr = GetTargetTextForVehicle(refId, textDescriptor, out multipleOutput);
                //            break;
                //    }
                //    multipleOutput = null;
                //    break;
                default:
                    multipleOutput = null;
                    break;
            }
            return multipleOutput is null ? targetFont.DrawString(ModInstance.Controller, targetStr, default, FontServer.instance.ScaleEffective) : null;
        }



        //        public string GetTargetTextForBuilding(BoardInstanceXml descriptor, ushort buildingId, BoardTextDescriptorGeneralXml textDescriptor, out IEnumerable<BasicRenderInformation> multipleOutput)
        //        {
        //            multipleOutput = null;
        //            var buildingDescriptor = descriptor as BoardInstanceBuildingXml;
        //            switch (type)
        //            {
        //                case VariableType.CurrentBuilding:
        //                    return buildingId == 0 || buildingDescriptor is null || !(subtype is VariableBuildingSubType targetSubtype2) || targetSubtype2 == VariableBuildingSubType.None
        //                        ? $"{prefix}{subtype}@currBuilding"
        //                        : $"{prefix}{targetSubtype2.GetFormattedString(buildingDescriptor.m_platforms, buildingId, this) ?? m_originalCommand}{suffix}";
        //                case VariableType.CityData:
        //                    if ((subtype is VariableCitySubType targetCitySubtype))
        //                    {
        //                        return $"{prefix}{targetCitySubtype.GetFormattedString(this) ?? m_originalCommand}{suffix}";
        //                    }
        //                    break;
        //            }
        //            return m_originalCommand;
        //        }

        //        public string GetTargetTextForVehicle(ushort vehicleId, BoardTextDescriptorGeneralXml textDescriptor, out IEnumerable<BasicRenderInformation> multipleOutput)
        //        {
        //            multipleOutput = null;
        //            switch (type)
        //            {
        //                case VariableType.CurrentBuilding:
        //                    var buildingId = VehicleManager.instance.m_vehicles.m_buffer[vehicleId].m_sourceBuilding;
        //                    return buildingId == 0 || !(subtype is VariableBuildingSubType targetSubtype) || targetSubtype == VariableBuildingSubType.None
        //                        ? $"{prefix}{subtype}@vehicleSrcBuilding"
        //                        : $"{prefix}{targetSubtype.GetFormattedString(null, buildingId, this) ?? m_originalCommand}{suffix}";
        //                case VariableType.CurrentVehicle:
        //                    return vehicleId == 0 || !(subtype is VariableVehicleSubType targetSubtype2) || targetSubtype2 == VariableVehicleSubType.None
        //                        ? $"{prefix}{subtype}@currVehicle"
        //                        : $"{prefix}{targetSubtype2.GetFormattedString(vehicleId, this) ?? m_originalCommand}{suffix}";
        //                case VariableType.CityData:
        //                    if ((subtype is VariableCitySubType targetCitySubtype))
        //                    {
        //                        return $"{prefix}{targetCitySubtype.GetFormattedString(this) ?? m_originalCommand}{suffix}";
        //                    }
        //                    break;
        //            }
        //            return m_originalCommand;
        //        }

        public string GetTargetTextForNet(BoardInstanceXml descriptor, ushort segmentId, BoardTextDescriptorGeneralXml textDescriptor, out IEnumerable<BasicRenderInformation> multipleOutput)
        {
            multipleOutput = null;
            var propDescriptor = descriptor as OnNetInstanceCacheContainerXml;
            switch (type)
            {
                case VariableType.SegmentTarget:
                    var targId = propDescriptor?.GetTargetSegment(index) ?? 0;
                    return targId == 0 || !(subtype is VariableSegmentTargetSubType targetSubtype) || targetSubtype == VariableSegmentTargetSubType.None
                        ? $"{prefix}{subtype}@targ{index}{suffix}"
                        : $"{prefix}{targetSubtype.GetFormattedString(propDescriptor, targId, this) ?? m_originalCommand}{suffix}";
                case VariableType.CurrentSegment:
                    return segmentId == 0 || !(subtype is VariableSegmentTargetSubType targetSubtype2) || targetSubtype2 == VariableSegmentTargetSubType.None
                        ? $"{prefix}{subtype}@currSeg"
                        : $"{prefix}{targetSubtype2.GetFormattedString(propDescriptor, segmentId, this) ?? m_originalCommand}{suffix}";
                case VariableType.CityData:
                    if ((subtype is VariableCitySubType targetCitySubtype))
                    {
                        return $"{prefix}{targetCitySubtype.GetFormattedString(this) ?? m_originalCommand}{suffix}";
                    }
                    break;
                case VariableType.Invalid:
                    return $"<UNSUPPORTED PATH: {m_originalCommand}>";
            }
            return m_originalCommand;
        }

        internal string TryFormat(float value, float multiplier)
        {
            try
            {
                return (value * multiplier).ToString(numberFormat);
            }
            catch
            {
                numberFormat = "0";
                return (value * multiplier).ToString(numberFormat);
            }
        }
        internal string TryFormat(long value)
        {
            try
            {
                return value.ToString(numberFormat);
            }
            catch
            {
                numberFormat = "0";
                return value.ToString(numberFormat);
            }
        }
        internal string TryFormat(FormatableString value) => value.GetFormatted(stringFormat);
    }
}
