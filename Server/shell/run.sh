basepath=$(cd `dirname $0`; cd ..; pwd)
cd $basepath
echo logger = \"log/$(date +"%y-%m-%d-%H-%M-%S").log\" > src/logger
./skynet/skynet src/config
