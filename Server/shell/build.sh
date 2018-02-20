basepath=$(cd `dirname $0`; cd ..; pwd)

function build() {
    echo "====================="
    echo "start build skyent..."
    cd $basepath/skynet
    make linux

    echo "====================="
    echo "start build lua-kcp..."
    cd $basepath/3rd/lua-kcp
    make
    mv lualib/lkcp.so $basepath/lib/lkcp.so
    mv lualib/lutil.so $basepath/lib/lutil.so

    echo "====================="
    echo "clean..."
    cd $basepath
    find . -name "*.o"  | xargs rm -f
}

function clean() {
	echo "====================="
	echo "start clean skyent..."
	cd $basepath/skynet
	make clean

	echo "====================="
	echo "start clean lua-kcp..."
	cd $basepath/3rd/lua-kcp
	make clean

	echo "====================="
	echo "start clean lualib..."
    cd $basepath
    rm -f lib/*.so
    find . -name "*.o"  | xargs rm -f
}

if [[ "$1" == "" ]]; then
	build
elif [[ "$1" == "clean" ]]; then
	clean
fi
