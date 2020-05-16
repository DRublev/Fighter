using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.Test
{
    public static class TestData
    {
        /*
            [
                [{"X":176,"Y":192},{"X":24,"Y":8}],
                [{"X":144,"Y":200},{"X":144,"Y":200}],
                [{"X":144,"Y":200},{"X":144,"Y":200}],
                [{"X":144,"Y":200},{"X":144,"Y":200}],
                [{"X":144,"Y":200},{"X":144,"Y":200}]
            ]
        */
        public static readonly List<Vector2[]> bonesCoordsFormServer = new List<Vector2[]>()
        {
            new Vector2[2] { new Vector2(176, 192), new Vector2(24, 8) },
            new Vector2[2] { new Vector2(144, 200), new Vector2(144, 200) },
            new Vector2[2] { new Vector2(144, 200), new Vector2(144, 200) },
            new Vector2[2] { new Vector2(144, 200), new Vector2(144, 200) },
            new Vector2[2] { new Vector2(144, 200), new Vector2(144, 200) },
        };
    }
}
