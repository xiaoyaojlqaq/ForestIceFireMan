# Pause Menu UI Assets

按提供的蓝灰色金属像素风参考制作。使用内置 imagegen，逐个生成，共 10 张独立 PNG。所有 PNG 均已检查：RGBA，存在透明区域与不透明主体。子文件夹和文件名均为英文。

| File | 用途 |
| --- | --- |
| Panels/pause_panel.png | 暂停界面主面板（空白） |
| Panels/settings_title.png | 设置标题牌（含“设置”） |
| Buttons/button_resume.png | 继续游戏按钮（含中文） |
| Buttons/button_level_select.png | 返回关卡选择按钮（含中文） |
| Buttons/button_close.png | 关闭按钮（X） |
| Buttons/button_blank.png | 无文字按钮底图 |
| Sliders/slider_background.png | 拖动条背景 / 底槽 |
| Sliders/slider_fill.png | 拖动条覆盖 / 填充条 |
| Sliders/slider_handle.png | 拖动按钮 / 滑块 |
| Icons/icon_volume.png | 音量图标（音乐、音效共用） |

## 组装说明

- 主面板、标题、各按钮分别叠加使用。
- 音乐与音效各建立一套滑动条，复用 Sliders 文件夹内的三个文件。
- slider_background 在最底层；slider_fill 放在底槽内，按音量裁切显示宽度；slider_handle 单独随数值移动。
- icon_volume 可用于两行；“音乐”“音效”标签可在引擎中添加文本。
- 带中文的按钮是已合成文字的图片。需要自行修改文案时使用 button_blank。
- PNG 保留生成原图与透明留白。可在引擎的精灵编辑器中裁定可见区域；AssetManifest.json 中的 bounds 是原始图片坐标，从左上角起算，右下边界不包含在内。半透明边缘可能使该范围略大于主体。

## 验收范围

已检查文件存在、命名、图片格式、尺寸、透明通道，并目视检查生成结果。未在 Unity 项目内组装或运行测试。

原始提示词见 GenerationPrompts.md。

