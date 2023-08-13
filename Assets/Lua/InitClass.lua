--常用别名声明

--准备导入的lua脚本
--面向对象相关
require("Object")
--字符串拆分
require("SplitTools")
--Json解析
Json=require("JsonUtility")


--Unity相关
GameObject =CS.UnityEngine.GameObject
Resources=CS.UnityEngine.Resources
Transform = CS.UnityEngine.Transform
RectTransform = CS.UnityEngine.RectTransform
--图集对象类
SpriteAtlas=CS.UnityEngine.U2D.SpriteAtlas
--数学
Vector3 = CS.UnityEngine.Vector3
Vector2 = CS.UnityEngine.Vector2

--UI相关
UI=CS.UnityEngine.UI
Image = UI.Image
Button =UI.Button
Text = UI.Text
Toggle = UI.Toggle
ScrollRect = UI.ScrollRect
--文本相关
TextAsset =CS.UnityEngine.TextAsset

--自定义的C#脚本
--直接的到AB资源管理器的单例对象
ABManager =CS.ABManager.Instance

