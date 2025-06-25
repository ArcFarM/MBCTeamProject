using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.U2D.IK;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 180f;
    public float pathSmoothness = 0.7f;
    public float nodeReachDistance = 0.3f;

    [Header("Path Settings")]
    public Transform startPoint;
    public Transform endPoint;
    public float gridSize = 1f;

    [Header("Optional: 중간 경유지점")]
    public Transform[] viaPoints;   // Inspector에서 배열로 노출

    [Header("Tilemap Settings")]
    public Tilemap roadTilemap; // 도로 타일맵 참조
    public TileBase[] walkableTiles; // 걸을 수 있는 타일들 (RuleTile 포함)

    [Header("Alternative: Single Road Tile")]
    public TileBase roadTile; // 단일 도로 타일 (RuleTile 등)

    [Header("Debug")]
    public bool showPath = true;
    public Color pathColor = Color.green;
    public Color smoothPathColor = Color.blue;

    private List<Vector2> originalPath; // Vector3 -> Vector2로 변경
    private List<Vector2> smoothPath;   // Vector3 -> Vector2로 변경
    private int currentTargetIndex = 0;
    public bool isMoving = false;

    void Start()
    {
        // 타일맵 자동 감지
        if (roadTilemap == null)
        {
            roadTilemap = GameObject.FindWithTag("RoadTilemap")?.GetComponent<Tilemap>();
            if (roadTilemap == null)
                roadTilemap = FindObjectOfType<Tilemap>();
        }

        // Start/End Point 자동 찾기
        if (startPoint == null)
        {
            GameObject startObj = GameObject.FindWithTag("StartPoint");
            if (startObj != null)
                startPoint = startObj.transform;
        }

        if (endPoint == null)
        {
            GameObject endObj = GameObject.FindWithTag("EndPoint");
            if (endObj != null)
                endPoint = endObj.transform;
        }

        InitializeMovement();
    }

    void Update()
    {
        if (isMoving && smoothPath != null && smoothPath.Count > 0)
        {
            MoveAlongPath();
        }
    }

    public float waypointSpacing = 0.5f;  // Inspector에서 조절 가능한 필드 추가

    public void InitializeMovement()
    {
        // 1) 시작점/도착점을 2D 좌표로 변환
        Vector2 start2D = new Vector2(startPoint.position.x, startPoint.position.y);
        Vector2 end2D = new Vector2(endPoint.position.x, endPoint.position.y);

        // 2) 원본 경로 세팅: viaPoints 유무에 따라 분기
        if (viaPoints != null && viaPoints.Length > 0)
        {
            // 여러 구간(Path) 이어 붙이기
            var fullPath = new List<Vector2>();
            Vector2 prev = start2D;

            foreach (var via in viaPoints)
            {
                if (via == null) continue;
                Vector2 via2D = new Vector2(via.position.x, via.position.y);

                // prev → via
                var segment = FindPath(prev, via2D);
                if (segment != null && segment.Count > 1)
                {
                    fullPath.AddRange(segment.Skip(1));
                    prev = via2D;
                }
            }

            // 마지막 via → end
            var lastSeg = FindPath(prev, end2D);
            if (lastSeg != null && lastSeg.Count > 1)
                fullPath.AddRange(lastSeg.Skip(1));

            originalPath = fullPath;
        }
        else
        {
            // viaPoints 없으면 start → end 단일 경로
            originalPath = FindPath(start2D, end2D);
        }

        // 3) 이후 기존 로직 (스무딩, 분할, 필터 등) 계속…
        if (originalPath != null && originalPath.Count > 1)
        {
            // 베지어 스무딩
            var bezierPath = SmoothPath(originalPath);

            // 균등 분할 → 웨이포인트 생성
            var rawWaypoints = GenerateWaypoints(bezierPath, waypointSpacing);

            // 역방향 점 제거
            var filtered = new List<Vector2>();
            float lastDist = Vector2.Distance(rawWaypoints[0], end2D);
            filtered.Add(rawWaypoints[0]);
            for (int i = 1; i < rawWaypoints.Count; i++)
            {
                float d = Vector2.Distance(rawWaypoints[i], end2D);
                if (d <= lastDist)
                {
                    filtered.Add(rawWaypoints[i]);
                    lastDist = d;
                }
            }
            smoothPath = filtered;

            // 이동 시작 세팅
            transform.position = new Vector3(smoothPath[0].x, smoothPath[0].y, transform.position.z);
            currentTargetIndex = 1;
            isMoving = true;
        }
        else
        {
            Debug.LogError("No path found!");
        }
    }

    /// <summary>
    /// 원본 path를 segmentLength 단위로 쪼개
    /// 웨이포인트를 촘촘하게 생성합니다.
    /// </summary>
    List<Vector2> GenerateWaypoints(List<Vector2> path, float segmentLength)
    {
        var waypoints = new List<Vector2>();
        if (path == null || path.Count < 2) return waypoints;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 a = path[i];
            Vector2 b = path[i + 1];
            float dist = Vector2.Distance(a, b);
            // 최소 1개 분할, 거리/단위로 몇 개 분할할지 계산
            int segments = Mathf.Max(1, Mathf.CeilToInt(dist / segmentLength));

            for (int j = 0; j < segments; j++)
            {
                float t = (float)j / segments;
                Vector2 pt = Vector2.Lerp(a, b, t);
                waypoints.Add(pt);
            }
        }
        // 마지막 도착점 추가
        waypoints.Add(path[path.Count - 1]);
        return waypoints;
    }

    void MoveAlongPath()
    {
        if (currentTargetIndex >= smoothPath.Count)
        {
            // 목적지 도착
            isMoving = false;
            OnReachedDestination();
            return;
        }

        Vector2 targetPosition2D = smoothPath[currentTargetIndex];
        Vector3 targetPosition = new Vector3(targetPosition2D.x, targetPosition2D.y, transform.position.z);
        Vector2 direction = (targetPosition2D - (Vector2)transform.position).normalized;

        // 가는 도중 회전 구현하려면 여기서 회전 로직 추가

        // 이동 (2D)
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // 다음 포인트로 이동할지 체크
        if (Vector2.Distance(transform.position, targetPosition2D) < nodeReachDistance)
        {
            currentTargetIndex++;
        }
    }

    List<Vector2> FindPath(Vector2 start, Vector2 end)
    {
        // 간단한 A* 알고리즘 구현 (2D용)
        List<Node2D> openList = new List<Node2D>();
        List<Node2D> closedList = new List<Node2D>();

        // 안전장치 추가
        int maxIterations = 1000;
        int currentIteration = 0;

        Node2D startNode = new Node2D(start);
        Node2D endNode = new Node2D(end);

        openList.Add(startNode);

        while (openList.Count > 0 && currentIteration < maxIterations)
        {
            currentIteration++;

            Node2D currentNode = openList.OrderBy(n => n.fCost).First();
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // 목적지 도달 체크
            if (Vector2.Distance(currentNode.position, endNode.position) < gridSize)
            {
                return RetracePath(startNode, currentNode);
            }

            // 인접 노드들 체크
            foreach (Node2D neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Any(n => Vector2.Distance(n.position, neighbor.position) < 0.1f))
                    continue;

                if (!IsWalkableByTile(neighbor.position))
                    continue;

                float newGCost = currentNode.gCost + Vector2.Distance(currentNode.position, neighbor.position);

                Node2D existingNode = openList.FirstOrDefault(n => Vector2.Distance(n.position, neighbor.position) < 0.1f);

                if (existingNode == null)
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Vector2.Distance(neighbor.position, endNode.position);
                    neighbor.parent = currentNode;
                    openList.Add(neighbor);
                }
                else if (newGCost < existingNode.gCost)
                {
                    existingNode.gCost = newGCost;
                    existingNode.parent = currentNode;
                }
            }
        }

        if (currentIteration >= maxIterations)
        {
            Debug.LogError("Pathfinding exceeded maximum iterations!");
        }

        Debug.LogWarning("No path found!");
        return null;
    }

    List<Node2D> GetNeighbors(Node2D node)
    {
        List<Node2D> neighbors = new List<Node2D>();

        // 8방향 체크 (2D)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2 neighborPos = node.position + new Vector2(x * gridSize, y * gridSize);
                neighbors.Add(new Node2D(neighborPos));
            }
        }

        return neighbors;
    }

    // 타일 기반 검사 방법들 (2D용으로 수정)
    bool IsWalkableByTile(Vector2 worldPosition)
    {
        // 방법 1: 특정 타일들 배열로 체크 (추천)
        if (walkableTiles != null && walkableTiles.Length > 0)
        {
            return IsWalkableByTileArray(worldPosition);
        }

        // 방법 2: 단일 타일로 체크
        if (roadTile != null)
        {
            return IsWalkableBySingleTile(worldPosition);
        }

        // 방법 3: 타일 이름으로 체크
        return IsWalkableByTileName(worldPosition);
    }

    // 방법 1: 걸을 수 있는 타일 배열로 체크 (RuleTile 포함)
    bool IsWalkableByTileArray(Vector2 worldPosition)
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(new Vector3(worldPosition.x, worldPosition.y, 0));
        TileBase tileAtPosition = roadTilemap.GetTile(cellPosition);

        if (tileAtPosition == null) return false;

        // 배열에 있는 타일들 중 하나인지 체크
        foreach (TileBase walkableTile in walkableTiles)
        {
            if (tileAtPosition == walkableTile)
                return true;
        }

        return false;
    }

    // 방법 2: 단일 타일로 체크 (RuleTile 등)
    bool IsWalkableBySingleTile(Vector2 worldPosition)
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(new Vector3(worldPosition.x, worldPosition.y, 0));
        TileBase tileAtPosition = roadTilemap.GetTile(cellPosition);

        return tileAtPosition == roadTile;
    }

    // 방법 3: 타일 이름으로 체크 (유연하지만 느림)
    bool IsWalkableByTileName(Vector2 worldPosition)
    {
        Vector3Int cellPosition = roadTilemap.WorldToCell(new Vector3(worldPosition.x, worldPosition.y, 0));
        TileBase tileAtPosition = roadTilemap.GetTile(cellPosition);

        if (tileAtPosition == null) return false;

        string tileName = tileAtPosition.name.ToLower();

        // 도로 관련 이름들을 체크
        return tileName.Contains("road") ||
               tileName.Contains("path") ||
               tileName.Contains("street") ||
               tileName.Contains("도로");
    }

    List<Vector2> RetracePath(Node2D startNode, Node2D endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node2D currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }

        path.Add(startNode.position);
        path.Reverse();

        return path;
    }

    // 도로 타일들만 추출하는 헬퍼 메서드
    public List<Vector3Int> GetAllRoadTiles()
    {
        List<Vector3Int> roadTiles = new List<Vector3Int>();
        BoundsInt bounds = roadTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = roadTilemap.GetTile(position);

                if (tile != null && IsWalkableTile(tile))
                {
                    roadTiles.Add(position);
                }
            }
        }

        return roadTiles;
    }

    bool IsWalkableTile(TileBase tile)
    {
        if (walkableTiles != null && walkableTiles.Length > 0)
        {
            return System.Array.IndexOf(walkableTiles, tile) >= 0;
        }

        if (roadTile != null)
        {
            return tile == roadTile;
        }

        return tile.name.ToLower().Contains("road");
    }

    List<Vector2> SmoothPath(List<Vector2> originalPath)
    {
        if (originalPath == null || originalPath.Count < 3)
            return originalPath;

        List<Vector2> smoothedPath = new List<Vector2>();

        // 시작점 추가
        smoothedPath.Add(originalPath[0]);

        // 중간 점들을 부드럽게 처리
        for (int i = 1; i < originalPath.Count - 1; i++)
        {
            Vector2 prev = originalPath[i - 1];
            Vector2 current = originalPath[i];
            Vector2 next = originalPath[i + 1];

            // 베지어 곡선 스타일 스무딩
            Vector2 controlPoint1 = Vector2.Lerp(prev, current, pathSmoothness);
            Vector2 controlPoint2 = Vector2.Lerp(current, next, pathSmoothness);

            // 곡선상의 여러 점들 생성
            int segments = 3;
            for (int j = 0; j < segments; j++)
            {
                float t = (float)j / segments;
                Vector2 smoothPoint = CalculateBezierPoint(prev, controlPoint1, controlPoint2, next, t);

                // 도로 위에 있는지 확인
                if (IsWalkableByTile(smoothPoint))
                {
                    smoothedPath.Add(smoothPoint);
                }
            }
        }

        // 끝점 추가
        smoothedPath.Add(originalPath[originalPath.Count - 1]);

        return smoothedPath;
    }

    Vector2 CalculateBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    void OnReachedDestination()
    {
        Debug.Log("Enemy reached destination!");
        // 여기에 도착 시 로직 추가 (예: 적 제거, 데미지 등)

        

        // 적 오브젝트 제거
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        if (!showPath) return;

        // 원본 경로 그리기
        if (originalPath != null && originalPath.Count > 1)
        {
            Gizmos.color = pathColor;
            for (int i = 0; i < originalPath.Count - 1; i++)
            {
                Vector3 pos1 = new Vector3(originalPath[i].x, originalPath[i].y, 0);
                Vector3 pos2 = new Vector3(originalPath[i + 1].x, originalPath[i + 1].y, 0);
                Gizmos.DrawLine(pos1, pos2);
                Gizmos.DrawWireSphere(pos1, 0.1f);
            }
        }

        // 부드러운 경로 그리기
        if (smoothPath != null && smoothPath.Count > 1)
        {
            Gizmos.color = smoothPathColor;
            for (int i = 0; i < smoothPath.Count - 1; i++)
            {
                Vector3 pos1 = new Vector3(smoothPath[i].x, smoothPath[i].y, 0);
                Vector3 pos2 = new Vector3(smoothPath[i + 1].x, smoothPath[i + 1].y, 0);
                Gizmos.DrawLine(pos1, pos2);
            }

            // 현재 타겟 포인트 표시
            if (currentTargetIndex < smoothPath.Count)
            {
                Gizmos.color = Color.red;
                Vector3 targetPos = new Vector3(smoothPath[currentTargetIndex].x, smoothPath[currentTargetIndex].y, 0);
                Gizmos.DrawWireSphere(targetPos, 0.2f);
            }
        }
    }
}

// 2D용 노드 클래스
[System.Serializable]
public class Node2D
{
    public Vector2 position;
    public float gCost; // 시작점으로부터의 거리
    public float hCost; // 목적지까지의 추정 거리
    public float fCost { get { return gCost + hCost; } }
    public Node2D parent;

    public Node2D(Vector2 pos)
    {
        position = pos;
    }
}


