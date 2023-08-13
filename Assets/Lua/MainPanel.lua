--只要是新的对象（面板）那么就新建一张表
MainPanel = {}
--需要做 实例化面板对象
--写在这比较清晰
MainPanel.panelObj = nil
MainPanel.btnRole = nil
MainPanel.btnSkill = nil
MainPanel.btnBag = nil

--初始化该面板 实例化对象 控件事件监听
function MainPanel:Init()
    if self.panelObj == nil then
        local panel = ABManager:LoadResource("ui", "MainPanel", typeof(GameObject))
        local father = GameObject.Find("Canvas")
        panel.name = "主面板"
        self.panelObj = GameObject.Instantiate(panel, father.transform)

        --找到对应脚本
        self.btnRole = self.panelObj.transform:Find("Role"):GetComponent(typeof(Button))
        self.btnBag = self.panelObj.transform:Find("Bag"):GetComponent(typeof(Button))
        self.btnSkill = self.panelObj.transform:Find("Skill"):GetComponent(typeof(Button))

        --绑定事件
        self.btnBag.onClick:AddListener(
            function()
                self:BtnBagClick()
            end
        )
    end
end

function MainPanel:ShowMe()
    self:Init()
    self.panelObj:SetActive(true)
end

function MainPanel:HideMe()
    self.panelObj:SetActive(false)
end

function MainPanel:BtnBagClick()
    print("背包打开")
    BagPanel:ShowMe()
    self:HideMe()
 
end

