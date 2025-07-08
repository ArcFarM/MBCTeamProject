using System.Collections.Generic;
using UnityEngine;

namespace JiHoon
{
    public class EnemyGroup : MonoBehaviour
    {
        private List<EnemyMovement> members = new List<EnemyMovement>();
        private EnemyMovement leader;

        public void SetLeader(EnemyMovement leaderEnemy)
        {
            leader = leaderEnemy;
            if (!members.Contains(leaderEnemy))
            {
                members.Add(leaderEnemy);
            }
        }

        public void AddMember(EnemyMovement enemy)
        {
            if (!members.Contains(enemy))
            {
                members.Add(enemy);
                enemy.SetGroup(this, leader);
            }
        }

        public void RemoveMember(EnemyMovement enemy)
        {
            members.Remove(enemy);

            // 모든 멤버가 제거되면 그룹 삭제
            if (members.Count == 0)
            {
                Destroy(gameObject);
            }
        }

        public List<EnemyMovement> GetMembers()
        {
            // null 체크 및 정리
            members.RemoveAll(m => m == null);
            return members;
        }

        void Update()
        {
            // 죽은 멤버 자동 정리
            members.RemoveAll(m => m == null);
        }
    }
}