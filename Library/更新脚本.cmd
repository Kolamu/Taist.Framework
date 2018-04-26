::
::本文件用于更新最新版本
::感谢张志学提供的思路,在他之前脚本的基础上做了一些改进
::使用方法为编辑本文件中的SVN路径（SVN_PATH），然后Copy到Runner文件夹下执行即可
::如果Runner在共享文件夹中，RUNNER_PATH不能为空，需要设置为Runner的绝对路径，感谢贾晓翰的补充
::

@echo off

set SVN_PATH="c:\SVN"
set RUNNER_PATH=""

if %RUNNER_PATH% == "" set RUNNER_PATH="%cd%"

TortoiseProc /command:update /path:%SVN_PATH% /closeonend:3

xcopy "%SVN_PATH%\Taist.Framework\Binary\Mock\*.dll" "%RUNNER_PATH%\Bin\" /e /i /y
xcopy "%SVN_PATH%\Taist.Framework\Binary\Mock\*.pdb" "%RUNNER_PATH%\" /e /i /y

xcopy "%SVN_PATH%\Taist.Business\Binary\Business\*.dll" "%RUNNER_PATH%\Bin\" /e /i /y
xcopy "%SVN_PATH%\Taist.Business\Binary\Business\*.pdb" "%RUNNER_PATH%\" /e /i /y

xcopy "%SVN_PATH%\Taist.Business\Binary\DataStore\*.dll" "%RUNNER_PATH%\Bin\" /e /i /y
xcopy "%SVN_PATH%\Taist.Business\Binary\DataStore\*.pdb" "%RUNNER_PATH%\" /e /i /y

xcopy "%SVN_PATH%\Taist.Business\Binary\Core\*.dll" "%RUNNER_PATH%\Bin\" /e /i /y
xcopy "%SVN_PATH%\Taist.Business\Binary\Core\*.pdb" "%RUNNER_PATH%\" /e /i /y

pause