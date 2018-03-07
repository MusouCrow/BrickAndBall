# Brick & Ball
Brick & Ball is a realtime-mobile-fighting game, it issue in [TapTap](https://www.taptap.com/app/81845).

## Client
* The client is made with [Unity](https://unity3d.com), you should use Unity 2017.3 or highter to open it.

## Server
* The server is made with [Skynet](https://github.com/cloudwu/skynet), I recommend it runs on [Ubuntu](https://www.ubuntu.com).
* When build before, you should ensure have [Autoconf](http://www.gnu.org/software/autoconf/autoconf.html).  
* If you want to have error warning, you should have [Mailutils](http://mailutils.org) and set the property of mail in *src/config*.
* Shell

```bash
shell/build.sh # Build.
shell/build.sh clean # Clean the compiled files.
shell/run.sh # Normally run.
shell/watch.sh # Run and watching the process, if it crash, restart it.
shell/cleanLog.sh # Clean the log files.
```
