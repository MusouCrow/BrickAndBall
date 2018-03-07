basepath=$(cd `dirname $0`; cd ..; pwd)
cd $basepath

while true
do
	count=`ps -ef | grep skynet | grep -v "grep" | wc -l`

	if [ $count -gt 0 ]; then
	    :
	else
	    echo "program has crashed, restarting..."
	    screen shell/run.sh
	fi
	
	sleep 10
done
