using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MakePlacePlugin.Util
{
    struct Location
    {
        public float x;
        public float y;
        public float z;
        public float rotation;
        public string size;

        public Location(float _x, float _y, float _z, float rot, string _size)
        {
            x = _x;
            y = _y;
            z = _z;
            rotation = rot;
            size = _size;
        }

        public Vector3 ToVector()
        {
            return new Vector3(x, y, z);
        }
    }

    class Plots
    {

        public static Dictionary<string, Dictionary<int, Location>> Map = new Dictionary<string, Dictionary<int, Location>> {

            {"Goblet", new Dictionary<int, Location>{
                { 01, new Location(-38.00001f, -4f, -110f, 2.86234f, "s")},
                { 02, new Location(-64.00002f, 4f, -98.00002f, -0.6108653f, "s")},
                { 03, new Location(-85.64f, 6f, -80.02f, -0.8834857f, "s")},
                { 04, new Location(-82.203f, -4f, -121.07f, 2.548181f, "m")},
                { 05, new Location(-118.665f, 12f, -92.44f, 0.5962044f, "l")},
                { 06, new Location(-39.73f, 0f, -64.44f, 0.7853982f, "m")},
                { 07, new Location(-72.6f, 8f, -40.315f, 2.059489f, "s")},
                { 08, new Location(-117.81f, 10f, -23.37f, -1.343904f, "m")},
                { 09, new Location(-140.246f, 12f, -51.004f, 2.007129f, "s")},
                { 10, new Location(-147.905f, 15f, -24.543f, 0.1745329f, "s")},
                { 11, new Location(-158.78f, 15f, 18.875f, -1.833119f, "m")},
                { 12, new Location(-116f, 15f, 23.99996f, 1.308997f, "m")},
                { 13, new Location(-72f, 11.618f, 12f, -0.2391101f, "l")},
                { 14, new Location(-32.00002f, -20f, 17.99998f, 3.141593f, "s")},
                { 15, new Location(-14.018f, -16.02f, -36.035f, -0.7853982f, "s")},
                { 16, new Location(68.24998f, -16f, -39.56697f, -0.5235988f, "s")},
                { 17, new Location(67f, -20f, 0.2499695f, 3.141593f, "s")},
                { 18, new Location(73.188f, -24f, 28.851f, 2.094395f, "s")},
                { 19, new Location(44f, -24f, 53.81f, 2.094395f, "m")},
                { 20, new Location(0f, -24f, 79.99998f, 3.141593f, "s")},
                { 21, new Location(93.99998f, -26f, -70.00002f, 2.356194f, "s")},
                { 22, new Location(106f, -14f, -36.00003f, -0.3490659f, "s")},
                { 23, new Location(130.465f, -18f, -56.628f, 1.221731f, "s")},
                { 24, new Location(144.5f, -26f, -32.00003f, 0f, "s")},
                { 25, new Location(123.75f, -28f, 4f, -1.570796f, "m")},
                { 26, new Location(120f, -28f, 39.99997f, -1.570796f, "s")},
                { 27, new Location(130f, -36f, 71.99997f, 3.141593f, "s")},
                { 28, new Location(95.545f, -33.68f, 59.08f, 0.5235988f, "s")},
                { 29, new Location(184f, -44f, 23.49998f, 1.570796f, "s")},
                { 30, new Location(188f, -40f, 60f, 3.141593f, "l")},
                { 31, new Location(-594f, -4f, -742f, -1.850049f, "s")},
                { 32, new Location(-606f, 4f, -768f, 0.9599311f, "s")},
                { 33, new Location(-623.98f, 6f, -789.64f, 0.6873107f, "s")},
                { 34, new Location(-582.93f, -4f, -786.203f, -2.164208f, "m")},
                { 35, new Location(-611.56f, 12f, -822.665f, 2.167001f, "l")},
                { 36, new Location(-639.56f, 0f, -743.73f, 2.356195f, "m")},
                { 37, new Location(-663.685f, 8f, -776.6f, -2.6529f, "s")},
                { 38, new Location(-680.63f, 10f, -821.81f, 0.2268923f, "m")},
                { 39, new Location(-652.996f, 12f, -844.246f, -2.70526f, "s")},
                { 40, new Location(-679.457f, 15f, -851.905f, 1.745329f, "s")},
                { 41, new Location(-722.875f, 15f, -862.78f, -0.262323f, "m")},
                { 42, new Location(-727.9999f, 15f, -820f, 2.879793f, "m")},
                { 43, new Location(-716f, 11.618f, -776f, 1.331686f, "l")},
                { 44, new Location(-722f, -20f, -736f, -1.570796f, "s")},
                { 45, new Location(-667.965f, -16.02f, -718.018f, 0.7853982f, "s")},
                { 46, new Location(-664.433f, -16f, -635.75f, 1.047198f, "s")},
                { 47, new Location(-704.25f, -20f, -637f, -1.570796f, "s")},
                { 48, new Location(-732.851f, -24f, -630.812f, -2.617994f, "s")},
                { 49, new Location(-757.81f, -24f, -660f, -2.617994f, "m")},
                { 50, new Location(-784f, -24f, -704f, -1.570796f, "s")},
                { 51, new Location(-634f, -26f, -610f, -2.356195f, "s")},
                { 52, new Location(-668f, -14f, -598f, 1.22173f, "s")},
                { 53, new Location(-647.372f, -18f, -573.535f, 2.792527f, "s")},
                { 54, new Location(-672f, -26f, -559.5f, 1.570796f, "s")},
                { 55, new Location(-708f, -28f, -580.25f, 0f, "m")},
                { 56, new Location(-744f, -28f, -584f, 0f, "s")},
                { 57, new Location(-776f, -36f, -574f, -1.570796f, "s")},
                { 58, new Location(-763.08f, -33.68f, -608.455f, 2.094395f, "s")},
                { 59, new Location(-727.5f, -44f, -520f, -3.141593f, "s")},
                { 60, new Location(-764f, -40f, -516f, -1.570796f, "l")}

            } }
        };

    }
}
