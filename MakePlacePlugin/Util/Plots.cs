﻿// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Util.Plots
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;

namespace MakePlacePlugin.Util; 

internal class Plots {
    public static Dictionary<string, Dictionary<int, Location>> Map = new() {
        {
            "r1h1",
            new Dictionary<int, Location> {
                {
                    1,
                    new Location(90f, -18.5f, 177f, 3.141593f, "s", "Center")
                }, {
                    2,
                    new Location(144f, -38.5f, 129f, 0.0f, "m", "Center")
                }, {
                    3,
                    new Location(79f, -28.5f, 109f, 0.0f, "s", "Center")
                }, {
                    4,
                    new Location(101f, -28.5f, 78f, -1.570796f, "s", "Center")
                }, {
                    5,
                    new Location(179f, -48.5f, 85f, 3.141593f, "s", "Center")
                }, {
                    6,
                    new Location(184f, -48.5f, 44f, 0.0f, "s", "Center")
                }, {
                    7,
                    new Location(117f, -28.5f, 42f, -1.570796f, "m", "Center")
                }, {
                    8,
                    new Location(-36f, -18.5f, 86f, -0.9599311f, "m", "Center")
                }, {
                    9,
                    new Location(4f, -18.5f, 59f, 0.6108653f, "s", "Center")
                }, {
                    10,
                    new Location(-23f, -18.5f, 36f, 3.141593f, "s", "Center")
                }, {
                    11,
                    new Location(-71f, -8.5f, 135f, 0.0f, "s", "Center")
                }, {
                    12,
                    new Location(-114f, 1.5f, 78f, -0.9599311f, "l", "Center")
                }, {
                    13,
                    new Location(-81f, 1.5f, 42f, 0.0f, "s", "Center")
                }, {
                    14,
                    new Location(-23f, -18.5f, -36f, 0.0f, "s", "Center")
                }, {
                    15,
                    new Location(-9f, -18.5f, -80f, -0.6108653f, "s", "Center")
                }, {
                    16,
                    new Location(-41f, -8.5f, -94f, -2.181662f, "s", "Center")
                }, {
                    17,
                    new Location(-86f, 1.5f, -43f, -1.570796f, "m", "Center")
                }, {
                    18,
                    new Location(-116f, 11.5f, -85f, -2.181662f, "m", "Center")
                }, {
                    19,
                    new Location(-46f, 1.5f, -146f, -1.570796f, "s", "Center")
                }, {
                    20,
                    new Location(-77f, 1.5f, -147f, 0.0f, "s", "Center")
                }, {
                    21,
                    new Location(-72f, 11.5f, -190f, 0.0f, "m", "Center")
                }, {
                    22,
                    new Location(-140f, 21.5f, -136f, -1.570796f, "l", "Center")
                }, {
                    23,
                    new Location(114f, -18.5f, -37f, 0.9599311f, "s", "Center")
                }, {
                    24,
                    new Location(131f, -18.5f, -61f, 2.530727f, "s", "Center")
                }, {
                    25,
                    new Location(88f, -18.5f, -88f, 0.0f, "s", "Center")
                }, {
                    26,
                    new Location(185f, -38.5f, -54f, 0.0f, "m", "Center")
                }, {
                    27,
                    new Location(151f, -28.5f, -100f, 0.9599311f, "s", "Center")
                }, {
                    28,
                    new Location(44f, 1.5f, -135f, -1.570796f, "s", "Center")
                }, {
                    29,
                    new Location(52f, 1.5f, -176f, 0.0f, "s", "Center")
                }, {
                    30,
                    new Location(93f, 1.5f, -178f, 0.0f, "l", "Center")
                }, {
                    31,
                    new Location(-881f, -18.5f, -614f, -1.570796f, "s", "Center")
                }, {
                    32,
                    new Location(-833f, -38.5f, -560f, 1.570796f, "m", "Center")
                }, {
                    33,
                    new Location(-813f, -28.5f, -625f, 1.570796f, "s", "Center")
                }, {
                    34,
                    new Location(-782f, -28.5f, -603f, 0.0f, "s", "Center")
                }, {
                    35,
                    new Location(-789f, -48.5f, -525f, -1.570796f, "s", "Center")
                }, {
                    36,
                    new Location(-748f, -48.5f, -520f, 1.570796f, "s", "Center")
                }, {
                    37,
                    new Location(-746f, -28.5f, -587f, 0.0f, "m", "Center")
                }, {
                    38,
                    new Location(-790f, -18.5f, -740f, 0.6108653f, "m", "Center")
                }, {
                    39,
                    new Location(-763f, -18.5f, -700f, 2.181662f, "s", "Center")
                }, {
                    40,
                    new Location(-740f, -18.5f, -727f, -1.570796f, "s", "Center")
                }, {
                    41,
                    new Location(-839f, -8.5f, -775f, 1.570796f, "s", "Center")
                }, {
                    42,
                    new Location(-782f, 1.5f, -818f, 0.6108653f, "l", "Center")
                }, {
                    43,
                    new Location(-746f, 1.5f, -785f, 1.570796f, "s", "Center")
                }, {
                    44,
                    new Location(-668f, -18.5f, -727f, 1.570796f, "s", "Center")
                }, {
                    45,
                    new Location(-624f, -18.5f, -713f, 0.9599311f, "s", "Center")
                }, {
                    46,
                    new Location(-610f, -8.5f, -745f, -0.6108651f, "s", "Center")
                }, {
                    47,
                    new Location(-661f, 1.5f, -790f, 0.0f, "m", "Center")
                }, {
                    48,
                    new Location(-619f, 11.5f, -820f, -0.6108651f, "m", "Center")
                }, {
                    49,
                    new Location(-558f, 1.5f, -750f, 0.0f, "s", "Center")
                }, {
                    50,
                    new Location(-557f, 1.5f, -781f, 1.570796f, "s", "Center")
                }, {
                    51,
                    new Location(-514f, 11.5f, -776f, 1.570796f, "m", "Center")
                }, {
                    52,
                    new Location(-568f, 21.5f, -844f, 0.0f, "l", "Center")
                }, {
                    53,
                    new Location(-667f, -18.5f, -590f, 2.530728f, "s", "Center")
                }, {
                    54,
                    new Location(-643f, -18.5f, -573f, -2.181662f, "s", "Center")
                }, {
                    55,
                    new Location(-616f, -18.5f, -616f, 1.570796f, "s", "Center")
                }, {
                    56,
                    new Location(-650f, -38.5f, -519f, 1.570796f, "m", "Center")
                }, {
                    57,
                    new Location(-604f, -28.5f, -553f, 2.530728f, "s", "Center")
                }, {
                    58,
                    new Location(-569f, 1.5f, -660f, 0.0f, "s", "Center")
                }, {
                    59,
                    new Location(-528f, 1.5f, -652f, 1.570796f, "s", "Center")
                }, {
                    60,
                    new Location(-526f, 1.5f, -611f, 1.570796f, "l", "Center")
                }
            }
        }, {
            "e1h1",
            new Dictionary<int, Location> {
                {
                    1,
                    new Location(-34f, 2.02f, 116f, -1.308997f, "m", "Center")
                }, {
                    2,
                    new Location(-27f, 14.52f, 75f, -1.308997f, "s", "Center")
                }, {
                    3,
                    new Location(5f, 6.02f, 125f, -1.134464f, "s", "Center")
                }, {
                    4,
                    new Location(7f, 20.02f, 79f, -2.617994f, "s", "Center")
                }, {
                    5,
                    new Location(40f, 20.02f, 63f, 0.5235988f, "s", "Center")
                }, {
                    6,
                    new Location(45f, 10.02f, 99f, 0.0f, "s", "Center")
                }, {
                    7,
                    new Location(50f, 10.02f, 144f, 3.141593f, "l", "Center")
                }, {
                    8,
                    new Location(115f, 11.6f, 130f, 2.356194f, "m", "Center")
                }, {
                    9,
                    new Location(90f, 16.02f, 93f, -2.181662f, "s", "Center")
                }, {
                    10,
                    new Location(118f, 22.02f, 70f, 2.356194f, "s", "Center")
                }, {
                    11,
                    new Location(89f, 22.02f, 47f, 1.047198f, "s", "Center")
                }, {
                    12,
                    new Location(58f, 26.02f, 22f, 0.4363323f, "s", "Center")
                }, {
                    13,
                    new Location(68f, 31.59f, -18f, 0.4886922f, "m", "Center")
                }, {
                    14,
                    new Location(105f, 30.02f, 6f, 0.7853982f, "s", "Center")
                }, {
                    15,
                    new Location(131f, 34.02f, 32f, 2.356194f, "m", "Center")
                }, {
                    16,
                    new Location(136f, 40.02f, -30f, 0.7853982f, "l", "Center")
                }, {
                    17,
                    new Location(-105f, 2.02f, 74f, 2.094395f, "s", "Center")
                }, {
                    18,
                    new Location(-88f, 14.52f, 39f, 2.356194f, "s", "Center")
                }, {
                    19,
                    new Location(-135f, 2.02f, 50f, -0.7853982f, "m", "Center")
                }, {
                    20,
                    new Location(-107f, 10.02f, 12f, 0.9599311f, "s", "Center")
                }, {
                    21,
                    new Location(-164f, 6.02f, 22f, 2.879793f, "s", "Center")
                }, {
                    22,
                    new Location(-167f, 6.02f, -11f, 0.0f, "s", "Center")
                }, {
                    23,
                    new Location(-151f, 16.02f, -50f, -2.356194f, "s", "Center")
                }, {
                    24,
                    new Location(-123f, 17.51f, -26f, -2.356194f, "m", "Center")
                }, {
                    25,
                    new Location(-80f, 14.02f, -14f, 2.094395f, "s", "Center")
                }, {
                    26,
                    new Location(-97f, 16.02f, -56f, 0.7853982f, "s", "Center")
                }, {
                    27,
                    new Location(-59f, 20.02f, -55f, 0.0f, "s", "Center")
                }, {
                    28,
                    new Location(-75f, 25.02f, -117f, 0.0f, "m", "Center")
                }, {
                    29,
                    new Location(-116f, 26.62f, -93f, 1.134464f, "s", "Center")
                }, {
                    30,
                    new Location(-159f, 30.02f, -122f, -0.7853982f, "l", "Center")
                }, {
                    31,
                    new Location(-820f, 2.02f, -738f, 0.2617994f, "m", "Center")
                }, {
                    32,
                    new Location(-779f, 14.52f, -731f, 0.2617994f, "s", "Center")
                }, {
                    33,
                    new Location(-829f, 6.02f, -699f, 0.4363323f, "s", "Center")
                }, {
                    34,
                    new Location(-783f, 20.02f, -697f, -1.047197f, "s", "Center")
                }, {
                    35,
                    new Location(-767f, 20.02f, -664f, 2.094395f, "s", "Center")
                }, {
                    36,
                    new Location(-803f, 10.02f, -659f, 1.570796f, "s", "Center")
                }, {
                    37,
                    new Location(-848f, 10.02f, -654f, -1.570796f, "l", "Center")
                }, {
                    38,
                    new Location(-834f, 11.6f, -589f, -2.356195f, "m", "Center")
                }, {
                    39,
                    new Location(-797f, 16.02f, -614f, -0.6108653f, "s", "Center")
                }, {
                    40,
                    new Location(-774f, 22.02f, -586f, -2.356195f, "s", "Center")
                }, {
                    41,
                    new Location(-751f, 22.02f, -615f, 2.617994f, "s", "Center")
                }, {
                    42,
                    new Location(-726f, 26.02f, -646f, 2.007128f, "s", "Center")
                }, {
                    43,
                    new Location(-686f, 31.59f, -636f, 2.059489f, "m", "Center")
                }, {
                    44,
                    new Location(-710f, 30.02f, -599f, 2.356195f, "s", "Center")
                }, {
                    45,
                    new Location(-736f, 34.02f, -573f, -2.356195f, "m", "Center")
                }, {
                    46,
                    new Location(-674f, 40.02f, -568f, 2.356195f, "l", "Center")
                }, {
                    47,
                    new Location(-778f, 2.02f, -809f, -2.617994f, "s", "Center")
                }, {
                    48,
                    new Location(-743f, 14.52f, -792f, -2.356195f, "s", "Center")
                }, {
                    49,
                    new Location(-754f, 2.02f, -839f, 0.7853982f, "m", "Center")
                }, {
                    50,
                    new Location(-716f, 10.02f, -811f, 2.530728f, "s", "Center")
                }, {
                    51,
                    new Location(-726f, 6.02f, -868f, -1.832596f, "s", "Center")
                }, {
                    52,
                    new Location(-693f, 6.02f, -871f, 1.570796f, "s", "Center")
                }, {
                    53,
                    new Location(-654f, 16.02f, -855f, -0.7853979f, "s", "Center")
                }, {
                    54,
                    new Location(-678f, 17.51f, -827f, -0.7853979f, "m", "Center")
                }, {
                    55,
                    new Location(-690f, 14.02f, -784f, -2.617994f, "s", "Center")
                }, {
                    56,
                    new Location(-648f, 16.02f, -801f, 2.356195f, "s", "Center")
                }, {
                    57,
                    new Location(-649f, 20.02f, -763f, 1.570796f, "s", "Center")
                }, {
                    58,
                    new Location(-587f, 25.02f, -779f, 1.570796f, "m", "Center")
                }, {
                    59,
                    new Location(-611f, 26.62f, -820f, 2.70526f, "s", "Center")
                }, {
                    60,
                    new Location(-582f, 30.02f, -863f, 0.7853982f, "l", "Center")
                }
            }
        }, {
            "f1h1",
            new Dictionary<int, Location> {
                {
                    1,
                    new Location(144f, 46f, -78.375f, 1.570451f, "m", "Center")
                }, {
                    2,
                    new Location(78.5f, 51f, -100f, 0.0f, "s", "Center")
                }, {
                    3,
                    new Location(130f, 58.5f, -163f, 2.443461f, "l", "Far Right Side")
                }, {
                    4,
                    new Location(39.5f, 54.5f, -81f, -1.308997f, "s", "Center")
                }, {
                    5,
                    new Location(71.5f, 34.5f, -30.5f, 0.0f, "m", "Center")
                }, {
                    6,
                    new Location(143f, 23.5f, 2f, 2.007128f, "l", "Far Right Side")
                }, {
                    7,
                    new Location(116.5f, 9.625f, 52f, (float)Math.PI / 36f, "s", "Center")
                }, {
                    8,
                    new Location(85f, 8.75f, 63.5f, -0.7853982f, "s", "Center")
                }, {
                    9,
                    new Location(125f, 8f, 95f, 1.570451f, "s", "Center")
                }, {
                    10,
                    new Location(74.5f, 7.75f, 93.5f, -1.221731f, "s", "Center")
                }, {
                    11,
                    new Location(52f, 5f, 132f, 0.0f, "m", "Center")
                }, {
                    12,
                    new Location(64f, 25f, 22.5f, -1.570451f, "s", "Center")
                }, {
                    13,
                    new Location(27.74659f, 34f, 13f, -1.832596f, "s", "Center")
                }, {
                    14,
                    new Location(32f, 19.5f, 43.5f, -1.570451f, "s", "Center")
                }, {
                    15,
                    new Location(12f, 9f, 81.5f, -1.570451f, "s", "Center")
                }, {
                    16,
                    new Location(-92f, 7.75f, 98f, -0.6981317f, "m", "Center")
                }, {
                    17,
                    new Location(-30f, 10f, 78.5f, 0.7853978f, "s", "Center")
                }, {
                    18,
                    new Location(-31f, 21f, 44.5f, -1.570451f, "s", "Center")
                }, {
                    19,
                    new Location(-82f, 21.5f, 45.5f, -1.221731f, "s", "Center")
                }, {
                    20,
                    new Location(-8.5f, 33f, 1f, 1.570451f, "s", "Center")
                }, {
                    21,
                    new Location(-58f, 30.5f, -3f, 0.0f, "m", "Center")
                }, {
                    22,
                    new Location(-37f, 41.5f, -36.5f, -1.570451f, "s", "Center")
                }, {
                    23,
                    new Location(-65.5f, 32f, -35.5f, 1.570451f, "s", "Center")
                }, {
                    24,
                    new Location(-45.5f, 47f, -65.5f, -1.570451f, "s", "Center")
                }, {
                    25,
                    new Location(-4f, 46.5f, -73f, 1.570451f, "s", "Center")
                }, {
                    26,
                    new Location(-29f, 56.5f, -106f, 2.96706f, "s", "Center")
                }, {
                    27,
                    new Location(-114f, 31.5f, -40f, -1.308997f, "m", "Center")
                }, {
                    28,
                    new Location(-135.75f, 27.5f, -108.5f, -2.6529f, "l", "Left Side")
                }, {
                    29,
                    new Location(-105f, 34.5f, -154.5f, -0.7330383f, "s", "Center")
                }, {
                    30,
                    new Location(-50.5f, 56f, -150f, 0.0f, "m", "Center")
                }, {
                    31,
                    new Location(-625.625f, 46f, -560f, 3.141247f, "m", "Center")
                }, {
                    32,
                    new Location(-604f, 51f, -625.5f, 1.570796f, "s", "Center")
                }, {
                    33,
                    new Location(-541f, 58.5f, -574f, -2.268928f, "l", "Far Right Side")
                }, {
                    34,
                    new Location(-623f, 54.5f, -664.5f, 0.2617994f, "s", "Center")
                }, {
                    35,
                    new Location(-673.5f, 34.5f, -632.5f, 1.570796f, "m", "Center")
                }, {
                    36,
                    new Location(-706f, 23.5f, -561f, -2.70526f, "l", "Far Right Side")
                }, {
                    37,
                    new Location(-756f, 9.625f, -587.5f, 1.658063f, "s", "Center")
                }, {
                    38,
                    new Location(-767.5f, 8.75f, -619f, 0.7853982f, "s", "Center")
                }, {
                    39,
                    new Location(-799f, 8f, -579f, 3.141247f, "s", "Center")
                }, {
                    40,
                    new Location(-797.5f, 7.75f, -629.5f, 0.3490657f, "s", "Center")
                }, {
                    41,
                    new Location(-836f, 5f, -652f, 1.570796f, "m", "Center")
                }, {
                    42,
                    new Location(-726.5f, 25f, -640f, 0.0003454686f, "s", "Center")
                }, {
                    43,
                    new Location(-717f, 34f, -676.2534f, -0.2617998f, "s", "Center")
                }, {
                    44,
                    new Location(-747.5f, 19.5f, -672f, 0.0003454686f, "s", "Center")
                }, {
                    45,
                    new Location(-785.5f, 9f, -692f, 0.0003454686f, "s", "Center")
                }, {
                    46,
                    new Location(-802f, 7.75f, -796f, 0.8726646f, "m", "Center")
                }, {
                    47,
                    new Location(-782.5f, 10f, -734f, 2.356194f, "s", "Center")
                }, {
                    48,
                    new Location(-748.5f, 21f, -735f, 0.0003454686f, "s", "Center")
                }, {
                    49,
                    new Location(-749.5f, 21.5f, -786f, 0.3490657f, "s", "Center")
                }, {
                    50,
                    new Location(-705f, 33f, -712.5f, 3.141247f, "s", "Center")
                }, {
                    51,
                    new Location(-701f, 30.5f, -762f, 1.570796f, "m", "Center")
                }, {
                    52,
                    new Location(-667.5f, 41.5f, -741f, 0.0003454686f, "s", "Center")
                }, {
                    53,
                    new Location(-668.5f, 32f, -769.5f, 3.141247f, "s", "Center")
                }, {
                    54,
                    new Location(-638.5f, 47f, -749.5f, 0.0003454686f, "s", "Center")
                }, {
                    55,
                    new Location(-631f, 46.5f, -708f, 3.141247f, "s", "Center")
                }, {
                    56,
                    new Location(-598f, 56.5f, -733f, -1.745329f, "s", "Center")
                }, {
                    57,
                    new Location(-664f, 31.5f, -818f, 0.2617989f, "m", "Center")
                }, {
                    58,
                    new Location(-595.5f, 27.5f, -839.75f, -1.082104f, "l", "Left Side")
                }, {
                    59,
                    new Location(-549.5f, 34.5f, -809f, 0.8377581f, "s", "Center")
                }, {
                    60,
                    new Location(-554f, 56f, -754.5f, 1.570796f, "m", "Center")
                }
            }
        }, {
            "s1h1",
            new Dictionary<int, Location> {
                {
                    1,
                    new Location(-72f, 40f, -108f, -1.570451f, "m", "Center")
                }, {
                    2,
                    new Location(-144f, 36f, -100f, -0.7853982f, "l", "Center")
                }, {
                    3,
                    new Location(-96f, 32f, -60f, 3.141593f, "s", "Center")
                }, {
                    4,
                    new Location(-92f, 18f, -12f, -1.570451f, "m", "Right")
                }, {
                    5,
                    new Location(-148f, 20f, 28f, -2.094395f, "l", "Center")
                }, {
                    6,
                    new Location(-36f, 14f, -4f, 0.7853982f, "m", "Center")
                }, {
                    7,
                    new Location(-40f, 24f, -56f, 0.0f, "m", "Center")
                }, {
                    8,
                    new Location(-12f, 36f, -92f, 1.570451f, "s", "Right")
                }, {
                    9,
                    new Location(40f, 38f, -100f, 0.0f, "s", "Right Side")
                }, {
                    10,
                    new Location(64f, 42f, -124f, 0.0f, "s", "Right")
                }, {
                    11,
                    new Location(4f, 25f, -56f, 0.0f, "s", "Right Side")
                }, {
                    12,
                    new Location(48f, 26f, -60f, 0.0f, "s", "Center")
                }, {
                    13,
                    new Location(64f, 18f, -32f, 0.0f, "s", "Right")
                }, {
                    14,
                    new Location(83.628f, 29f, -63f, -0.7853982f, "m", "Far Right")
                }, {
                    15,
                    new Location(148f, 46f, -104f, 0.7853982f, "l", "Center")
                }, {
                    16,
                    new Location(136f, 28f, -34f, 1.570451f, "s", "Left")
                }, {
                    17,
                    new Location(112f, 22f, -20f, 1.570451f, "s", "Left")
                }, {
                    18,
                    new Location(72f, 17f, 4f, 0.0f, "s", "Far Right Side")
                }, {
                    19,
                    new Location(42f, 8f, 24f, 0.3490659f, "s", "Center")
                }, {
                    20,
                    new Location(66f, 6f, 36f, 0.5235988f, "s", "Center")
                }, {
                    21,
                    new Location(88f, 5f, 52f, 0.6981317f, "s", "Center")
                }, {
                    22,
                    new Location(120f, 18f, 56f, 0.0f, "s", "Right Side")
                }, {
                    23,
                    new Location(112f, 18f, 28f, 0.0f, "s", "Right Side")
                }, {
                    24,
                    new Location(150f, 22f, -6f, 0.0f, "s", "Center")
                }, {
                    25,
                    new Location(162f, 32f, 24f, 0.0f, "s", "Right Side")
                }, {
                    26,
                    new Location(168f, 28f, 56f, 1.570451f, "s", "Center")
                }, {
                    27,
                    new Location(186f, 36f, -6f, 0.0f, "s", "Center")
                }, {
                    28,
                    new Location(196f, 40f, 24f, 0.0f, "s", "Right")
                }, {
                    29,
                    new Location(196f, 38f, 64f, 1.570451f, "m", "Far Right")
                }, {
                    30,
                    new Location(140f, 20f, 100f, 2.617994f, "m", "Center")
                }, {
                    31,
                    new Location(-596f, 40f, -776f, 0.0003454686f, "m", "Center")
                }, {
                    32,
                    new Location(-604f, 36f, -848f, 0.7853982f, "l", "Center")
                }, {
                    33,
                    new Location(-644f, 32f, -800f, -1.570796f, "s", "Center")
                }, {
                    34,
                    new Location(-692f, 18f, -796f, 0.0003454686f, "m", "Right")
                }, {
                    35,
                    new Location(-732f, 20f, -852f, -0.523599f, "l", "Center")
                }, {
                    36,
                    new Location(-700f, 14f, -740f, 2.356195f, "m", "Center")
                }, {
                    37,
                    new Location(-648f, 24f, -744f, 1.570796f, "m", "Center")
                }, {
                    38,
                    new Location(-612f, 36f, -716f, 3.141247f, "s", "Right")
                }, {
                    39,
                    new Location(-604f, 38f, -664f, 1.570796f, "s", "Right Side")
                }, {
                    40,
                    new Location(-580f, 42f, -640f, 1.570796f, "s", "Right")
                }, {
                    41,
                    new Location(-648f, 25f, -700f, 1.570796f, "s", "Right Side")
                }, {
                    42,
                    new Location(-644f, 26f, -656f, 1.570796f, "s", "Center")
                }, {
                    43,
                    new Location(-672f, 18f, -640f, 1.570796f, "s", "Right")
                }, {
                    44,
                    new Location(-641f, 29f, -620.372f, 0.7853982f, "m", "Far Right")
                }, {
                    45,
                    new Location(-600f, 46f, -556f, 2.356195f, "l", "Center")
                }, {
                    46,
                    new Location(-670f, 28f, -568f, 3.141247f, "s", "Left")
                }, {
                    47,
                    new Location(-684f, 22f, -592f, 3.141247f, "s", "Left")
                }, {
                    48,
                    new Location(-708f, 17f, -632f, 1.570796f, "s", "Far Right Side")
                }, {
                    49,
                    new Location(-728f, 8f, -662f, 1.919862f, "s", "Center")
                }, {
                    50,
                    new Location(-740f, 6f, -638f, 2.094395f, "s", "Center")
                }, {
                    51,
                    new Location(-756f, 5f, -616f, 2.268928f, "s", "Center")
                }, {
                    52,
                    new Location(-760f, 18f, -584f, 1.570796f, "s", "Right Side")
                }, {
                    53,
                    new Location(-732f, 18f, -592f, 1.570796f, "s", "Right Side")
                }, {
                    54,
                    new Location(-698f, 22f, -554f, 1.570796f, "s", "Center")
                }, {
                    55,
                    new Location(-728f, 32f, -542f, 1.570796f, "s", "Right Side")
                }, {
                    56,
                    new Location(-760f, 28f, -536f, 3.141247f, "s", "Center")
                }, {
                    57,
                    new Location(-698f, 36f, -518f, 1.570796f, "s", "Center")
                }, {
                    58,
                    new Location(-728f, 40f, -508f, 1.570796f, "s", "Right")
                }, {
                    59,
                    new Location(-768f, 38f, -508f, 3.141247f, "m", "Far Right")
                }, {
                    60,
                    new Location(-804f, 20f, -564f, -2.094395f, "m", "Center")
                }
            }
        }, {
            "w1h1",
            new Dictionary<int, Location> {
                {
                    1,
                    new Location(-38.00001f, -4f, -110f, 41f * (float)Math.PI / 45f, "s", "Right")
                }, {
                    2,
                    new Location(-64.00002f, 4f, -98.00002f, -0.6108653f, "s", "Right")
                }, {
                    3,
                    new Location(-85.64f, 6f, -80.02f, -0.8834857f, "s", "Right")
                }, {
                    4,
                    new Location(-82.203f, -4f, -121.07f, 2.548181f, "m", "Left")
                }, {
                    5,
                    new Location(-118.665f, 12f, -92.44f, 0.5962044f, "l", "Right Side")
                }, {
                    6,
                    new Location(-39.73f, 0.0f, -64.44f, 0.7853982f, "m", "Right")
                }, {
                    7,
                    new Location(-72.6f, 8f, -40.315f, 2.059489f, "s", "Right")
                }, {
                    8,
                    new Location(-117.81f, 10f, -23.37f, -1.343904f, "m", "Center")
                }, {
                    9,
                    new Location(-140.246f, 12f, -51.004f, 2.007129f, "s", "Right")
                }, {
                    10,
                    new Location(-147.905f, 15f, -24.543f, 0.1745329f, "s", "Right")
                }, {
                    11,
                    new Location(-158.78f, 15f, 18.875f, -1.833119f, "m", "Right")
                }, {
                    12,
                    new Location(-116f, 15f, 23.99996f, 1.308997f, "m", "Right")
                }, {
                    13,
                    new Location(-72f, 11.618f, 12f, -0.2391101f, "l", "Far Left Side")
                }, {
                    14,
                    new Location(-32.00002f, -20f, 17.99998f, 3.141593f, "s", "Left")
                }, {
                    15,
                    new Location(-14.018f, -16.02f, -36.035f, -0.7853982f, "s", "Right")
                }, {
                    16,
                    new Location(68.24998f, -16f, -39.56697f, -0.5235988f, "s", "Right")
                }, {
                    17,
                    new Location(67f, -20f, 0.2499695f, 3.141593f, "s", "Right")
                }, {
                    18,
                    new Location(73.188f, -24f, 28.851f, 2.094395f, "s", "Right")
                }, {
                    19,
                    new Location(44f, -24f, 53.81f, 2.094395f, "m", "Right")
                }, {
                    20,
                    new Location(0.0f, -24f, 79.99998f, 3.141593f, "s", "Right")
                }, {
                    21,
                    new Location(93.99998f, -26f, -70.00002f, 2.356194f, "s", "Right")
                }, {
                    22,
                    new Location(106f, -14f, -36.00003f, -0.3490659f, "s", "Left")
                }, {
                    23,
                    new Location(130.465f, -18f, -56.628f, 1.221731f, "s", "Right")
                }, {
                    24,
                    new Location(144.5f, -26f, -32.00003f, 0.0f, "s", "Right")
                }, {
                    25,
                    new Location(123.75f, -28f, 4f, -1.570796f, "m", "Right")
                }, {
                    26,
                    new Location(120f, -28f, 39.99997f, -1.570796f, "s", "Right")
                }, {
                    27,
                    new Location(130f, -36f, 71.99997f, 3.141593f, "s", "Right")
                }, {
                    28,
                    new Location(95.545f, -33.68f, 59.08f, 0.5235988f, "s", "Right")
                }, {
                    29,
                    new Location(184f, -44f, 23.49998f, 1.570796f, "s", "Right")
                }, {
                    30,
                    new Location(188f, -40f, 60f, 3.141593f, "l", "Far Right Side")
                }, {
                    31,
                    new Location(-594f, -4f, -742f, -1.850049f, "s", "Right")
                }, {
                    32,
                    new Location(-606f, 4f, -768f, 0.9599311f, "s", "Right")
                }, {
                    33,
                    new Location(-623.98f, 6f, -789.64f, 0.6873107f, "s", "Right")
                }, {
                    34,
                    new Location(-582.93f, -4f, -786.203f, -2.164208f, "m", "Left")
                }, {
                    35,
                    new Location(-611.56f, 12f, -822.665f, 2.167001f, "l", "Right Side")
                }, {
                    36,
                    new Location(-639.56f, 0.0f, -743.73f, 2.356195f, "m", "Right")
                }, {
                    37,
                    new Location(-663.685f, 8f, -776.6f, -2.6529f, "s", "Right")
                }, {
                    38,
                    new Location(-680.63f, 10f, -821.81f, 0.2268923f, "m", "Center")
                }, {
                    39,
                    new Location(-652.996f, 12f, -844.246f, -2.70526f, "s", "Right")
                }, {
                    40,
                    new Location(-679.457f, 15f, -851.905f, 1.745329f, "s", "Right")
                }, {
                    41,
                    new Location(-722.875f, 15f, -862.78f, -0.262323f, "m", "Right")
                }, {
                    42,
                    new Location(-727.9999f, 15f, -820f, 2.879793f, "m", "Right")
                }, {
                    43,
                    new Location(-716f, 11.618f, -776f, 1.331686f, "l", "Far Left Side")
                }, {
                    44,
                    new Location(-722f, -20f, -736f, -1.570796f, "s", "Left")
                }, {
                    45,
                    new Location(-667.965f, -16.02f, -718.018f, 0.7853982f, "s", "Right")
                }, {
                    46,
                    new Location(-664.433f, -16f, -635.75f, 1.047198f, "s", "Right")
                }, {
                    47,
                    new Location(-704.25f, -20f, -637f, -1.570796f, "s", "Right")
                }, {
                    48,
                    new Location(-732.851f, -24f, -630.812f, -2.617994f, "s", "Right")
                }, {
                    49,
                    new Location(-757.81f, -24f, -660f, -2.617994f, "m", "Right")
                }, {
                    50,
                    new Location(-784f, -24f, -704f, -1.570796f, "s", "Right")
                }, {
                    51,
                    new Location(-634f, -26f, -610f, -2.356195f, "s", "Right")
                }, {
                    52,
                    new Location(-668f, -14f, -598f, 1.22173f, "s", "Left")
                }, {
                    53,
                    new Location(-647.372f, -18f, -573.535f, 2.792527f, "s", "Right")
                }, {
                    54,
                    new Location(-672f, -26f, -559.5f, 1.570796f, "s", "Right")
                }, {
                    55,
                    new Location(-708f, -28f, -580.25f, 0.0f, "m", "Right")
                }, {
                    56,
                    new Location(-744f, -28f, -584f, 0.0f, "s", "Right")
                }, {
                    57,
                    new Location(-776f, -36f, -574f, -1.570796f, "s", "Right")
                }, {
                    58,
                    new Location(-763.08f, -33.68f, -608.455f, 2.094395f, "s", "Right")
                }, {
                    59,
                    new Location(-727.5f, -44f, -520f, -3.141593f, "s", "Right")
                }, {
                    60,
                    new Location(-764f, -40f, -516f, -1.570796f, "l", "Far Right Side")
                }
            }
        }
    };
}
