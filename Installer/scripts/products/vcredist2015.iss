// requires Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires windows installer 3.1
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=en

[CustomMessages]
vcredist2015x86_title=MS Visual C++ 2015 Redistributable package (x86)
vcredist2015x64_title=MS Visual C++ 2015 Redistributable package (x64)

en.vcredist2015x86_size=13.5 MB
en.vcredist2015x64_size=14.3 MB
// specifiy the full /lcid parameter, including a trailing space! or leave it an empty string if default or unknown
// en.dotnetfx40client_lcid='/lcid 1033 '
en.vcredist2015_lcid=''

#ifdef dotnet_Passive
#define vcredist2015_passive "'/passive '"
#else
#define vcredist2015_passive "''"
#endif

[Code]
const
    vcredist2015x86_url = 'http://download.microsoft.com/download/d/e/c/dec58546-c2f5-40a7-b38e-4df8d60b9764/vc_redist.x86.exe';
    vcredist2015x64_url = 'http://download.microsoft.com/download/2/c/6/2c675af0-2155-4961-b32e-289d7addfcec/vc_redist.x64.exe';

procedure vcredist2015();
var
	version: cardinal;
begin
    // x86 (32 bit) runtime
    if not RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\VCRedist\x86', 'Installed', version) then
        RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x86', 'Installed', version);
        if version <> 1 then
    		AddProduct('vc_redist.x86.exe',
    			CustomMessage('vcredist2015_lcid') + '/q ' + {#vcredist2015_passive} + '/norestart',
    			CustomMessage('vcredist2015x86_title'),
    			CustomMessage('vcredist2015x86_size'),
    			vcredist2015x86_url,false,false);
    if isX64 then begin
        version := 0;
        // x64 (64 bit) runtime also.
	    if not RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\VCRedist\x64', 'Installed', version) then
            RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64', 'Installed', version);
        if version <> 1 then
    		AddProduct('vc_redist.x64.exe',
    			CustomMessage('vcredist2015_lcid') + '/q ' + {#vcredist2015_passive} + '/norestart',
    			CustomMessage('vcredist2015x64_title'),
    			CustomMessage('vcredist2015x64_size'),
    			vcredist2015x64_url,false,false);
    end;
end;
