# SqlCleaner
![screenshot](https://github.com/scompton93/SqlCleaner/blob/master/SqlCleaner/ScreenShot.png?raw=true)

This is a simple tool to convert 'sp_prepexec' queries into executable queries. 'sp_prepexec' Queries are typically the result of a parameterized query for SQL Server being copied out of SQL Profiler and are typically un-executable via SSMS or other clients without shuffling some parameters and string escaping around.

This tool utilizes DacFX to achieve this, while you could do this with regex, I have found DacFX to be the most reliable in producing a working query.
