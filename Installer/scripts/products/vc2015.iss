// requires Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires windows installer 3.1
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992&displaylang=en

[CustomMessages]
vc2015x86_title=MS Visual C++ 2015 Redistributable package (x86)
vc2015x64_title=MS Visual C++ 2015 Redistributable package (x64)

en.vc2015x86_size=13.5 MB
en.vc2015x64_size=14.3 MB
// specifiy the full /lcid parameter, including a trailing space! or leave it an empty string if default or unknown
// en.dotnetfx40client_lcid='/lcid 1033 '
en.vc2015_lcid=''

#ifdef dotnet_Passive
#define vc2015_passive "'/passive '"
#else
#define vc2015_passive "''"
#endif

[Code]
const
    vc2015x86_url = 'https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x86.exe';
    vc2015x64_url = 'https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x64.exe';

procedure vc2015();
var
	version: cardinal;
begin
    // x86 (32 bit) runtime
    if not RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\VCRedist\x86', 'Installed', version) then
        RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x86', 'Installed', version);
        if version <> 1 then
    		AddProduct('vc_redist.x86.exe',
    			CustomMessage('vc2015_lcid') + '/q ' + {#vc2015_passive} + '/norestart',
    			CustomMessage('vc2015x86_title'),
    			CustomMessage('vc2015x86_size'),
    			vc2015x86_url,false,false);
    if isX64 then begin
        version := 0;
        // x64 (64 bit) runtime also.
	    if not RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\VCRedist\x64', 'Installed', version) then
            RegQueryDWordValue(HKLM32, 'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64', 'Installed', version);
        if version <> 1 then
    		AddProduct('vc_redist.x64.exe',
    			CustomMessage('vc2015_lcid') + '/q ' + {#vc2015_passive} + '/norestart',
    			CustomMessage('vc2015x64_title'),
    			CustomMessage('vc2015x64_size'),
    			vc2015x64_url,false,false);
    end;
end;
