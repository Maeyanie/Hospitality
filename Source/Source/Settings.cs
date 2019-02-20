using System;
using HugsLib.Settings;
using Verse;

namespace Hospitality {
    internal class Settings
    {
        public static SettingHandle<int> minGuestWorkSkill;
        public static SettingHandle<int> maxGuestGroupSize;
        public static SettingHandle<bool> disableGuests;
        public static SettingHandle<bool> disableWork;
        public static SettingHandle<bool> disableGifts;
        public static SettingHandle<bool> disableLimits;
        public static SettingHandle<bool> disableArtAndCraft;
        public static SettingHandle<bool> disableOperations;
        public static SettingHandle<bool> disableGuestsTab;

        public Settings(ModSettingsPack settings)
        {
            disableGuests = settings.GetHandle("disableGuests", "DisableVisitors".Translate(), "DisableVisitorsDesc".Translate(), false);
            disableWork = settings.GetHandle("disableWork", "DisableGuestsHelping".Translate(), "DisableGuestsHelpingDesc".Translate(), false);
            disableArtAndCraft = settings.GetHandle("disableArtAndCraft", "DisableArtAndCraft".Translate(), "DisableArtAndCraftDesc".Translate(), true);
            disableOperations = settings.GetHandle("disableOperations", "DisableOperations".Translate(), "DisableOperationsDesc".Translate(), true);
            disableGifts = settings.GetHandle("disableGifts", "DisableGifts".Translate(), "DisableGiftsDesc".Translate(), false);
            minGuestWorkSkill = settings.GetHandle("minGuestWorkSkill", "MinGuestWorkSkill".Translate(), "MinGuestWorkSkillDesc".Translate(), 7, WorkSkillLimits());
            maxGuestGroupSize = settings.GetHandle("maxGuestGroupSize", "MaxGuestGroupSize".Translate(), "MaxGuestGroupSizeDesc".Translate(), 16, GroupSizeLimits());
            disableLimits = settings.GetHandle("disableLimits", "DisableLimits".Translate(), "DisableLimitsDesc".Translate(), false);
            disableGuestsTab = settings.GetHandle("disableGuestsTab", "DisableGuestsTab".Translate(), "DisableGuestsTabDesc".Translate(), false);
            
            string hiddenConfigFile = Path.Combine(GenFilePaths.ConfigFolderPath, "Hospitality.cfg");
            if (File.Exists(hiddenConfigFile))
            {
                try {
                    var reader = File.OpenText(hiddenConfigFile);
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#")) continue;
                        string[] keyVal = line.Split('=');
                        if (keyVal.Length != 2) continue;
                        string key = keyVal[0].Trim();
                        string val = keyVal[1].Trim();

                        switch (key)
                        {
                            case "PriceFactor":
                                Log.Message("[Hospitality] Setting PriceFactor to " + val);
                                JobDriver_BuyItem.PriceFactor = float.Parse(val);
                                break;

                            default:
                                Log.Message("[Hospitality] Unrecognized setting: " + key);
                                break;
                        }
                    }
                } catch (Exception e) {
                    Log.Error("[Hospitality] Exception loading Hospitality.cfg: " + e.Message);
                }
            }
        }

        private static SettingHandle.ValueIsValid WorkSkillLimits()
        {
            return AtLeast(() => disableLimits?.Value != false ? 0 : 6);
        }

        private static SettingHandle.ValueIsValid GroupSizeLimits()
        {
            return AtLeast(() => disableLimits?.Value != false ? 1 : 8);
        }

        private static SettingHandle.ValueIsValid AtLeast(Func<int> amount)
        {
            return delegate(string value) {
                int actual;
                return int.TryParse(value, out actual) && actual >= amount();
            };
        }
    }
}
