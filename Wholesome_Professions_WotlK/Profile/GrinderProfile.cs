using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wManager.Wow.Class;

namespace Wholesome_Professions_WotlK.Profile
{
    [Serializable]
    public class GrinderProfile
    {
        public List<GrinderZone> GrinderZones = new List<GrinderZone>();
    }

    [Serializable]
    public class GrinderZone
    {
        public string Name = "";
        public bool Hotspots;
        public uint MinLevel;
        public uint MaxLevel = 90;
        public uint MinTargetLevel;
        public uint MaxTargetLevel = 90;
        public List<int> TargetEntry = new List<int>();
        public List<uint> TargetFactions = new List<uint>();
        public List<Vector3> Vectors3 = new List<Vector3>();
        public List<Npc> Npc = new List<Npc>();
        public List<GrinderBlackListRadius> BlackListRadius = new List<GrinderBlackListRadius>();

        internal bool IsValid()
        {
            try
            {
                return Vectors3.Count > 0;
            }
            catch
            {
                return false;
            }
        }
    }

    [Serializable]
    public class GrinderBlackListRadius
    {
        public Vector3 Position = new Vector3();
        public float Radius;
    }
}
