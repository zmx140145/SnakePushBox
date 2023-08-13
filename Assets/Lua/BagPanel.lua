BagPanel = {}

--成员变量
--面板对象
BagPanel.panelObj = nil

--各个组件
BagPanel.btnClose = nil
BagPanel.togEquip = nil
BagPanel.togMaterial = nil
BagPanel.togProp = nil
BagPanel.togAttribute = nil
BagPanel.svBag = nil
BagPanel.Content = nil
--成员方法

function BagPanel:Init()
    --实例化对象
    if self.panelObj == nil then
        local panel = ABManager:LoadResource("ui", "BagPanel", typeof(GameObject))
        local father = GameObject.Find("Canvas")
        panel.name = "背包面板"
        self.panelObj = GameObject.Instantiate(panel, father.transform)

        --找组件

        self.btnClose = self.panelObj.transform:Find("Panel"):Find("BtnClose"):GetComponent(typeof(Button))
        --找三个Toggle
        local group = self.panelObj.transform:Find("ToggleGroup")
        self.togEquip = group:Find("Equip"):GetComponent(typeof(Toggle))
        self.togAttribute = group:Find("Attribute"):GetComponent(typeof(Toggle))
        self.togMaterial = group:Find("Material"):GetComponent(typeof(Toggle))
        self.togProp = group:Find("Prop"):GetComponent(typeof(Toggle))
        --列表
        self.svBag = self.panelObj.transform:Find("ItemPanel"):Find("Scroll View"):GetComponent(typeof(ScrollRect))
        self.Content = self.svBag.transform:Find("Viewport"):Find("Content")
        --加事件

        --关闭按钮
        self.btnClose.onClick:AddListener(function()
            self:HideMe()
            MainPanel:ShowMe()
        end)

        self.togEquip.onValueChanged:AddListener(function (value)
            if value == true then
              self:ChangeType(1)
            end
        end)
        self.togProp.onValueChanged:AddListener(function (v)
            if v == true then
              self:ChangeType(2)
            end
        end)
        self.togMaterial.onValueChanged:AddListener(function (v)
            if v == true then
              self:ChangeType(3)
            end
        end)
        self.togAttribute.onValueChanged:AddListener(function (v)
            if v == true then
              self:ChangeType(4)
            end
        end)
    end
end


function BagPanel:OnEquipSelect()
    self:Init();
    self.panelObj:SetActive(true)
end

function BagPanel:ShowMe()
    self:Init();
    self.panelObj:SetActive(true)
end

function BagPanel:HideMe()
    print("背包关闭")
    self.panelObj:SetActive(false)
end

--逻辑处理

--1装备 2道具 3材料 4属性
function BagPanel:ChangeType(value)
    print("改变")
end
