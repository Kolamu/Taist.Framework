::
::���ļ����ڸ������°汾
::��л��־ѧ�ṩ��˼·,����֮ǰ�ű��Ļ���������һЩ�Ľ�
::ʹ�÷���Ϊ�༭���ļ��е�SVN·����SVN_PATH����Ȼ��Copy��Runner�ļ�����ִ�м���
::���Runner�ڹ����ļ����У�RUNNER_PATH����Ϊ�գ���Ҫ����ΪRunner�ľ���·������л�������Ĳ���
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