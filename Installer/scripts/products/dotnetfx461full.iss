// requires Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires windows installer 3.1
// WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=en

[CustomMessages]
dotnetfx461full_title=.NET 4.6.1 Full Framework

dotnetfx461full_size=64.5 MB
// specifiy the full /lcid parameter, including a trailing space! or leave it an empty string if default or unknown
// en.dotnetfx461full_lcid='/lcid 1033 '
en.dotnetfx461full_lcid=''

#ifdef dotnet_Passive
#define dotnetfx461full_passive "'/passive '"
#else
#define dotnetfx461full_passive "''"
#endif

[Code]
const
	dotnetfx461full_url = 'http://download.microsoft.com/download/E/4/1/E4173890-A24A-4936-9FC9-AF930FE3FA40/NDP461-KB3102436-x86-x64-AllOS-ENU.exe';

function dotnetfx461full(checkOnly : boolean) : boolean;
var
	version: cardinal;
begin
    result := true;
	RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\full', 'Install', version);
	if version <> 1 then begin
        result := false;
        if not checkOnly then
    		AddProduct('NDP461-KB3102436-x86-x64-AllOS-ENU.exe',
    			CustomMessage('dotnetfx461full_lcid') + '/q ' + {#dotnetfx461full_passive} + '/norestart',
    			CustomMessage('dotnetfx461full_title'),
    			CustomMessage('dotnetfx461full_size'),
    			dotnetfx461full_url,false,true);
    end;
end;
