using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MainGame.Units;
namespace JiHoon
{
    public class UnitPlacementManager : MonoBehaviour
    {
        // 여기에 인스펙터에서 할당하세요
        [Header("Context Menu Prefab")]
        public GameObject contextMenuPrefab;    // 유닛 컨텍스트 메뉴 프리팹
        private UnitContextMenu _menuInstance;  // 유닛 컨텍스트 메뉴 인스턴스 (한 번만 생성됨)
        [HideInInspector]
        public bool placementEnabled = false;   // !! 이게 true일 때만 재배치/설치 모드 허용

        [Header("References")]
        public UnitSpawner spawner;      // UnitSpawner 오브젝트
        public GridManager gridManager;  // GridManager 오브젝트

        private int selectedPreset = -1; // 선택된 유닛 인덱스
        private UnitCardUI selectedCardUI; // 선택된 카드 UI

        // 상점 아이템 처리용 추가 변수
        private bool isUsingShopItem = false;
        private ItemData selectedShopItem = null;

        //이동용 필드
        private GameObject movingUnit = null; // 현재 이동 중인 유닛
        private Vector3Int originalCell; // 이동 시작 셀
        private HashSet<Vector3Int> originalOccupiedCells;  // 이동 시작 시점의 점유 셀 목록

        [Header("Raycast LayerMasks")]
        [SerializeField] LayerMask unitLayerMask;   // Unit 레이어만 검사
        [SerializeField] LayerMask roadLayerMask;   // Road 레이어만 검사 (선택)

        // 싱글턴 프로퍼티
        public static UnitPlacementManager Instance { get; private set; }

        void Awake()
        {
            // ② 중복 방지 & 인스턴스 저장
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // 메뉴 인스턴스 반환 (한 번만 생성)
        public UnitContextMenu Menu
        {
            get
            {
                if (_menuInstance == null && contextMenuPrefab != null)
                {
                    var go = Instantiate(contextMenuPrefab,
                                         FindObjectOfType<Canvas>().transform,
                                         false);
                    _menuInstance = go.GetComponent<UnitContextMenu>();
                }
                return _menuInstance;
            }
        }

        // UI 버튼에서 호출 - 기존 preset 방식과 상점 아이템 방식 모두 처리
        public void OnClickSelectUmit(UnitCardUI card)
        {
            if (!placementEnabled)
            {
                Debug.Log("카드 설치모드가 아닙니다");
                return;
            }

            // 상점에서 구매한 카드인지 확인
            if (card.isFromShop)
            {
                // 상점 아이템 방식
                selectedPreset = -1; // preset 사용 안함
                selectedShopItem = card.shopItemData;
                isUsingShopItem = true;
                selectedCardUI = card;

                Debug.Log($"상점 아이템 {selectedShopItem.itemName} 설치 모드 활성화");
            }
            else
            {
                // 기존 preset 방식
                selectedPreset = card.presetIndex;
                selectedShopItem = null;
                isUsingShopItem = false;
                selectedCardUI = card;

                Debug.Log($"프리셋 유닛 설치 모드 활성화");
            }

            // 베이지 길 셀만 파란색으로 하이라이트
            gridManager.HighlightAllowedCells();
        }

        // 카드에서만 호출되던 Init 대신, 직접 이동 시작용 메서드 추가
        public void BeginMove(GameObject unit)
        {
            if (!placementEnabled)
            {
                Debug.Log("재배치 모드가 아닙니다.");
                return;
            }
            movingUnit = unit;

            // 1) gridManager 안의 자료구조와 분리된 복사본을 만들자
            var cells = gridManager.GetOccupiedCellsFor(unit);
            originalOccupiedCells = new HashSet<Vector3Int>(cells);

            // 2) 그 셀들에서 점유 해제
            gridManager.FreeCells(originalOccupiedCells);

            // 원위치 복귀용 셀도 저장
            originalCell = gridManager.WorldToCell(unit.transform.position);

            // 3) 하이라이트
            gridManager.HighlightAllowedCells();
        }

        void Update()
        {
            // 이동/설치 모드 모두 placementEnabled가 false면 리턴
            if (!placementEnabled) return;

            if (Input.GetMouseButtonDown(0)
            && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 ws = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                ws.z = 0;
                // Unit 클릭만 잡도록 레이어 마스크 전달
                var hit = Physics2D.Raycast(ws, Vector2.zero, Mathf.Infinity, unitLayerMask);
                bool clickedOnUnit = hit.collider != null
                                      && hit.collider.GetComponent<ClickableUnit>() != null;
                if (!clickedOnUnit)
                    Menu?.Hide();
            }

            // 이동모드
            if (movingUnit != null)
            {
                // 1-1) 마우스→월드→셀
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;
                Vector3Int cell = gridManager.WorldToCell(worldPos);

                // 1-2) 프리뷰 초기화
                gridManager.ClearPreview();

                // 1-3) 해당 셀이 도로이고 다른 유닛 점유 중이 아니어야 함
                bool canMove = gridManager.IsRoadCell(cell)
                            && !gridManager.GetAllOccupiedCells().Contains(cell);

                if (canMove)
                {
                    // 빨간색 프리뷰 타일
                    gridManager.previewTilemap.SetTile(cell, gridManager.placementPreviewTile);
                }

                // 1-4) 클릭 확정 → 실제 이동
                if (canMove
                    && Input.GetMouseButtonDown(0)
                    && !EventSystem.current.IsPointerOverGameObject())
                {
                    // 이동
                    movingUnit.transform.position = gridManager.CellToWorldCenter(cell);

                    // 점유 정보 재등록
                    var newCells = gridManager.GetCellsFor(movingUnit, cell);
                    gridManager.OccupyCells(newCells, movingUnit);

                    // 모드 종료
                    movingUnit = null;
                    originalOccupiedCells.Clear(); // 원래 점유 셀 목록 초기화
                    gridManager.ClearAllHighlights();

                    return; // 이동 후 더 이상 처리할 필요 없음
                }

                // 1-5) ESC 취소 → 원위치 복귀 + 점유 복원
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    movingUnit.transform.position = gridManager.CellToWorldCenter(originalCell);
                    gridManager.OccupyCells(originalOccupiedCells, movingUnit);

                    movingUnit = null;
                    gridManager.ClearAllHighlights();
                }

                // 이동 모드가 활성화된 동안엔 설치 모드 건너뛰기
                return;
            }

            // ────────────────
            // 2) 설치(카드) 모드
            // ────────────────
            if (selectedPreset < 0 && !isUsingShopItem)
                return;

            // 2-1) 마우스→월드→셀
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            wp.z = 0;
            Vector3Int baseCell = gridManager.WorldToCell(wp);

            // 2-2) footprint 크기 계산
            GameObject prefabObj = null;
            if (isUsingShopItem)
            {
                // 상점 아이템 사용
                prefabObj = selectedShopItem.unitPrefab;
            }
            else
            {
                // 기존 preset 사용
                prefabObj = spawner.unitPresets[selectedPreset].prefab;
            }

            var unitBase = prefabObj.GetComponent<MainGame.Units.UnitBase>();
            int w = unitBase.GetBaseRawSize;
            int h = unitBase.GetBaseColSize;

            var footprint = new List<Vector3Int>(w * h);
            for (int dx = 0; dx < w; dx++)
                for (int dy = 0; dy < h; dy++)
                    footprint.Add(new Vector3Int(baseCell.x + dx,
                                                 baseCell.y + dy,
                                                 baseCell.z));

            // 2-3) 미리보기 (빨간)
            gridManager.ClearPreview();
            bool canPlace = true;
            foreach (var cell in footprint)
            {
                if (!gridManager.IsRoadCell(cell) || gridManager.GetAllOccupiedCells().Contains(cell))
                {
                    canPlace = false;
                    break;
                }
            }
            if (canPlace)
                foreach (var cell in footprint)
                    gridManager.previewTilemap.SetTile(cell, gridManager.placementPreviewTile);

            // 2-4) 설치 확정
            if (canPlace
                && Input.GetMouseButtonDown(0)
                && !EventSystem.current.IsPointerOverGameObject())
            {
                // 중앙 좌표 계산
                Vector3 sum = Vector3.zero;
                foreach (var cell in footprint)
                    sum += gridManager.CellToWorldCenter(cell);
                Vector3 spawnPos = sum / footprint.Count;
                GameObject spawnedUnit = null;

                if (isUsingShopItem)
                {
                    // 상점 아이템 직접 스폰
                    spawnedUnit = Instantiate(selectedShopItem.unitPrefab, spawnPos, Quaternion.identity);
                    Debug.Log($"상점 아이템 {selectedShopItem.itemName} 스폰 완료");
                }
                else
                {
                    // 기존 preset 방식 스폰
                    spawner.SpawnAtPosition(selectedPreset, spawnPos);
                    spawnedUnit = spawner.GetLastSpawnedUnit();
                }

                // GridManager에 점유 정보 등록
                if (spawnedUnit != null)
                {
                    gridManager.OccupyCells(new HashSet<Vector3Int>(footprint), spawnedUnit);
                }

                if (selectedCardUI != null)
                {
                    var cardDeck = FindFirstObjectByType<SimpleCardDeck>();
                    if (cardDeck != null)
                    {
                        // 하스스톤 덱에서만 제거
                        cardDeck.OnCardPlaced();
                    }
                    else
                    {
                        // 기존 방식은 직접 제거
                        Destroy(selectedCardUI.gameObject);
                    }
                    selectedCardUI = null;
                }

                // 카드덱에 배치 완료 알림 (추가)
                var card_Deck = FindFirstObjectByType<SimpleCardDeck>();
                if (card_Deck != null)
                {
                    card_Deck.OnCardPlaced();
                }

                // 모드 종료
                selectedPreset = -1;
                isUsingShopItem = false;
                selectedShopItem = null;
                gridManager.ClearAllHighlights();
            }

            // 2-5) ESC 취소
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectedPreset = -1;
                isUsingShopItem = false;
                selectedShopItem = null;
                gridManager.ClearAllHighlights();
            }
        }

        public void SellUnit(GameObject unit)
        {
            // 셀 점유 해제
            var cells = gridManager.GetOccupiedCellsFor(unit);
            gridManager.FreeCells(cells);

            //TODO : 유닛 판매 로직 구현
            Destroy(unit);
        }
        public void RequestPlaceUnit(GameObject prefab)
        {
            // 1) 마우스 위치 → 월드 좌표
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;

            // 2) Instantiate GameObject
            var go = Instantiate(prefab, worldPos, Quaternion.identity);

            // 3) GridManager에 점유 정보 등록하려면 UnitBase 컴포넌트를 뽑아와서 footprint 계산
            var ub = go.GetComponent<UnitBase>();
            // footprint 계산 로직 호출… (기존 코드 활용)

            Debug.Log($"상점 유닛 {prefab.name} 스폰 완료");
        }

    }
}