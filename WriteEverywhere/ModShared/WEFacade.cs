﻿extern alias VS;
using WriteEverywhere.UI;

namespace WriteEverywhere.ModShared
{
    public class WEFacade
    {
        public static bool IsWEVehicleEditorOpen => WTSVehicleLiteUI.Instance?.Visible ?? false;
        public static bool IsAnyEditorOpen => WTSVehicleLiteUI.Instance.Visible || BuildingLiteUI.Instance.Visible || WTSOnNetLiteUI.Instance.Visible;
        public static string CurrentSelectedSkin => WTSVehicleLiteUI.Instance.CurrentSkin;
        public static ushort CurrentGrabbedVehicleId => WTSVehicleLiteUI.Instance.CurrentGrabbedId;
    }
}