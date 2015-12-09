--[[
	Entry scene main script, this will transition into loading and main menu scene
]]--

function Update(gameTime)
	-- Transition into the Main Menu
	SceneTransition("MainMenu", "{Resource Scripts\MainMenu\Init.lua}");
end
