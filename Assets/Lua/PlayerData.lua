PlayerData={}
--背包数据

PlayerData.equips = {}
PlayerData.prop = {}
PlayerData.attribute={}
PlayerData.material={}


--为玩家数据写一个初始化方法 以后直接改这里的数据来源
function  PlayerData:Init()
    table.insert(self.equips,{ID = 1 ,Count = 1})
    table.insert(self.equips,{ID = 2 ,Count = 1})
    table.insert(self.material,{ID = 3 ,Count = 50})
    table.insert(self.prop,{ID = 4 ,Count = 30})
    table.insert(self.prop,{ID = 5 ,Count = 88})
end
PlayerData:Init()
