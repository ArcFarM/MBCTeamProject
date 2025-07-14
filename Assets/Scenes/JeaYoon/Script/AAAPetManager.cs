using System.Collections.Generic;
using UnityEngine;

/* [0] 개요 : AAAPetManager
		- 펫 매니저 (기본 구조)
*/

namespace JeaYoon
{
	public class AAAPetManager : MonoBehaviour
	{

        // [1] Variable.
        #region ▼▼▼▼▼ Variable ▼▼▼▼▼
        // [◆] - ▶▶▶ List.
        public List<GameObject> currentPets = new List<GameObject>();
        #endregion ▲▲▲▲▲ Variable ▲▲▲▲▲





        // [2] Custom Method.
        #region ▼▼▼▼▼ Custom Method ▼▼▼▼▼
        // [◆] - ▶▶▶ AddPet.
        public void AddPet(GameObject pet)
        {
            currentPets.Add(pet);
            // 펫 활성화 및 플레이어 따라다니기 설정
            pet.SetActive(true);

            // 펫 위치 설정 (플레이어 주변)
            Vector3 petPosition = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            pet.transform.position = petPosition;
        }
        #endregion ▲▲▲▲▲ Custom Method ▲▲▲▲▲
    }
}