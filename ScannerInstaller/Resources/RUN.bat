@echo on
cls
cd /d %~dp0
msiexec /i "%1"  /l*vx %tmp%\%~n1.log 
"c:\Program Files (x86)\Notepad++\notepad++.exe" "%tmp%\%~n1.log"