using System.Collections.Generic;
using System.Linq;
using Wholesome_Professions_WotlK.Helpers;
using wManager.Wow.Helpers;

static class FrameHelper
{
    private static List<string[]> _debugStringList = new List<string[]>();
    private static List<string[]> _broadcastStringList = new List<string[]>();

    public static void CreateBroadcastFrame()
    {
        Lua.LuaDoString(@"
        if not WPBroadcast then
            WPBroadcast = CreateFrame('Frame')
            WPBroadcast:ClearAllPoints()
            WPBroadcast:SetFrameStrata('BACKGROUND')

            WPBroadcast:SetWidth(380)
            WPBroadcast:SetHeight(190)

            WPBroadcast: SetBackdrop(StaticPopup1:GetBackdrop())

            WPBroadcast:SetPoint('TOP', 0, 0)
            WPBroadcast:SetBackdropBorderColor(1, 0, 0, 0.8)
            WPBroadcast:SetMovable(true)
            WPBroadcast:EnableMouse(true)

            WPBroadcast.text = WPBroadcast:CreateFontString(nil, 'BACKGROUND', 'GameFontNormal')
            WPBroadcast.text:SetTextColor(1, 1, 0, 1)
            WPBroadcast.text:SetPoint('TOPLEFT', 20, -20)
            WPBroadcast.text:SetPoint('BOTTOMRIGHT', -20, 20)
            WPBroadcast.text:SetJustifyV('TOP');
            WPBroadcast.text:SetJustifyH('CENTER');
            WPBroadcast.text:SetText('Loading...')

            WPBroadcast:SetScript('OnMouseDown', function() WPBroadcast:StartMoving() end)
            WPBroadcast:SetScript('OnMouseUp', function() WPBroadcast:StopMovingOrSizing() end)

            WPBroadcast.Close = CreateFrame('BUTTON', nil, WPBroadcast, 'UIPanelCloseButton')
            WPBroadcast.Close:SetWidth(20)
            WPBroadcast.Close:SetHeight(20)
            WPBroadcast.Close:SetPoint('TOPRIGHT', WPBroadcast, 3, 3)
            WPBroadcast.Close:SetScript('OnClick', function()
                WPBroadcast:Hide()
                DEFAULT_CHAT_FRAME:AddMessage('WPBroadcast closed. Write /WPBroadcast to enable again.')
            end)
            SLASH_WHATEVERYOURFRAMESARECALLED1='/WPBroadcast'
            SlashCmdList.WHATEVERYOURFRAMESARECALLED = function()
                if WPBroadcast:IsShown() then
                    WPBroadcast:Hide()
                else
                    WPBroadcast:Show()
                end
            end
        end", false);
    }

    private static void SetBroadcastFrameText(string str)
    {
        Lua.LuaDoString(FormatLua("WPBroadcast.text:SetText(\"{0}\")", new object[] { str }), false);
    }

    public static void UpdateBroadcastFrame(string key, string value)
    {
        if (value == null)
            return;
        // If the key doesn't exist, we create it, otherwise we replace the value
        if (!_broadcastStringList.Exists(e => e[0] == key))
            _broadcastStringList.Add(new string[] { key, value });
        else
            _broadcastStringList.Find(e => e[0] == key)[1] = value;

        string result = BuildBroadcastString(_broadcastStringList);
        SetBroadcastFrameText(result);
    }

    private static string BuildBroadcastString(List<string[]> listStrings)
    {
        string result = "";
        foreach (string[] stringArray in listStrings)
        {
            result += stringArray[1] + "\\r";
        }
        return result;
    }

    public static void ClearBroadcastString()
    {
        _broadcastStringList.Clear();
    }

    public static void CreateDebugFrame()
    {
        Lua.LuaDoString(@"
        if not WPHelperFrame then
            WPHelperFrame = CreateFrame('Frame')
            WPHelperFrame:ClearAllPoints()
            WPHelperFrame:SetFrameStrata('BACKGROUND')

            WPHelperFrame:SetWidth(400)
            WPHelperFrame:SetHeight(400)

            WPHelperFrame: SetBackdrop(StaticPopup1:GetBackdrop())

            WPHelperFrame.text = WPHelperFrame:CreateFontString(nil, 'BACKGROUND', 'GameFontNormal')
            WPHelperFrame.text:SetTextColor(1, 1, 0, 1)
            WPHelperFrame.text:SetPoint('TOPLEFT', 20, -20)
            WPHelperFrame.text:SetPoint('BOTTOMRIGHT', -20, 20)
            WPHelperFrame.text:SetJustifyV('TOP');
            WPHelperFrame.text:SetJustifyH('LEFT');
            WPHelperFrame.text:SetText('Loading...')

            WPHelperFrame:SetPoint('LEFT', 0, 100)
            WPHelperFrame:SetBackdropBorderColor(1, 0, 0, 0.8)
            WPHelperFrame:SetMovable(true)
            WPHelperFrame:EnableMouse(true)

            WPHelperFrame:SetScript('OnMouseDown', function() WPHelperFrame:StartMoving() end)
            WPHelperFrame:SetScript('OnMouseUp', function() WPHelperFrame:StopMovingOrSizing() end)

            WPHelperFrame.Close = CreateFrame('BUTTON', nil, WPHelperFrame, 'UIPanelCloseButton')
            WPHelperFrame.Close:SetWidth(20)
            WPHelperFrame.Close:SetHeight(20)
            WPHelperFrame.Close:SetPoint('TOPRIGHT', WPHelperFrame, 3, 3)
        end", false);
    }

    private static void SetDebugFrameText(string str)
    {
        Lua.LuaDoString(FormatLua("WPHelperFrame.text:SetText(\"{0}\")", new object[] { str }), false);
    }

    private static string FormatLua(string str, params object[] names)
    {
        return string.Format(str, (from s in names select s.ToString().Replace("'", "\\'").Replace("\"", "\\\"")).ToArray());
    }

    public static void UpdateDebugFrame(string key, string value)
    {
        // If the key doesn't exist, we create it, otherwise we replace the value
        if (!_debugStringList.Exists(e => e[0] == key))
            _debugStringList.Add(new string[] { key, value });
        else
            _debugStringList.Find(e => e[0] == key)[1] = value;

        string result = BuildDebugString(_debugStringList);
        SetDebugFrameText(result);
    }

    private static string BuildDebugString(List<string[]> listStrings)
    {
        string result = "";
        foreach (string[] stringArray in listStrings)
        {
            result += stringArray[0] + " : " + stringArray[1] + "\\r";
        }
        return result;
    }

    public static void ClearDebugString()
    {
        _debugStringList.Clear();
    }
}
