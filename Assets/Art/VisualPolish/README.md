# 视觉资源说明

本套样式使用 Unity 内置渲染管线、现有双面 Shader 和标准 UI 组件，没有新增运行时脚本或渲染插件。

- `Materials/Floor.mat`：棋盘地面，共用 `BoardGrid.png`。地面网格对应现有 0.6 格子。
- `Materials/Walls.mat`：墙体。
- `Materials/Block.mat`、`Arrow.mat`：可放置方块与方向标记。
- `Materials/Target.mat`、`TargetMark.mat`：青色终点与顶部标记。
- `Materials/Launcher.mat`、`LauncherAccent.mat`：发射器。
- `Materials/Pulse.mat`、`PulseTrail.mat`：子弹与短拖尾。
- `Materials/Indicator.mat`：现有鼠标与格子指示器，继续使用原有双面 Shader。
- `Meshes/SoftCube.asset`：轻微圆角的显示模型。碰撞体保持原样。
- `Meshes/*-Floor.asset`：各场景的地面显示网格，UV 对齐现有放置网格。
- `UI`：可九宫格拉伸的圆角背景与方向、删除、取消、完成图标。

每个场景的 `VisualPolish` 根物体只包含棋盘底座、背景和补光，没有碰撞体或玩法脚本。目标上的 `Polish target glow` 会随目标关闭。

在 `UI` 中可以直接修改工具栏、说明卡片、胜利卡片和左上角关卡文字。保留原有 Button 组件和 On Click 事件即可继续使用原来的逻辑。新文字位于 `Polish Label`、`Polish Card` 等子物体，旧文字保留但停用。

Canvas 使用 1600 × 900 参考分辨率并随屏幕缩放；相机仍为透视相机。原有黄色 Scene 参考网格保持不变，默认关闭它在 Game 视图中的显示，游戏内改用低对比度的地面网格。

本次未修改原有 C#、关卡位置、碰撞体、按钮事件、构建场景顺序或渲染管线配置。Level7 的 Win 根物体在原始工作区中已开启，本次保留该状态；正常关卡流程可从 Level0 进入。

修改前的场景、Prefab、材质和配置副本保存在 `Library/VisualPolishBackup/20261020`。此目录属于本地 Library，清理 Library 前请另行保留需要的副本。
