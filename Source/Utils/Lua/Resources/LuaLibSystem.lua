function enum(o)
	local e = o:GetEnumerator()
	return function()
		if e:MoveNext() then return e.Current end
	end 
end

function print(...)
	local line = ""
	for k, v in pairs({...}) do
		line = line .. tostring(v)
	end
	PrintLine(line)
end

print "Registered System!" 
