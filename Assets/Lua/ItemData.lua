--从AB包中加载Json表
--加载Text文件对象
local txt= ABManager:LoadResource("json","ItemData",typeof(TextAsset))
--解析Json表
local itemList= Json.decode(txt.text)

--转成表
ItemData = {}
for _,value in pairs(itemList) do
    ItemData[value.id]=value
    end

   