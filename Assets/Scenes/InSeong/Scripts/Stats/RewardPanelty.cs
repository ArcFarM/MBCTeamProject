using UnityEngine;
using System.Collections.Generic;
using MainGame.Enum;

namespace MainGame.Units {
    public class RewardPenalty : MonoBehaviour {
        //StatManager에서 리스트를 받아와서 해당 리스트별로 id와 패널티를 계산하여 적용하고 다시 넘겨준다
        public static List<StatStruct> CalcPenalty(List<StatStruct> input) {
             Dictionary<Stats, int> dict = new Dictionary<Stats, int> {
                { Stats.Unrest, 0 },
                { Stats.Money, 0 },
                { Stats.Dominance, 0 },
                { Stats.Manpower, 0 }
            };

            //StatManager에서 받아온 리스트를 하나로 통합
            foreach(var ss in input) {
                if (dict.ContainsKey(ss.stat)) {
                    dict[ss.stat] += ss.value;
                }
            }

            //빠른 계산을 위해 Dictionary로 선언한 리스트를 다시 리스트로 변환
            List<StatStruct> result = new();
            foreach (var keyValue in dict) {
                result.Add(new StatStruct { stat = keyValue.Key, value = keyValue.Value });
            }

            return result;
        }
    }


}
