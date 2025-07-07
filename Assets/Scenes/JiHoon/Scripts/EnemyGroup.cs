using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace JiHoon
{
    public class EnemyGroup : MonoBehaviour
    {
        [Header("그룹 설정")]
        public int groupId;
        public List<EnemyMovement> members = new List<EnemyMovement>();
        public EnemyMovement leader;

        [Header("대형 설정")]
        public float formationSpacing = 1.5f;
        public FormationType formation = FormationType.VFormation;

        public enum FormationType
        {
            Line,           // 일렬
            VFormation,     // V자 대형
            Box,            // 사각형
            Circle          // 원형
        }

        void Update()
        {
            // 리더가 없거나 죽었으면 새로운 리더 선정
            if (leader == null && members.Count > 0)
            {
                AssignNewLeader();
            }

            // 죽은 멤버들 제거
            members.RemoveAll(m => m == null);

            // 그룹이 비어있으면 삭제
            if (members.Count == 0)
            {
                Destroy(gameObject);
            }
        }

        public void AddMember(EnemyMovement enemy)
        {
            if (!members.Contains(enemy))
            {
                members.Add(enemy);
                enemy.SetGroup(this);

                // 첫 번째 멤버를 리더로 설정
                if (leader == null)
                {
                    SetLeader(enemy);
                }
                else
                {
                    // 팔로워로 설정
                    enemy.SetAsFollower();
                }

                // 대형 위치 업데이트
                UpdateFormation();
            }
        }

        public void RemoveMember(EnemyMovement enemy)
        {
            if (members.Contains(enemy))
            {
                members.Remove(enemy);

                // 리더가 죽었으면 새로운 리더 선정
                if (leader == enemy)
                {
                    leader = null;
                    if (members.Count > 0)
                    {
                        AssignNewLeader();
                    }
                }

                UpdateFormation();
            }
        }

        void SetLeader(EnemyMovement newLeader)
        {
            leader = newLeader;
            newLeader.SetAsLeader();
        }

        void AssignNewLeader()
        {
            if (members.Count > 0)
            {
                // 가장 앞에 있는 적을 새 리더로 선정
                var newLeader = members.OrderBy(m => m.currentTargetIndex).First();
                SetLeader(newLeader);
            }
        }

        void UpdateFormation()
        {
            if (leader == null) return;

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i] != leader)
                {
                    Vector3 formationPos = CalculateFormationPosition(i);
                    members[i].SetFormationOffset(formationPos);
                }
            }
        }

        public Vector3 CalculateFormationPosition(int index)
        {
            // 리더를 제외한 인덱스 계산
            int formationIndex = index;
            if (members[index] == leader)
                return Vector3.zero;

            // 리더보다 앞에 있는 멤버들의 인덱스 조정
            for (int i = 0; i < index; i++)
            {
                if (members[i] == leader)
                {
                    formationIndex--;
                    break;
                }
            }

            switch (formation)
            {
                case FormationType.Line:
                    return new Vector3((formationIndex - (members.Count - 1) / 2f) * formationSpacing, 0, -0.5f);

                case FormationType.VFormation:
                    float side = (formationIndex % 2 == 0) ? 1 : -1;
                    float row = (formationIndex + 1) / 2;
                    return new Vector3(side * row * formationSpacing, 0, -row * formationSpacing * 0.5f);

                case FormationType.Box:
                    int cols = Mathf.CeilToInt(Mathf.Sqrt(members.Count - 1));
                    int x = formationIndex % cols;
                    int z = formationIndex / cols;
                    return new Vector3((x - cols / 2f) * formationSpacing, 0, -(z + 1) * formationSpacing);

                case FormationType.Circle:
                    float angle = formationIndex * 2 * Mathf.PI / (members.Count - 1);
                    return new Vector3(Mathf.Cos(angle) * formationSpacing, 0, Mathf.Sin(angle) * formationSpacing);

                default:
                    return Vector3.zero;
            }
        }

        public Vector3 GetLeaderPosition()
        {
            return leader != null ? leader.transform.position : transform.position;
        }

        public void SetFormation(FormationType formationType, float spacing)
        {
            this.formation = formationType;
            this.formationSpacing = spacing;
            UpdateFormation();  // 멤버 배치를 바로 갱신
        }
    }
}