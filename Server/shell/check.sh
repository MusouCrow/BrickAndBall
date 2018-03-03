while true
do
	count=`ps -ef | grep $1 | grep -v "grep" | wc -l`
	echo $count

	if [ $count -gt 0 ]; then
        	echo "sb"
	else
        	echo "bb"
	fi
	
	sleep 10
done
