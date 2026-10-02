using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网格参考线（预制体版）。
/// 把 Prefab/GridReference 拖进任意关卡，它就在自己的位置上按格子大小画出一片网格，
/// 用来做地形、摆方块时的对齐参考。不依赖场景里的 Grid，单独一个空物体就能用。
///
/// 用法：
///   1. 拖进关卡，摆到你要参考的位置：它的位置就是网格上的一个格点，它的朝向就是网格的朝向
///   2. Cell Size 要和建造系统的格子大小一致（本项目 Grid 是 0.6）；也可以把场景里的 Grid 拖进 Grid 字段，格子大小就自动跟着它
///   3. Scene 视图用 Gizmos 画，不运行也看得见（做地形时看的就是这个）；
///      想在运行时也看到，就把 Show In Game View 勾上
///   4. 组件右上角齿轮菜单 → “对齐到场景里的 Grid”，可以一键把参考网格对准真正的建造网格
///   5. 运行时按 G 键可以临时开关
/// </summary>
[ExecuteAlways]
public class GridVisualizer : MonoBehaviour
{
    /// <summary>网格范围怎么算</summary>
    public enum AreaMode
    {
        /// <summary>以自己为中心画固定格数</summary>
        SizeInCells = 0,
        /// <summary>自动贴合 areaTarget 的包围盒</summary>
        TargetBounds = 1
    }

    [Header("格子大小")]
    [Tooltip("可以留空。留空就用下面的 Cell Size；把场景里的 Grid 拖进来就能自动跟上它的格子大小")]
    [SerializeField] private Grid grid;
    [Tooltip("每格的世界尺寸，要和建造系统的格子大小一致（本项目是 0.6）")]
    [SerializeField] private Vector2 cellSize = new Vector2(0.6f, 0.6f);

    [Header("范围")]
    [Tooltip("SizeInCells：以自己为中心画固定格数；TargetBounds：自动贴合 areaTarget 的包围盒")]
    [SerializeField] private AreaMode areaMode = AreaMode.SizeInCells;
    [Tooltip("以自己为中心，横向 / 纵向各画多少格（本项目这一关大约 26x26 格，默认给大一点好覆盖）")]
    [SerializeField] private Vector2Int sizeInCells = new Vector2Int(32, 32);
    [Tooltip("只在 TargetBounds 模式下用到，一般填一块地面")]
    [SerializeField] private Transform areaTarget;

    [Header("外观")]
    [Tooltip("网格线在自己所在平面上方抬一点，避免和地面重叠时闪烁")]
    [SerializeField] private float localYOffset = 0.02f;
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.35f);
    [Tooltip("每隔几格画一条醒目的线，方便数格子；小于等于 1 表示不强调")]
    [SerializeField] private int emphasizeEvery = 5;
    [SerializeField] private Color emphasizeColor = new Color(1f, 0.85f, 0.2f, 0.9f);
    [Tooltip("最外圈边框的颜色")]
    [SerializeField] private Color borderColor = new Color(0.35f, 0.9f, 1f, 1f);
    [Tooltip("线条数量上限，防止格子太小时画出几万条线把帧率拖垮")]
    [SerializeField] private int maxLines = 4000;

    [Header("显示开关")]
    [Tooltip("Scene 视图里是否显示（做地形 / 关卡时看的就是这个）")]
    [SerializeField] private bool showInSceneView = true;
    [Tooltip("Game 视图和打包后是否显示。纯粹当参考用的话建议关掉，免得进游戏还看得到这些线")]
    [SerializeField] private bool showInGameView = false;
    [Tooltip("运行时按下面的键临时开关")]
    [SerializeField] private bool toggleWithKey = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.G;

    /// <summary>一条待绘制的线（坐标是本地坐标，画的时候再乘上自己的变换矩阵）</summary>
    private struct Line
    {
        public Vector3 a;
        public Vector3 b;
        public Color color;
    }

    private readonly List<Line> lines = new List<Line>();
    private Material lineMaterial;
    private Transform cachedTarget;
    private Renderer cachedRenderer;
    private Collider cachedCollider;
    private int cachedHash;
    private bool cached;
    private bool gridLookedUp;
    private bool warnedTooMany;

    private void OnEnable()
    {
        cached = false;
    }

    private void OnValidate()
    {
        cached = false;//改 Inspector 参数后重新生成线
        gridLookedUp = false;
    }

    private void OnDestroy()
    {
        if (lineMaterial == null) return;
        if (Application.isPlaying) Destroy(lineMaterial);
        else DestroyImmediate(lineMaterial);
        lineMaterial = null;
    }

    private void Update()
    {
        if (!toggleWithKey || !Application.isPlaying) return;
#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(toggleKey)) ToggleVisible();
#endif
    }

    /// <summary>开关网格（Scene 视图和 Game 视图一起切）</summary>
    public void ToggleVisible()
    {
        SetVisible(!showInGameView);
    }

    /// <summary>设置网格显示与否</summary>
    public void SetVisible(bool visible)
    {
        showInGameView = visible;
        showInSceneView = visible;
    }

    /// <summary>一键把参考网格对准场景里真正的 Grid（位置和朝向都跟过去）</summary>
    [ContextMenu("对齐到场景里的 Grid")]
    private void AlignToSceneGrid()
    {
        Grid sceneGrid = FindFirstObjectByType<Grid>();
        if (sceneGrid == null)
        {
            Debug.LogWarning("[GridVisualizer] 场景里没有 Grid，没法对齐。", this);
            return;
        }

        grid = sceneGrid;
        transform.SetPositionAndRotation(sceneGrid.transform.position, sceneGrid.transform.rotation);
        cached = false;
        gridLookedUp = true;
        Debug.Log($"[GridVisualizer] 已对齐到 {sceneGrid.name}", this);
    }

    // ---------------- 绘制 ----------------

    // Scene 视图：用 Gizmos 画，不运行也看得见
    private void OnDrawGizmos()
    {
        if (!showInSceneView) return;
        Refresh();

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;//线是本地坐标，这里套上自己的变换
        for (int i = 0; i < lines.Count; i++)
        {
            Gizmos.color = lines[i].color;
            Gizmos.DrawLine(lines[i].a, lines[i].b);
        }
        Gizmos.matrix = oldMatrix;
    }

    // Game 视图 / 打包后：用 GL 画
    private void OnRenderObject()
    {
        if (!showInGameView) return;

        Camera cam = Camera.current;
        if (cam == null || cam.cameraType != CameraType.Game) return;//Scene 视图交给 Gizmos，避免画两遍

        Refresh();
        if (lines.Count == 0 || !EnsureMaterial()) return;

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);//线是本地坐标，这里套上自己的变换
        GL.Begin(GL.LINES);
        for (int i = 0; i < lines.Count; i++)
        {
            GL.Color(lines[i].color);
            GL.Vertex(lines[i].a);
            GL.Vertex(lines[i].b);
        }
        GL.End();
        GL.PopMatrix();
    }

    /// <summary>参数变了才重新算线，平时每帧直接复用</summary>
    private void Refresh()
    {
        int hash = ComputeHash();
        if (cached && hash == cachedHash) return;

        cachedHash = hash;
        cached = true;
        Rebuild();
    }

    private void Rebuild()
    {
        lines.Clear();
        warnedTooMany = false;

        Vector2 cell = EffectiveCellSize();
        if (cell.x <= 0f || cell.y <= 0f) return;//格子大小是 0 就画不出来

        int minX, maxX, minZ, maxZ;
        float y = localYOffset;

        if (areaMode == AreaMode.TargetBounds && TryGetTargetBounds(out Bounds bounds))
        {
            //把地面的世界包围盒转到自己的本地空间，看要盖住多少格
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = new Vector3(
                    (i & 1) == 0 ? bounds.min.x : bounds.max.x,
                    (i & 2) == 0 ? bounds.min.y : bounds.max.y,
                    (i & 4) == 0 ? bounds.min.z : bounds.max.z);
                Vector3 local = transform.InverseTransformPoint(corner);
                min = Vector3.Min(min, local);
                max = Vector3.Max(max, local);
            }

            minX = Mathf.FloorToInt(min.x / cell.x);
            maxX = Mathf.CeilToInt(max.x / cell.x) - 1;
            minZ = Mathf.FloorToInt(min.z / cell.y);
            maxZ = Mathf.CeilToInt(max.z / cell.y) - 1;
            y = max.y + localYOffset;//贴在地面顶部
        }
        else
        {
            int countX = Mathf.Max(1, sizeInCells.x);
            int countZ = Mathf.Max(1, sizeInCells.y);
            minX = -(countX / 2);
            maxX = minX + countX - 1;
            minZ = -(countZ / 2);
            maxZ = minZ + countZ - 1;
        }

        //横线：每个 z 一条，沿本地 X 方向
        for (int z = minZ; z <= maxZ + 1; z++)
        {
            float zz = z * cell.y;
            Vector3 a = new Vector3(minX * cell.x, y, zz);
            Vector3 b = new Vector3((maxX + 1) * cell.x, y, zz);
            AddLine(a, b, PickColor(z, minZ, maxZ + 1));
        }

        //竖线：每个 x 一条，沿本地 Z 方向
        for (int x = minX; x <= maxX + 1; x++)
        {
            float xx = x * cell.x;
            Vector3 a = new Vector3(xx, y, minZ * cell.y);
            Vector3 b = new Vector3(xx, y, (maxZ + 1) * cell.y);
            AddLine(a, b, PickColor(x, minX, maxX + 1));
        }
    }

    private void AddLine(Vector3 a, Vector3 b, Color color)
    {
        if (lines.Count >= maxLines)
        {
            if (!warnedTooMany)
            {
                warnedTooMany = true;
                Debug.LogWarning($"[GridVisualizer] 线条数超过上限 {maxLines}，只画了一部分。把格数调小，或者把 Max Lines 调大。", this);
            }
            return;
        }
        lines.Add(new Line { a = a, b = b, color = color });
    }

    /// <summary>最外圈用边框色，每隔 emphasizeEvery 格用强调色，其余用普通色</summary>
    private Color PickColor(int index, int min, int max)
    {
        if (index == min || index == max) return borderColor;
        if (emphasizeEvery > 1 && index % emphasizeEvery == 0) return emphasizeColor;
        return lineColor;
    }

    /// <summary>有 Grid 就跟着 Grid 的格子大小，没有就用自己填的</summary>
    private Vector2 EffectiveCellSize()
    {
        if (!gridLookedUp)
        {
            gridLookedUp = true;
            if (grid == null) grid = GetComponent<Grid>();
        }
        if (grid != null)
        {
            Vector3 size = grid.cellSize;
            if (size.x > 0f && size.z > 0f) return new Vector2(size.x, size.z);
        }
        return cellSize;
    }

    private bool TryGetTargetBounds(out Bounds bounds)
    {
        bounds = default;
        if (areaTarget == null)
        {
            cachedTarget = null;
            cachedRenderer = null;
            cachedCollider = null;
            return false;
        }

        if (cachedTarget != areaTarget)
        {
            cachedTarget = areaTarget;
            cachedRenderer = areaTarget.GetComponentInChildren<Renderer>();
            cachedCollider = cachedRenderer == null ? areaTarget.GetComponentInChildren<Collider>() : null;
        }

        if (cachedRenderer != null)
        {
            bounds = cachedRenderer.bounds;
            return true;
        }
        if (cachedCollider != null)
        {
            bounds = cachedCollider.bounds;
            return true;
        }
        return false;
    }

    private int ComputeHash()
    {
        unchecked
        {
            int h = 17;
            h = h * 31 + EffectiveCellSize().GetHashCode();
            h = h * 31 + (int)areaMode;
            h = h * 31 + sizeInCells.GetHashCode();
            h = h * 31 + localYOffset.GetHashCode();
            h = h * 31 + maxLines;
            h = h * 31 + emphasizeEvery;
            h = h * 31 + lineColor.GetHashCode();
            h = h * 31 + emphasizeColor.GetHashCode();
            h = h * 31 + borderColor.GetHashCode();

            if (areaMode == AreaMode.TargetBounds)
            {
                //这个模式下范围跟自己的位置有关，动一下就要重算
                Transform t = transform;
                h = h * 31 + t.position.GetHashCode();
                h = h * 31 + t.rotation.GetHashCode();
                h = h * 31 + t.lossyScale.GetHashCode();
                if (TryGetTargetBounds(out Bounds bounds))
                {
                    h = h * 31 + bounds.center.GetHashCode();
                    h = h * 31 + bounds.size.GetHashCode();
                }
            }
            return h;
        }
    }

    /// <summary>Game 视图画线用的材质，只用一次，用完自己销毁</summary>
    private bool EnsureMaterial()
    {
        if (lineMaterial != null) return true;

        Shader shader = Shader.Find("Hidden/Internal-Colored");
        if (shader == null) shader = Shader.Find("Sprites/Default");//打包后被裁剪时的备胎
        if (shader == null)
        {
            Debug.LogWarning("[GridVisualizer] 找不到能画线的 Shader，Game 视图里的网格画不出来（Scene 视图不受影响）。");
            return false;
        }

        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
        lineMaterial.SetInt("_ZBias", 0);
        return true;
    }
}
